using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameTools.Web.Middleware
{
    public class ClientIdInjector
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration config;

        public ClientIdInjector(RequestDelegate next, IConfiguration config)
        {
            this.next = next;
            this.config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            this.BeginInvoke(context);
            await this.next.Invoke(context);
            this.EndInvoke(context);
        }

        private void BeginInvoke(HttpContext context)
        {
            context.Items["GameTools:Authentication:Google:ClientId"] = config["GameTools:Authentication:Google:ClientId"];
        }

        private void EndInvoke(HttpContext context)
        {

        }

    }
}
