using System.Collections;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace MvcApp.Translation
{
    public static class ResourceManagerExtension
    {
        public static string GetKeyByValue(this ResourceManager rs, string value, CultureInfo currentCulture)
        {
            var entry = rs.GetResourceSet(currentCulture, true, true)
                    .OfType<DictionaryEntry>()
                    .FirstOrDefault(e => e.Value.ToString() == value);
            var key = entry.Key.ToString();
            return key;
        }
    }
}