using System;
using System.Net;

namespace Solomon_Server.Utils
{
    public class ResponseStatus
    {
        public static int OK = ConvertHttpStatusCodeToInt(HttpStatusCode.OK);
        public static int NOT_FOUND = ConvertHttpStatusCodeToInt(HttpStatusCode.NotFound);
        public static int UNAUTHORIZED = ConvertHttpStatusCodeToInt(HttpStatusCode.Unauthorized);
        public static int BAD_REQUEST = ConvertHttpStatusCodeToInt(HttpStatusCode.BadRequest);
        public static int INTERNAL_SERVER_ERROR = ConvertHttpStatusCodeToInt(HttpStatusCode.InternalServerError);
        public static int CONFLICT = ConvertHttpStatusCodeToInt(HttpStatusCode.Conflict);
        public static int CREATED = ConvertHttpStatusCodeToInt(HttpStatusCode.Created);

        public static int ConvertHttpStatusCodeToInt(HttpStatusCode httpStatusCode)
        {
            return Convert.ToInt32(httpStatusCode);
        }
    }
}
