using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.DataBase;
using Solomon_Server.JWT.Models;
using Solomon_Server.JWT.Services;
using Solomon_Server.Model.Member;
using Solomon_Server.Results.MemberResult;
using Solomon_Server.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
                    id.Trim().Length > 0 && pw.Trim().Length > 0 && name.Trim().Length > 0 && email.Trim().Length > 0)
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

                        if (await userDBManager.InsertAsync(db, insertSql, model) == 1)
                        {
                            await userDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("member_idx", "member_tb"));
                            ComDef.ShowResponseResult("SIGN UP", ConTextColor.LIGHT_GREEN, ResponseStatus.CREATED, ConTextColor.WHITE); ;
                            return new Response { message = "성공적으로 회원가입이 되었습니다.", status = ResponseStatus.CREATED };
                        }
                        else
                        {
                            ComDef.ShowResponseResult("SIGN UP", ConTextColor.RED, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                            return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("SIGN UP ERROR : " + e.Message);
                    ComDef.ShowResponseResult("SIGN UP", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // 검증 오류.
            {
                ComDef.ShowResponseResult("SIGN UP", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response<MemberResult>> Login(string id, string pw)
        {
            UserModel tempModel = new UserModel();
            string tempToken = "";
            string tempRefreshToken = "";
            
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

                        if (response != null) 
                        {
                            user.id = id;
                            user.name = response.name;
                            user.email = response.email;

                            IAuthContainerModel model = JWTService.GetJWTContainerModel(user.name, user.email);
                            IAuthService authService = new JWTService(model.SecretKey);

                            string token = authService.GenerateToken(model);
                            // TODO : RefreshToken 발급. => 현재 임시로 빈 값 보냄

                            if (!authService.IsTokenValid(token))
                            {
                                throw new UnauthorizedAccessException();
                            }
                            else
                            {
                                List<Claim> claims = authService.GetTokenClaims(token).ToList();
                                Console.WriteLine("Login UserName : " + claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name)).Value);
                                Console.WriteLine("Login Eamil : " + claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email)).Value);

                                ComDef.ShowResponseResult("LOGIN", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response<MemberResult> { data = new MemberResult { token = token, refreshToken = tempRefreshToken, member = user }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                        }
                        else
                        {
                            Console.WriteLine("LOGIN", ConTextColor.RED, ResponseStatus.UNAUTHORIZED, ConTextColor.WHITE);
                            return new Response<MemberResult> { data = new MemberResult { token = tempToken, refreshToken = tempRefreshToken, member = tempModel }, message = ResponseMessage.UNAUTHORIZED, status = ResponseStatus.UNAUTHORIZED };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("LOGIN ERROR : " + e.Message);
                    ComDef.ShowResponseResult("LOGIN", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                    return new Response<MemberResult> { data = new MemberResult { token = tempToken, refreshToken = tempRefreshToken, member = tempModel }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // 검증 오류.
            {
                ComDef.ShowResponseResult("LOGIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response<MemberResult> { data = new MemberResult { token = tempToken, refreshToken = tempRefreshToken, member = tempModel }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response<UserModel>> GetMemberInformation(string member_idx)
        {
            UserModel tempModel = new UserModel();

            if (member_idx != null && member_idx.Trim().Length > 0)
            {
                try
                {
                    UserModel user = new UserModel();

                    using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                    {
                        db.Open();

                        string selectSql = $@"
SELECT
    *
FROM
    member_tb
WHERE
    member_idx = '{member_idx}'
";
                        user = await userDBManager.GetSingleDataAsync(db, selectSql, member_idx);

                        if (user != null)
                        {
                            ComDef.ShowResponseResult("GET SPECIFIC MEMBER INFORMATION", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                            return new Response<UserModel> { data = user, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            ComDef.ShowResponseResult("GET SPECIFIC MEMBER INFORMATION", ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE);
                            return new Response<UserModel> { data = tempModel, message = "회원 정보가 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("GET MEMBER INFORMATION ERROR : " + e.Message);
                    ComDef.ShowResponseResult("GET SPECIFIC MEMBER INFORMATION", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                    return new Response<UserModel> { data = tempModel, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else
            {
                ComDef.ShowResponseResult("GET SPECIFIC MEMBER INFORMATION", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response<UserModel> { data = tempModel, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}