using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace TrioServer.Database.Interfaces
{
    public interface IDatabaseClient : IDisposable
    {
        void Connect();
        void Disconnect();
        IQueryAdapter GetQueryReactor();
        MySqlCommand CreateNewCommand();
        void ReportDone();
    }
}
