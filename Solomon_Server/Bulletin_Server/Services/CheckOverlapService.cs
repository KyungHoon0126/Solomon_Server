using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.DataBase;
using Solomon_Server.Model.Member;
using Solomon_Server.Utils;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Solomon_Server.Services
{
    public partial class SolomonService : IService
    {
        DBManager<UserModel> checkOverlapDBManager = new DBManager<UserModel>();

        public async Task<Response> CheckEmailOverlap(string email)
        {
            if (email != null && email.Length > 0 && email.Trim().Length > 0)
            {
                try
                {
                    using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                    {
                        db.Open();

                        string selectSql = $@"
SELECT
    id
FROM
    member_tb
WHERE
    email = '{email}'
;";
                        var response = await checkOverlapDBManager.GetSingleDataAsync(db, selectSql, email);

                        if (response == null)
                        {
                            Console.WriteLine("이메일 중복 확인 : " + ResponseStatus.OK);
                            return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            Console.WriteLine("이메일 중복 확인 : " + ResponseStatus.BAD_REQUEST);
                            return new Response { message = "중복된 이메일.", status = ResponseStatus.BAD_REQUEST };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("이메일 중복 확인 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                    Console.WriteLine("CHECK EMAIL OVERLAP ERROR : " + e.Message);
                    return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // 검증 오류.
            {
                Console.WriteLine("이메일 중복 확인 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
    }
}
