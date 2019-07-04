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

        public void LoadRadio(int sn)
        {
            DataRow data = null;
            IRadioTrio radio = null;

            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM radios WHERE serial_number = @sn LIMIT 1;");
                dbClient.AddParameter("sn", sn);
                data = dbClient.getRow();

                //TODO : Carregar Rádio
            }
        }
    }
}
