using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Randstad.Solutions.AspNetCoreRouting.Extensions;
using Randstad.Solutions.AspNetCoreRouting.Models;

namespace MvcApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            var languages = new[] {"fr", "en"};
            services.AddLocalizedRouting(languages, "fr");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.AddLocalizedRouting(d =>
            {
                d.Add(
                    new TranslationRouteRule(
                        "products",
                        "detail",
                        new TranslationRewriteRule[]
                        {
                            //new TranslationRewriteRule("", "", true), 
                        },
                        (controller, action, data) => { return string.Empty; }));
            });
        }
    }
}