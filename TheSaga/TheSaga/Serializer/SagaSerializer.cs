using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheSaga.Serializer
{
    internal sealed class SagaSerializer : ISagaSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public SagaSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings
            {
                Converters = new List<Newtonsoft.Json.JsonConverter>
                {
                    new Newtonsoft.Json.Converters.StringEnumConverter(true)
                },
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
        public string Serialize<T>(T value) => JsonConvert.SerializeObject(value, _settings);
        public string Serialize(object value) => JsonConvert.SerializeObject(value, _settings);
        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, _settings);
        public object Deserialize(string value) => JsonConvert.DeserializeObject(value, _settings);
    }
}
