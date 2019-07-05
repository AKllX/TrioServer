using System;
using System.Linq;
using System.Net.Sockets;

using TrioServer.Communication;

namespace TrioServer.Sessions
{
    public class Session : IDisposable
    {
        private uint mId;
        private Socket mSocket;
        private Channel mChannel;
        private byte[] mBuffer;
        private double mStoppedTimestamp;
        private bool mAuthProcessed;

        public uint Id
        {
            get
            {
                return mId;
            }
        }

        public double TimeStopped
        {
            get
            {
                return (Program.GetCurrentTimestamp() - mStoppedTimestamp);
            }
        }

        public bool Stopped
        {
            get
            {
                return (mSocket == null);
            }
        }

        public Session(uint Id, Socket Socket, Channel channel)
        {
            mId = Id;
            mSocket = Socket;
            mBuffer = new byte[512];
            mChannel = channel;

            mSocket.Blocking = false;

            BeginReceive();

            Console.WriteLine("Started client " + Id + ".");
        }

        private void BeginReceive()
        {
            try
            {
                if (mSocket != null)
                {
                    mSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), null);
                }
            }
            catch (Exception)
            {
                SessionManager.StopSession(mId);
            }
        }

        private void OnReceiveData(IAsyncResult Result)
        {
            Console.WriteLine("Mensagem recebida");
            int ByteCount = 0;

            try
            {
                if (mSocket != null)
                {
                    ByteCount = mSocket.EndReceive(Result);
                }
            }
            catch (Exception) { }

            if (ByteCount < 1 || ByteCount >= mBuffer.Length)
            {
                SessionManager.StopSession(mId);
                return;
            }

            //TODO: ProcessData

            Console.WriteLine(BitConverter.ToString(mBuffer));

            BeginReceive();
        }

        public void SendData(byte[] Data)
        {
            try
            {
                if (mSocket != null)
                {
                    mSocket.BeginSend(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnDataSent), null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[SND] Socket is null!\n\n" + e.StackTrace);
            }
            /*
             * TODO: catch all exceptions => Stop()
             */
        }

        private void OnDataSent(IAsyncResult Result)
        {
            try
            {
                if (mSocket != null)
                {
                    mSocket.EndSend(Result);
                }
            }
            catch (Exception)
            {
                SessionManager.StopSession(mId);
            }
        }

        private void ProcessData(byte[] Data)
        {
            //TODO
        }

        public void Stop()
        {
            if (Stopped)
            {
                return;
            }
            mSocket.Close();
            mSocket = null;

        }

        public void Dispose()
        {
            mBuffer = null;
        }

    }
}
