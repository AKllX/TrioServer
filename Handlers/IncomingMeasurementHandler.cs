using System;
using System.Collections.Generic;
using System.Text;

using TrioServer.Communication;

namespace TrioServer.Handlers
{
    public static class IncomingMeasurementHandler
    {
        public static void Deserialize(RadioMessage message, byte counterCheck)
        {
            short temperature;
            short rx;
            short tx;
            short freqErr;
            short voltage;
            short reversePower;

            double d_Temperature;
            double d_Rx;
            double d_Tx;
            double d_FreqErr;
            double d_Voltage;
            double d_ReverseTx;
            double vswr;
            byte errors;

            
            if (counterCheck == 0x67)
            {
                message.GetByte();
                message.GetByte();

                errors = message.GetByte();

                if (errors == 0x02)
                {
                    Console.WriteLine("Erro de cálculo de diagnóstico");
                    return;
                }

                temperature = message.GetSignedInt16();
                rx = message.GetSignedInt16();
                tx = message.GetSignedInt16();
                freqErr = message.GetSignedInt16();
                voltage = message.GetSignedInt16();
                reversePower = message.GetSignedInt16();

                d_Temperature = ((double)temperature / 10);
                d_Rx = ((double)rx / 10);
                d_Tx = ((double)tx / 10);
                d_FreqErr = ((double)freqErr);
                d_Voltage = ((double)voltage / 10);
                d_ReverseTx = ((double)reversePower / 10);

                vswr = (1 + (Math.Sqrt((Math.Pow(10, (d_ReverseTx / 10)) / 1000) / (Math.Pow(10, (d_Tx / 10)) / 1000)))) / (1 - (Math.Sqrt((Math.Pow(10, (d_ReverseTx / 10)) / 1000) / (Math.Pow(10, (d_Tx / 10)) / 1000))));

                Core.GetRadioManager().SaveMeasurement(message.RadioSerialNumber, d_Temperature, d_Voltage, d_FreqErr, d_Rx, d_Tx, vswr);
            }

            else if(counterCheck == 0x72)
            {
                byte counter = message.GetByte();
                message.GetSignedInt16();
                errors = message.GetByte();

                if(errors == 0x02)
                {
                    Console.WriteLine("Erro de cálculo de diagnóstico");
                    return;
                }

                temperature = message.GetSignedInt16();
                rx = message.GetSignedInt16();
                tx = message.GetSignedInt16();
                freqErr = message.GetSignedInt16();
                voltage = message.GetSignedInt16();
                reversePower = message.GetSignedInt16();

                d_Temperature = ((double)temperature/10);
                d_Rx = ((double)rx/10);
                d_Tx = ((double)tx/10);
                d_FreqErr = ((double)freqErr);
                d_Voltage = ((double)voltage/10);
                d_ReverseTx = ((double)reversePower/10);

                vswr = (1 + (Math.Sqrt((Math.Pow(10, (d_ReverseTx / 10)) / 1000) / (Math.Pow(10, (d_Tx / 10)) / 1000)))) / (1 - (Math.Sqrt((Math.Pow(10, (d_ReverseTx / 10)) / 1000) / (Math.Pow(10, (d_Tx / 10)) / 1000))));

                Core.GetRadioManager().SaveMeasurement(message.RadioSerialNumber, d_Temperature, d_Voltage, d_FreqErr, d_Rx, d_Tx, vswr);
            }
        }
    }
}
