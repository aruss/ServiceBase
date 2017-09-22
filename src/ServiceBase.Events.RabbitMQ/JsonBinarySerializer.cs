namespace ServiceBase.Events.RabbitMQ
{
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    public class JsonBinarySerializer : IBinarySerializer
    {
        public TObject Deserialize<TObject>(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<TObject>(reader);
            }
        }

        public byte[] Serialize(object obj)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);

                return ms.ToArray();
            }
        }
    }
}
