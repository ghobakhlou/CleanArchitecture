using global::Domain.Entities;
using global::Domain.Events;
using Shared.ValueObjects;
using System;
using Xunit;

namespace DomainTests;



public class StudentTests
{
    private Name CreateTestName() => Name.Create("John", "Doe").Value;
    private Email CreateTestEmail(string emailAddress = "john.doe@example.com") => Email.Create(emailAddress).Value;

    private Course CreateTestCourse(string courseName) => new Course() { Id = Guid.NewGuid(), Name = courseName};

    [Fact]
    public void CreatingStudent_Should_EnrollInInitialCourse()
    {
        // Arrange
        var initialCourse = CreateTestCourse("Mathematics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();

        // Act
        var student = new Student(id, name, email, initialCourse);

        // Assert
        Assert.NotNull(student);
        Assert.Single(student.Enrollments);
    }

    [Fact]
    public void EnrollIn_NewCourse_Should_Succeed()
    {
        // Arrange
        var initialCourse = CreateTestCourse("Mathematics");
        var additionalCourse = CreateTestCourse("Physics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();

        var student = new Student(id, name, email, initialCourse);

        // Act
        var result = student.EnrollIn(additionalCourse);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, student.Enrollments.Count);
    }

    [Fact]
    public void EnrollIn_ExistingCourse_Should_Fail()
    {
        // Arrange
        var course = CreateTestCourse("Mathematics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();

        var student = new Student(id, name, email, course);

        // Act
        var result = student.EnrollIn(course);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Already enrolled", result.Error);
        Assert.Single(student.Enrollments);
    }

    [Fact]
    public void Disenroll_EnrolledCourse_Should_Succeed()
    {
        // Arrange
        var initialCourse = CreateTestCourse("Mathematics");
        var additionalCourse = CreateTestCourse("Physics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();

        var student = new Student(id, name, email, initialCourse);
        student.EnrollIn(additionalCourse);

        // Act
        var result = student.Disenroll(additionalCourse);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(student.Enrollments);
    }

    [Fact]
    public void Disenroll_NotEnrolledCourse_Should_Fail()
    {
        // Arrange
        var course = CreateTestCourse("Mathematics");
        var notEnrolledCourse = CreateTestCourse("Physics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();

        var student = new Student(id, name, email, course);

        // Act
        var result = student.Disenroll(notEnrolledCourse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not enrolled", result.Error);
        Assert.Single(student.Enrollments);
    }

    [Fact]
    public void EditPersonalInfo_WithEmailChange_Should_AddDomainEvent()
    {
        // Arrange
        var initialCourse = CreateTestCourse("Mathematics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();
        var student = new Student(id, name, email, initialCourse);

        var newEmail = CreateTestEmail("new.email@example.com");

        // Act
        var result = student.EditPersonalInfo(name, newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, student.Email);
        Assert.NotEmpty(student.DomainEvents);
        var domainEvent = student.DomainEvents[0] as StudentEmailChangedEvent;
        Assert.NotNull(domainEvent);
        Assert.Equal(student.Id, domainEvent.StudentId);
    }

    [Fact]
    public void EditPersonalInfo_WithNullName_Should_Fail()
    {
        // Arrange
        var initialCourse = CreateTestCourse("Mathematics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();
        var student = new Student(id, name, email, initialCourse);

        // Act
        var result = student.EditPersonalInfo(null, email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Name is empty", result.Error);
    }

    [Fact]
    public void EditPersonalInfo_WithNullEmail_Should_Fail()
    {
        // Arrange
        var initialCourse = CreateTestCourse("Mathematics");
        var id = Guid.NewGuid();
        var name = CreateTestName();
        var email = CreateTestEmail();
        var student = new Student(id, name, email, initialCourse);

        // Act
        var result = student.EditPersonalInfo(name, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Email is empty", result.Error);
    }
}
