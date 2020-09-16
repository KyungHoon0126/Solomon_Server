namespace Solomon_Server.Model.Member
{
    public class UserModel
    {
        public string token { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

    public class MemberModel : UserModel
    {
        public string pw { get; set; }
    }
}
