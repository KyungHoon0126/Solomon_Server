using Solomon_Server.Model.Member;

namespace Solomon_Server.Results.MemberResult
{
    public class MemberResult
    {
        private string _token;
        public string token
        {
            get => _token;
            set => _token = value;
        }

        private string _refreshToken;
        public string refreshToken
        {
            get => _refreshToken;
            set => _refreshToken = value;
        }

        private UserModel _member;
        public UserModel member
        {
            get => _member;
            set => _member = value;
        }
    }
}
