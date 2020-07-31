using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.Routing.Translation.Extensions
{
    public static class RouteValueDictionaryExtensions
    {
        public static StringValues GetParameterValue(this RouteValueDictionary values, string parameterName)
        {
            if (values.TryGetValue(parameterName, out var parameterValue))
            {
                if (parameterValue is StringValues stringValues)
                {
                    return stringValues;
                }

                return new StringValues(parameterValue.ToString());
            }

            return default;
        }
    }
}