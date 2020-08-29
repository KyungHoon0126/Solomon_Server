using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin_Server.DataBase
{
    public class DBManager<T>
    {
        public List<T> GetList(IDbConnection conn, IDbTransaction tran, string search)
        {
            string sql = @"";
            return SqlMapper.Query<T>(conn, sql, new { search = search }).ToList();
        }

        public async Task<T> Insert(IDbConnection conn , IDbTransaction tran = null)
        {
            var resp = await SqlMapper.ExecuteAsync(conn, "insert query", this, tran);
            return resp;
        }

        public int Update(IDbConnection conn, IDbTransaction tran = null)
        {
            return SqlMapper.Execute(conn, "update query", this, tran);
        }

        public int Delete(IDbConnection conn, IDbTransaction tran = null)
        {
            return SqlMapper.Execute(conn, "delete query", this, tran);
        }
    }
}
