using Solomon_Server.JWT.Models;
using Solomon_Server.JWT.Services;
using Solomon_Server.Utils;
using System;
using System.ServiceModel.Web;

namespace Solomon_Server.Common
{
    // TODO : Repository Public 시, JWT Secret Key, Meal API Key, DB 정보 제외.
    public class ComDef
    {   
        // Token Verification
        public static JWTContainerModel jWTContainerModel = new JWTContainerModel();
        public static JWTService jwtService = new JWTService(jWTContainerModel.SecretKey);

        public static readonly string DATA_BASE_URL = $"SERVER=localhost;DATABASE=Bulletin;UID=root;PASSWORD=#kkh03kkh#;allow user variables=true";

        // Request Time Log
        public static string CheckRequestTime()
        {
            DateTime time = DateTime.Now;
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            int hour = time.Hour;
            int minute = time.Minute;
            int second = time.Second;

            string msg = string.Empty;
            msg += "[";    
            msg += year.ToString() + "년 ";
            msg += month.ToString() + "월 ";
            msg += day.ToString() + "일 ";
            msg += hour.ToString() + "시 ";
            msg += minute.ToString() + "분 ";
            msg += second.ToString() + "초 ";
            msg += "] ";
            return msg;
        }

        // Request Log
        public static void ShowResponseResult(string apiName, ConTextColor preColor, int status, ConTextColor setColor)
        {
            Console.Write(CheckRequestTime() + $"{apiName} responded ");
            WrapAPI.SetConsoleTextColor(preColor);
            Console.WriteLine(status);
            WrapAPI.SetConsoleTextColor(setColor);
        }

        /// <summary>
        /// Inpection Request Header's Value
        /// </summary>
        /// <param name="webOperationContext"></param>
        /// <returns></returns>
        public static bool InspectionHeaderValue(WebOperationContext webOperationContext)
        {
            if (webOperationContext.IncomingRequest.Headers == null)
            {
                return false;
            }
            else
            {
                return true;
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