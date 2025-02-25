using Application.Common.Interfaces;
using System;

namespace Infrastructure.Services;

public class GuidService : IGuidService
{
    public Guid NewGuid => Guid.NewGuid();
}
