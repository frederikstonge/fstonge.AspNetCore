using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Randstad.Solutions.AspNetCoreRouting.Factories;
using Randstad.Solutions.AspNetCoreRouting.Providers;
using Randstad.Solutions.AspNetCoreRouting.Transformers;

namespace Randstad.Solutions.AspNetCoreRouting.Extensions
{
    public static class StartupExtensions
    {
        public static void AddLocalizedRouting(this IApplicationBuilder app)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            // TODO: Make logic to inject config for rewrite and generate url
            var rewriteOptions = new RewriteOptions();
            var assembly = Assembly.GetEntryAssembly();
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

            var supportedLanguages = locOptions.Value.SupportedCultures.Select(c => c.TwoLetterISOLanguageName.ToLower());
            var defaultLanguage = locOptions.Value.DefaultRequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
            
            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture}/{controller=home}/{action=index}/{*id}",
                    constraints: new
                    {
                        culture = new RegexRouteConstraint($"^({string.Join('|', supportedLanguages)})?$")
                    });

                endpoints.MapDynamicControllerRoute<TranslationTransformer>(
                    "{culture="+ defaultLanguage +"}/{controller=home}/{action=index}/{*id}");
            });
        }

        public static void AddLocalizedRouting(
            this IServiceCollection services,
            IEnumerable<string> supportedLanguages,
            string defaultLanguage)
        {
            services.AddRouting();
            services.Replace(new ServiceDescriptor(typeof(IUrlHelperFactory), new CustomUrlHelperFactory()));
            services.AddSingleton<TranslationTransformer>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = supportedLanguages.Select(l => new CultureInfo(l)).ToArray();
                options.DefaultRequestCulture = new RequestCulture(defaultLanguage, defaultLanguage);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(
                    1, new RouteCultureProvider(options.DefaultRequestCulture));
            });
        }
    }
}