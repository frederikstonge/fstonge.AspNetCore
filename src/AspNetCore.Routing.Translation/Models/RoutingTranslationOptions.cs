namespace AspNetCore.Routing.Translation.Models
{
    public class RoutingTranslationOptions
    {
        public const string RoutingTranslation = "RoutingTranslation";

        public string[] SupportedCultures { get; set; }
        
        public string DefaultCulture { get; set; }
    }
}