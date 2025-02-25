using System;

namespace Application.Messaging.Building;

public class OutletSubSystemUpdatedMessage
{
    public Guid DeclarativeBuildingId { get; set; }

    public Guid PartId { get; set; }

    public Guid SubSystemId { get; set; }
}
