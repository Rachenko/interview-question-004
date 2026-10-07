using Example.Domain.Persons;

namespace Example.Application.Occupations;

public interface IOccupationRepository
{
    Task<IReadOnlyList<Occupation>> ListAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
