using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MvcApp.Translation;

namespace MvcApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddLocalizedRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.AddLocalizedRouting();
        }
    }
}