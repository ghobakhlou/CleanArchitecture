using Shared.Common;
using Shared.ValueObjects;
using System;

namespace Domain.Events;
public sealed class StudentEmailChangedEvent(Guid studentId, Email newEmail) : DomainEvent(TopicNames.StudentEmailChanged.TopicName)
{
    public Guid StudentId { get; } = studentId;
    public Email NewEmail { get; } = newEmail;
}
