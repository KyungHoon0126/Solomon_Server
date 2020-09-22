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
    public partial class SolomonService : MySqlDBConnectionManager, IService
    {
        DBManager<BulletinModel> bulletinDBManager = new DBManager<BulletinModel>();

        #region Bulletin_Service
        public delegate Response<BulletinsResult> BulletinsBadResponse(ConTextColor preColor, int status, ConTextColor setColor, string msg);
        public delegate Response<BulletinModel> BulletinBadResponse(ConTextColor preColor, int status, ConTextColor setColor, string msg);

        #region API Method
        public async Task<Response<BulletinsResult>> GetAllBulletins()
        {
            string apiName = "GET ALL BULLETINS";

            #region Anonymous Method
            BulletinsBadResponse bulletinsBadResponse = delegate (ConTextColor preColor, int status, ConTextColor setColor, string msg)
            {
                List<BulletinModel> tempArr = new List<BulletinModel>();
                ServiceManager.ShowRequestResult(apiName, preColor, status, setColor);
                return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = tempArr }, message = msg, status = status };
            };
            #endregion

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
            {
                try
                {
                    List<BulletinModel> bulletins = new List<BulletinModel>();

                    using (IDbConnection db = GetConnection())
                    {
                        db.Open();

                        string selectSql = @"
SELECT
    *
FROM
    bulletin_tb
";
                        await bulletinDBManager.IndexSortSqlAsync(db, ServiceManager.GetIndexSortSQL("bulletin_idx", "bulletin_tb"));
                        bulletins = await bulletinDBManager.GetListAsync(db, selectSql, "");

                        if (bulletins != null && bulletins.Count > 0)
                        {
                            ServiceManager.ShowRequestResult(apiName, ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                            return new Response<BulletinsResult> { data = new BulletinsResult { bulletins = bulletins }, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            return bulletinsBadResponse(ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE, "게시글이 존재하지 않습니다.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(apiName + " ERROR : " + e.Message);
                    return bulletinsBadResponse(ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE, ResponseMessage.INTERNAL_SERVER_ERROR);
                }
            }
            else
            {
                return bulletinsBadResponse(ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE, ResponseMessage.BAD_REQUEST);
            }
        }

        public async Task<Response> WriteBulletin(string title, string content, string writer)
        {
            string apiName = "WRITE BULLETIN";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
            {
                if (title != null && content != null && writer != null &&
                        title.Trim().Length > 0 && content.Trim().Length > 0 && writer.Trim().Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = GetConnection())
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
                            if (await bulletinDBManager.InsertAsync(db, insertSql, model) == QueryExecutionResult.SUCCESS)
                            {
                                await bulletinDBManager.IndexSortSqlAsync(db, ServiceManager.GetIndexSortSQL("bulletin_idx", "bulletin_tb"));
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

        public async Task<Response> DeleteBulletin(string writer, int bulletin_idx)
        {
            string apiName = "DELETE BULLETIN";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
            {
                if (bulletin_idx.ToString() != null && bulletin_idx.ToString().Length > 0 && writer != null && writer.Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = GetConnection())
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
                            if (await bulletinDBManager.DeleteAsync(db, deleteSql, model) == QueryExecutionResult.SUCCESS)
                            {
                                await bulletinDBManager.IndexSortSqlAsync(db, ServiceManager.GetIndexSortSQL("bulletin_idx", "bulletin_tb"));
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

        public async Task<Response> PutBulletin(string title, string content, string writer, int bulletin_idx)
        {
            string apiName = "PUT BULLETIN";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
            {
                if (title != null && title.Trim().Length > 0 && content != null && title.Trim().Length > 0 && bulletin_idx.ToString().Length > 0)
                {
                    try
                    {
                        using (IDbConnection db = GetConnection())
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
                            if (await bulletinDBManager.UpdateAsync(db, updateSql, model) == QueryExecutionResult.SUCCESS)
                            {
                                await bulletinDBManager.IndexSortSqlAsync(db, updateSql);
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

        public async Task<Response<BulletinModel>> GetSpecificBulletin(string bulletin_idx)
        {
            string apiName = "GET SPECIFIC BULLETIN";

            BulletinBadResponse bulletinBadResponse = delegate (ConTextColor preColor, int status, ConTextColor setColor, string msg)
            {
                BulletinModel tempModel = new BulletinModel();
                ServiceManager.ShowRequestResult(apiName, preColor, status, setColor);
                return new Response<BulletinModel> { data = tempModel, message = msg, status = status };
            };

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
            {
                try
                {
                    BulletinModel bulletin = new BulletinModel();

                    using (IDbConnection db = GetConnection())
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
                            ServiceManager.ShowRequestResult(apiName, ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE);
                            return new Response<BulletinModel> { data = bulletin, message = ResponseMessage.OK, status = ResponseStatus.OK };
                        }
                        else
                        {
                            return bulletinBadResponse(ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE, "게시글이 존재하지 않습니다.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(apiName + " ERROR : " + e.Message);
                    return bulletinBadResponse(ConTextColor.PURPLE, ResponseStatus.INTERNAL_SERVER_ERROR, ConTextColor.WHITE, ResponseMessage.INTERNAL_SERVER_ERROR);
                }
            }
            else 
            {
                return bulletinBadResponse(ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE, ResponseMessage.BAD_REQUEST);
            }
        }
        #endregion
        #endregion
    }
}
