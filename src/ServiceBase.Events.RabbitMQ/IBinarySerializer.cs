namespace ServiceBase.Events.RabbitMQ
{
    public interface IBinarySerializer
    {
        byte[] Serialize(object obj);

        TObject Deserialize<TObject>(byte[] bytes);
    }
}