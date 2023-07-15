using MordenDoors.CustomFilter;
using System.Web.Mvc;

namespace MordenDoors
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionHandlerAttribute());
        }
    }
}
