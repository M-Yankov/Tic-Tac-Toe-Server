[assembly: Microsoft.Owin.OwinStartup(typeof(TicTacToe.WebApi.Startup))]

namespace TicTacToe.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
