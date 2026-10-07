using Example.Domain.Persons;

namespace Example.Application.Occupations;

public class OccupationService : IOccupationService
{
    private readonly IOccupationRepository _occupations;

    public OccupationService(IOccupationRepository occupations)
    {
        _occupations = occupations;
    }

    public Task<IReadOnlyList<Occupation>> ListAsync(CancellationToken cancellationToken = default)
        => _occupations.ListAsync(cancellationToken);
}
