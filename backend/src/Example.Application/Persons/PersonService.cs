using Example.Application.Occupations;
using Example.Domain.Persons;

namespace Example.Application.Persons;

/// <summary>
/// Validates and registers a new person, returning the generated database Id.
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _persons;
    private readonly IOccupationRepository _occupations;

    public PersonService(IPersonRepository persons, IOccupationRepository occupations)
    {
        _persons = persons;
        _occupations = occupations;
    }

    public async Task<int> RegisterAsync(RegisterPersonRequest request, CancellationToken cancellationToken = default)
    {
        var errors = RegisterPersonValidator.Validate(request);
        if (errors.Count > 0)
            throw new ValidationException(errors);

        if (!await _occupations.ExistsAsync(request.OccupationId!.Value, cancellationToken))
        {
            throw new ValidationException(new Dictionary<string, string>
            {
                ["occupation"] = "Please selected any Occupation"
            });
        }

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
            OccupationId = request.OccupationId!.Value,
            Sex = sex,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _persons.AddAsync(person, cancellationToken);
        return saved.Id;
    }
}
