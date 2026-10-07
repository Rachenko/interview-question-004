using Example.Domain.Persons;

namespace Example.Application.Persons;

public interface IPersonRepository
{
    Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default);
}
