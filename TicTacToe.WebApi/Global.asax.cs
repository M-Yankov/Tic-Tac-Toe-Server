namespace TicTacToe.WebApi
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Data;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            DatabaseConfig.Initialize(DefaultDbContext.Create());
            AutoMapperConfig.RegisterMappings("TicTacToe.WebApi");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            TimeSpan elapsed = stopWatch.Elapsed;

            stopWatch.Stop();
            string result = elapsed.ToString();

            this.Application.Add("elapsed", result);
        }
    }
}
