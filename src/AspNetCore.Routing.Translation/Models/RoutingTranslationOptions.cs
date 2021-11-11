using System.Linq;

namespace AspNetCore.Routing.Translation.Models
{
    internal class RoutingTranslationOptions
    {
        public const string RoutingTranslation = "RoutingTranslation";

        public string SupportedCultures { get; set; }
        
        public string DefaultCulture { get; set; }

        public string[] GetSupportedCultures()
        {
            return SupportedCultures?
                .Split(",")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToArray();
        }
    }
}