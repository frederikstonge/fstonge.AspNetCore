using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MvcApp
{
    /// <summary>
    /// https://stackoverflow.com/questions/38170739/handle-culture-in-route-url-via-requestcultureproviders
    /// </summary>
    public class RouteCultureProvider : IRequestCultureProvider
    {
        private readonly CultureInfo defaultCulture;
        private readonly CultureInfo defaultUICulture;

        public RouteCultureProvider(RequestCulture requestCulture)
        {
            defaultCulture = requestCulture.Culture;
            defaultUICulture = requestCulture.UICulture;
        }

        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var parts = httpContext.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
            // Test any culture in route
            if (!parts.Any())
            {
                // Set default Culture and default UICulture
                return Task.FromResult(
                    new ProviderCultureResult(
                        this.defaultCulture.TwoLetterISOLanguageName
                        , this.defaultUICulture.TwoLetterISOLanguageName)
                    );
            }

            
            var culture = parts.First();

            // Test if the culture is properly formatted
            if (!Regex.IsMatch(culture, @"^[a-z]{2}(-[A-Z]{2})*$"))
            {
                // Set default Culture and default UICulture
                return Task.FromResult(
                    new ProviderCultureResult(
                        this.defaultCulture.TwoLetterISOLanguageName
                        , this.defaultUICulture.TwoLetterISOLanguageName)
                    );
            }

            // Set Culture and UICulture from route culture parameter
            return Task.FromResult(new ProviderCultureResult(culture, culture));
        }
    }
}
