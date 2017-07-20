using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc
{
    /// <summary>
    /// Provide support to apply action filters using custom criteria that
    /// can be added in global.asax Application_Start(). Reference:
    /// http://haacked.com/archive/2011/04/25/conditional-filters.aspx/
    /// </summary>
    public class ConditionalFilterProvider : IFilterProvider
    {
        private readonly
            IEnumerable<Func<ControllerContext, ActionDescriptor, object>> _conditions;

        public ConditionalFilterProvider(
            IEnumerable<Func<ControllerContext, ActionDescriptor, object>> conditions)
        {
            _conditions = conditions;
        }

        public IEnumerable<Filter> GetFilters(
            ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var result = (from condition in _conditions
               select condition(controllerContext, actionDescriptor) into filter
               where filter != null
               select new Filter(filter, FilterScope.Global, null)).Cast<Filter>();

            return result;
        }
    }
}