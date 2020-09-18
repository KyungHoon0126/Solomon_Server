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
                                Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.OK);
                                return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = bulletins }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.NOT_FOUND);
                                return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                        Console.WriteLine("GET ALL BULLETINS ERROR : " + e.Message);
                        return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.BAD_REQUEST);
                    return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.BAD_REQUEST);
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
                                    Console.WriteLine("게시글 작성 : " + ResponseStatus.OK);
                                    return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                                }
                                else
                                {
                                    Console.WriteLine("게시글 작성 : " + ResponseStatus.BAD_REQUEST);
                                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("게시글 작성 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                            Console.WriteLine("WRITE BULLETIN ERROR : " + e.Message);
                            return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                        }
                    }
                    else
                    {
                        Console.WriteLine("게시글 작성 : " + ResponseStatus.BAD_REQUEST);
                        return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    Console.WriteLine("게시글 작성 : " + ResponseStatus.BAD_REQUEST);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                Console.WriteLine("게시글 작성 : " + ResponseStatus.BAD_REQUEST);
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
                                    Console.WriteLine("게시글 삭제 : " + ResponseStatus.OK);
                                    return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                                }
                                else
                                {
                                    Console.WriteLine("게시글 삭제 : " + ResponseStatus.BAD_REQUEST);
                                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("게시글 삭제 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                            Console.WriteLine("WRITE BULLETIN ERROR : " + e.Message);
                            return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                        }
                    }
                    else
                    {
                        Console.WriteLine("게시글 삭제 : " + ResponseStatus.BAD_REQUEST);
                        return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    Console.WriteLine("게시글 삭제 : " + ResponseStatus.BAD_REQUEST);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                Console.WriteLine("게시글 삭제 : " + ResponseStatus.BAD_REQUEST);
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
                                    Console.WriteLine("게시글 수정 : " + ResponseStatus.OK);
                                    return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                                }
                                else
                                {
                                    Console.WriteLine("게시글 수정 : " + ResponseStatus.BAD_REQUEST);
                                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("게시글 수정 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                            Console.WriteLine("PUT BULLETIN ERROR : " + e.Message);
                            return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                        }
                    }
                    else
                    {
                        Console.WriteLine("게시글 수정 : " + ResponseStatus.BAD_REQUEST);
                        return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    Console.WriteLine("게시글 수정 : " + ResponseStatus.BAD_REQUEST);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                Console.WriteLine("게시글 수정 : " + ResponseStatus.BAD_REQUEST);
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
                                Console.WriteLine("특정 게시글 조회 : " + ResponseStatus.OK);
                                return new Response<BulletinResult> { data = new BulletinResult { bulletin = bulletin }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                Console.WriteLine("특정 게시글 조회 : " + ResponseStatus.NOT_FOUND);
                                return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("GET SPECIFIC BULLETIN ERROR : " + e.Message);
                        return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else // 토큰이 유효하지 않음. => 검증 오류.
                {
                    Console.WriteLine("특정 게시글 조회 : " + ResponseStatus.BAD_REQUEST);
                    return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                Console.WriteLine("특정 게시글 조회 : " + ResponseStatus.BAD_REQUEST);
                return new Response<BulletinResult> { data = new BulletinResult { bulletin = tempModel }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
            #endregion
        }
    }
}
