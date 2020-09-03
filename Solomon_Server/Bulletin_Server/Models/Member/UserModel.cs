using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin_Server.Model.Member
{
    public class UserModel
    {
        public string token { get; set; }

        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

    public class Member : UserModel
    {
        public string pw { get; set; }
    }
}
