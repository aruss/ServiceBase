namespace ServiceBase.Events.RabbitMQ
{
    public class MessagePackSerializer : IBinarySerializer
    {
        public TObject Deserialize<TObject>(byte[] bytes)
        {
            return MessagePack.MessagePackSerializer
                .Deserialize<TObject>(bytes);
        }

        public byte[] Serialize(object obj)
        {
            return MessagePack.MessagePackSerializer.Serialize(obj);
        }
    }
}