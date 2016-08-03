using kuujinbo.ASP.NET.Mvc.Services;
using System;
using System.Data;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class ReportParams
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            System.Threading.Thread.Sleep(760);
            return PartialView();
        }

        [HttpPost]
        public ActionResult Index(ReportParams reportParams)
        {
            return File(
                new SimpleExcelFile().Create(GetData()),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "test.xlsx"
            );
        }

        private DataTable GetData()
        {
            using (var table = new DataTable())
            {
                for (var i = 0; i < 76; ++i)
                {
                    if (i % 2 == 0) { table.Columns.Add(i.ToString("D2"), typeof(DateTime)); }
                    else { table.Columns.Add(i.ToString("D2"), typeof(int)); }
                }

                for (var i = 0; i < 76; ++i)
                {
                    DataRow row = table.NewRow();
                    for (var j = 0; j < 76; ++j)
                    {
                        if (j % 2 == 0) { row[j.ToString("D2")] = DateTime.Now.AddDays(j); }
                        else { row[j.ToString("D2")] = j; }
                    }
                    table.Rows.Add(row);
                }

                return table;            
            }
        }
    }
}