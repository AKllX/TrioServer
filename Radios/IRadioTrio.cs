using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TrioServer.Radios
{
    public interface IRadioTrio
    {
        int Id { get; }
        int SerialNumber { get; }
        string Desc { get; }
        RadioType Type { get; }
        IPEndPoint Address { get; }
        OperationMode OpMode { get; }
        //TODO : Sessions
        bool Initialized { get; }

        void Initialize();
        void Update();
        void Close();
    }
}
