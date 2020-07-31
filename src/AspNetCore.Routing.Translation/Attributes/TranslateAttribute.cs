using System;

namespace AspNetCore.Routing.Translation.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]  
    public class TranslateAttribute : Attribute
    {
        public TranslateAttribute(string culture, string value)
        {
            Culture = culture;
            Value = value;
        }
        public string Culture { get; }
        public string Value { get; }
    }
}