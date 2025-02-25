using System;

namespace Application.Student.Inputs;

public class EnrollStudentInput
{
    public string EmailAddress { get; set; }
    public string FirstName { get;  set; }
    public string LastName { get;  set; }
    public Guid CourseId { get;  set; }
}
