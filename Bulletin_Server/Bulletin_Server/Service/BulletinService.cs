using Bulletin_Server.Model.Meal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using hap = HtmlAgilityPack;

namespace Bulletin_Server.Service
{
    public class BulletinService : IService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, 
                   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/meal")]
        public Task GetMealData()
        {
            WebClient webClient = new WebClient();
            webClient.Headers["Content-Type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            string html = webClient.DownloadString("https://open.neis.go.kr/hub/mealServiceDietInfo?ATPT_OFCDC_SC_CODE=D10&SD_SCHUL_CODE=7240393&MLSV_YMD="
                                                   + DateTime.Now.ToString("yyyyMM") + "&type=json&KEY=9b89605504b946bfab0c06c0ceb0a69a");
            hap.HtmlDocument document = new hap.HtmlDocument();
            document.LoadHtml(html);
            JObject jObject = JObject.Parse(document.Text);
            MealInfo mealData = JsonConvert.DeserializeObject<MealInfo>(jObject.ToString());
            List<Row> rowItems = new List<Row>();
            
            return null;
        }
    }
}
