using System.Collections.Generic;

namespace kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables
{
    public interface IIdentifiable { int Id { get; } }

    public interface ITable
    {
        int Draw { get; set; }
        int RecordsTotal { get; set; }
        int RecordsFiltered { get; set; }
        List<List<object>> Data { get; set; }
        bool SaveAs { get; set; }
        string[] Headers { get; set; }

        void SetColumns<T>() where T : class, IIdentifiable;
        void ExecuteRequest<T>(IEnumerable<T> entities) where T : class, IIdentifiable;
    }
}