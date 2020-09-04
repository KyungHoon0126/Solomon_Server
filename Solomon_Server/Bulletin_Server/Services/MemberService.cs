using Bulletin_Server.Common;
using Bulletin_Server.DataBase;
using Bulletin_Server.JWT.Models;
using Bulletin_Server.JWT.Services;
using Bulletin_Server.Model.Member;
using Bulletin_Server.Utils;
using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bulletin_Server.Service
{
    public partial class SolomonService : IService
    {
        DBManager<UserModel> userDBManager = new DBManager<UserModel>();

        #region Member_Service
        public async Task<Response> SignUp(string id, string pw, string name, string email)
        {
            if (id != null && pw != null && name != null && email != null)
            {
                try
                {
                    using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                    {
                        db.Open();

                        var model = new Member();
                        model.id = id;
                        model.pw = pw;
                        model.name = name;
                        model.email = email;

                        string insertSql = @"
INSERT INTO member_tb(
    id,
    pw,
    name,
    email
) 
VALUES(
    @id,
    @pw,
    @name,
    @email
);";

                        await userDBManager.InsertAsync(db, insertSql, model);
                        await userDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("member_tb"));
                        Console.WriteLine("회원 가입 : " + ResponseStatus.OK);
                        var resp = new Response { message = "성공적으로 회원가입이 신청되었습니다.", status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("회원 가입 : " + ResponseStatus.InternalServerError);
                    Console.WriteLine("SIGNUP ERROR : " + e.Message);
                    var resp = new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("회원 가입 : " + ResponseStatus.BadRequest);
                var resp = new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BadRequest };
                return resp;
            }
        }

        public async Task<Response<UserModel>> Login(string id, string pw)
        {
            if (id != null && id.Trim().Length > 0 && pw != null && pw.Trim().Length > 0)
            {
                try
                {
                    UserModel user = new UserModel();

                    using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                    {
                        db.Open();

                        string selectSql = $@"
SELECT
    name, 
    email
FROM 
    member_tb
WHERE
    id = '{id}'
AND
    pw = '{pw}'
;";
                        var response = await userDBManager.GetSingleDataAsync(db, selectSql, id);
                        
                        if (response != null)
                        {
                            user.id = id;
                            user.name = response.name;
                            user.email = response.email;

                            IAuthContainerModel model = JWTService.GetJWTContainerModel(user.name, user.email);
                            IAuthService authService = new JWTService(model.SecretKey);

                            string token = authService.GenerateToken(model);
                            user.token = token;

                            if (!authService.IsTokenValid(token))
                            {
                                throw new UnauthorizedAccessException();
                            }
                            else
                            {
                                List<Claim> claims = authService.GetTokenClaims(token).ToList();
                                Console.WriteLine("Login UserName : " + claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name)).Value);
                                Console.WriteLine("Login Eamil : " + claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email)).Value);

                                Console.WriteLine("로그인 : " + ResponseStatus.OK);
                                var resp = new Response<UserModel> { data = user, message = ResponseMessage.OK, status = ResponseStatus.OK };
                                return resp;
                            }
                        }
                        else
                        {
                            Console.WriteLine("로그인 : " + ResponseStatus.Unauthorized);
                            var resp = new Response<UserModel> { message = ResponseMessage.UNAUTHORIZED, status = ResponseStatus.Unauthorized };
                            return resp;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("로그인 : " + ResponseStatus.InternalServerError);
                    Console.WriteLine("LOGIN ERROR : " + e.Message);
                    var resp = new Response<UserModel> { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("로그인 : " + ResponseStatus.BadRequest);
                var resp = new Response<UserModel> { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BadRequest };
                return resp;
            }
        }
        #endregion
    }
}