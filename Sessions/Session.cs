using System;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Runtime;
using System.Threading;


using TrioServer.Composers;
using TrioServer.Communication;
using TrioServer.Handlers;
using TrioServer.Radios;

namespace TrioServer.Sessions
{
    public class Session : IDisposable
    {
        private Socket mSocket;
        private readonly Channel mChannel;
        private byte[] mBuffer;
        private double mStoppedTimestamp;
        private bool mAuthProcessed;
        private byte packetCounter;
        private Timer mMonitorThread;
        private DateTime mLastReceived;

        public DateTime LastReceived
        {
            get
            {
                return mLastReceived;
            }
        }

        public Channel Channel
        {
            get
            {
                return mChannel;
            }
        }

        public uint Id { get; }

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

        public int AuthMessageCounter { get; set; }
        public bool MAuthProcessed { get => mAuthProcessed; set => mAuthProcessed = value; }

        public Session(uint Id, Socket Socket, Channel channel)
        {
            mLastReceived = DateTime.MinValue;
            this.Id = Id;
            mSocket = Socket;
            mBuffer = new byte[512];
            mChannel = channel;
            packetCounter = 0;
            AuthMessageCounter = 0;

            mSocket.Blocking = false;
            BeginReceive();

            Console.WriteLine("Canal aberto: " +Socket.LocalEndPoint.ToString());
        }

        public byte PacketTick()
        {
            return packetCounter++;
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
                SessionManager.StopSession(Id);
            }
        }

        private void OnReceiveData(IAsyncResult Result)
        {
            int ByteCount = 0;
            byte[] filteredData;

            mLastReceived = DateTime.Now;
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
                SessionManager.StopSession(Id);
                return;
            }

            filteredData = new byte[ByteCount];
            Buffer.BlockCopy(mBuffer, 0, filteredData, 0, ByteCount);

            ProcessData(filteredData);

            BeginReceive();
        }

        public void SendData(byte[] Data)
        {
            try
            {
                if (mSocket != null)
                {
                    mSocket.BeginSendTo(Data, 0, Data.Length, SocketFlags.None,this.Channel.RemoteEndPoint, new AsyncCallback(OnDataSent), null);
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
                SessionManager.StopSession(Id);
            }
        }

        private void ProcessData(byte[] Data)
        {
            if (Data.Length == 0)
            {
                return;
            }
            else
            {
                int i = 4;
                int radioSn;
                byte[] content = new byte[Data.Length - 7];

                radioSn = Program.GetSerialNumberFromBytes(new byte[] { Data[i++], Data[i++], Data[i++] });
                for(int j =0; j < content.Length; j++)
                {
                    content[j] = Data[i];
                    i++;
                }
                RadioMessage message = new RadioMessage(radioSn, content);
                handleMessage(message);
            }
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

        public void TryAcknowledge()
        {
            MemoryStream packet;
            packet = AcknowledgeComposer.Serialize(this);
            SendData(packet.ToArray());
        }

        public void RunMeasurement(object state)
        {
            IRadioTrio myRadio = mChannel.GetNextRadio();
            //Console.WriteLine(myRadio.Desc);
            if(myRadio != null)
            {
                if (MAuthProcessed)
                {
                    if (myRadio.Type == RadioType.QB || myRadio.Type == RadioType.QR)
                    {
                        SendData(UpdateMeasurements.Serialize(this,myRadio).ToArray());
                    }
                    else
                    {
                        SendData(UpdateMeasurementsM.Serialize(this,myRadio).ToArray());
                    }
                }
            }
        }

        private void handleMessage(RadioMessage message)
        {
            IRadioTrio myRadio = Core.GetRadioManager().LoadRadio(message.RadioSerialNumber);

            if(myRadio != null)
            { 
                RadioType radioType = Core.GetRadioManager().LoadRadio(message.RadioSerialNumber).Type;
       
                //Console.WriteLine(message.RadioSerialNumber);
                switch (AuthMessageCounter)
                {
                    case 1:
                        {
                            if ((radioType == RadioType.QR) || (radioType == RadioType.QB))
                            {
                                SendData(HandshakeComposer.Serialize(this).ToArray());
                            }
                            else
                            {
                                SendData(HandshakeComposerM.Serialize(this).ToArray());
                            }
                            break;
                        }
                    case 2:
                        {
                            if ((radioType == RadioType.QR) || (radioType == RadioType.QB))
                            {
                                SendData(CalibrationComposer.Serialize(this).ToArray());
                            }

                            else
                            {
                                SendData(CalibrationComposerM.Serialize(this).ToArray());
                            }

                            mMonitorThread = new Timer(new TimerCallback(RunMeasurement), null, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(Channel.PoolingInterval));

                            MAuthProcessed = true;
                            break;
                        }
                    default:
                        {
                            if (mAuthProcessed)
                            {
                                byte counterCheck = message.GetByte();
                                if (counterCheck == 0x67 || counterCheck == 0x72)
                                {
                                    IncomingMeasurementHandler.Deserialize(message,counterCheck);
                                }
                            }
                            break;
                        }
                }
            }
        }
    }
}
