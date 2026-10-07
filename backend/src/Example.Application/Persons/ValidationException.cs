namespace Example.Application.Persons;

public class ValidationException : Exception
{
    public ValidationException(IReadOnlyDictionary<string, string> errors)
        : base("The request failed validation.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string> Errors { get; }
}
