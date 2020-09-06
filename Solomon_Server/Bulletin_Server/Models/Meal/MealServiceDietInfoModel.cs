using Newtonsoft.Json;
using System.Collections.Generic;

namespace Solomon_Server.Model.Meal
{
    public class MealServiceDietInfo
    {
        //[JsonProperty("head")]
        //public List<Head> Head { get; set; }

        [JsonProperty("row")]
        public List<Row> row { get; set; }
    }
}
