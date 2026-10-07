using Example.Application.Persons;
using Example.Domain.Persons;
using Xunit;

namespace Example.Application.Tests;

public class RegisterPersonHandlerTests
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

    private static RegisterPersonRequest ValidRequest() => new()
    {
        FirstName = "Somchai",
        LastName = "Jaidee",
        Email = "somchai@example.com",
        Phone = "081-234-5678",
        Profile = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
        BirthDay = "15/08/1995",
        Occupation = "Developer",
        Sex = "female"
    };

    [Fact]
    public async Task Saves_person_and_returns_generated_id()
    {
        var repo = new FakePersonRepository();
        var handler = new RegisterPersonHandler(repo);

        var id = await handler.HandleAsync(ValidRequest());

        Assert.Equal(42, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(new DateOnly(1995, 8, 15), repo.Saved!.BirthDay);
        Assert.Equal(Sex.Female, repo.Saved.Sex);
    }

    [Fact]
    public async Task Invalid_request_throws_and_does_not_save()
    {
        var repo = new FakePersonRepository();
        var handler = new RegisterPersonHandler(repo);
        var request = ValidRequest();
        request.Email = "bad";

        var ex = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(request));

        Assert.Contains("email", ex.Errors.Keys);
        Assert.Null(repo.Saved);
    }
}
