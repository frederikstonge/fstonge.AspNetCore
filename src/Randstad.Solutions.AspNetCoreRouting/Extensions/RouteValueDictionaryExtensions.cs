using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Routing
{
    public static class RouteValueDictionaryExtensions
    {
        public static StringValues GetParameterValue(this RouteValueDictionary values, string parameterName)
        {
            if (values.TryGetValue(parameterName, out var parameterValue))
            {
                return (StringValues)parameterValue;
            }

            return default;
        }
    }
}