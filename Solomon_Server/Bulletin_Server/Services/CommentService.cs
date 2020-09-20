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
            List<CommentModel> tempArr = new List<CommentModel>();

            if (ComDef.jwtService.IsTokenValid(ComDef.GetHeaderValue(WebOperationContext.Current)))
            {
                try
                {
                    List<CommentModel> comments = new List<CommentModel>();
                    using (IDbConnection db = GetConnection())
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
                            ComDef.ShowRequestResult("GET ALL COMMETNS", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                            return new Response<CommentsResult> { data = new CommentsResult { comments = comments }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            ComDef.ShowRequestResult("GET ALL COMMETNS", ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE);
                            return new Response<CommentsResult> { data = new CommentsResult { comments = comments }, message = "댓글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND }; ;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("GET ALL COMMENTS ERROR : " + e.Message);
                    ComDef.ShowRequestResult("GET ALL COMMETNS", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                    return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                }
            }
            else // 토큰이 유효하지 않음. => 검증 오류.
            {
                ComDef.ShowRequestResult("GET ALL COMMETNS", ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE);
                return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> WriteComment(int bulletin_idx, string writer, string content)
        {
            if (ComDef.jwtService.IsTokenValid(ComDef.GetHeaderValue(WebOperationContext.Current)))
            {
                if (bulletin_idx.ToString().Length > 0 && writer != null && content != null &&
                        writer.Trim().Length > 0 && content.Trim().Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = GetConnection())
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
                                ComDef.ShowRequestResult("WRITE COMMENT", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                ComDef.ShowRequestResult("WRITE COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("WRITE COMMENT ERROR : " + e.Message);
                        ComDef.ShowRequestResult("WRITE COMMENT", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                        return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    ComDef.ShowRequestResult("WRITE COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // 토큰이 유효하지 않음. => 검증 오류.
            {
                ComDef.ShowRequestResult("WRITE COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> DeleteComment(string writer, int comment_idx)
        {
            if (ComDef.jwtService.IsTokenValid(ComDef.GetHeaderValue(WebOperationContext.Current)))
            {
                if (comment_idx.ToString() != null && comment_idx.ToString().Length > 0 && writer != null && writer.Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = GetConnection())
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
                                ComDef.ShowRequestResult("DELETE COMMENT", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                ComDef.ShowRequestResult("DELETE COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("DELETE COMMENT ERROR : " + e.Message);
                        ComDef.ShowRequestResult("DELETE COMMENT", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                        return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    ComDef.ShowRequestResult("DELETE COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // 토큰이 유효하지 않음. => 검증 오류.
            {
                ComDef.ShowRequestResult("DELETE COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response> PutComment(string content, string writer, int comment_idx)
        {
            if (ComDef.jwtService.IsTokenValid(ComDef.GetHeaderValue(WebOperationContext.Current)))
            {
                if (content != null && content.Trim().Length > 0 && writer != null &&
                        writer.Trim().Length > 0 && comment_idx.ToString().Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = GetConnection())
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
                                ComDef.ShowRequestResult("PUT COMMENT", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                ComDef.ShowRequestResult("PUT COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("PUT COMMENT ERROR : " + e.Message);
                        ComDef.ShowRequestResult("PUT COMMENT", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                        return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    ComDef.ShowRequestResult("PUT COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // 토큰이 유효하지 않음. => 검증 오류.
            {
                ComDef.ShowRequestResult("PUT COMMENT", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }

        public async Task<Response<CommentsResult>> GetSpecificComments(string bulletin_idx)
        {
            List<CommentModel> tempArr = new List<CommentModel>();

            if (ComDef.jwtService.IsTokenValid(ComDef.GetHeaderValue(WebOperationContext.Current)))
            {
                if (bulletin_idx.Length > 0 && bulletin_idx != null)
                {
                    try
                    {
                        List<CommentModel> comments = new List<CommentModel>();
                        using (IDbConnection db = GetConnection())
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
                                ComDef.ShowRequestResult("GET SPECIFIC BULLETIN COMMENTS", ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response<CommentsResult> { data = new CommentsResult { comments = comments }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                ComDef.ShowRequestResult("GET SPECIFIC BULLETIN COMMENTS", ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE);
                                return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = "댓글이 존재하지 않습니다.", status = ResponseStatus.NOT_FOUND };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("GET SPECIFIC BULLETIN COMMENTS ERROR : " + e.Message);
                        ComDef.ShowRequestResult("GET SPECIFIC BULLETIN COMMENTS", ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE);
                        return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                    }
                }
                else
                {
                    ComDef.ShowRequestResult("GET SPECIFIC BULLETIN COMMENTS", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                    return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else // 토큰이 유효하지 않음. => 검증 오류.
            {
                ComDef.ShowRequestResult("GET SPECIFIC BULLETIN COMMENTS", ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE);
                return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}
