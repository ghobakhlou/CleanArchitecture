namespace Shared.Common;

public interface ISoftDeleteable
{
    bool IsDeleted { get; set; }
}

