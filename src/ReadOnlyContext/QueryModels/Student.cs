using Shared.Common;
using Shared.ValueObjects;
using System;
using System.Collections.Generic;

namespace ReadOnlyContext.QueryModels
{
    /// <summary>
    /// Read-only representation of a Student for query operations.
    /// 
    /// DESIGN RATIONALE:
    /// - **CQRS (Command-Query Responsibility Segregation):**
    ///   This model is optimized for read operations and decoupled from the write model,
    ///   enabling efficient data retrieval without side effects.
    /// 
    /// - **Domain-Driven Design (DDD):**
    ///   The use of value objects (Name, Email) ensures consistency with domain invariants
    ///   and promotes reuse of business rules across the system.
    /// 
    /// - **Separation of Concerns:**
    ///   By isolating the query model from the domain write model, we maintain clear boundaries,
    ///   improving maintainability and performance in data access scenarios.
    /// </summary>
    public class Student : AggregateEntity<Guid>
    {
        public Name Name { get; private set; }
        public Email Email { get; private set; }

        // Private backing list to maintain encapsulation of enrollment details.
        private readonly List<Enrollment> _enrollments = [];
        public virtual IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();
    }
}
