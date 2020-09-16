using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.DataBase;
using Solomon_Server.Models.Bulletin;
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
        DBManager<CommentModel> commentDBManager = new DBManager<CommentModel>();

        #region Bulletin_Service
        public async Task<Response<List<BulletinModel>>> GetAllBulletins()
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            List<BulletinModel> tempArr = new List<BulletinModel>();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
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
                        await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_tb"));
                        bulletins = await bulletinDBManager.GetListAsync(db, selectSql, "");

                        if (bulletins != null && bulletins.Count > 0)
                        {
                            Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.OK);
                            return new Response<List<BulletinModel>> { data = bulletins, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.NOT_FOUND);
                            return new Response<List<BulletinModel>> { data = tempArr, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                    Console.WriteLine("GET ALL BULLETINS ERROR : " + e.Message);
                    return new Response<List<BulletinModel>> { data = tempArr, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("전체 게시글 조회 : " + ResponseStatus.BAD_REQUEST);
                var resp = new Response<List<BulletinModel>> { data = tempArr, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                return resp;
            }
        }

        public async Task<Response> WriteBulletin(string title, string content, string writer)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
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
                                await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_tb"));
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
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("게시글 작성 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> DeleteBulletin(string writer, int bulletin_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
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
                                await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_tb"));
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
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("게시글 삭제 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> PutBulletin(string title, string content, string writer, int bulletin_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
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
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("게시글 수정 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response<BulletinModel>> GetSpecificBulletin(string bulletin_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            BulletinModel tempModel = new BulletinModel();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
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
                            return new Response<BulletinModel> { data = bulletin, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            Console.WriteLine("특정 게시글 조회 : " + ResponseStatus.NOT_FOUND);
                            return new Response<BulletinModel> { data = tempModel, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("GET SPECIFIC BULLETIN ERROR : " + e.Message);
                    return new Response<BulletinModel> { data = tempModel, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("특정 게시글 조회 : " + ResponseStatus.BAD_REQUEST);
                return new Response<BulletinModel> { data = tempModel, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion

        #region Comment_Service
        public async Task<Response<List<CommentModel>>> GetAllComments()
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            List<CommentModel> tempArr = new List<CommentModel>();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
            if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
            {
                try
                {
                    List<CommentModel> comments = new List<CommentModel>();
                    using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                    {
                        db.Open();

                        string selectSql = @"
SELECT
    *
FROM
    comment_tb
";
                        await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("comment_tb"));
                        comments = await commentDBManager.GetListAsync(db, selectSql, "");

                        if (comments != null && comments.Count > 0)
                        {
                            Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.OK);
                            return new Response<List<CommentModel>> { data = comments, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.NOT_FOUND);
                            return new Response<List<CommentModel>> { data = tempArr, message = "댓글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                    Console.WriteLine("GET ALL COMMENTS ERROR : " + e.Message);
                    return new Response<List<CommentModel>> { data = tempArr, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.BAD_REQUEST);
                return new Response<List<CommentModel>> { data = tempArr, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> WriteComment(int bulletin_idx, string writer, string content)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
            if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
            {
                if (bulletin_idx.ToString().Length > 0 && writer != null && content != null && 
                        writer.Trim().Length > 0 && content.Trim().Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                        {
                            db.Open();

                            var model = new CommentModel();
                            model.bulletin_idx = bulletin_idx;
                            model.writer = writer;
                            model.content = content;

                            string insertSql = @"
INSERT INTO comment_tb(
    bulletin_idx,
    writer,
    content
)
VALUES(
    @bulletin_idx,
    @writer,
    @content
);";
                            if (await commentDBManager.InsertAsync(db, insertSql, model) == 1)
                            {
                                await commentDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("comment_tb"));
                                Console.WriteLine("댓글 작성 : " + ResponseStatus.OK);
                                return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                Console.WriteLine("댓글 작성 : " + ResponseStatus.BAD_REQUEST);
                                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("댓글 작성 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                        Console.WriteLine("WRITE COMMENT ERROR : " + e.Message);
                        return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    Console.WriteLine("댓글 작성 : " + ResponseStatus.BAD_REQUEST);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("댓글 작성 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> DeleteComment(string writer, int comment_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
            if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
            {
                if (comment_idx.ToString() != null && comment_idx.ToString().Length > 0 && writer != null && writer.Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                        {
                            db.Open();

                            var model = new CommentModel();
                            model.writer = writer;
                            model.comment_idx = comment_idx;

                            string deleteSql = $@"
DELETE FROM
    comment_tb
WHERE
    writer = '{writer}'
AND
    idx = '{comment_idx}'    
;";
                            if (await bulletinDBManager.DeleteAsync(db, deleteSql, model) == 1)
                            {
                                await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("comment_tb"));
                                Console.WriteLine("댓글 삭제 : " + ResponseStatus.OK);
                                return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                Console.WriteLine("댓글 삭제 : " + ResponseStatus.BAD_REQUEST);
                                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("댓글 삭제 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                        Console.WriteLine("DELETE COMMENT ERROR : " + e.Message);
                        return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    Console.WriteLine("댓글 삭제 : " + ResponseStatus.BAD_REQUEST);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("댓글 삭제 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> PutComment(string content, string writer, int comment_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
            if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
            {
                if (content != null && content.Trim().Length > 0 && writer != null && 
                        writer.Trim().Length > 0 && comment_idx.ToString().Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                        {
                            db.Open();

                            var model = new CommentModel();
                            model.content = content;
                            model.writer = writer;
                            model.comment_idx = comment_idx;

                            string updateSql = $@"
UPDATE 
    comment_tb
SET
    content = '{content}'
WHERE
    writer = '{writer}'
AND
    idx = '{comment_idx}'
;";
                            if (await commentDBManager.UpdateAsync(db, updateSql, model) == 1)
                            {
                                await commentDBManager.IndexSortSqlAsync(db, updateSql);
                                Console.WriteLine("댓글 수정 : " + ResponseStatus.OK);
                                return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                Console.WriteLine("댓글 수정 : " + ResponseStatus.BAD_REQUEST);
                                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("댓글 수정 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                        Console.WriteLine("PUT COMMENT ERROR : " + e.Message);
                        return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    Console.WriteLine("댓글 수정 : " + ResponseStatus.BAD_REQUEST);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("댓글 수정 : " + ResponseStatus.BAD_REQUEST);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response<List<CommentModel>>> GetSpecificComments(string bulletin_idx)
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            List<CommentModel> tempArr = new List<CommentModel>();

            // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
            if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
            {
                if (bulletin_idx.Length > 0 && bulletin_idx != null)
                {
                    try
                    {
                        List<CommentModel> comments = new List<CommentModel>();
                        using (IDbConnection db = new MySqlConnection(ComDef.DATA_BASE_URL))
                        {
                            db.Open();

                            string selectSql = $@"
SELECT
    *
FROM
    comment_tb
WHERE
    bulletin_idx = '{bulletin_idx}'
";
                            comments = await commentDBManager.GetListAsync(db, selectSql, "");

                            if (comments != null && comments.Count > 0)
                            {
                                Console.WriteLine("특정 게시글 댓글 전체 조회 : " + ResponseStatus.OK);
                                return new Response<List<CommentModel>> { data = comments, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                Console.WriteLine("특정 게시글 댓글 전체 조회 : " + ResponseStatus.NOT_FOUND);
                                return new Response<List<CommentModel>> { data = tempArr, message = "댓글 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("특정 게시물 댓글 전체 조회 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                        Console.WriteLine("GET SPECIFIC BULLETIN COMMENTS ERROR : " + e.Message);
                        return new Response<List<CommentModel>> { data = tempArr, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    Console.WriteLine("특정 게시물 댓글 전체 조회 : " + ResponseStatus.BAD_REQUEST);
                    return new Response<List<CommentModel>> { data = tempArr, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("특정 게시물 댓글 전체 조회 : " + ResponseStatus.BAD_REQUEST);
                return new Response<List<CommentModel>> { data = tempArr, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}
