namespace Example.Domain.Persons;

public enum Sex
{
    Male,
    Female
}

public class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    /// <summary>Profile picture stored as a Base64-encoded image.</summary>
    public string ProfileBase64 { get; set; } = string.Empty;

    public DateOnly BirthDay { get; set; }

    public string Occupation { get; set; } = string.Empty;

    public Sex Sex { get; set; }

    public DateTime CreatedAt { get; set; }
}
