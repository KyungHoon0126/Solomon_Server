using Solomon_Server.Model.Meal;
using Solomon_Server.Model.Member;
using Solomon_Server.Models.Bulletin;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Solomon_Server
{
    [ServiceContract]
    public partial interface IService
    {
        #region Meal_Service
        /// <summary>
        /// 급식 조회
        /// Headers : token(string)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedResponse, UriTemplate = "/meal")]
        [return: MessageParameter(Name = "Meal")]
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
        /// Headers : token(string)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Bulletin")]
        Task<Response<List<BulletinModel>>> GetAllBulletins();

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
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Write_Bulletin")]
        Task<Response> WriteBulletin(string title, string content, string writer);

        /// <summary>
        /// 게시글 삭제
        /// Headers : token(string)
        /// </summary>
        /// <param name="idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Delete_Bulletin")]
        Task<Response> DeleteBulletin(string writer, int idx);

        /// <summary>
        /// 게시글 수정
        /// Headers : token(string)
        /// </summary>
        /// <param name="title", 게시글 제목></param>
        /// <param name="content", 게시글 내용></param>
        /// <param name="writer", 작성자></param>
        /// <param name="idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin")]
        [return: MessageParameter(Name = "Put_Bulletin")]
        Task<Response> PutBulletin(string title, string content, string writer, int idx);

        /// <summary>
        /// 특정 게시물 조회
        /// Headers : token(string)
        /// </summary>
        /// <param name="idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin/{idx}")]
        [return: MessageParameter(Name = "Specific_Bulletin")]
        Task<Response<BulletinModel>> GetSpecificBulletin(string idx);
        #endregion

        #region Bulletin_Comment_Service
        /// <summary>
        /// 전체 댓글 조회
        /// Headers : token(string)
        /// </summary>
        /// <returns>Comments</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Comment")]
        Task<Response<List<CommentModel>>> GetAllComments();

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
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Write_Comment")]
        Task<Response> WriteComment(int bulletin_idx, string writer, string content);

        /// <summary>
        /// 댓글 삭제
        /// Headers : token(string)
        /// </summary>
        /// <param name="writer", 작성자></param>
        /// <param name="idx", 댓글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Delete_Comment")]
        Task<Response> DeleteComment(string writer, int idx);

        /// <summary>
        /// 댓글 수정
        /// Headers : token(string)
        /// </summary>
        /// <param name="content", 댓글 내용></param>
        /// <param name="writer", 작성자></param>
        /// <param name="idx", 댓글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin/comment")]
        [return: MessageParameter(Name = "Put_Comment")]
        Task<Response> PutComment(string content, string writer, int idx);

        /// <summary>
        /// 특정 게시물 전체 댓글 조회
        /// Headers : token(string)
        /// </summary>
        /// <param name="bulletin_idx", 게시글 idx></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/bulletin/comment/{bulletin_idx}")]
        Task<Response<List<CommentModel>>> GetSpecificComments(string bulletin_idx);
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
