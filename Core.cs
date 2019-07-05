using System;
using System.Collections.Generic;
using System.Text;

using TrioServer.Database;
using TrioServer.Radios;
using TrioServer.Sessions;
using TrioServer.Communication;

namespace TrioServer
{
    public static class Core
    {

        private static DatabaseManager m_DatabaseManager;
        private static RadioManager m_RadioManager;
        private static ChannelManager m_ChannelManager;

        private static string host = "10.160.211.99";
        private static string db = "radios";

        public static void Initialize()
        {
            m_DatabaseManager = new DatabaseManager(ConnectionStringBuilder.BuildMySQL(db, host));
            m_ChannelManager = new ChannelManager();
            m_RadioManager = new RadioManager();
            SessionManager.Initialize();
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return m_DatabaseManager;
        }

        public static RadioManager GetRadioManager()
        {
            return m_RadioManager;
        }

    }
}
