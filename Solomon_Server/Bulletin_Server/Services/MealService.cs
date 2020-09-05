using Bulletin_Server.Common;
using Bulletin_Server.Model.Meal;
using Bulletin_Server.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using hap = HtmlAgilityPack;

namespace Bulletin_Server.Service
{
    public partial class SolomonService : IService
    {
        public Response<MealInfo> GetMealData()
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

            MealInfo tempModel = new MealInfo();

            if (ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
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

                for (int i = 0; i < mealData.meal.Count; i++)
                {
                    if (mealData.meal[i].row == null)
                    {
                        mealData.meal.Remove(mealData.meal[i]);
                    }
                }

                if (mealData == null || mealData.meal.Count < 0)
                {
                    Console.WriteLine("급식 : " + ResponseStatus.NotFound);
                    var resp = new Response<MealInfo> { data = tempModel, message = "급식 설정이 필요합니다.", status = ResponseStatus.NotFound };
                    return resp;
                }
                else
                {
                    Console.WriteLine("급식 : " + ResponseStatus.OK);
                    var resp = new Response<MealInfo> { data = mealData, message = "급식 조회에 성공하였습니다.", status = ResponseStatus.OK };
                    return resp;
                }
            }
            else
            {
                Console.WriteLine("급식 : " + ResponseStatus.BadRequest);
                var resp = new Response<MealInfo> { data = tempModel, message = "토큰이 만료되었습니다.", status = ResponseStatus.BadRequest };
                return resp;
            }
        }
    }
}
