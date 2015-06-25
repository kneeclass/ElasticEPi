using Elasticsearch.Net.Serialization;
using Nest;

namespace ElasticEPi.Serialization {
    public class NestInheritenceSerializer : NestSerializer {
        public NestInheritenceSerializer(IConnectionSettingsValues settings) : base(settings) {}

        public override byte[] Serialize(object data, SerializationFormatting formatting = SerializationFormatting.Indented) {

            

            return base.Serialize(data, formatting);
        }
    }
}
