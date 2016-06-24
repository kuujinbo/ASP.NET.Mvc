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
        public byte[] Create(DataTable dataTable)
        {
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
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), 
                        SheetId = 1,
                        Name = "sheet1" 
                    });

                    using (var writer = OpenXmlWriter.Create(worksheetPart))
                    {
                        writer.WriteStartElement(new Worksheet());
                        writer.WriteStartElement(new SheetData());

                        WriteHeaderRow(dataTable, writer);
                        WriteDataRows(dataTable, writer);

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private void WriteHeaderRow(DataTable dataTable, OpenXmlWriter writer)
        {
            writer.WriteStartElement(new Row());
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                writer.WriteElement(new Cell
                {
                    CellValue = new CellValue(dataTable.Columns[i].ColumnName),
                    DataType = CellValues.String
                });
            }
            writer.WriteEndElement();
        }

        private void WriteDataRows(DataTable dataTable, OpenXmlWriter writer)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                writer.WriteStartElement(new Row());
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    writer.WriteElement(new Cell
                    {
                        CellValue = new CellValue(row[i].ToString()),
                        DataType = CellValues.String
                    });
                }
                writer.WriteEndElement();
            }
        }
    }
}