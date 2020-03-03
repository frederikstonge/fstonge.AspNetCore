using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Randstad.Solutions.AspNetCoreRouting.Extensions;

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
            app.AddLocalizedRouting();
        }
    }
}