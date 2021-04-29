using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Routing.Translation.Extensions;

namespace MvcApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var languages = new[] {"fr", "en"};
            services.AddLocalizedRouting(languages, "fr");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLocalizedRoutingEndpoints();
        }
    }
}