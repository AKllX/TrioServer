using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using TrioServer.Database.Interfaces;

namespace TrioServer.Radios
{
    public class RadioManager
    {
        public List<IRadioTrio> LoadedRadios { get; private set; }

        public RadioManager()
        {
            this.LoadedRadios = new List<IRadioTrio>();
        }

        public IRadioTrio GetRadioForId(int id)
        {
            IRadioTrio myRadio = null;
            if (!LoadedRadios.Exists(x => x.Id == id))
            {
                //TODO: Tentar carregar o rádio
            }
            else
            {
                myRadio = LoadedRadios.Find(x => x.Id == id);
            }

            return myRadio;
        }
        public IRadioTrio GetRadioFromSN(int sn)
        {
            IRadioTrio wantedRadio = LoadedRadios.Find(x => x.SerialNumber == sn);
            if (!wantedRadio.Equals(null))
            {
                return wantedRadio;
            }
            return null;
        }

        public IRadioTrio LoadRadio(DataRow data)
        {
            IRadioTrio radio = null;
            RadioType radioType;
            OperationMode opMode;

            if (data != null)
            {
                int sn = Convert.ToInt32(data["serial_number"]);

                if (!LoadedRadios.Exists(x => x.SerialNumber == sn))
                {
                    if (data != null)
                    {
                        string type = Convert.ToString(data["radiotype"]);

                        if (type == "qr")
                        {
                            radioType = RadioType.QR;
                        }
                        else if (type == "qb")
                        {
                            radioType = RadioType.QB;
                        }
                        else if (type == "e")
                        {
                            radioType = RadioType.E;
                        }
                        else
                        {
                            radioType = RadioType.MR;
                        }

                        if (Convert.ToChar(data["opmod"]) == 'm')
                        {
                            opMode = OperationMode.Mode_M;
                        }

                        else
                        {
                            opMode = OperationMode.Mode_Q;
                        }

                        radio = new Radio(
                            Convert.ToInt32(data["id"]),
                            Convert.ToInt32(data["serial_number"]),
                            Convert.ToString(data["desc"]),
                            radioType,
                            opMode,
                            Convert.ToInt32(data["master_id"]),
                            Convert.ToInt32(data["channel_id"]));

                        LoadedRadios.Add(radio);
                    }
                }

                else
                {
                    radio = LoadedRadios.Find(x => x.SerialNumber == sn);
                }
            }

            return radio;
        }
        public IRadioTrio LoadRadio(int sn)
        {
            DataRow data = null;
            IRadioTrio radio = null;

            if (!LoadedRadios.Exists(x => x.SerialNumber == sn))
            {

                using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM radios WHERE serial_number = @sn LIMIT 1;");
                    dbClient.AddParameter("sn", sn);
                    data = dbClient.getRow();

                    radio = LoadRadio(data);
                }
            }
            else
            {
                radio = LoadedRadios.Find(x => x.SerialNumber == sn);
            }

            return radio;
        }

        public List<IRadioTrio> LoadChannel(int channelId)
        {
            DataTable dataTable = null;
            List<IRadioTrio> radios = new List<IRadioTrio>();

            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM radios WHERE channel_id = @cId;");
                dbClient.AddParameter("cId", channelId);
                dataTable = dbClient.getTable();

                foreach (DataRow dr in dataTable.Rows)
                {
                    radios.Add(LoadRadio(dr));
                    //Adicionado pela Laila
                    this.SetCommStatus(LoadRadio(dr).SerialNumber, 0);                }
            }

            return radios;
        }

        //Só salva se tiver recebido resposta do rádio
        public void SaveMeasurement(int radio_sn,double temperature, double volts, double freqerr, double rxsig, double txpwr, double vswr)
        {
            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO measurements (radio_sn,temperature,volts,freqerr,rxsig,txpwr,vswr) VALUES " +
                    "(@rsn,@tempe,@v,@freq,@rx,@tx,@vswr);");
                dbClient.AddParameter("rsn", radio_sn);
                dbClient.AddParameter("tempe", temperature);
                dbClient.AddParameter("v", volts);
                dbClient.AddParameter("freq", freqerr);
                dbClient.AddParameter("rx", rxsig);
                dbClient.AddParameter("tx", txpwr);
                dbClient.AddParameter("vswr", vswr);
                dbClient.RunQuery();
            }
        }

        //  Adicionado pela Laila
        public void SetCommStatus(int radio_sn, int comm_status)
        {
            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery(String.Concat("UPDATE radios SET comm_status = ",comm_status.ToString()," WHERE radios.serial_number = ", radio_sn.ToString()));
            }
        }
    }
}
