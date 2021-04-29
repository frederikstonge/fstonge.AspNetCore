using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Routing.Translation.Extensions;
using AspNetCore.Routing.Translation.Filters;
using AspNetCore.Routing.Translation.Transformers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using MvcApp.Filters;

namespace MvcApp
{
    public class Startup
    {
        private string[] _supportedLanguages;
        private string _defaultLanguage;
        
        public Startup(IConfiguration configuration)
        {
            _supportedLanguages = configuration.GetValue<string>("SupportedLanguages").Split(",", StringSplitOptions.RemoveEmptyEntries);
            _defaultLanguage = configuration.GetValue<string>("DefaultLanguage");
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddControllersWithViews(options =>
            {
                options.Filters.Add<SetCultureCookieActionFilter>();
                options.Filters.Add<SetLanguageActionFilter>();
            });
            
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
            builder.AddDataAnnotationsLocalization();
            
            services.AddLocalizedRouting(_supportedLanguages, _defaultLanguage);
            
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLocalizedRouting();
            app.UseStaticFiles();
            app.UseLocalizedEndpoints(_supportedLanguages);
        }
    }
}