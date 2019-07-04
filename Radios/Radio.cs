using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TrioServer.Radios
{
    public class Radio : IRadioTrio
    {
        
        public int Id { get; private set; }
        public int SerialNumber { get; private set; }
        public string Desc { get; private set; }
        public RadioType Type { get; private set; }
        public IPEndPoint Address { get; private set; }
        public bool Initialized { get; private set; }
        public OperationMode OpMode { get; private set; }

        public Radio(int id, int serialNum, string descp, RadioType type, String ip, int port, OperationMode opm)
        {
            Id = id;
            SerialNumber = serialNum;
            Desc = descp;
            Type = type;
            OpMode = opm;
            IPAddress myAddress;

            if (IPAddress.TryParse(ip, out myAddress))
                Address = new IPEndPoint(myAddress, port);
            else
                throw new ArgumentException(String.Format("O endereço {0}:{1} não é válido",ip,port));

        }
        public void Initialize()
        {
            //TODO: Inicializar Sessão
        }


        public void Update()
        {

        }

        public void Close()
        {

        }
    }
}
