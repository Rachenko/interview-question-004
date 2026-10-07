namespace Example.Application.Persons;

public class RegisterPersonRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    /// <summary>Profile picture as a Base64 string (may include a data: URI prefix).</summary>
    public string? Profile { get; set; }

    /// <summary>Birth day in day/month/year format, e.g. 15/08/1995.</summary>
    public string? BirthDay { get; set; }

    public string? Occupation { get; set; }

    public string? Sex { get; set; }
}
