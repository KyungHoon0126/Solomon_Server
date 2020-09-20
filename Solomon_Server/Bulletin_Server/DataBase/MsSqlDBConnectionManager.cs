using System;
using System.Data;

namespace Solomon_Server.DataBase
{
    public class MsSqlDBConnectionManager : DBConnectionManager
    {
        public override IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }
    }
}
