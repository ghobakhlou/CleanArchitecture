using HotChocolate.Types;
using ReadOnlyContext.QueryModels;

namespace ReadOnlyContext.ObjectTypes;
public class AssuranceDataObjectType : ObjectType<AssuranceData>
{
    protected override void Configure(IObjectTypeDescriptor<AssuranceData> descriptor)
    {

        descriptor.Ignore(p => p.IsDeleted);
        descriptor.Ignore(p => p.Tenants);
        base.Configure(descriptor);
    }
}

