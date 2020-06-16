namespace SoneparCanada.OpenCatalog.AspNetCoreRouting.Models
{
    public class RewriteRule
    {
        public RewriteRule(string regex, string replacement, bool skipRemainingRules = true)
        {
            Regex = regex;
            Replacement = replacement;
            SkipRemainingRules = skipRemainingRules;
        }

        public string Regex { get; }

        public string Replacement { get; }

        public bool SkipRemainingRules { get; }
    }
}