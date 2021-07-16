using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Routing.Translation.Extensions;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.Routing.Translation.Models
{
    public class RouteInfo
    {
        public string Culture => RouteValues.FirstOrDefault(r => r.Key.Equals(nameof(Culture), StringComparison.OrdinalIgnoreCase)).Value;

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Id => RouteValues.FirstOrDefault(r => r.Key.Equals(nameof(Id), StringComparison.OrdinalIgnoreCase)).Value;

        public IList<KeyValuePair<string, StringValues>> RouteValues { get; set; }

        public void ChangeQueryValue(string key, object value)
        {
            RouteValues.ChangeParameterValue(key, value.ToString());
        }

        public void AddQueryValue(string key, object value)
        {
            if (value != null)
            {
                RouteValues.AddParameterValue(key, value.ToString());
            }
        }

        public void RemoveQueryValue(string key, object value)
        {
            if (value != null)
            {
                RouteValues.RemoveParameterValue(key, value.ToString());
            }
        }

        public void RemoveQueryKey(string key)
        {
            if (RouteValues.Any(r => r.Key == key))
            {
                var keyObject = RouteValues.FirstOrDefault(r => r.Key == key);
                RouteValues.Remove(keyObject);
            }
        }

        public bool HasQueryValue(string key, object value)
        {
            return RouteValues
                .Any(r => r.Key.Equals(key, StringComparison.OrdinalIgnoreCase) &&
                          r.Value.Any(s => s.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase)));
        }

        public bool HasQueryKey(string key)
        {
            return RouteValues.Any(r => r.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public string GetQueryValue(string key)
        {
            if (HasQueryKey(key))
            {
                return RouteValues.FirstOrDefault(r => r.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
            }

            return null;
        }
    }
}