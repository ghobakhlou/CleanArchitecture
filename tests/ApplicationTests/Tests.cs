using Application.Student.Commands;
using Application.Student.Inputs;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests;


// In-memory test suite for the EnrollStudentCommandHandler.
public class EnrollStudentCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private Course CreateTestCourse(string courseName) => new() { Id = Guid.NewGuid(), Name = courseName };

    public EnrollStudentCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CourseDoesNotExist()
    {
        // Arrange: Create input with a random course Id (course not in the DB).
        var input = new EnrollStudentInput
        {
            CourseId = Guid.NewGuid(),
            EmailAddress = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var command = new EnrollStudentCommand(input);
        var handler = new EnrollStudentCommandHandler(_context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Course doesn't exist.", result.Error);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmailIsInvalid()
    {
        // Arrange: Create a valid course.
        var course = CreateTestCourse("Mathematics");

        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        // Arrange: Invalid email input.
        var input = new EnrollStudentInput
        {
            CourseId = course.Id,
            EmailAddress = "invalid-email", // Expected to fail validation.
            FirstName = "John",
            LastName = "Doe"
        };
        var command = new EnrollStudentCommand(input);
        var handler = new EnrollStudentCommandHandler(_context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Email is invalid.", result.Error);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange: Create a valid course.
        var course = CreateTestCourse("Mathematics");
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        // Arrange: Invalid name input (empty first name).
        var input = new EnrollStudentInput
        {
            CourseId = course.Id,
            EmailAddress = "john.doe@example.com",
            FirstName = "", // Invalid name.
            LastName = "Doe"
        };
        var command = new EnrollStudentCommand(input);
        var handler = new EnrollStudentCommandHandler(_context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Name is invalid.", result.Error);
    }

    [Fact]
    public async Task Handle_Should_CreateNewStudent_When_StudentDoesNotExist()
    {
        // Arrange: Create a valid course.
        var course = CreateTestCourse("Mathematics");
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        var input = new EnrollStudentInput
        {
            CourseId = course.Id,
            EmailAddress = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var command = new EnrollStudentCommand(input);
        var handler = new EnrollStudentCommandHandler(_context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        // Verify the student is persisted and has one enrollment.
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == "john.doe@example.com");
        Assert.NotNull(student);
        Assert.Single(student.Enrollments);
    }

    [Fact]
    public async Task Handle_Should_EnrollExistingStudent_InNewCourse()
    {
        // Arrange: Create two valid courses.
        var course1 = CreateTestCourse("Mathematics");
        var course2 = CreateTestCourse("Physics");

        await _context.Courses.AddAsync(course1);
        await _context.Courses.AddAsync(course2);
        await _context.SaveChangesAsync();

        // First command: Enroll student in course1.
        var input1 = new EnrollStudentInput
        {
            CourseId = course1.Id,
            EmailAddress = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var command1 = new EnrollStudentCommand(input1);
        var handler = new EnrollStudentCommandHandler(_context);
        var result1 = await handler.Handle(command1, CancellationToken.None);
        Assert.True(result1.IsSuccess);

        // Second command: Enroll the same student in course2.
        var input2 = new EnrollStudentInput
        {
            CourseId = course2.Id,
            EmailAddress = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var command2 = new EnrollStudentCommand(input2);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.True(result2.IsSuccess);
        // Verify the student now has two enrollments.
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == "john.doe@example.com");
        Assert.NotNull(student);
        Assert.Equal(2, student.Enrollments.Count);
    }
}
