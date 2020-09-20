using System.Data;

namespace Solomon_Server.DataBase
{
    public abstract class DBConnectionManager
    {
        public abstract IDbConnection GetConnection();
    }
}
