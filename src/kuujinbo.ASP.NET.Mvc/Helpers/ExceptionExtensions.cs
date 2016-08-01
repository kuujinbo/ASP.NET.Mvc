using System;
using System.Collections.Generic;

namespace kuujinbo.ASP.NET.Mvc.Helpers
{
    public static class ExceptionExtensions
    {
        public static IList<Exception> GetInnerExceptions(this Exception exception)
        {
            var result = new List<Exception>();

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                result.Add(exception);
            }

            return result;
        }
    }
}