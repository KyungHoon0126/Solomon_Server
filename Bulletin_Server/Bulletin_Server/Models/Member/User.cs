using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin_Server.Model.Member
{
    public class User
    {
        public string id { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string token { get; set; }
    }
}
