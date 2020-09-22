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
            string apiName = "CHECK EMAIL OVERLAP";

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

                        if (response is null)
                        {
                            return ServiceManager.Result(apiName, ResponseType.OK);
                        }
                        else
                        {
                            ServiceManager.ShowRequestResult(apiName, ConTextColor.RED, ResponseStatus.CONFLICT, ConTextColor.WHITE);
                            return new Response { message = "중복된 이메일.", status = ResponseStatus.CONFLICT };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(apiName + " ERROR : " + e.Message);
                    return ServiceManager.Result(apiName, ResponseType.INTERNAL_SERVER_ERROR);
                }
            }
            else
            {
                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
            }
        }
        #endregion
    }
}
