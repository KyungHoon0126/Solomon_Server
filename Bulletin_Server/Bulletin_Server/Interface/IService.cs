using Bulletin_Server.Model.Meal;
using Bulletin_Server.Model.Member;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Bulletin_Server
{
    [ServiceContract]
    public interface IService
    {
        #region MealService
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/meal")]
        [return: MessageParameter(Name = "Meal")]
        Response<MealInfo> GetMealData();
        #endregion

        #region MemberService
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/auth/register")]
        [return: MessageParameter(Name = "SignUp")]
        Response<UserModel> SignUp(string id, string pw, string name, string email);
        

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/auth/login")]
        [return: MessageParameter(Name = "Login")]
        Response<UserModel> Login(string id, string password, string email);
        #endregion
    }

    public class Response<T>
    {
        public string message { get; set; }
        public int status { get; set; }
        public T data { get; set; }
    }
}
