using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Routing.Translation.Extensions;
using AspNetCore.Routing.Translation.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using MvcApp.Filters;
using MvcApp.Translations;

namespace MvcApp
{
    public class Startup
    {
        private readonly string[] _supportedLanguages;
        private readonly string _defaultLanguage;
        
        public Startup(IConfiguration configuration)
        {
            _supportedLanguages = configuration.GetValue<string>("SupportedLanguages").Split(",", StringSplitOptions.RemoveEmptyEntries);
            _defaultLanguage = configuration.GetValue<string>("DefaultLanguage");
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddControllersWithViews(options =>
            {
                // Add filter to set current filter in cookies
                options.AddCultureCookieFilter();
                
                // Set current language of user in a session somewhere
                options.Filters.Add<SetLanguageActionFilter>();
            });
            
            // Add localization for resources, views and data annotations
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
            builder.AddDataAnnotationsLocalization();
            
            // Inject required services, add routing and replace current UrlHelperFactory
            services.AddRoutingLocalization(_supportedLanguages, _defaultLanguage);
            
            // Add custom translations as singleton
            services.AddSingleton<ICustomTranslation, ProductTranslation>();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Setup Request localization, Rewriter and Routing
            app.UseRoutingLocalization();
            app.UseStaticFiles();
            
            // Preset the UseEndpoints with correct routes for culture
            app.UseEndpointsLocalization(_supportedLanguages);
        }
    }
}