using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
        private OpenXmlWriter _writer;
        private int _columnCount;
        private Dictionary<int, CellValues> _cellTypes = new Dictionary<int, CellValues>();

        public static Row ROW = new Row();
        public static readonly Cell CELL_INLINE_STRING = new Cell()
        { 
            DataType = CellValues.InlineString 
        };
        public static readonly Cell CELL_NUMBER = new Cell() 
        { 
            DataType = CellValues.Number 
        };

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

                    using (_writer = OpenXmlWriter.Create(worksheetPart))
                    {
                        _writer.WriteStartElement(new Worksheet());
                        _writer.WriteStartElement(new SheetData());

                        WriteHeaderRow(dataTable
                            .Columns.Cast<DataColumn>()
                            .Select(x => x.ColumnName)
                            .ToArray()
                        );

                        // DB result set
                        SetCellTypes(dataTable.Rows[0].ItemArray);
                        foreach (DataRow r in dataTable.Rows) WriteRow(r.ItemArray);

                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private void WriteHeaderRow(string[] columns)
        {
            _writer.WriteStartElement(ROW);
            for (int i = 0; i < _columnCount; i++)
            {
                _writer.WriteStartElement(CELL_INLINE_STRING);
                _writer.WriteElement(new InlineString(new Text(columns[i])));
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();
        }

        private void SetCellTypes(object[] row)
        {
            for (int i = 0; i < _columnCount; i++)
            {
                _cellTypes.Add(
                    i, Information.IsNumeric(row[i]) 
                       ? CellValues.Number : CellValues.InlineString
                );
            }
        }

        private void WriteRow(object[] row)
        {
            _writer.WriteStartElement(ROW);
            for (int i = 0; i < _columnCount; i++)
            {
                var val = row[i].ToString();
                var type = _cellTypes[i];
                if (type == CellValues.Number)
                {
                    _writer.WriteStartElement(CELL_NUMBER);
                    _writer.WriteElement(new CellValue(val));
                }
                else
                {
                    _writer.WriteStartElement(CELL_INLINE_STRING);
                    _writer.WriteElement(new InlineString(new Text(val)));
                }
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();
        }
    }
}