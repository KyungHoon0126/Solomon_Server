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
        #region Anonymous Method
        public delegate Response<CommentsResult> CommentsBadResponse(string apiName, ConTextColor preColor, int status, ConTextColor setColor, string msg);
        CommentsBadResponse commentsBadResponse = delegate (string apiName, ConTextColor preColor, int status, ConTextColor setColor, string msg)
        {
            List<CommentModel> tempArr = new List<CommentModel>();
            ServiceManager.ShowRequestResult(apiName, preColor, status, setColor);
            return new Response<CommentsResult> { data = new CommentsResult { comments = tempArr }, message = msg, status = status };
        };
        #endregion

        #region API Method
        public async Task<Response<CommentsResult>> GetAllComments(string bulletin_idx)
        {
            string apiName = "GET ALL COMMENTS";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
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
                                ServiceManager.ShowRequestResult(apiName, ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                                return new Response<CommentsResult> { data = new CommentsResult { comments = comments }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                            }
                            else
                            {
                                return commentsBadResponse(apiName, ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE, "댓글이 존재하지 않습니다.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(apiName + " ERROR : " + e.Message);
                        return commentsBadResponse(apiName, ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE, ResponseMessage.INTERNAL_SERVER_ERROR);
                    }
                }
                else
                {
                    return commentsBadResponse(apiName, ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE, ResponseMessage.BAD_REQUEST);
                }
            }
            else
            {
                return commentsBadResponse(apiName, ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE, ResponseMessage.BAD_REQUEST);
            }
        }

        public async Task<Response> WriteComment(int bulletin_idx, string writer, string content)
        {
            string apiName = "WRITE COMMENT";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
            {
                var writeArgs = ComUtil.GetStringLengths(writer, content);

                if (bulletin_idx.ToString().Length > 0 && writer != null && content != null &&
                        writeArgs[0] > 0 && writeArgs[1] > 0)
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
                            if (await commentDBManager.InsertAsync(db, insertSql, model) == QueryExecutionResult.SUCCESS)
                            {
                                // await commentDBManager.IndexSortSqlAsync(db, ServiceManager.GetIndexSortSQL("comment_idx", "comment_tb"));
                                return ServiceManager.Result(apiName, ResponseType.CREATED);
                            }
                            else
                            {
                                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(apiName + " ERROR : " + e.Message);
                        return ServiceManager.Result(apiName, ResponseType.INTERNAL_SERVER_ERROR);
                    }
                }
                else
                {
                    return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
                }
            }
            else 
            {
                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
            }
        }

        public async Task<Response> DeleteComment(string writer, int comment_idx)
        {
            string apiName = "DELETE COMMENT";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
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
    comment_idx = '{comment_idx}'    
;";
                            if (await bulletinDBManager.DeleteAsync(db, deleteSql, model) == QueryExecutionResult.SUCCESS)
                            {
                                // await bulletinDBManager.IndexSortSqlAsync(db, ServiceManager.GetIndexSortSQL("comment_idx", "comment_tb"));
                                return ServiceManager.Result(apiName, ResponseType.OK);
                            }
                            else
                            {
                                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(apiName + " ERROR : " + e.Message);
                        return ServiceManager.Result(apiName, ResponseType.INTERNAL_SERVER_ERROR);
                    }
                }
                else
                {
                    return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
                }
            }
            else
            {
                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
            }
        }

        public async Task<Response> PutComment(string content, string writer, int comment_idx)
        {
            string apiName = "PUT COMMENT";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
            {
                var putArgs = ComUtil.GetStringLengths(content, writer);

                if (content != null && writer != null &&
                        putArgs[1] > 0 && putArgs[0] > 0 &&
                            comment_idx.ToString().Length > 0)
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
                            if (await commentDBManager.UpdateAsync(db, updateSql, model) == QueryExecutionResult.SUCCESS)
                            {
                                await commentDBManager.IndexSortSqlAsync(db, updateSql);
                                return ServiceManager.Result(apiName, ResponseType.OK);
                            }
                            else
                            {
                                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(apiName + " ERROR : " + e.Message);
                        return ServiceManager.Result(apiName, ResponseType.INTERNAL_SERVER_ERROR);
                    }
                }
                else
                {
                    return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
                }
            }
            else
            {
                return ServiceManager.Result(apiName, ResponseType.BAD_REQUEST);
            }
        }
        #endregion
        #endregion
    }
}
