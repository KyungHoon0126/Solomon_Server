﻿using Newtonsoft.Json;
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
        public delegate Response<MealInfoModel> MealBadResponse(string apiName, ConTextColor preColor, int status, ConTextColor setColor, string msg);
        MealBadResponse mealBadResponse = delegate (string apiName, ConTextColor preColor, int status, ConTextColor setColor, string msg)
        {
            MealInfoModel tempModel = new MealInfoModel();
            ServiceManager.ShowRequestResult(apiName, preColor, status, setColor);
            return new Response<MealInfoModel> { data = tempModel, status = status, message = msg };
        };

        #region Meal_Service
        public Response<MealInfoModel> GetMealData()
        {
            string apiName = "MEAL";

            if (ComDef.jwtService.IsTokenValid(ServiceManager.GetHeaderValue(WebOperationContext.Current)))
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
                    if (mealData.meal[i].row is null)
                    {
                        mealData.meal.Remove(mealData.meal[i]);
                    }
                }

                if (mealData is null || mealData.meal.Count < 0)
                {
                    return mealBadResponse(apiName, ConTextColor.RED, ResponseStatus.NOT_FOUND, ConTextColor.WHITE, "급식 설정이 필요합니다.");
                }
                else
                {
                    ServiceManager.ShowRequestResult(apiName, ConTextColor.LIGHT_GREEN, ResponseStatus.OK, ConTextColor.WHITE); ;
                    return new Response<MealInfoModel> { data = mealData, message = "급식 조회에 성공하였습니다.", status = ResponseStatus.OK };
                }
            }
            else 
            {
                return mealBadResponse(apiName, ConTextColor.RED, ResponseStatus.BAD_REQUEST, ConTextColor.WHITE, ResponseMessage.BAD_REQUEST);
            }
        }
        #endregion
    }
}