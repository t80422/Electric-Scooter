using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Electric_Scooter.Helpers
{
    public static class CityHelper
    {
        public static IEnumerable<string> GetCities()
        {
            return new[]
            {
                "基隆市","新北市","臺北市","桃園市","新竹縣","新竹市",
                "苗栗縣","臺中市","彰化縣","南投縣","雲林縣","嘉義縣",
                "嘉義市","臺南市","高雄市","屏東縣","臺東縣","花蓮縣",
                "宜蘭縣","澎湖縣","金門縣","連江縣"
            };
        }
    }
}