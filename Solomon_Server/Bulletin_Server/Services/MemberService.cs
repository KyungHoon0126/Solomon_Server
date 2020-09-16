using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.DataBase;
using Solomon_Server.JWT.Models;
using Solomon_Server.JWT.Services;
using Solomon_Server.Model.Member;
using Solomon_Server.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Solomon_Server.Services
{
    public partial class SolomonService : IService
    {
        DBManager<UserModel> userDBManager = new DBManager<UserModel>();

        #region Member_Service
        public async Task<Response> SignUp(string id, string pw, string name, string email)
        {
            if (id != null && pw != null && name != null && email != null && 
                    id.Trim().Length > 0 && pw.Trim().Length > 0 && name.Trim().Length > 0 && email.Trim().Length  > 0)
            {
                try
                {
                    using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                    {
                        db.Open();

                        var model = new MemberModel();
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
                        return new Response { message = "성공적으로 회원가입이 신청되었습니다.", status = ResponseStatus.OK };
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("회원 가입 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                    Console.WriteLine("SIGNUP ERROR : " + e.Message);
                    return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // 검증 오류.
            {
                Console.WriteLine("회원 가입 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response<UserModel>> Login(string id, string pw)
        {
            if (id != null && pw != null && id.Trim().Length > 0 && pw.Trim().Length > 0)
            {
                try
                {
                    UserModel user = new UserModel();

                    using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
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
                        
                        if (response != null) // 회원정보 조회 시, 값이 제대로 들어왔는지 확인.
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
                                return new Response<UserModel> { data = user, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                        }
                        else
                        {
                            Console.WriteLine("로그인 : " + ResponseStatus.UNAUTHORIZED);
                            return new Response<UserModel> { message = ResponseMessage.UNAUTHORIZED, status = ResponseStatus.UNAUTHORIZED };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("로그인 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                    Console.WriteLine("LOGIN ERROR : " + e.Message);
                    return new Response<UserModel> { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // 검증 오류.
            {
                Console.WriteLine("로그인 : " + ResponseStatus.BAD_REQUEST);
                return new Response<UserModel> { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}