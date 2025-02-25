using MediatR;
using System.Text.Json.Serialization;

namespace Shared.Common;

public interface IHasDomainEvent
{
    public List<DomainEvent> DomainEvents { get; set; }
}

public abstract class DomainEvent : INotification
{
    protected DomainEvent(string topicName)
    {
        TopicName = topicName;
    }

    [JsonIgnore]
    public string TopicName { get; }
}
