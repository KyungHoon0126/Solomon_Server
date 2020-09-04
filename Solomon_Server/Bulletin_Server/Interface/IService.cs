using Bulletin_Server.Model.Meal;
using Bulletin_Server.Model.Member;
using Solomon_Server.Models.Bulletin;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Bulletin_Server
{
    [ServiceContract]
    public partial interface IService
    {
        #region Meal_Service
        /// <summary>
        /// 급식 정보
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/meal")]
        [return: MessageParameter(Name = "Meal")]
        Response<MealInfo> GetMealData();
        #endregion

        #region Member_Service
        /// <summary>
        /// 회원 가입
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pw", Sha512Hash></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/auth/register")]
        [return: MessageParameter(Name = "SignUp")]
        Task<Response> SignUp(string id, string pw, string name, string email);
        

        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pw", Sha512Hash></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/auth/login")]
        [return: MessageParameter(Name = "Login")]
        Task<Response<UserModel>> Login(string id, string pw);
        #endregion

        #region Bulletin_Service
        /// <summary>
        /// 전체 게시글 조회
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Bulletin")]
        Task<Response<List<BulletinModel>>> GetAllBulletins();

        /// <summary>
        /// 게시글 작성
        /// </summary>
        /// <param name="title", 게시글 제목></param>
        /// <param name="content", 게시글 내용></param>
        /// <param name="writer", 작성자></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Write Bulletin")]
        Task<Response> WriteBulletin(string title, string content, string writer);

        /// <summary>
        /// 게시글 삭제
        /// </summary>
        /// <param name="idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Delete Bulletin")]
        Task<Response> DeleteBulletin(int idx);
       
        /// <summary>
        /// 게시글 수정
        /// </summary>
        /// <param name="title", 게시글 제목></param>
        /// <param name="content", 게시글 내용></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "UPDATE", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Update Bulletin")]
        Task<Response> UpdateBulletin(string title, string content);
        #endregion

        #region Bulletin_Comment_Service

        #endregion
    }

    public class Response
    {
        public string message { get; set; }
        public int status { get; set; }
    }

    public class Response<T>
    {
        public string message { get; set; }
        public int status { get; set; }
        public T data { get; set; }
    }
}
