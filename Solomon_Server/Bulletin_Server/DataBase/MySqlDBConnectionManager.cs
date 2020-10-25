using MySql.Data.MySqlClient;
using System.Data;

namespace Solomon_Server.DataBase
{
    // TODO : Repository Public 시, DB 정보 제외.
    public class MySqlDBConnectionManager : DBConnectionManager
    {
        private readonly string DATA_BASE_URL = "";

        public override IDbConnection GetConnection()
        {
            IDbConnection db = new MySqlConnection(DATA_BASE_URL);
            return db;
        }
    }
}
