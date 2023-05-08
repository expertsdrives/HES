using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter
{
    public class SmartMeterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SmartMeter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SmartMeter_default",
                "SmartMeter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}