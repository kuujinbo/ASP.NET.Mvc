using System.Data;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace kuujinbo.ASP.NET.Mvc.Services
{
    public interface ISimpleExcelFile
    {
        byte[] Create(DataTable dataTable);
    }

    public class SimpleExcelFile : ISimpleExcelFile
    {
        private int _columnCount;

        public byte[] Create(DataTable dataTable)
        {
            _columnCount = dataTable.Columns.Count;

            using (var memoryStream = new MemoryStream())
            {
                using (var spreadsheetDocument = SpreadsheetDocument.Create(
                    memoryStream,
                    SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheets = workbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                    sheets.Append(new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "sheet1"
                    });

                    using (var writer = OpenXmlWriter.Create(worksheetPart))
                    {
                        writer.WriteStartElement(new Worksheet());
                        writer.WriteStartElement(new SheetData());

                        // header row
                        var headerRow = new object[_columnCount];
                        for (int i = 0; i < _columnCount; i++)
                        {
                            headerRow[i] = dataTable.Columns[i].ColumnName;
                        }
                        WriteRow(writer, headerRow);

                        // DB result set
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            WriteRow(writer, dataRow.ItemArray);
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private void WriteRow(OpenXmlWriter writer, object[] row)
        {
            writer.WriteStartElement(new Row());
            for (int i = 0; i < _columnCount; i++)
            {
                writer.WriteStartElement(new Cell() { DataType = CellValues.InlineString });
                writer.WriteElement(new InlineString(new Text(row[i].ToString())));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}