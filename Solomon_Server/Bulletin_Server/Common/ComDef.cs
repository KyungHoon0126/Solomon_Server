using Solomon_Server.JWT.Models;
using Solomon_Server.JWT.Services;
using Solomon_Server.Utils;
using System;
using System.ServiceModel.Web;

namespace Solomon_Server.Common
{
    // TODO : Repository Public 시, JWT Secret Key, Meal API Key 정보 제외.
    public class ComDef
    {   
        // Token Verification
        public static JWTContainerModel jWTContainerModel = new JWTContainerModel();
        public static JWTService jwtService = new JWTService(jWTContainerModel.SecretKey);

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
        /// Request Log
        /// </summary>
        /// <param name="apiName"></param>
        /// <param name="preColor"></param>
        /// <param name="status"></param>
        /// <param name="setColor"></param>
        public static void ShowResponseResult(string apiName, ConTextColor preColor, int status, ConTextColor setColor)
        {
            Console.Write(CheckRequestTime() + $"{apiName} responded ");
            WrapAPI.SetConsoleTextColor(preColor);
            Console.WriteLine(status);
            WrapAPI.SetConsoleTextColor(setColor);
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
    }
}