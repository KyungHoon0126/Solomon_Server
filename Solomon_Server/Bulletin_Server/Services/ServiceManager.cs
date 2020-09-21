using Solomon_Server.Common;
using Solomon_Server.Utils;
using System;
using System.ServiceModel.Web;

namespace Solomon_Server.Services
{
    public enum ResponseType
    {
        OK,
        BAD_REQUEST,
        INTERNAL_SERVER_ERROR,
    }

    public class ServiceManager
    {
        public delegate Response RequestResult(ResponseType type);
        public static RequestResult Result = delegate (ResponseType type)
        {
            switch (type)
            {
                case ResponseType.OK:
                    return new Response { message = ResponseMessage.OK, status = ResponseStatus.OK };
                case ResponseType.BAD_REQUEST:
                    return new Response { message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST }; ;
                case ResponseType.INTERNAL_SERVER_ERROR:
                    return new Response { message = ResponseMessage.INTERNAL_SERVER_ERROR, status = ResponseStatus.INTERNAL_SERVER_ERROR };
                default:
                    return null;
            }
        };


        /// <summary>
        /// Request Time Log
        /// </summary>
        /// <returns></returns>
        public static string CheckRequestTime()
        {
            DateTime time = DateTime.Now;
            string msg = string.Empty;
            msg += "[";
            msg += time.Year.ToString() + "년 ";
            msg += time.Month.ToString() + "월 ";
            msg += time.Day.ToString() + "일 ";
            msg += time.Hour.ToString() + "시 ";
            msg += time.Minute.ToString() + "분 ";
            msg += time.Second.ToString() + "초 ";
            msg += "] ";
            return msg;
        }


        /// <summary>
        /// Get Header's Token
        /// </summary>
        /// <param name="webOperationContext"></param>
        /// <returns></returns>
        public static string GetHeaderValue(WebOperationContext webOperationContext)
        {
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();
            if (webOperationContext.IncomingRequest.Headers != null && requestHeaderValue != null)
            {
                return requestHeaderValue;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Sort data indexes after insert, delete execution
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetIndexSortSQL(string idx, string tableName)
        {
            string sortSql = $@"
ALTER 
    TABLE bulletin.{tableName} AUTO_INCREMENT = 1;
SET 
    @COUNT = 0;
UPDATE
    bulletin.{tableName} SET {idx} = @COUNT:= @COUNT + 1
;";
            return sortSql;
        }


        /// <summary>
        /// Request Log
        /// </summary>
        /// <param name="apiName"></param>
        /// <param name="preColor"></param>
        /// <param name="status"></param>
        /// <param name="setColor"></param>
        public static void ShowRequestResult(string apiName, ConTextColor preColor, int status, ConTextColor setColor)
        {
            Console.Write(CheckRequestTime() + $"{apiName} responded ");
            WrapAPI.SetConsoleTextColor(preColor);
            Console.WriteLine(status);
            WrapAPI.SetConsoleTextColor(setColor);
        }
    }
}
