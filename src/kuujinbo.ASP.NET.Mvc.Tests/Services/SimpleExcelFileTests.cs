using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using kuujinbo.ASP.NET.Mvc.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace kuujinbo.ASP.NET.Mvc.Tests.Services
{
    public class SimpleExcelFileTests
    {
        [Fact]
        public void Create_WithDataTable_CreatesSpreadSheet()
        {
            var table = GetTestDataTable();
            using (table)
            {
                var excel = new SimpleExcelFile().Create(table);
                var cells = ParseExcel(excel);

                Assert.IsType<byte[]>(excel);

                Assert.Equal(4, cells.Count);
                Assert.Equal(table.Columns[0].ColumnName, cells[0]);
                Assert.Equal(table.Columns[1].ColumnName, cells[1]);
                Assert.Equal(table.Rows.Count, 1);
                Assert.Equal(table.Rows[0].ItemArray.Length, 2);
                Assert.Equal(table.Rows[0].ItemArray[0].ToString(), cells[2]);
                Assert.Equal(table.Rows[0].ItemArray[1].ToString(), cells[3]);
            }
        }

        [Fact]
        public void Create_WithList_CreatesSpreadSheet()
        {
            var list = new List<List<object>>()
            {
                new List<object>() { "h0", "h1"},
                new List<object>() { 1, null}
            };
            var excel = new SimpleExcelFile().Create(list);
            var cells = ParseExcel(excel);

            Assert.IsType<byte[]>(excel);

            Assert.Equal(4, cells.Count);
            Assert.Equal(list[0][0], cells[0]);
            Assert.Equal(list[0][1], cells[1]);
            Assert.Equal(list[1][0].ToString(), cells[2]);
            Assert.Equal(string.Empty, cells[3]);
        }

        private DataTable GetTestDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("00", typeof(int));
            table.Columns.Add("01", typeof(int));

            DataRow row = table.NewRow();
            row.ItemArray = new object[] { 0, 1 };
            table.Rows.Add(row);

            return table;
        }

        private List<string> ParseExcel(byte[] excel)
        {
            var result = new List<string>();
            using (var stream = new MemoryStream(excel))
            {
                using (var doc = SpreadsheetDocument.Open(stream, false))
                {
                    var sheetData = doc.WorkbookPart.WorksheetParts.First()
                        .Worksheet.Elements<SheetData>().First();
                    foreach (Row r in sheetData.Elements<Row>())
                    {
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            result.Add(c.InnerText);
                        }
                    }
                    return result;
                }
            }
        }
    }
}