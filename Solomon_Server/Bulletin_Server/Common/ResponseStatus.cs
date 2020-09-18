using System;
using System.Net;

namespace Solomon_Server.Utils
{
    public class ResponseStatus
    {
        public static int OK = Convert.ToInt32(HttpStatusCode.OK);
        public static int NOT_FOUND = Convert.ToInt32(HttpStatusCode.NotFound);
        public static int UNAUTHORIZED = Convert.ToInt32(HttpStatusCode.Unauthorized);
        public static int BAD_REQUEST = Convert.ToInt32(HttpStatusCode.BadRequest);
        public static int INTERNAL_SERVER_ERROR = Convert.ToInt32(HttpStatusCode.InternalServerError);
        public static int CONFLICT = Convert.ToInt32(HttpStatusCode.Conflict);
        public static int CREATED = Convert.ToInt32(HttpStatusCode.Created);
    }
}
