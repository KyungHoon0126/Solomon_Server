using Bulletin_Server.Common;
using Bulletin_Server.DataBase;
using Bulletin_Server.Utils;
using MySql.Data.MySqlClient;
using Solomon_Server.Common;
using Solomon_Server.Models.Bulletin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bulletin_Server.Service
{
    public partial class SolomonService : IService
    {
        DBManager<BulletinModel> bulletinDBManager = new DBManager<BulletinModel>();
        DBManager<CommentModel> commentDBManager = new DBManager<CommentModel>();

        #region Bulletin
        public async Task<Response<List<BulletinModel>>> GetAllBulletins()
        {
            List<BulletinModel> tempArr = new List<BulletinModel>();

            try
            {
                List<BulletinModel> bulletins = new List<BulletinModel>();
                using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                {
                    db.Open();

                    string selectSql = @"
SELECT
    *
FROM
    bulletin_tb
";

                    bulletins = await bulletinDBManager.GetListAsync(db, selectSql, "");
                    
                    if(bulletins != null && bulletins.Count > 0)
                    {
                        Console.WriteLine("게시글 전체 조회 : " + ResponseStatus.OK);
                        var response = new Response<List<BulletinModel>> { data = bulletins, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        return response;
                    }
                    else
                    {
                        Console.WriteLine("게시글 전체 조회 : " + ResponseStatus.NotFound);
                        var response = new Response<List<BulletinModel>> { data = tempArr, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NotFound };
                        return response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GET ALL BULLETINS ERROR : " + e.Message);
            }

            var resp = new Response<List<BulletinModel>> { data = tempArr, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
            return resp;
        }

        public async Task<Response> WriteBulletin(string title, string content, string writer)
        {
            if (title != null && content != null && writer != null && title.Trim().Length > 0 && content.Trim().Length > 0 && writer.Trim().Length > 0)
            {
                try
                {
                    using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
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
                        await bulletinDBManager.InsertAsync(db, insertSql, model);
                        await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_tb"));
                        Console.WriteLine("게시글 작성 : " + ResponseStatus.OK);
                        var resp = new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("게시글 작성 : " + ResponseStatus.InternalServerError);
                    Console.WriteLine("Write Bulletin ERROR : " + e.Message);
                    var resp = new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("게시글 작성 : " + ResponseStatus.BadRequest);
                var resp = new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BadRequest };
                return resp;
            }
        }

        public async Task<Response> DeleteBulletin(int idx)
        {
            if (idx.ToString() != null && idx.ToString().Length > 0)
            {
                try
                {
                    using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                    {
                        db.Open();

                        var model = new BulletinModel();
                        model.idx = idx;

                        string deleteSql = $@"
DELETE FROM
    bulletin_tb
WHERE
    idx = '{idx}'
;";
                        await bulletinDBManager.DeleteAsync(db, deleteSql, model);
                        await bulletinDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("bulletin_tb"));
                        Console.WriteLine("게시글 삭제 : " + ResponseStatus.OK);
                        var resp = new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("게시글 삭제 : " + ResponseStatus.InternalServerError);
                    Console.WriteLine("Write Bulletin ERROR : " + e.Message);
                    var resp = new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("게시글 삭제 : " + ResponseStatus.BadRequest);
                var resp = new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BadRequest };
                return resp;
            }
        }

        public async Task<Response> PutBulletin(string title, string content, int idx)
        {
            if (title != null && title.Trim().Length > 0 && content != null && title.Trim().Length > 0 && idx.ToString().Length > 0)
            {
                try
                {
                    using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                    {
                        db.Open();

                        var model = new BulletinModel();
                        model.title = title;
                        model.content = content;
                        model.idx = idx;

                        string updateSql = $@"
UPDATE 
    bulletin_tb
SET
    title = '{title}',
    content = '{content}'
WHERE
    idx = '{idx}'
;";
                        await bulletinDBManager.UpdateAsync(db, updateSql, model);
                        await bulletinDBManager.IndexSortSqlAsync(db, updateSql);
                        Console.WriteLine("게시글 수정 : " + ResponseStatus.OK);
                        var resp = new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("게시글 수정 : " + ResponseStatus.InternalServerError);
                    Console.WriteLine("Put Bulletin ERROR : " + e.Message);
                    var resp = new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("게시글 수정 : " + ResponseStatus.BadRequest);
                var resp = new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BadRequest };
                return resp;
            }
        }
        #endregion

        #region Comment
        public async Task<Response<List<CommentModel>>> GetAllComments()
        {
            List<CommentModel> tempArr = new List<CommentModel>();

            try
            {
                List<CommentModel> comments = new List<CommentModel>();
                using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                {
                    db.Open();

                    string selectSql = @"
SELECT
    *
FROM
    comment_tb
";

                    comments = await commentDBManager.GetListAsync(db, selectSql, "");

                    if (comments != null && comments.Count > 0)
                    {
                        Console.WriteLine("게시글 전체 조회 : " + ResponseStatus.OK);
                        var response = new Response<List<CommentModel>> { data = comments, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        return response;
                    }
                    else
                    {
                        Console.WriteLine("게시글 전체 조회 : " + ResponseStatus.NotFound);
                        var response = new Response<List<CommentModel>> { data = tempArr, message = "게시글이 존재하지 않습니다.", status = ResponseStatus.NotFound };
                        return response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GET ALL COMMENTS ERROR : " + e.Message);
            }

            var resp = new Response<List<CommentModel>> { data = tempArr, message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
            return resp;
        }   

        public async Task<Response> WriteComment(string writer, string content)
        {
            if (writer != null && content != null && writer.Trim().Length > 0 && content.Trim().Length > 0)
            {
                try
                {
                    using (IDbConnection db = new MySqlConnection(ComDef.DATABASE_URL))
                    {
                        db.Open();

                        var model = new CommentModel();
                        model.writer = writer;
                        model.content = content;

                        string insertSql = @"
INSERT INTO comment_tb(
    writer,
    content
)
VALUES(
    @writer,
    @content
);";
                        await commentDBManager.InsertAsync(db, insertSql, model);
                        await commentDBManager.IndexSortSqlAsync(db, ComDef.GetIndexSortSQL("comment_tb"));
                        Console.WriteLine("댓글 작성 : " + ResponseStatus.OK);
                        var resp = new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("댓글 작성 : " + ResponseStatus.InternalServerError);
                    Console.WriteLine("Write Comment ERROR : " + e.Message);
                    var resp = new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.InternalServerError };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("댓글 작성 : " + ResponseStatus.BadRequest);
                var resp = new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BadRequest };
                return resp;
            }
        }

        public async Task<Response> DeleteComment(int idx)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> PutComment(string content, int idx)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
