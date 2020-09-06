using Newtonsoft.Json;
using Prism.Mvvm;
using System.Collections.Generic;

namespace Solomon_Server.Model.Meal
{
    public class MealInfoModel : BindableBase
    {
        private List<MealServiceDietInfo> Meal;
        [JsonProperty("mealServiceDietInfo")]
        public List<MealServiceDietInfo> meal
        {
            get => Meal;
            set
            {
                SetProperty(ref Meal, value);
            }
        }
    }
}
