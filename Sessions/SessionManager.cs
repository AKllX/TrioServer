using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using TrioServer.Communication;
using TrioServer.Radios;
using TrioServer.Database.Interfaces;

namespace TrioServer.Sessions
{
    public static class SessionManager
    {
        private static Dictionary<uint, Session> mSessions;
        private static uint mCounter;
        private static List<uint> mSessionsToStop;
        private static Timer mMonitorThread;
        private static object mSyncRoot;

        public static Dictionary<uint, Session> Sessions
        {
            get
            {
                Dictionary<uint, Session> Copy = new Dictionary<uint, Session>();

                lock (mSessions)
                {
                    foreach (KeyValuePair<uint, Session> Session in mSessions)
                    {
                        if (Session.Value.Stopped)
                        {
                            continue;
                        }

                        Copy.Add(Session.Key, Session.Value);
                    }
                }

                return new Dictionary<uint, Session>(Copy);
            }
        }

        public static void StopSession(uint SessionId)
        {
            lock (mSessionsToStop)
            {
                mSessionsToStop.Add(SessionId);
            }
        }

        public static int ActiveConnections
        {
            get
            {
                lock (mSessions)
                {
                    return mSessions.Count;
                }
            }
        }

        public static void Initialize()
        {
            mSessions = new Dictionary<uint, Session>();
            mSessionsToStop = new List<uint>();
            mCounter = 0;

            mMonitorThread = new Timer(new TimerCallback(ExecuteMonitor), null, TimeSpan.FromMilliseconds(4000), TimeSpan.FromMilliseconds(4000));
            
            mSyncRoot = new object();

        }

        private static void ExecuteMonitor(object state)
        {
            List<Session> ToDispose = new List<Session>();
            List<Session> ToStop = new List<Session>();

            lock (mSessions)
            {
                lock (mSessionsToStop)
                {
                    foreach (uint SessionId in mSessionsToStop)
                    {
                        if (mSessions.ContainsKey(SessionId))
                        {
                            ToStop.Add(mSessions[SessionId]);
                        }
                    }

                    mSessionsToStop.Clear();
                }

                foreach (Session Session in mSessions.Values)
                {
                    if (ToStop.Contains(Session))
                    {
                        continue;
                    }

                    if (Session.Stopped)
                    {
                        if (Session.TimeStopped > 15)
                        {
                            ToDispose.Add(Session);
                        }

                        continue;
                    }

                    if(Session.LastReceived == DateTime.MinValue)
                    {
                        Session.TryAcknowledge();
                    }

                    if((DateTime.Now - Session.LastReceived).TotalMinutes > 10)
                    {
                        Session.MAuthProcessed = false;
                        Session.AuthMessageCounter = 0;
                        Session.TryAcknowledge();
                    }
                }

                // Adicionado pela Laila
                Console.WriteLine("Iniciando check de tempo");
                DateTime timeinit = DateTime.Now;
                foreach (Session Session in mSessions.Values)
                {
                    foreach (Radio Radio in Session.Channel.Radios)
                    {
                        if (((DateTime.Now - Radio.TimeStamp).TotalMinutes > 10) && (Radio.CommStatus == 1))
                        {
                            Core.GetRadioManager().SetCommStatus(Radio.SerialNumber, 0);
                            Radio.CommStatus = 0;
                        }
                    }
                }
                Console.WriteLine(String.Concat("Tempo Gasto: ", Convert.ToString((DateTime.Now - timeinit).TotalMilliseconds)));
            }

            foreach (Session SessionDispose in ToDispose)
            {
                SessionDispose.Dispose();

                lock (mSessions)
                {
                    if (mSessions.ContainsKey(SessionDispose.Id))
                    {
                        mSessions.Remove(SessionDispose.Id);
                    }
                }
            }
        }

        public static void MakeConnection(Channel channel)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(channel.LocalEndPoint);

            lock(mSyncRoot)
            {
                uint id = mCounter++;
                mSessions.Add(id, new Session(id, socket, channel));
            }
        }

        public static void InitCommuncations()
        {
            foreach(Session s in mSessions.Values)
            {
                s.TryAcknowledge();
            }
        }
    }
}
