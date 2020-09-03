using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bulletin_Server.DataBase
{
    public class DBManager<T>
    {
        public List<T> GetList(IDbConnection conn, string sql, string search, IDbTransaction tran)
        {
            return SqlMapper.Query<T>(conn, sql, new { search = search }).ToList();
        }

        public async Task<T> GetSingleDataAsync(IDbConnection conn, string sql, string search, IDbTransaction tran)
        {
            return await SqlMapper.QueryFirstOrDefaultAsync<T>(conn, sql, new { search = search });
        }

        public async Task<int> InsertAsync(IDbConnection conn, string sql, object param, IDbTransaction tran = null)
        {
            return await SqlMapper.ExecuteAsync(conn, sql, param, tran);
        }

        public async Task<int> UpdateAsync(IDbConnection conn, string sql, object param, IDbTransaction tran = null)
        {
            return await SqlMapper.ExecuteAsync(conn, sql, param, tran);
        }

        public async Task<int> DeleteAsync(IDbConnection conn, string sql, object param, IDbTransaction tran = null)
        {
            return await SqlMapper.ExecuteAsync(conn, sql, param, tran);
        }
    }
}
