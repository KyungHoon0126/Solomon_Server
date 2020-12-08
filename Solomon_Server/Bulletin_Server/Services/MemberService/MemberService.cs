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
        public delegate Response<MemberResult> LoginBadResponse(ConTextColor preColor, int status, ConTextColor setColor, string msg);
        public delegate Response<UserModel> GetMemberInformationBadResponse(ConTextColor preColor, int status, ConTextColor setColor, string msg);

        #region API Method
        public async Task<Response> SignUp(string id, string pw, string name, string email, int birth_year, string gender)
        {
            string apiName = "SIGN UP";

            var signupArgs = ComUtil.GetStringLengths(id, pw, name, email, gender);

            if (id != null && pw != null && name != null && email != null && birth_year.ToString() != null && gender != null &&
                    signupArgs[0] > 0 && signupArgs[1] > 0 && signupArgs[2] > 0 && signupArgs[3] > 0 && birth_year.ToString().Length > 0 && signupArgs[4] > 0)
            {
                try
                {
                    using (IDbConnection db = GetConnection())
                    {
                        db.Open();

                        var model = new MemberModel();
                        model.id = id;
                        model.pw = pw;
                        model.name = name;
                        model.email = email;
                        model.birth_year = birth_year;
                        model.gender = gender;
                        

                        string insertSql = @"
INSERT INTO member_tb(
    id,
    pw,
    name,
    email,
    birth_year,
    gender
) 
VALUES(
    @id,
    @pw,
    @name,
    @email,
    @birth_year,
    @gender
);";

                        if (await userDBManager.InsertAsync(db, insertSql, model) == QueryExecutionResult.SUCCESS)
                        {
                            // await userDBManager.IndexSortSqlAsync(db, ServiceManager.GetIndexSortSQL("member_idx", "member_tb"));
                            ServiceManager.ShowRequestResult(apiName, ConTextColor.LIGHT_GREEN, ResponseStatus.CREATED, ConTextColor.WHITE);
                            return new Response { message = "성공적으로 회원가입이 되었습니다.", status = ResponseStatus.CREATED };
                        }
                        else
                        {
                            return ServiceManager.Result(apiName, ResponseType.INTERNAL_SERVER_ERROR);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(apiName + "ERROR : " + e.Message);
                    return ServiceManager.Result(apiName, ResponseType.INTERNAL_SERVER_ERROR);
                }
            }
            else 
            {
                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
            }
        }

        public async Task<Response<MemberResult>> Login(string id, string pw)
        {
            string apiName = "LOGIN";

            #region Anonymous Method
            LoginBadResponse memberBadResponse = delegate (ConTextColor preColor, int status, ConTextColor setColor, string msg)
            {
                UserModel tempModel = new UserModel();
                string tempToken = "";
                string tempRefreshToken = "";

                ServiceManager.ShowRequestResult(apiName, preColor, status, setColor);
                return new Response<MemberResult> { data = new MemberResult { token = tempToken, refreshToken = tempRefreshToken, member = tempModel }, status = status, message = msg };
            };
            #endregion

            var loginArgs = ComUtil.GetStringLengths(id, pw);

            if (id != null && pw != null && loginArgs[0] > 0 && loginArgs[1] > 0)
            {
                try
                {
                    UserModel user = new UserModel();

                    using (IDbConnection db = GetConnection())
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

                                ServiceManager.ShowRequestResult("LOGIN", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response<MemberResult> { data = new MemberResult { token = token, refreshToken = "", member = user }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                        }
                        else
                        {
                            return memberBadResponse(ConTextColor.RED, ResponseStatus.UNAUTHORIZED, ConTextColor.WHITE, ResponseMessage.UNAUTHORIZED);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(apiName + " ERROR : " + e.Message);
                    return memberBadResponse(ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE, ResponseMessage.INTERNAL_SERVER_ERROR);
                }
            }
            else 
            {
                return memberBadResponse(ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE, ResponseMessage.BAD_REQUEST);
            }
        }

        public async Task<Response<UserModel>> GetMemberInformation(string id)
        {
            string apiName = "GET MEMBER INFORMATION";

            #region Anonymous Method
            GetMemberInformationBadResponse getMemberInformationBadResponse = delegate (ConTextColor preColor, int status, ConTextColor setColor, string msg)
            {
                UserModel tempModel = new UserModel();
                ServiceManager.ShowRequestResult(apiName, ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                return new Response<UserModel> { data = tempModel, message = msg, status = status };
            };
            #endregion

            if (id != null && id.Trim().Length > 0)
            {
                try
                {
                    UserModel user = new UserModel();

                    using (IDbConnection db = GetConnection())
                    {
                        db.Open();

                        string selectSql = $@"
SELECT
    *
FROM
    member_tb
WHERE
    id = '{id}'
";
                        user = await userDBManager.GetSingleDataAsync(db, selectSql, id);

                        if (user != null)
                        {
                            ServiceManager.ShowRequestResult(apiName, ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                            return new Response<UserModel> { data = user, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            return getMemberInformationBadResponse(ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE, "회원 정보가 존재하지 않습니다.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(apiName + " ERROR : " + e.Message);
                    return getMemberInformationBadResponse(ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE, ResponseMessage.INTERNAL_SERVER_ERROR);
                }
            }
            else
            {
                return getMemberInformationBadResponse(ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE, ResponseMessage.BAD_REQUEST);
            }
        }
        #endregion
        #endregion
    }
}