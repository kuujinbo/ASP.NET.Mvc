using System.Collections.Generic;

namespace kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables
{
    public interface IIdentifiable { int Id { get; } }

    public interface ITable
    {
        void SetColumns<T>() where T : class, IIdentifiable;
        object GetData<T>(IEnumerable<T> entities) where T : class, IIdentifiable;
        string GetJson<T>(IEnumerable<T> entities) where T : class, IIdentifiable;
    }
}