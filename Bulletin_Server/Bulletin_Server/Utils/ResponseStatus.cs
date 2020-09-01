using System;
using System.Net;

namespace Bulletin_Server.Utils
{
    public class ResponseStatus
    {
        public static int OK = Convert.ToInt32(HttpStatusCode.OK);
        public static int NotFound = Convert.ToInt32(HttpStatusCode.NotFound);
        public static int Unauthorized = Convert.ToInt32(HttpStatusCode.Unauthorized);
        public static int BadRequest = Convert.ToInt32(HttpStatusCode.BadRequest);
        public static int InternalServerError = Convert.ToInt32(HttpStatusCode.InternalServerError);
    }
}
