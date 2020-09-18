using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solomon_Server.Common;
using Solomon_Server.Model.Meal;
using Solomon_Server.Utils;
using System;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using hap = HtmlAgilityPack;

namespace Solomon_Server.Services
{
    public partial class SolomonService : IService
    {
        #region Meal_Service
        public Response<MealInfoModel> GetMealData()
        {
            WebOperationContext webOperationContext = WebOperationContext.Current;
            MealInfoModel tempModel = new MealInfoModel();
            
            if (ComDef.InspectionHeaderValue(webOperationContext))
            {
                string requestHeaderValue = webOperationContext.IncomingRequest.Headers["token"].ToString();

                // Header에 토큰 값이 제대로 들어왔는지 확인 & 토큰이 유효한지 확인
                if (!(requestHeaderValue == null) && ComDef.jwtService.IsTokenValid(requestHeaderValue) == true)
                {
                    WebClient webClient = new WebClient();
                    webClient.Headers["Content-Type"] = "application/json";
                    webClient.Encoding = Encoding.UTF8;

                    string html = webClient.DownloadString("https://open.neis.go.kr/hub/mealServiceDietInfo?ATPT_OFCDC_SC_CODE=D10&SD_SCHUL_CODE=7240393&MLSV_YMD="
                                                            + DateTime.Now.ToString("yyyyMM") + "&type=json&KEY=9b89605504b946bfab0c06c0ceb0a69a");
                    hap.HtmlDocument document = new hap.HtmlDocument();
                    document.LoadHtml(html);

                    JObject jObject = JObject.Parse(document.Text);

                    MealInfoModel mealData = JsonConvert.DeserializeObject<MealInfoModel>(jObject.ToString());

                    for (int i = 0; i < mealData.meal.Count; i++)
                    {
                        if (mealData.meal[i].row == null)
                        {
                            mealData.meal.Remove(mealData.meal[i]);
                        }
                    }

                    if (mealData == null || mealData.meal.Count < 0)
                    {
                        Console.WriteLine("급식 : " + ResponseStatus.NOT_FOUND);
                        return new Response<MealInfoModel> { data = tempModel, message = "급식 설정이 필요합니다.", status = ResponseStatus.NOT_FOUND };
                    }
                    else
                    {
                        Console.WriteLine("급식 : " + ResponseStatus.OK);
                        return new Response<MealInfoModel> { data = mealData, message = "급식 조회에 성공하였습니다.", status = ResponseStatus.OK };
                    }
                }
                else // Header에 토큰이 전송되지 않음 or 토큰이 유요하지 않음. => 검증 오류.
                {
                    Console.WriteLine("급식 : " + ResponseStatus.BAD_REQUEST);
                    return new Response<MealInfoModel> { data = tempModel, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
                }
            }
            else
            {
                Console.WriteLine("급식 : " + ResponseStatus.BAD_REQUEST);
                return new Response<MealInfoModel> { data = tempModel, message = ResponseMessage.BAD_REQUEST, status = ResponseStatus.BAD_REQUEST };
            }
        }
        #endregion
    }
}
