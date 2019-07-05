using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TrioServer.Communication
{
    public class Channel
    {
        public int Id { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public int Port { get; private set; }
        public int PoolingInterval { get; set; }

        public Channel(int id, string ip, int port, int poolingInterval)
        {
            Id = id;
            Port = port;
            PoolingInterval = poolingInterval;

            IPAddress myAddress;
            if (IPAddress.TryParse(ip, out myAddress))
                EndPoint = new IPEndPoint(myAddress, port);
            else
                throw new ArgumentException(String.Format("O endereço {0}:{1} não é válido", ip, port));
        }
    }
}
