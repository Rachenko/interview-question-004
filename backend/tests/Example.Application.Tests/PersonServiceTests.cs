using Example.Application.Occupations;
using Example.Application.Persons;
using Example.Domain.Persons;
using Xunit;

namespace Example.Application.Tests;

public class PersonServiceTests
{
    private sealed class FakePersonRepository : IPersonRepository
    {
        public Person? Saved;

        public Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default)
        {
            Saved = person;
            person.Id = 42; // simulate database-generated Id
            return Task.FromResult(person);
        }
    }

    private sealed class FakeOccupationRepository : IOccupationRepository
    {
        public Task<IReadOnlyList<Occupation>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Occupation>>(new List<Occupation>());

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(id == 1);
    }

    private static RegisterPersonRequest ValidRequest() => new()
    {
        FirstName = "Somchai",
        LastName = "Jaidee",
        Email = "somchai@example.com",
        Phone = "081-234-5678",
        Profile = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
        BirthDay = "15/08/1995",
        OccupationId = 1,
        Sex = "female"
    };

    [Fact]
    public async Task Saves_person_and_returns_generated_id()
    {
        var repo = new FakePersonRepository();
        var service = new PersonService(repo, new FakeOccupationRepository());

        var id = await service.RegisterAsync(ValidRequest());

        Assert.Equal(42, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(new DateOnly(1995, 8, 15), repo.Saved!.BirthDay);
        Assert.Equal(Sex.Female, repo.Saved.Sex);
        Assert.Equal(1, repo.Saved.OccupationId);
    }

    [Fact]
    public async Task Invalid_request_throws_and_does_not_save()
    {
        var repo = new FakePersonRepository();
        var service = new PersonService(repo, new FakeOccupationRepository());
        var request = ValidRequest();
        request.Email = "bad";

        var ex = await Assert.ThrowsAsync<ValidationException>(() => service.RegisterAsync(request));

        Assert.Contains("email", ex.Errors.Keys);
        Assert.Null(repo.Saved);
    }

    [Fact]
    public async Task Unknown_occupation_throws_and_does_not_save()
    {
        var repo = new FakePersonRepository();
        var service = new PersonService(repo, new FakeOccupationRepository());
        var request = ValidRequest();
        request.OccupationId = 999;

        var ex = await Assert.ThrowsAsync<ValidationException>(() => service.RegisterAsync(request));

        Assert.Equal("Please selected any Occupation", ex.Errors["occupation"]);
        Assert.Null(repo.Saved);
    }
}
