using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using kuujinbo.ASP.NET.Mvc.Misc.ViewModels;
using Newtonsoft.Json.Converters;

namespace kuujinbo.ASP.NET.Mvc.Misc.Controllers
{
    public class jQueryDataTablesController : Controller
    {
        private JqDataTablesHelper helper = new JqDataTablesHelper(7);
        private JqueryDataTable InitDataTable(UrlHelper url)
        {
            return new JqueryDataTable()
            {
                ActionButtons = new List<JqueryDataTable.ActionButton>()
                {
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Success,
                        Url = url.Action("Create"),
                        Text = "Create",
                        IsButton = false
                    },
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Primary,
                        Url = url.Action("Rollover"),
                        Text = "Rollover"
                    },
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Success,
                        Url = url.Action("Approve"),
                        Text = "Approve"
                    },
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Danger,
                        Url = url.Action("Disapprove"),
                        Text = "Disapprove"
                    }
                },
                DataUrl = url.Action("JsonData"),
                DeleteRowUrl = url.Action("DeleteOne"),
                EditRowUrl = url.Action("Update"),
                LastColumnIndex = 7
            };
        }

        // GET: jQueryDataTables
        public ActionResult Index()
        {
            return View("_jQueryDataTables", InitDataTable(Url));
        }

        [HttpAjaxPost]
        // public ActionResult JsonData()
        public ActionResult JsonData(JqueryDataTable table)
        {
            var debugging = true;
            return new JsonNetResult(_getTestData());
        }

        private object GetBatchUpdateResponseObject(IEnumerable<int> ids)
        {
            return string.Format(
                "XHR sent to:{2}[{0}]{2}with POST data [{1}]{2}succeeded!",
                Request.Url, string.Join(", ",  ids), Environment.NewLine
            );
        }

        [HttpAjaxPost]
        public ActionResult Approve(IEnumerable<int> ids)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(ids));
        }

        [HttpAjaxPost]
        public ActionResult Disapprove(IEnumerable<int> ids)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(ids));
        }

        [HttpAjaxPost]
        public ActionResult Rollover(IEnumerable<int> ids)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(ids));
        }


        [HttpAjaxPost]
        public ActionResult DeleteOne(int id)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(new int[] {id}));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int id)
        {
            System.Threading.Thread.Sleep(2000);
            return new JsonNetResult(
                string.Format(
                    "XHR POST sent to [{0}] succeeded!", Request.Url
                )
            );
        }

        public ActionResult Update(int id)
        {
            return View(id);
        }

        private void ConvertToTestModel(string[][] raw)
        {
            var rows = new List<TestModel>();
            foreach (var row in raw)
            {
                rows.Add(
                    new TestModel()
                    {
                        Id = int.Parse(row[0]), 
                        Name =row[1], 
                        Position = row[2] , 
                        Office = row[3], 
                        Extension = int.Parse(row[4]),
                        StartDate = DateTime.Parse(row[5])
                    }
                );
            }
            System.IO.File.WriteAllText(
                Server.MapPath("~/app_data/dataTablesObjectData.json"),
                JsonConvert.SerializeObject(
                    rows,
                    Formatting.Indented,
                    new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" }
                )
            );
        }


        private object _getTestData()
        {
            helper.GetClientParams(Request);

            //string dataFile = Server.MapPath("~/app_data/dataTablesObjectData.json");
            //string json = System.IO.File.ReadAllText(dataFile);
            //var dataFromFile = JsonConvert.DeserializeObject<IEnumerable<TestModel>>(json);
            string dataFile = Server.MapPath("~/app_data/dataTablesArrayData.json");
            string json = System.IO.File.ReadAllText(dataFile);
            var dataFromFile = JsonConvert.DeserializeObject<string[][]>(json);
            //ConvertToTestModel(dataFromFile);
            object result;

            //var data = dataFromFile
            //    .Skip(table.Start)
            //    .Take(table.Length);
            //return GetJsonData(data, table);


            if (helper.HasSearchValue)
            {
                var contains = dataFromFile
                    .Where(s => s[helper.SearchColumn]
                    .IndexOf(helper.SearchValue, StringComparison.OrdinalIgnoreCase) != -1);

                helper.RecordsTotal = contains.Count();
                var data = helper.SortAcsending
                    ? contains.OrderBy(x => x[helper.SortColumn])
                        .Skip(helper.start)
                        .Take(helper.length)
                    : contains.OrderByDescending(x => x[helper.SortColumn])
                        .Skip(helper.start)
                        .Take(helper.length)
                ;
                result = helper.GetJsonData(data);
            }
            else
            {
                helper.RecordsTotal = dataFromFile.Length;
                var data = helper.SortAcsending
                  ? dataFromFile.OrderBy(x => x[helper.SortColumn])
                    .Skip(helper.start)
                    .Take(helper.length)
                  : dataFromFile.OrderByDescending(x => x[helper.SortColumn])
                    .Skip(helper.start)
                    .Take(helper.length)
                ;
                result = helper.GetJsonData(data);
            }
            return result;
        }
    }

    public class JqDataTablesHelper
    {
        // URL-encoded parameters __ALWAYS__ sent to server
        const string DRAW = "draw";
        const string START = "start";
        const string LENGTH = "length";

        const string ORDER_COLUMN = "order[0][column]";
        const string ORDER_DIR = "order[0][dir]";
        const string ORDER_ASC = "asc";
        const string ORDER_DESC = "desc";

        const string SEARCH_VALUE = "search[value]";
        const string SEARCH_REGEX = "search[regex]";
        const string COLUMNS_DATA = "columns[{0}][data]";
        const string COLUMNS_NAME = "columns[{0}][name]";
        const string COLUMNS_SEARCHABLE = "columns[{0}][searchable]";
        const string COLUMNS_ORDERABLE = "columns[{0}][orderable]";
        const string COLUMNS_SEARCH_VALUE = "columns[{0}][search][value]";
        const string COLUMNS_SEARCH_REGEX = "columns[{0}][search][regex]";

        public int NumberOfColumns { get; set; }

        private int _draw, _start, _length, _sortColumn;
        public int draw { get { return _draw; } }
        public int start { get { return _start; } }
        public int length { get { return _length; } }
        public int RecordsTotal { get; set; }

        public int SortColumn { get { return _sortColumn; } }
        public bool SortAcsending { get; private set; }
        public bool HasSearchValue { get; private set; }
        public int SearchColumn { get; private set; }
        public string SearchValue { get; private set; }

        public JqDataTablesHelper(int numberOfColumns)
        {
            NumberOfColumns = numberOfColumns;
        }

        public void GetClientParams(HttpRequestBase Request)
        {
            Int32.TryParse(Request[DRAW], out _draw);
            Int32.TryParse(Request[START], out _start);
            Int32.TryParse(Request[LENGTH], out _length);
            Int32.TryParse(Request[ORDER_COLUMN], out _sortColumn);
            string sortOrder = Request[ORDER_DIR];
            SortAcsending = !string.IsNullOrEmpty(sortOrder)
                && sortOrder == ORDER_ASC ? true : false;

            for (int i = 0; i < NumberOfColumns; ++i)
            {
                string paramName = string.Format(COLUMNS_SEARCH_VALUE, i);
                string search = Request[paramName];
                if (!string.IsNullOrEmpty(search))
                {
                    HasSearchValue = true;
                    SearchColumn = i;
                    SearchValue = search;
                    break;
                }
            }
        }

        public object GetJsonData<T>(IEnumerable<T> data)
        {
            return new
            {
                draw = this.draw,
                recordsTotal = this.RecordsTotal,
                recordsFiltered = this.RecordsTotal,
                data = data
            };
        }
    }
}