﻿using Newtonsoft.Json;
using Prism.Mvvm;
using System.Collections.Generic;

namespace Bulletin_Server.Model.Meal
{
    public class MealInfo : BindableBase
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
