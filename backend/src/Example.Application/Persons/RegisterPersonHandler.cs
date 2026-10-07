using Example.Domain.Persons;

namespace Example.Application.Persons;

/// <summary>
/// Validates and registers a new person, returning the generated database Id.
/// </summary>
public class RegisterPersonHandler
{
    private readonly IPersonRepository _persons;

    public RegisterPersonHandler(IPersonRepository persons)
    {
        _persons = persons;
    }

    /// <exception cref="ValidationException">Thrown when the request fails validation.</exception>
    public async Task<int> HandleAsync(RegisterPersonRequest request, CancellationToken cancellationToken = default)
    {
        var errors = RegisterPersonValidator.Validate(request);
        if (errors.Count > 0)
            throw new ValidationException(errors);

        RegisterPersonValidator.TryParseBirthDay(request.BirthDay!, out var birthDay);
        RegisterPersonValidator.TryParseSex(request.Sex!, out var sex);

        var person = new Person
        {
            FirstName = request.FirstName!.Trim(),
            LastName = request.LastName!.Trim(),
            Email = request.Email!.Trim(),
            Phone = request.Phone!.Trim(),
            ProfileBase64 = request.Profile!,
            BirthDay = birthDay,
            Occupation = request.Occupation!.Trim(),
            Sex = sex,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _persons.AddAsync(person, cancellationToken);
        return saved.Id;
    }
}

public class ValidationException : Exception
{
    public ValidationException(IReadOnlyDictionary<string, string> errors)
        : base("The request failed validation.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string> Errors { get; }
}
