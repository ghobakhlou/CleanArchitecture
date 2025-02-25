using Shared.Common;
using System;

namespace Domain.Entities;

public class Course : AggregateEntity<Guid>
{
    public string Name { get; set; }
  
}
