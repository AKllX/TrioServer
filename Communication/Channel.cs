using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;

using TrioServer.Radios;

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
        public int MasterSerialNumber { get; private set; }

        public List<IRadioTrio> Radios { get; private set; }

        private int cycleCounter;

        public Channel(int id, string ip, int r_port, int l_port, int poolingInterval, int masterSn)
        {
            Id = id;
            RemotePort = r_port;
            LocalPort = l_port;
            PoolingInterval = poolingInterval;
            MasterSerialNumber = masterSn;

            IPAddress myAddress;
            if (IPAddress.TryParse(ip, out myAddress))
            {
                RemoteEndPoint = new IPEndPoint(myAddress, r_port);
                LocalEndPoint = new IPEndPoint(IPAddress.Any, l_port);
            }
            else
            {
                throw new ArgumentException(String.Format("O endereço {0}:{1} não é válido", ip, r_port));
            }

            Radios = Core.GetRadioManager().LoadChannel(this.Id);

            foreach(IRadioTrio r in Radios)
            {
                Console.WriteLine("Carregando Rádio: " + r.Desc);
            }
            cycleCounter = 0;
        }

        public byte[] SerialNumberParse()
        {
            string digits = MasterSerialNumber.ToString("D6");

            byte[] myData = Enumerable.Range(0, digits.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(digits.Substring(x, 2), 16))
                         .ToArray();

            return myData;
        }

        public IRadioTrio GetNextRadio()
        {
            IRadioTrio myRadio = Radios[cycleCounter];
            if(cycleCounter == Radios.Count -1)
            {
                cycleCounter = 0;
            }
            else
            {
                cycleCounter++;
            }
            return myRadio;
        }
    }
}
