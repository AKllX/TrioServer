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
        public bool Initialized { get; private set; }
        public OperationMode OpMode { get; private set; }
        public int ChannelId { get; private set; }

        public int MasterId { get; private set; }

        public Radio(int id, int serialNum, string descp, RadioType type, String ip, int port, OperationMode opm, int mId, int cId)
        {
            Id = id;
            MasterId = mId;
            SerialNumber = serialNum;
            Desc = descp;
            Type = type;
            OpMode = opm;
            ChannelId = cId;
        }
        public void Initialize()
        {
            
        }

        public void Update()
        {

        }

        public void Close()
        {

        }
    }
}
