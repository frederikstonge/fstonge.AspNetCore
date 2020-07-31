using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.Routing.Translation.Providers
{
    internal class RouteCultureProvider : IRequestCultureProvider
    {
        private readonly CultureInfo _defaultCulture;
        private readonly CultureInfo _defaultUiCulture;

        public RouteCultureProvider(RequestCulture requestCulture)
        {
            _defaultCulture = requestCulture.Culture;
            _defaultUiCulture = requestCulture.UICulture;
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
                        _defaultCulture.TwoLetterISOLanguageName
                        , _defaultUiCulture.TwoLetterISOLanguageName)
                    );
            }

            
            var culture = parts.First();

            // Test if the culture is properly formatted
            if (!Regex.IsMatch(culture, @"^[a-z]{2}(-[A-Z]{2})*$"))
            {
                // Set default Culture and default UICulture
                return Task.FromResult(
                    new ProviderCultureResult(
                        _defaultCulture.TwoLetterISOLanguageName
                        , _defaultUiCulture.TwoLetterISOLanguageName)
                    );
            }

            // Set Culture and UICulture from route culture parameter
            return Task.FromResult(new ProviderCultureResult(culture, culture));
        }
    }
}
