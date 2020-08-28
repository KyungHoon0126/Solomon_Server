using Bulletin_Server.Model.Meal;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Bulletin_Server
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/meal")]
        Response<MealInfo> GetMealData();
    }

    public class Response<T>
    {
        public string message { get; set; }
        public int status { get; set; }
        public T data { get; set; }
    }
}
