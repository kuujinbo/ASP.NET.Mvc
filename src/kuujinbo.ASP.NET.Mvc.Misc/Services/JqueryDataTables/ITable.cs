using kuujinbo.ASP.NET.Mvc.Misc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables
{
    public interface ITable
    {
        void SetColumns<T>() where T : class, IIdentifiable;
        object GetData<T>(IEnumerable<T> entities) where T : class, IIdentifiable;
    }
}