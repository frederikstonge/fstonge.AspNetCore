using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Factories;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Models;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Providers;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Services;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Transformers;

namespace SoneparCanada.OpenCatalog.AspNetCoreRouting.Extensions
{
    public static class StartupExtensions
    {
        public static void AddLocalizedRouting(
            this IApplicationBuilder app, 
            Action<List<CustomTranslation>> translationRouteRules)
        {
            // Use Request localization
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            
            // Use Endpoints Configuration
            var supportedLanguages = locOptions.Value.SupportedCultures
                .Select(c => c.TwoLetterISOLanguageName.ToLower());
            app.UseRequestLocalization(locOptions.Value);
            
            // Use Rewrite rules
            var routeService = app.ApplicationServices.GetRequiredService<IRouteService>();
            translationRouteRules.Invoke(routeService.RouteRules);
            var rewriteOptions = new RewriteOptions();
            foreach (var routeRule in routeService.RouteRules)
            {
                foreach (var rewriteRule in routeRule.RewriteRules)
                {
                    rewriteOptions.AddRewrite(rewriteRule.Regex, rewriteRule.Replacement, rewriteRule.SkipRemainingRules);
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
                        culture = new RegexRouteConstraint($"^({string.Join('|', supportedLanguages)})?$")
                    });
                
                endpoints.MapControllerRoute(
                    name: "default_root_redirection_with_multiple_cultures",
                    pattern: "{controller=Redirect}/{action=Index}"
                );

                endpoints.MapDynamicControllerRoute<TranslationTransformer>(
                    "{culture}/{controller=home}/{action=index}/{*id}");
            });
        }

        public static void AddLocalizedRouting(
            this IServiceCollection services,
            ICollection<string> supportedLanguages,
            string defaultLanguage, 
            string resourcePath = "Resources")
        {
            if (string.IsNullOrEmpty(defaultLanguage) ||
                supportedLanguages == null ||
                !supportedLanguages.Contains(defaultLanguage))

            {
                throw new InvalidOperationException("Supported languages must contain the default language.");
            }
            
            // Inject required services
            services.AddRouting();
            
            
            services.AddSingleton<IRouteService, RouteService>();
            services.AddSingleton<TranslationTransformer>();
            services.Replace(new ServiceDescriptor(typeof(IUrlHelperFactory), new CustomUrlHelperFactory()));
            
            // Setup localizer
            services.AddLocalization(options => options.ResourcesPath = resourcePath);
            
            // Setup Request localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = supportedLanguages.Select(l => new CultureInfo(l)).ToArray();
                options.DefaultRequestCulture = new RequestCulture(defaultLanguage, defaultLanguage);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(
                    0, new RouteCultureProvider(options.DefaultRequestCulture));
            });
        }
    }
}