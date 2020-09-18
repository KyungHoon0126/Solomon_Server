using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.DataBase;
using Solomon_Server.Models.Bulletin;
using Solomon_Server.Results.CommentResults;
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
        DBManager<CommentModel> commentDBManager = new DBManager<CommentModel>();

        #region Bulletin_Comment_Service
        public async Task<Response<CommentsResult>> GetAllComments()
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
                        await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("comment_idx", "comment_tb"));
                        comments = await commentDBManager.GetListAsync(db, selectSql, "");

                        if (comments != null && comments.Count > 0)
                        {
                            Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.OK);
                            return new Response<CommentsResult> { data = new CommentsResult { comments = comments }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.NOT_FOUND);
                            return new Response<CommentsResult> { data = new CommentsResult { comments = comments }, message = "댓글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND }; ;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                    Console.WriteLine("GET ALL COMMENTS ERROR : " + e.Message);
                    return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("전체 댓글 조회 : " + ResponseStatus.BAD_REQUEST);
                return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
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
                                await commentDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("comment_idx", "comment_tb"));
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
                                await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("comment_idx", "comment_tb"));
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

        public async Task<Response<CommentsResult>> GetSpecificComments(string bulletin_idx)
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
                                return new Response<CommentsResult> { data = new CommentsResult { comments = comments }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                Console.WriteLine("특정 게시글 댓글 전체 조회 : " + ResponseStatus.NOT_FOUND);
                                return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = "댓글 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("특정 게시물 댓글 전체 조회 : " + ResponseStatus.INTERNAL_SERVER_ERROR);
                        Console.WriteLine("GET SPECIFIC BULLETIN COMMENTS ERROR : " + e.Message);
                        return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    Console.WriteLine("특정 게시물 댓글 전체 조회 : " + ResponseStatus.BAD_REQUEST);
                    return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
            {
                Console.WriteLine("특정 게시물 댓글 전체 조회 : " + ResponseStatus.BAD_REQUEST);
                return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}
