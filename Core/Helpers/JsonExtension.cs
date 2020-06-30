using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nwassa.Core.Helpers
{
    public static class JsonExtensions
    {
        public static string ToJsonString<T>(this T data, CasingType casingType = CasingType.Pascal)
        {
            var settings = GetSelializationSettings(casingType);

            if (settings != null)
            {
                return JsonConvert.SerializeObject(data, settings);
            }

            return JsonConvert.SerializeObject(data);
        }

        public static T FromJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public static object FromJson(this string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        private static JsonSerializerSettings GetSelializationSettings(CasingType casingType)
        {
            var settings = default(JsonSerializerSettings);

            switch (casingType)
            {
                case CasingType.Camel:
                    settings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    break;

                case CasingType.Snake:
                    settings = new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
                    };
                    break;
            }

            return settings;
        }
    }
}
