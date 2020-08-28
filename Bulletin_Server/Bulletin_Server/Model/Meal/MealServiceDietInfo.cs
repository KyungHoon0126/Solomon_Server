using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin_Server.Model.Meal
{
    public class MealServiceDietInfo
    {
        [JsonProperty("list_total_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? ListTotalCount { get; set; }
        [JsonProperty("RESULT", NullValueHandling = NullValueHandling.Ignore)]
        public Result Result { get; set; }
    }
}
