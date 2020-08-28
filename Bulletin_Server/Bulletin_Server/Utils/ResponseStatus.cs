using System;
using System.Net;

namespace Bulletin_Server.Utils
{
    public class ResponseStatus
    {
        public static int OK = Convert.ToInt32(HttpStatusCode.OK);
        public static int NotFound = Convert.ToInt32(HttpStatusCode.NotFound);
    }
}
