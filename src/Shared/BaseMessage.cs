namespace Shared;

public class BaseMessage<T>
{
    public BaseMessage()
    {

    }
    public BaseMessage(T message, string? objectId, string? email)
    {
        Message = message;
        ObjectId = objectId;
        Email = email;
    }
    public DateTimeOffset DateOccurred { get; set; } = DateTimeOffset.UtcNow;

    public string? ObjectId { get; set; }

    public string? Email { get; set; }

    public T Message { get; set; } = default!;
}