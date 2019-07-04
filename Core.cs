using System;
using System.Collections.Generic;
using System.Text;

using TrioServer.Database;
using TrioServer.Radios;

namespace TrioServer
{
    public static class Core
    {

        private static DatabaseManager m_DatabaseManager;
        private static RadioManager m_RadioManager;

        private static string host = "10.160.211.99";
        private static string db = "radios";

        public static void Initialize()
        {
            m_DatabaseManager = new DatabaseManager(ConnectionStringBuilder.BuildMySQL(db, host));
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return m_DatabaseManager;
        }
    }
}
