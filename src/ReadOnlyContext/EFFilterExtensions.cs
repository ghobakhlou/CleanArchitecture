using Microsoft.EntityFrameworkCore;
using Shared.Common;
using System;
using System.Linq;
using System.Reflection;

namespace ReadOnlyContext;

public static class EFFilterExtensions
{
    public static void SetSoftDeleteFilter(this ModelBuilder modelBuilder, Type entityType)
    {
        SetSoftDeleteFilterMethod.MakeGenericMethod(entityType)
            .Invoke(null, new object[] { modelBuilder });
    }

    /// <summary>
    /// This method is called by <seealso cref="SetSoftDeleteFilterMethod"/>
    /// </summary>
    public static void SetSoftDeleteFilter<TEntity>(this ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeleteable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(x => !x.IsDeleted);
    }

    private static readonly MethodInfo SetSoftDeleteFilterMethod = typeof(EFFilterExtensions)
               .GetMethods(BindingFlags.Public | BindingFlags.Static)
               .Single(t => t.IsGenericMethod && t.Name == "SetSoftDeleteFilter");

}