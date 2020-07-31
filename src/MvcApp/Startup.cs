using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Routing.Translation.Extensions;
using AspNetCore.Routing.Translation.Models;

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
            app.ConfigureLocalizedRouting();
        }
    }
}