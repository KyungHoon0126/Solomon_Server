using Bulletin_Server.DataBase;
using Bulletin_Server.Model.Member;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin_Server.Service
{
    public class MemberService
    {
        DBManager<User> dBManager = new DBManager<User>();

        public async Task GetMemberData()
        {
            using (var conn = new SqlConnection(""))
            {
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
