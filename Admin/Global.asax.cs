using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Application has rendered page, we want to audit if anything funky went on
        /// </summary>
        void Application_EndRequest(object sender, System.EventArgs e)
        {
            string aspxerrorpath = Request.RawUrl;

            // If the user is not authorised to see this page or access this function, send them to the error page.
            switch (Response.StatusCode)
            {
                case 401:
                    if (Request.Headers["Authorization"] != null)
                    { // failed password attempt
                        AuditUtil.AuditGenericError(this.Context, AtnaApi.Model.OutcomeIndicator.EpicFail, AtnaApi.Model.EventIdentifierType.ApplicationActivity, AtnaApi.Model.EventIdentificationType.EventType_UserAuthentication, aspxerrorpath);
                        Response.RedirectToRoute("Default", new { controller = "Error", action = "Unauthorized", aspxerrorpath = Request.RawUrl });
                    }
                    break;
                case 403:
                    Response.ClearContent();
                    AuditUtil.AuditGenericError(this.Context, AtnaApi.Model.OutcomeIndicator.EpicFail, AtnaApi.Model.EventIdentifierType.ApplicationActivity, AtnaApi.Model.EventIdentificationType.EventType_UseOfARestrictedFunction, aspxerrorpath);
                    Response.RedirectToRoute("Default", new { controller = "Error", action = "Forbidden", aspxerrorpath = Request.RawUrl });
                    break;
                case 404:
                    Response.ClearContent();
                    AuditUtil.AuditGenericError(this.Context, AtnaApi.Model.OutcomeIndicator.SeriousFail, AtnaApi.Model.EventIdentifierType.ApplicationActivity, AtnaApi.Model.EventIdentificationType.EventType_Query, aspxerrorpath);
                    Response.RedirectToRoute("Default", new { controller = "Error", action = "NotFound", aspxerrorpath = Request.RawUrl });
                    break;
                case 503:
                    Response.ClearContent();
                    Response.RedirectToRoute("Default", new { controller = "Error", action = "Unavailable", aspxerrorpath = Request.RawUrl });
                    break;

            }

        }
    }
}
