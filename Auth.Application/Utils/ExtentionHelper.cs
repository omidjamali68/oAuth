using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json;

namespace Auth.Application.Utils;
public static class ExtentionHelper 
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName() ?? enumValue.ToString();
        }

        public static T DeserializeData<T>(this object data)
        {
            string json = JsonConvert.SerializeObject(data);
            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }