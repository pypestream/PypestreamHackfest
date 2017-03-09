using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace PypestreamHackathon
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            AuthBot.Models.AuthSettings.Mode = "v1";
            AuthBot.Models.AuthSettings.EndpointUrl = "https://login.microsoftonline.com";
            AuthBot.Models.AuthSettings.Tenant = ConfigurationManager.AppSettings["aad:Tenant"];
            AuthBot.Models.AuthSettings.RedirectUrl = ConfigurationManager.AppSettings["aad:Redirect"];
            AuthBot.Models.AuthSettings.ClientId = ConfigurationManager.AppSettings["aad:ClientId"];
            AuthBot.Models.AuthSettings.ClientSecret = ConfigurationManager.AppSettings["aad:ClientSecret"];
        }
    }
}
