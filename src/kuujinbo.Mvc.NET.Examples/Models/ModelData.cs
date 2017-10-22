using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Hosting;
using Newtonsoft.Json;

namespace kuujinbo.Mvc.NET.Examples.Models
{
    [ExcludeFromCodeCoverage]
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