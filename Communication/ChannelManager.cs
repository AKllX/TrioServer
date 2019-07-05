using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

using TrioServer.Database;
using TrioServer.Database.Interfaces;

namespace TrioServer.Communication
{
    public class ChannelManager
    {
        public List<Channel> Channels { get; private set; }

        public ChannelManager()
        {
            Channels = new List<Channel>();
        }

        public Channel GetChannel(int channelId)
        {
            if (Channels.Any(x => x.Id == channelId))
                return Channels.Single(x => x.Id == channelId);
            else
                throw new ArgumentException("Não é possível referenciar um canal não carregado:" + channelId);
        }

        public void LoadChannel(DataRow row)
        {
            int id;
            string ip;
            int r_port;
            int l_port;
            int poolingInterval;
            int msn;

            try
            {
                id = Convert.ToInt32(row["id"]);
                ip = Convert.ToString(row["ip_address"]);
                r_port = Convert.ToInt32(row["port_remote"]);
                l_port = Convert.ToInt32(row["port_local"]);
                poolingInterval = Convert.ToInt32(row["pooling_t"]);
                msn = Convert.ToInt32(row["master_serial"]);

                Channels.Add(new Channel(id, ip, r_port,l_port, poolingInterval,msn));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void LoadAllChannels()
        {
            DataTable dt = null;
            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM channels");
                dt = dbClient.getTable();
            }

            foreach(DataRow dr in dt.Rows)
            {
                LoadChannel(dr);
            }
        }
    }
}
