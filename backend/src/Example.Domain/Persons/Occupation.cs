namespace Example.Domain.Persons;

/// <summary>Master data: a selectable occupation for the registration form.</summary>
public class Occupation
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
