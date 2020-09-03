using Newtonsoft.Json;

namespace Bulletin_Server.Model.Meal
{
    public class Row
    {
        //[JsonProperty("ATPT_OFCDC_SC_CODE")]
        //public string ATPT_OFCDC_SC_CODE { get; set; }

        //[JsonProperty("ATPT_OFCDC_SC_NM")]
        //public string ATPT_OFCDC_SC_NM { get; set; }

        //[JsonProperty("SD_SCHUL_CODE")]
        //public string SD_SCHUL_CODE { get; set; }

        //[JsonProperty("SCHUL_NM")]
        //public string SCHUL_NM { get; set; }

        //[JsonProperty("MMEAL_SC_CODE")]
        //public string MMEAL_SC_CODE { get; set; }

        [JsonProperty("MMEAL_SC_NM")]
        public string MEAL_TYPE { get; set; }

        //[JsonProperty("MLSV_YMD")]
        //public string MLSV_YMD { get; set; }

        //[JsonProperty("MLSV_FGR")]
        //public string MLSV_FGR { get; set; }

        [JsonProperty("DDISH_NM")]
        public string MEAL_MENUS { get; set; }

        //[JsonProperty("ORPLC_INFO")]
        //public string ORPLC_INFO { get; set; }

        //[JsonProperty("CAL_INFO")]
        //public string CAL_INFO { get; set; }

        [JsonProperty("NTR_INFO")]
        public string DETAIL_INFO { get; set; }

        //[JsonProperty("MLSV_FROM_YMD")]
        //public string MLSV_FROM_YMD { get; set; }

        //[JsonProperty("MLSV_TO_YMD")]
        //public string MLSV_TO_YMD { get; set; }
    }
}
