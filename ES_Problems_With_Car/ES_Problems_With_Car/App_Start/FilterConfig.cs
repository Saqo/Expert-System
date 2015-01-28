using System.Web;
using System.Web.Mvc;

namespace ES_Problems_With_Car
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}