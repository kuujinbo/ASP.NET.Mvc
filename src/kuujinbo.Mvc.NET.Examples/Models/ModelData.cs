using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Hosting;

namespace kuujinbo.Mvc.NET.Examples.Models
{
    public static class ModelData
    {
        public static readonly ICollection<TestModel> Data;

        static ModelData()
        {
            Data = JsonConvert.DeserializeObject<ICollection<TestModel>>(
                System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/app_data/jsonData.json"))
            );
        }
    }
}