using Bulletin_Server.Common;
using Bulletin_Server.DataBase;
using Bulletin_Server.JWT.Models;
using Bulletin_Server.JWT.Services;
using Bulletin_Server.Model.Member;
using Bulletin_Server.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bulletin_Server.Service
{
    public partial class BulletinService : IService
    {
        DBManager<UserModel> userDBManager = new DBManager<UserModel>();

        public Response<UserModel> SignUp(string id, string pw, string name, string email)
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
                        userDBManager.Insert(db, insertSql, model);
                        var resp = new Response<UserModel> { data = { }, message = "성공적으로 회원가입이 신청되었습니다.", status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("SIGNUP ERROR : " + e.Message);
                    return new Response<UserModel> { data = { }, message = "서버 오류", status = ResponseStatus.InternalServerError };
                }
            }
            else
            {
                var resp = new Response<UserModel> { data = { }, message = "검증 오류", status = ResponseStatus.BadRequest };
                return resp;
            }
        }

        public Response<UserModel> Login(string id, string pw)
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
                        var resp = userDBManager.GetSingleData(db, selectSql, id, null);
                        user.id = id;
                        user.name = resp.name;
                        user.email = resp.email;
                    }

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
                        var resp = new Response<UserModel> { data = user, message = "성공적으로 처리되었습니다.", status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("로그인 : " + ResponseStatus.InternalServerError);
                    Console.WriteLine("LOGIN ERROR : " + e.Message);
                    var resp = new Response<UserModel> { message = "서버 오류", status = ResponseStatus.InternalServerError };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("로그인 : " + ResponseStatus.BadRequest);
                var resp = new Response<UserModel> { message = "검증 오류", status = ResponseStatus.BadRequest };
                return resp;
            }
        }
    }
}