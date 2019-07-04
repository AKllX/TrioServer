using System;
using System.Collections.Generic;
using System.Text;

namespace TrioServer.Database.Interfaces
{
    public interface IQueryAdapter : IRegularQueryAdapter, IDisposable
    {
        long InsertQuery();
        void RunQuery();
    }
}
