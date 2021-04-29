using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AspNetCore.Routing.Translation.Factories;
using AspNetCore.Routing.Translation.Models;
using AspNetCore.Routing.Translation.Providers;
using AspNetCore.Routing.Translation.Services;
using AspNetCore.Routing.Translation.Transformers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Routing.Translation.Extensions
{
    public static class StartupExtensions
    {
        public static void AddLocalizedRouting(
            this IServiceCollection services,
            ICollection<string> supportedLanguages,
            string defaultLanguage)
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

            var customTranslations = Assembly.GetCallingAssembly().GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(ICustomTranslation)));

            foreach (var customTranslation in customTranslations)
            {
                services.AddSingleton(typeof(ICustomTranslation), customTranslation);
            }

            // Setup Request localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = supportedLanguages.Select(l => new CultureInfo(l)).ToArray();
                options.DefaultRequestCulture = new RequestCulture(defaultLanguage, defaultLanguage);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(1, new RouteCultureProvider()
                {
                    Options = options
                });
            });
        }

        public static void UseLocalizedRouting(
            this IApplicationBuilder app)
        {
            // Use Request localization
            var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            var translationRouteRules = app.ApplicationServices.GetServices<ICustomTranslation>();
            app.UseRequestLocalization(locOptions.Value);

            // Use Rewrite rules
            var routeService = app.ApplicationServices.GetRequiredService<IRouteService>();
            routeService.RouteRules.AddRange(translationRouteRules);

            var rewriteOptions = new RewriteOptions();
            foreach (var routeRule in routeService.RouteRules)
            {
                foreach (var rewriteRule in routeRule.RewriteRules)
                {
                    rewriteOptions.AddRewrite(rewriteRule.Regex, rewriteRule.Replacement,
                        rewriteRule.SkipRemainingRules);
                }
            }

            app.UseRewriter(rewriteOptions);
            app.UseRouting();
        }
        
        
        public static void UseLocalizedEndpoints(
            this IApplicationBuilder app, string[] supportedLanguages)
        {
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
                    pattern: "{controller=Redirect}/{action=Index}/{*id}"
                );

                endpoints.MapDynamicControllerRoute<TranslationTransformer>(
                    "{culture}/{controller=home}/{action=index}/{*id}");
            });
        }

    }
}