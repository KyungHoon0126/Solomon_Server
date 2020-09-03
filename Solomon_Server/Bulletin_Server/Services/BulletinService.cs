using Bulletin_Server.Common;
using Bulletin_Server.DataBase;
using Bulletin_Server.Utils;
using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.Models.Bulletin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bulletin_Server.Service
{
    public partial class SolomonService : IService
    {
        DBManager<BulletinModel> bulletinDBManager = new DBManager<BulletinModel>();

        public async Task<Response<List<BulletinModel>>> GetAllBulletins()
        {
            List<BulletinModel> tempArr = new List<BulletinModel>();

            try
            {
                List<BulletinModel> bulletins = new List<BulletinModel>();
                using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                {
                    db.Open();

                    string selectSql = @"
SELECT
    *
FROM
    bulletin_tb
";

                    bulletins = await bulletinDBManager.GetListAsync(db, selectSql, "");
                    
                    if(bulletins != null && bulletins.Count > 0)
                    {
                        Console.WriteLine("게시글 전체 조회 : " + ResponseStatus.OK);
                        var response = new Response<List<BulletinModel>> { data = bulletins, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        return response;
                    }
                    else
                    {
                        Console.WriteLine("게시글 전체 조회 : " + ResponseStatus.NotFound);
                        var response = new Response<List<BulletinModel>> { data = tempArr, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NotFound };
                        return response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GET ALL BULLETINS ERROR : " + e.Message);
            }

            var resp = new Response<List<BulletinModel>> { data = tempArr, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
            return resp;
        }

        public Task<Response> WriteBulletin(string title, string content, string writer)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteBulletin(int idx)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateBulletin(string title, string content)
        {
            throw new NotImplementedException();
        }
    }
}
