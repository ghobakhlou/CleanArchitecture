using CSharpFunctionalExtensions;
using Domain.Events;
using MediatR;
using Shared.Common;
using Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a Student aggregate in the domain.
    /// 
    /// DESIGN HIGHLIGHTS:
    /// - **Domain-Driven Design (DDD):**
    ///   * Uses value objects (Name, Email) to enforce invariants and encapsulate business rules.
    ///   * A student is created with an initial enrollment, aligning with the business process, 
    ///     but may later disenroll from all courses while still existing as an entity.
    /// 
    /// - **CQRS & Functional Programming:**
    ///   * Methods return explicit Result types (using CSharpFunctionalExtensions) to manage outcomes
    ///     in a functional, predictable manner.
    /// 
    /// - **Domain Events:**
    ///   * Emits events (e.g., StudentEmailChangedEvent) to decouple side effects and support eventual consistency.
    /// 
    /// ASSUMPTION:
    /// - A student is created when they first enroll in a course, but subsequent disenrollment from all courses 
    ///   does not remove the student from the system.
    /// </summary>
    public class Student : AggregateEntity<Guid>, IHasDomainEvent
    {
        public virtual Name Name { get; private set; }
        public Email Email { get; private set; }

        // Private backing list to maintain encapsulation of enrollment details.
        private readonly List<Enrollment> _enrollments = new List<Enrollment>();
        public virtual IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();

        // Collection to track domain events for integration with event handlers.
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        // Parameterless constructor for ORM and serialization purposes.
        protected Student()
        {
        }

        /// <summary>
        /// Initializes a new Student and enforces the business process that a student is created upon their first enrollment.
        /// </summary>
        /// <param name="id">Unique identifier for the student.</param>
        /// <param name="name">Validated name value object.</param>
        /// <param name="email">Validated email value object.</param>
        /// <param name="course">The course for initial enrollment.</param>
        public Student(Guid id, Name name, Email email, Course course)
        {
            Name = name;
            Email = email;
            Id = id;
            // Enforce that the student is created with an enrollment.
            EnrollIn(course);
        }

        /// <summary>
        /// Enrolls the student in the specified course if not already enrolled.
        /// Returns a Result indicating success or failure using a functional approach.
        /// </summary>
        /// <param name="course">The course to enroll in.</param>
        /// <returns>A success Result if enrolled; otherwise, a failure Result with an error message.</returns>
        public Result<Unit> EnrollIn(Course course)
        {
            if (_enrollments.Any(x => x.Course == course))
                return Result.Failure<Unit>($"Already enrolled in course '{course.Name}'");

            var enrollment = new Enrollment(course, this);
            _enrollments.Add(enrollment);

            return Result.Success(Unit.Value);
        }

        /// <summary>
        /// Disenrolls the student from the specified course.
        /// Returns a Result to clearly indicate the outcome.
        /// </summary>
        /// <param name="course">The course to disenroll from.</param>
        /// <returns>A success Result if disenrollment occurs; otherwise, a failure Result with an error message.</returns>
        public Result<Unit> Disenroll(Course course)
        {
            Enrollment enrollment = _enrollments.FirstOrDefault(x => x.Course == course);

            if (enrollment == null)
            {
                return Result.Failure<Unit>($"Student is not enrolled in course '{course.Name}'");
            }

            _enrollments.Remove(enrollment);
            return Result.Success(Unit.Value);
        }

        /// <summary>
        /// Updates the student's personal information.
        /// If the email changes, a domain event is emitted to handle side effects.
        /// Returns a Result indicating whether the update was successful.
        /// </summary>
        /// <param name="name">The new name value object.</param>
        /// <param name="email">The new email value object.</param>
        /// <returns>A success Result if updated; otherwise, a failure Result with an error message.</returns>
        public Result<Unit> EditPersonalInfo(Name name, Email email)
        {
            if (name is null)
            {
                return Result.Failure<Unit>($"Name is empty");
            }
            if (email is null)
            {
                return Result.Failure<Unit>($"Email is empty");
            }

            // Emit a domain event if the email is changed to decouple side effects.
            if (Email != email)
            {
                DomainEvents.Add(new StudentEmailChangedEvent(Id, email));
            }

            Name = name;
            Email = email;

            return Result.Success(Unit.Value);
        }
    }
}
