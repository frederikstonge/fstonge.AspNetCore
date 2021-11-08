using System;
using System.Globalization;
using System.Linq;
using AspNetCore.Routing.Translation.Filters;
using AspNetCore.Routing.Translation.Helpers;
using AspNetCore.Routing.Translation.Models;
using AspNetCore.Routing.Translation.Providers;
using AspNetCore.Routing.Translation.Services;
using AspNetCore.Routing.Translation.Transformers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Routing.Translation.Extensions
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Add filter to set culture in cookies
        /// </summary>
        /// <param name="options">MvcOptions from AddControllersWithViews</param>
        public static void AddCultureCookieFilter(this MvcOptions options)
        {
            options.Filters.Add<SetCultureCookieActionFilter>();
        }
        
        /// <summary>
        /// Inject required services, add routing and replace current UrlHelperFactory
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <exception cref="InvalidOperationException">Supported languages must contain the default language.</exception>
        public static void AddRoutingLocalization(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            RoutingTranslationOptions translationRoutingOptions = new RoutingTranslationOptions();
            configuration.GetSection(RoutingTranslationOptions.RoutingTranslation).Bind(translationRoutingOptions);
            
            if (string.IsNullOrEmpty(translationRoutingOptions.DefaultCulture) ||
                translationRoutingOptions.SupportedCultures == null ||
                !translationRoutingOptions.SupportedCultures.Contains(translationRoutingOptions.DefaultCulture))
            {
                throw new InvalidOperationException("Supported cultures must contain the default culture.");
            }
            
            // Setup Request localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = translationRoutingOptions.SupportedCultures
                    .Select(l => new CultureInfo(l))
                    .ToList();
                
                options.DefaultRequestCulture = new RequestCulture(
                    translationRoutingOptions.DefaultCulture, 
                    translationRoutingOptions.DefaultCulture);
                
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(1, new RouteCultureProvider()
                {
                    Options = options
                });
            });

            // Inject required services
            services.AddRouting();
            services.AddSingleton<IRouteService, RouteService>();
            services.AddSingleton<TranslationTransformer>();

            var serviceProvider = services.BuildServiceProvider();
            var defaultLinkGenerator = serviceProvider.GetRequiredService<LinkGenerator>();

            services.Replace(new ServiceDescriptor(
                typeof(LinkGenerator),
                provider =>
                {
                    var routeService = provider.GetService<IRouteService>();
                    var requestLocalizationOptions = provider.GetService<IOptions<RequestLocalizationOptions>>();
                    return new LocalizedLinkGenerator(defaultLinkGenerator, routeService, requestLocalizationOptions);
                },
                ServiceLifetime.Singleton));
        }

        /// <summary>
        /// Setup Request localization, Rewriter and Routing
        /// </summary>
        /// <param name="app">Application builder</param>
        public static void UseRoutingLocalization(this IApplicationBuilder app)
        {
            // Use Request localization
            var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            var translationRouteRules = app.ApplicationServices.GetServices<ICustomTranslation>().ToList();
            if (translationRouteRules.Any())
            {
                // Use Rewrite rules
                var routeService = app.ApplicationServices.GetRequiredService<IRouteService>();
                routeService.RouteRules.AddRange(translationRouteRules);

                var rewriteOptions = new RewriteOptions();
                foreach (var routeRule in routeService.RouteRules)
                {
                    foreach (var rewriteRule in routeRule.RewriteRules)
                    {
                        rewriteOptions.AddRewrite(
                            rewriteRule.Regex, 
                            rewriteRule.Replacement,
                            rewriteRule.SkipRemainingRules);
                    }
                }

                app.UseRewriter(rewriteOptions);
            }

            app.UseRouting();
        }
        
        /// <summary>
        /// Preset the UseEndpoints with correct routes for culture
        /// </summary>
        /// <param name="app">Application builder</param>
        public static void UseEndpointsLocalization(this IApplicationBuilder app)
        {
            var transOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();

            if (transOptions.Value.SupportedCultures.Count > 1)
            {
                var cultureRegex =
                    new RegexRouteConstraint(
                        $"^({string.Join('|', transOptions.Value.SupportedCultures)})?$");

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{culture}/{controller=home}/{action=index}/{*id}",
                        constraints: new
                        {
                            culture = cultureRegex
                        });

                    endpoints.MapControllerRoute(
                        name: "default_root_redirection_with_multiple_cultures",
                        pattern: "{controller=Redirect}/{action=Index}/{*id}");

                    endpoints.MapDynamicControllerRoute<TranslationTransformer>(
                        "{culture}/{controller=home}/{action=index}/{*id}");
                });
            }
            else
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=home}/{action=index}/{*id}");
                    
                    endpoints.MapDynamicControllerRoute<TranslationTransformer>(
                        "{controller=home}/{action=index}/{*id}");
                });
            }
        }
    }
}