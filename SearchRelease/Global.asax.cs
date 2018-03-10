using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SearchRelease
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            #region AntiForgeryConfig靜態設定
            /*
             * Cshtml在使用Claims將無法使用【@Html.AntiForgeryToken()】
             * 因為在Claims並無指定IdentityName給予使用
             * 故在Start up指定使用給AntiForgeryConfig即可
             */
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            ClaimsIdentity ClaimsIdentity = new ClaimsIdentity("Ksu", ClaimTypes.Name, ClaimTypes.Role);
            #endregion
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
