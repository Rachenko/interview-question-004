using Example.Application.Occupations;
using Example.Domain.Persons;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure.Persistence;

public class OccupationRepository : IOccupationRepository
{
    private readonly AppDbContext _db;

    public OccupationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Occupation>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.Occupations.OrderBy(o => o.Id).ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _db.Occupations.AnyAsync(o => o.Id == id, cancellationToken);
}
