﻿using System;
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
        private List<Channel> Channels { get; set; }

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
            int port;
            int poolingInterval;

            try
            {
                id = Convert.ToInt32(row["id"]);
                ip = Convert.ToString(row["ip_address"]);
                port = Convert.ToInt32(row["port"]);
                poolingInterval = Convert.ToInt32(row["pooling_t"]);

                Channels.Add(new Channel(id, ip, port, poolingInterval));
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