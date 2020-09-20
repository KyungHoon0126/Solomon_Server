using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.DataBase;
using Solomon_Server.Models.Bulletin;
using Solomon_Server.Results;
using Solomon_Server.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Solomon_Server.Services
{
    public partial class SolomonService : IService
    {
        DBManager<BulletinModel> bulletinDBManager = new DBManager<BulletinModel>();

        #region Bulletin_Service
        public async Task<Response<BulletinsResult>> GetAllBulletins()
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            List<BulletinModel> tempArr = new List<BulletinModel>();

            if (ComDef.InspectionHeaderValue(webOperationContext))
            {
                string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

                if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
                {
                    try
                    {
                        List<BulletinModel> bulletins = new List<BulletinModel>();
                        using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                        {
                            db.Open();

                            string selectSql = @"
SELECT
    *
FROM
    bulletin_tb
";
                            await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_idx", "bulletin_tb"));
                            bulletins = await bulletinDBManager.GetListAsync(db, selectSql, "");

                            if (bulletins != null && bulletins.Count > 0)
                            {
                                ComDef.ShowResponseResult("GET ALL BULLETINS", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = bulletins }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                ComDef.ShowResponseResult("GET ALL BULLETINS", ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE);
                                return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("GET ALL BULLETINS ERROR : " + e.Message);
                        ComDef.ShowResponseResult("GET ALL BULLETINS", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                        return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    ComDef.ShowResponseResult("GET ALL BULLETINS", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                ComDef.ShowResponseResult("GET ALL BULLETINS", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> WriteBulletin(string title, string content, string writer)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;

            if (ComDef.InspectionHeaderValue(webOperationContext))
            {
                string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

                if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
                {
                    if (title != null && content != null && writer != null &&
                            title.Trim().Length > 0 && content.Trim().Length > 0 && writer.Trim().Length > 0)
                    {
                        try
                        {
                            using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                            {
                                db.Open();

                                var model = new BulletinModel();
                                model.title = title;
                                model.content = content;
                                model.writer = writer;

                                string insertSql = @"
INSERT INTO bulletin_tb(
    title,
    content,
    writer
)
VALUES(
    @title,
    @content,
    @writer
);";
                                if (await bulletinDBManager.InsertAsync(db, insertSql, model) == 1)
                                {
                                    await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_idx", "bulletin_tb"));
                                    ComDef.ShowResponseResult("WRITE BULLETIN", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                    return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                                }
                                else
                                {
                                    ComDef.ShowResponseResult("WRITE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("WRITE BULLETIN ERROR : " + e.Message);
                            ComDef.ShowResponseResult("WRITE BULLETIN", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                            return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                        }
                    }
                    else
                    {
                        ComDef.ShowResponseResult("WRITE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                        return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    ComDef.ShowResponseResult("WRITE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                ComDef.ShowResponseResult("WRITE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> DeleteBulletin(string writer, int bulletin_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;

            if (ComDef.InspectionHeaderValue(webOperationContext))
            {
                string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

                if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
                {
                    if (bulletin_idx.ToString() != null && bulletin_idx.ToString().Length > 0 && writer != null && writer.Length > 0)
                    {
                        try
                        {
                            using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                            {
                                db.Open();

                                var model = new BulletinModel();
                                model.bulletin_idx = bulletin_idx;
                                model.writer = writer;

                                string deleteSql = $@"
DELETE FROM
    bulletin_tb
WHERE
    writer = '{writer}'
AND
    idx = '{bulletin_idx}'    
;";
                                if (await bulletinDBManager.DeleteAsync(db, deleteSql, model) == 1)
                                {
                                    await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_idx", "bulletin_tb"));
                                    ComDef.ShowResponseResult("DELETE BULLETIN", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                    return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                                }
                                else
                                {
                                    ComDef.ShowResponseResult("DELETE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("DELETE BULLETIN ERROR : " + e.Message);
                            ComDef.ShowResponseResult("DELETE BULLETIN", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                            return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                        }
                    }
                    else
                    {
                        ComDef.ShowResponseResult("DELETE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                        return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    ComDef.ShowResponseResult("DELETE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                ComDef.ShowResponseResult("DELETE BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> PutBulletin(string title, string content, string writer, int bulletin_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
         
            if (ComDef.InspectionHeaderValue(webOperationContext))
            {
                string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

                if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
                {
                    if (title != null && title.Trim().Length > 0 && content != null && title.Trim().Length > 0 && bulletin_idx.ToString().Length > 0)
                    {
                        try
                        {
                            using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                            {
                                db.Open();

                                var model = new BulletinModel();
                                model.title = title;
                                model.content = content;
                                model.writer = writer;
                                model.bulletin_idx = bulletin_idx;

                                string updateSql = $@"
UPDATE 
    bulletin_tb
SET
    title = '{title}',
    content = '{content}'
WHERE
    writer = '{writer}'
AND
    idx = '{bulletin_idx}'
;";
                                if (await bulletinDBManager.UpdateAsync(db, updateSql, model) == 1)
                                {
                                    await bulletinDBManager.IndexSortSqlAsync(db, updateSql);
                                    ComDef.ShowResponseResult("PUT BULLETIN", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                    return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                                }
                                else
                                {
                                    ComDef.ShowResponseResult("PUT BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("PUT BULLETIN ERROR : " + e.Message);
                            ComDef.ShowResponseResult("PUT BULLETIN", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                            return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                        }
                    }
                    else
                    {
                        ComDef.ShowResponseResult("PUT BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                        return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    ComDef.ShowResponseResult("PUT BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                ComDef.ShowResponseResult("PUT BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }            
        }

        public async Task<Response<BulletinResult>> GetSpecificBulletin(string bulletin_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            BulletinModel tempModel = new BulletinModel();

            if (ComDef.InspectionHeaderValue(webOperationContext))
            {
                string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

                if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
                {
                    try
                    {
                        BulletinModel bulletin = new BulletinModel();

                        using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                        {
                            db.Open();

                            string selectSql = $@"
SELECT
    *
FROM
    bulletin_tb
WHERE
    idx = '{bulletin_idx}'
";
                            bulletin = await bulletinDBManager.GetSingleDataAsync(db, selectSql, "");

                            if (bulletin != null)
                            {
                                ComDef.ShowResponseResult("GET SPECIFIC BULLETIN", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response<BulletinResult> { data = new BulletinResult { bulletin = bulletin }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                ComDef.ShowResponseResult("GET SPECIFIC BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                                return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("GET SPECIFIC BULLETIN ERROR : " + e.Message);
                        ComDef.ShowResponseResult("GET SPECIFIC BULLETIN", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                        return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    ComDef.ShowResponseResult("GET SPECIFIC BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                ComDef.ShowResponseResult("GET SPECIFIC BULLETIN", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}
