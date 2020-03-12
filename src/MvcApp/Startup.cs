using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
                    new CustomTranslation(
                        "products",
                        "detail",
                        new []
                        {
                            new RewriteRule(
                                @"^([a-zA-Z]{2})\/([^\/]+)\/[-\/a-zA-Z0-9]+\/p-([=._a-zA-Z0-9]+)-.*$", 
                                "$1/$2/detail/$3"),
                        },
                        (culture, controllerValue, actionValue, values, fragment) =>
                        {
                            var id = values.GetParameterValue("id");

                            return $"/{culture}/" +
                                   $"{controllerValue}/" +
                                   $"10-control-and-testing/" +
                                   $"14-testing-string/" +
                                   $"p-{id}-testing-product-string";
                        }));
            });
        }
    }
}