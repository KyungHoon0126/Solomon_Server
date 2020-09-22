using Solomon_Server.DataBase;
using Solomon_Server.Model.Member;
using Solomon_Server.Utils;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Solomon_Server.Services
{
    public partial class SolomonService : IService
    {
        DBManager<UserModel> checkOverlapDBManager = new DBManager<UserModel>();


        #region CheckOverlap_Service
        public delegate Response CheckOverLapResponse(string apiName, ConTextColor preColor, int status, ConTextColor setColor, string msg);

        #region Anonymous Method
        CheckOverLapResponse checkOverLapResponse = delegate (string apiName, ConTextColor preColor, int status, ConTextColor setColor, string msg)
        {
            ServiceManager.ShowRequestResult(apiName, preColor, status, setColor);
            return new Response { message = msg, status = status };
        };
        #endregion

        #region API Method
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
                            return checkOverLapResponse(apiName, ConTextColor.RED, ResponseStatus.CONFLICT, ConTextColor.WHITE, "중복된 이메일.");
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

        public async Task<Response> CheckIdOverlap(string id)
        {
            string apiName = "CheckIdOverlap";

            if (id != null && id.Trim().Length > 0)
            {
                try
                {
                    using (IDbConnection db = GetConnection())
                    {
                        string selectSql = $@"
SELECT
    email
FROM
    member_tb
WHERE
    id = '{id}'
;";
                        var resposne = await checkOverlapDBManager.GetSingleDataAsync(db, selectSql, id);

                        if (resposne is null)
                        {
                            return ServiceManager.Result(apiName, ResponseType.OK);
                        }
                        else
                        {
                            return checkOverLapResponse(apiName, ConTextColor.RED, ResponseStatus.CONFLICT, ConTextColor.WHITE, "중복된 아이디.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(apiName + " ERROR : " + e.Message);
                    return ServiceManager.Result(apiName, ResponseType.INTERNAL_SERVER_ERROR);
                }
            }
            else
            {
                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
            }
        }
        #endregion
        #endregion
    }
}
