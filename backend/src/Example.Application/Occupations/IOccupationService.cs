using Example.Domain.Persons;

namespace Example.Application.Occupations;

public interface IOccupationService
{
    Task<IReadOnlyList<Occupation>> ListAsync(CancellationToken cancellationToken = default);
}
