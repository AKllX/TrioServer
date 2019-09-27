using System;
using System.Net;
using System.Net.Sockets;

namespace TrioServer.Communication
{
    public delegate void OnNewConnectionCallback(Socket Socket);
    public class TrioUdpListener : IDisposable
    {
        private Socket mSocket;
        private OnNewConnectionCallback mCallback;

        public TrioUdpListener(IPEndPoint LocalEndpoint, int Backlog, OnNewConnectionCallback Callback)
        {
            mCallback = Callback;

            mSocket = new Socket(LocalEndpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            mSocket.Bind(LocalEndpoint);
            mSocket.Listen(Backlog);
            mSocket.Blocking = false;

            BeginAccept();
        }

        private void BeginAccept()
        {
            try
            {
                mSocket.BeginAccept(OnAccept, null);
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            if (mSocket != null)
            {
                mSocket.Dispose();
                mSocket = null;
            }
        }

        private void OnAccept(IAsyncResult Result)
        {
            try
            {
                Socket ResultSocket = (Socket)mSocket.EndAccept(Result);
                mCallback.Invoke(ResultSocket);
            }
            catch (Exception) { }

            BeginAccept();
        }
    }
}
