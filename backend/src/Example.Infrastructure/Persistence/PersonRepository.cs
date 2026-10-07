using Example.Application.Persons;
using Example.Domain.Persons;

namespace Example.Infrastructure.Persistence;

public class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _db;

    public PersonRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        _db.Persons.Add(person);
        await _db.SaveChangesAsync(cancellationToken);
        return person;
    }
}
