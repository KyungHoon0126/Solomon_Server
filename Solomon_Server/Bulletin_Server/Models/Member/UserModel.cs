namespace Solomon_Server.Model.Member
{
    public class UserModel
    {
        public int member_idx { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int birth_year { get; set; }
    }

    public class MemberModel : UserModel
    {
        public string pw { get; set; }
    }
}
