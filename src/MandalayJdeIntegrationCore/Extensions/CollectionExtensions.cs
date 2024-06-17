using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace MandalayJdeIntegrationCore.Extensions
{
    public static class CollectionExtensions
    {
        public static string ToCsv<T>(this IEnumerable<T> data)
        {
            if (data.Count() == 0)
            {
                return string.Empty;
            }

            bool isFirstIteration = true;
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (T item in data)
            {
                string[] propertyNames = item.GetType().GetProperties().Select(p => p.Name).ToArray();
                foreach (var prop in propertyNames)
                {
                    if (isFirstIteration == true)
                    {
                        for (int j = 0; j < propertyNames.Length; j++)
                        {
                            sb.Append("\"" + propertyNames[j] + "\"" + ',');
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("\r\n");
                        isFirstIteration = false;
                    }
                    object propValue = item.GetType().GetProperty(prop).GetValue(item, null);
                    sb.Append("\"" + propValue + "\"" + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        public static string ToJson<T>(this IEnumerable<T> data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}
