using System;
using System.Collections.Generic;

namespace Application.Messaging.Building;
public class DeclarativeBuildingCreatedMessage
{
    public Guid DeclarativeBuildingId { get; set; }

    public List<PartConfiguration> OutletSubSystems { get; set; } = [];

    public List<PartConfiguration> SwitchSubSystems { get; set; } = [];

    public List<PartConfiguration> ExternalWallSubSystems { get; set; } = [];

    public List<PartConfiguration> InternalWallsSubSystems { get; set; } = [];

    public List<PartConfiguration> RoofsSubSystems { get; set; } = [];

    public List<PartConfiguration> FloorsSubSystems { get; set; } = [];
}

public class PartConfiguration
{
    public Guid PartId { get; set; }

    public Guid SubSystemId { get; set; }
}

