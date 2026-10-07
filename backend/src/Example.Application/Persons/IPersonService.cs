namespace Example.Application.Persons;

public interface IPersonService
{
    /// <exception cref="ValidationException">Thrown when the request fails validation.</exception>
    Task<int> RegisterAsync(RegisterPersonRequest request, CancellationToken cancellationToken = default);
}
