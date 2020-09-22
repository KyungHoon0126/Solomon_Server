using Solomon_Server.Model.Meal;
using Solomon_Server.Model.Member;
using Solomon_Server.Results;
using Solomon_Server.Results.CommentResults;
using Solomon_Server.Results.MemberResult;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Solomon_Server
{
    [ServiceContract]
    public partial interface IService
    {
        // TODO : 급식 반환 형태 고치기.
        #region Meal_Service
        /// <summary>
        /// 급식 조회
        /// Headers : token(string)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/meal")]
        Response<MealInfoModel> GetMealData();
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
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/auth/register")]
        Task<Response> SignUp(string id, string pw, string name, string email);


        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pw", Sha512Hash></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/auth/login")]
        Task<Response<MemberResult>> Login(string id, string pw);

        /// <summary>
        /// member_idx로 회원정보 조회
        /// </summary>
        /// <param name="member_idx"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/member/{member_idx}")]
        Task<Response<UserModel>> GetMemberInformation(string member_idx);
        #endregion

        #region Bulletin_Service
        /// <summary>
        /// 전체 게시글 조회
        /// Headers : token(string)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/post")]
        Task<Response<BulletinsResult>> GetAllBulletins();

        /// <summary>
        /// 게시글 작성
        /// Headers : token(string)
        /// </summary>
        /// <param name="title", 게시글 제목></param>
        /// <param name="content", 게시글 내용></param>
        /// <param name="writer", 작성자></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/post")]
        Task<Response> WriteBulletin(string title, string content, string writer);

        /// <summary>
        /// 게시글 삭제
        /// Headers : token(string)
        /// </summary>
        /// <param name="bulletin_idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/post")]
        Task<Response> DeleteBulletin(string writer, int bulletin_idx);

        /// <summary>
        /// 게시글 수정
        /// Headers : token(string)
        /// </summary>
        /// <param name="title", 게시글 제목></param>
        /// <param name="content", 게시글 내용></param>
        /// <param name="writer", 작성자></param>
        /// <param name="bulletin_idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/post")]
        Task<Response> PutBulletin(string title, string content, string writer, int bulletin_idx);

        /// <summary>
        /// 특정 게시물 조회
        /// Headers : token(string)
        /// </summary>
        /// <param name="bulletin_idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/post/{bulletin_idx}")]
        Task<Response<BulletinResult>> GetSpecificBulletin(string bulletin_idx);
        #endregion

        #region Bulletin_Comment_Service
        /// <summary>
        /// 전체 댓글 조회
        /// Headers : token(string)
        /// </summary>
        /// <returns>Comments</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Comment")]
        Task<Response<CommentsResult>> GetAllComments();

        /// <summary>
        /// 댓글 작성
        /// Headers : token(string)
        /// </summary>
        /// <param name="bulletin_idx", 게시글 idx></param>
        /// <param name="writer", 작성자></param>
        /// <param name="content", 댓글 내용></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Write_Comment")]
        Task<Response> WriteComment(int bulletin_idx, string writer, string content);

        /// <summary>
        /// 댓글 삭제
        /// Headers : token(string)
        /// </summary>
        /// <param name="writer", 작성자></param>
        /// <param name="comment_idx", 댓글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Delete_Comment")]
        Task<Response> DeleteComment(string writer, int comment_idx);

        /// <summary>
        /// 댓글 수정
        /// Headers : token(string)
        /// </summary>
        /// <param name="content", 댓글 내용></param>
        /// <param name="writer", 작성자></param>
        /// <param name="comment_idx", 댓글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Put_Comment")]
        Task<Response> PutComment(string content, string writer, int comment_idx);

        /// <summary>
        /// 특정 게시물 전체 댓글 조회
        /// Headers : token(string)
        /// </summary>
        /// <param name="bulletin_idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/bulletin/comment/{bulletin_idx}")]
        Task<Response<CommentsResult>> GetSpecificComments(string bulletin_idx);
        #endregion

        #region CheckOverlap_Service
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/auth/check/email?email={email}")]
        Task<Response> CheckEmailOverlap(string email);
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
        
        //public Data<T> data { get; set; }
    }

    //public class Data<T>
    //{
    //    public T data { get; set; }
    //}
}
