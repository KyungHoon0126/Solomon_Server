using Bulletin_Server.Model.Meal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin_Server
{
    [ServiceContract]
    public interface IService
    {
        Response<MealInfo> GetMealData();
    }

    public class Response<T>
    {
        public string message { get; set; }
        public int status { get; set; }
        public T data { get; set; }
    }
}
