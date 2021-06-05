namespace AspNetCore.Routing.Translation.Models
{
    public class TranslationRoutingOptions
    {
        public const string TranslationRouting = "TranslationRouting";

        public string[] SupportedLanguages { get; set; }
        
        public string DefaultLanguage { get; set; }
    }
}