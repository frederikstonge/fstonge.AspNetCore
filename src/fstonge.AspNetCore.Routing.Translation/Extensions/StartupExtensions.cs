using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fstonge.AspNetCore.Routing.Translation.Filters;
using fstonge.AspNetCore.Routing.Translation.Helpers;
using fstonge.AspNetCore.Routing.Translation.Models;
using fstonge.AspNetCore.Routing.Translation.Providers;
using fstonge.AspNetCore.Routing.Translation.Services;
using fstonge.AspNetCore.Routing.Translation.Transformers;
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

namespace fstonge.AspNetCore.Routing.Translation.Extensions
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
                translationRoutingOptions.GetSupportedCultures() == null ||
                !translationRoutingOptions.GetSupportedCultures().Contains(translationRoutingOptions.DefaultCulture))
            {
                throw new InvalidOperationException("Supported cultures must contain the default culture.");
            }

            // Setup Request localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = translationRoutingOptions.GetSupportedCultures()
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
                    var customRules = provider.GetServices<ICustomTranslation>();
                    return new LocalizedLinkGenerator(
                        defaultLinkGenerator,
                        routeService,
                        requestLocalizationOptions,
                        customRules);
                },
                ServiceLifetime.Scoped));
        }

        /// <summary>
        /// Setup Request localization, Rewriter and Routing
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="rewriteRules">Rewrite rules</param>
        /// <param name="redirectRules">Redirect rules</param>
        public static void UseRoutingLocalization(
            this IApplicationBuilder app,
            IEnumerable<RewriteRule> rewriteRules = null,
            IEnumerable<RedirectRule> redirectRules = null)
        {
            // Use Request localization
            var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            var rewriteOptions = new RewriteOptions();

            using var scope = app.ApplicationServices.CreateScope();
            var translationRouteRules = scope.ServiceProvider.GetServices<ICustomTranslation>().ToList();
            if (translationRouteRules.Any())
            {
                foreach (var routeRule in translationRouteRules)
                {
                    foreach (var rewriteRule in routeRule.RewriteRules)
                    {
                        rewriteOptions.AddRewrite(
                            rewriteRule.Regex,
                            rewriteRule.Replacement,
                            rewriteRule.SkipRemainingRules);
                    }
                }
            }

            if (rewriteRules != null)
            {
                foreach (var rewriteRule in rewriteRules)
                {
                    rewriteOptions.AddRewrite(
                        rewriteRule.Regex,
                        rewriteRule.Replacement,
                        rewriteRule.SkipRemainingRules);
                }
            }

            if (redirectRules != null)
            {
                foreach (var redirectRule in redirectRules)
                {
                    if (redirectRule.StatusCode.HasValue)
                    {
                        rewriteOptions.AddRedirect(
                            redirectRule.Regex,
                            redirectRule.Replacement,
                            redirectRule.StatusCode.Value);
                    }
                    else
                    {
                        rewriteOptions.AddRedirect(
                            redirectRule.Regex,
                            redirectRule.Replacement);
                    }
                }
            }

            app.UseRewriter(rewriteOptions);
            app.UseRouting();
        }

        /// <summary>
        /// Preset the UseEndpoints with correct routes for culture
        /// </summary>
        /// <param name="app">Application builder</param>
        public static void UseEndpointsLocalization(this IApplicationBuilder app)
        {
            var transOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();

            if (transOptions.Value.SupportedCultures!.Count > 1)
            {
                var culturePattern = $"^({string.Join('|', transOptions.Value.SupportedCultures)})?$";
                var cultureRegex = new RegexRouteConstraint(culturePattern);

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
                        $"{{culture:regex({culturePattern})}}/{{controller=home}}/{{action=index}}/{{*id}}");
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