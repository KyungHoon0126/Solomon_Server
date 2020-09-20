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

        #region CheckOverlap_Service
        public async Task<Response> CheckEmailOverlap(string email)
        {
            if (email != null && email.Length > 0 && email.Trim().Length > 0)
            {
                try
                {
                    using (IDbConnection db = GetConnection())
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
                            ComDef.ShowRequestResult("CHECK EMAIL OVERLAP", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                            return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            ComDef.ShowRequestResult("CHECK EMAIL OVERLAP", ConTextColor.RED, ResponseStatus.CONFLICT, ConTextColor.WHITE);
                            return new Response { message = "중복된 이메일.", status = ResponseStatus.CONFLICT };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("CHECK EMAIL OVERLAP ERROR : " + e.Message);
                    ComDef.ShowRequestResult("CHECK EMAIL OVERLAP", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // 검증 오류.
            {
                ComDef.ShowRequestResult("CHECK EMAIL OVERLAP", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}
