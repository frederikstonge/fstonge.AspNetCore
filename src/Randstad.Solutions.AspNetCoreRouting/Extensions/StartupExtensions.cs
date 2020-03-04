using System;
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
using Randstad.Solutions.AspNetCoreRouting.Models;
using Randstad.Solutions.AspNetCoreRouting.Providers;
using Randstad.Solutions.AspNetCoreRouting.Services;
using Randstad.Solutions.AspNetCoreRouting.Transformers;

namespace Randstad.Solutions.AspNetCoreRouting.Extensions
{
    public static class StartupExtensions
    {
        public static void AddLocalizedRouting(this IApplicationBuilder app, Action<List<TranslationRouteRule>> translationRouteRules)
        {
            // Use Request localization
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
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

            // Use Endpoints Configuration
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
            // Inject required services
            services.AddRouting();
            services.AddSingleton<IRouteService, RouteService>();
            services.AddSingleton<TranslationTransformer>();
            services.Replace(new ServiceDescriptor(typeof(IUrlHelperFactory), new CustomUrlHelperFactory()));
            
            // Setup localizer
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
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