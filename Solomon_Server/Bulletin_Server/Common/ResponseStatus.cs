using System;
using System.Net;

namespace Solomon_Server.Utils
{
    public class ResponseStatus
    {
        public static int OK = ConvertToIntHttpStatusCode(HttpStatusCode.OK);
        public static int NOT_FOUND = ConvertToIntHttpStatusCode(HttpStatusCode.NotFound);
        public static int UNAUTHORIZED = ConvertToIntHttpStatusCode(HttpStatusCode.Unauthorized);
        public static int BAD_REQUEST = ConvertToIntHttpStatusCode(HttpStatusCode.BadRequest);
        public static int INTERNAL_SERVER_ERROR = ConvertToIntHttpStatusCode(HttpStatusCode.InternalServerError);
        public static int CONFLICT = ConvertToIntHttpStatusCode(HttpStatusCode.Conflict);
        public static int CREATED = ConvertToIntHttpStatusCode(HttpStatusCode.Created);

        public static int ConvertToIntHttpStatusCode(HttpStatusCode httpStatusCode)
        {
            return Convert.ToInt32(httpStatusCode);
        }
    }
}
