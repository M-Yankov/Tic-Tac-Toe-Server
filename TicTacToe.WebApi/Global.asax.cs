namespace TicTacToe.WebApi
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Http;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            DatabaseConfig.Initialize();
            AutoMapperConfig.RegisterMappings("TicTacToe.WebApi");
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
