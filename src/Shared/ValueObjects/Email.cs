using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace Shared.ValueObjects;

public class Email : ValueObject
{
    public string EmailAddress { get;private set; }

    public Email() { }

    private Email(string emailAddress)
    {
        EmailAddress = emailAddress;
    }

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>("Email should not be empty");

        email = email.Trim();

        if (email.Length > 200)
            return Result.Failure<Email>("Email is too long");

        if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
            return Result.Failure<Email>("Email is invalid");

        return Result.Success(new Email(email));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EmailAddress;
    }

    public static implicit operator string(Email email)
    {
        return email.EmailAddress;
    }
}



