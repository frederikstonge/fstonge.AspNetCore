namespace Randstad.Solutions.AspNetCoreRouting.Models
{
    public class TranslationRewriteRule
    {
        public TranslationRewriteRule(string regex, string replacement, bool skipRemainingRules)
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