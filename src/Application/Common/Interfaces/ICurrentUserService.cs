#nullable enable
using System;

namespace Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? Email { get; }
    string? ObjectId { get; }
    Guid? CompanyId { get; }

    void Set(string? objectId, string? email, Guid? companyId);
}
