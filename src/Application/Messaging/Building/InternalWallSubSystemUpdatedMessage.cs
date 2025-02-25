﻿using System;

namespace Application.Messaging.Building;

public class InternalWallSubSystemUpdatedMessage
{
    public Guid DeclarativeBuildingId { get; set; }

    public Guid PartId { get; set; }

    public Guid SubSystemId { get; set; }
}
