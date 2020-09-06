using Newtonsoft.Json;

namespace Solomon_Server.Model.Meal
{
    public class Head
    {
        [JsonProperty("list_total_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? ListTotalCount { get; set; }
        [JsonProperty("RESULT", NullValueHandling = NullValueHandling.Ignore)]
        public Result Result { get; set; }
    }
}
