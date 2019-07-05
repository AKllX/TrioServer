using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TrioServer.Communication
{
    public class Channel
    {
        public int Id { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
        public int LocalPort { get; private set; }
        public int RemotePort { get; private set; }
        public int PoolingInterval { get; set; }

        public Channel(int id, string ip, int r_port, int l_port, int poolingInterval)
        {
            Id = id;
            RemotePort = r_port;
            LocalPort = l_port;
            PoolingInterval = poolingInterval;

            IPAddress myAddress;
            if (IPAddress.TryParse(ip, out myAddress))
            {
                RemoteEndPoint = new IPEndPoint(myAddress, r_port);
                LocalEndPoint = new IPEndPoint(IPAddress.Any, l_port);
            }
            else
                throw new ArgumentException(String.Format("O endereço {0}:{1} não é válido", ip, r_port));
        }
    }
}
