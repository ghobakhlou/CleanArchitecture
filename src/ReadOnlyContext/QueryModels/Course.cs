using Shared.Common;
using System;

namespace ReadOnlyContext.QueryModels;

public class Course : AggregateEntity<Guid>
{
    public string Name { get; }

    protected Course()
    {
    }
}
