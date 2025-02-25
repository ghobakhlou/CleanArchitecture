using System;

namespace Application.Messaging.Building;

public class SubSystemFileUploadedMessage
{
    public Guid SubSystemId { get; set; }

    public string FileUrl { get; set; }

}