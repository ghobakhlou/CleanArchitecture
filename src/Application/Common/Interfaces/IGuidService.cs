using System;

namespace Application.Common.Interfaces;

public interface IGuidService
{
    Guid NewGuid { get; }
}
