using System;
using System.Collections.Generic;
using System.Text;

namespace TrioServer.Radios
{
    public class Measurement
    {
        public int Id { get; private set; }
        public int RadioId { get; private set; }
        public double Temperature { get; private set; }
        public double Voltage { get; private set; }
        public double FrequencyError { get; private set; }
        public double RxSignal { get; private set; }
        public double TxPower { get; private set; }
        public double VSWR { get; private set; }
        public DateTime Time { get; private set; }

        public Measurement(int id, int radio_id, double temperature, double volts, double freqerr, double rxsig, double txpwr, double vswr, DateTime mTime)
        {
            Id = id;
            RadioId = radio_id;
            Temperature = temperature;
            Voltage = volts;
            FrequencyError = freqerr;
            RxSignal = rxsig;
            TxPower = txpwr;
            VSWR = vswr;
            Time = mTime;
        }
    }
}
