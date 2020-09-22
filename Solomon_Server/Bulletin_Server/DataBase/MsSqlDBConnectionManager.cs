using System;
using System.Data;

namespace Solomon_Server.DataBase
{
    public class MsSqlDBConnectionManager : DBConnectionManager
    {
        // TODO : Image 내부 DB 저장.
        public override IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }
    }
}
