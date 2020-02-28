using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MvcApp.Translation
{
    public static class StartupExtension
    {
        public static void AddLocalizedRouting(this IApplicationBuilder app)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            var rewriteOptions = new RewriteOptions();
            var assembly = typeof(StartupExtension).Assembly;
            var resourceName = $"{assembly.GetName().Name}.Translation.ApacheModRewrite.txt";
            using (var apacheModRewriteStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (apacheModRewriteStream != null)
                {
                    using (StreamReader apacheModRewriteStreamReader = new StreamReader(apacheModRewriteStream))
                    {
                        rewriteOptions.AddApacheModRewrite(apacheModRewriteStreamReader);
                    }
                }
            }

            app.UseRewriter(rewriteOptions);
            
            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture}/{controller=home}/{action=index}/{*id}",
                    constraints: new
                    {
                        culture = new RegexRouteConstraint($"^(en|fr)?$")
                    });

                endpoints.MapDynamicControllerRoute<TranslationTransformer>(
                    "{culture=en}/{controller=home}/{action=index}/{*id}");
            });
        }

        public static void AddLocalizedRouting(this IServiceCollection services)
        {
            services.AddRouting();
            services.Replace(new ServiceDescriptor(typeof(IUrlHelperFactory), new CustomUrlHelperFactory()));
            services.AddSingleton<TranslationTransformer>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("fr")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(
                    0, new RouteCultureProvider(options.DefaultRequestCulture));
            });
        }
    }
}