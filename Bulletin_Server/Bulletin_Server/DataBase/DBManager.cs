using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Bulletin_Server.DataBase
{
    public class DBManager<T>
    {
        public List<T> GetList(IDbConnection conn, string sql, string search, IDbTransaction tran)
        {
            return SqlMapper.Query<T>(conn, sql, new { search = search }).ToList();
        }

        public int Insert(IDbConnection conn , string sql, object param, IDbTransaction tran = null)
        {
            return SqlMapper.Execute(conn, sql, param, tran);
        }

        public int Update(IDbConnection conn, string sql, object param, IDbTransaction tran = null)
        {
            return SqlMapper.Execute(conn, sql, param, tran);
        }

        public int Delete(IDbConnection conn, string sql, object param, IDbTransaction tran = null)
        {
            return SqlMapper.Execute(conn, sql, param, tran);
        }
    }
}
