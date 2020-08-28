using Newtonsoft.Json;

namespace Bulletin_Server.Model.Meal
{
    public class Result
    {
        [JsonProperty("CODE")]
        public string Code { get; set; }

        [JsonProperty("MESSAGE")]
        public string Message { get; set; }
    }
}
