namespace TicTacToe.WebApi
{
    using System.Web;
    using System.Web.Http;
    using Data;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            DatabaseConfig.Initialize(DefaultDbContext.Create());
            AutoMapperConfig.RegisterMappings("TicTacToe.WebApi");
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
