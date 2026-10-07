using System.Globalization;
using System.Text.RegularExpressions;

namespace Example.Application.Persons;

/// <summary>
/// Validates a <see cref="RegisterPersonRequest"/> and collects one error per
/// field, so the API can return all problems to the client at once.
/// </summary>
public static class RegisterPersonValidator
{
    private static readonly Regex EmailPattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private static readonly Regex PhonePattern =
        new(@"^\+?[0-9][0-9\- ]{7,14}$", RegexOptions.Compiled);

    public const string BirthDayFormat = "dd/MM/yyyy";

    public static Dictionary<string, string> Validate(RegisterPersonRequest request)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors["firstName"] = "First Name is required.";

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors["lastName"] = "Last Name is required.";

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors["email"] = "Email is required.";
        }
        else if (!EmailPattern.IsMatch(request.Email))
        {
            errors["email"] = "Please provide a valid Email";
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            errors["phone"] = "Phone is required.";
        }
        else if (!PhonePattern.IsMatch(request.Phone))
        {
            errors["phone"] = "Please provide a valid Phone";
        }

        if (string.IsNullOrWhiteSpace(request.Profile))
        {
            errors["profile"] = "Please selected any profile";
        }
        else if (!IsBase64(request.Profile))
        {
            errors["profile"] = "Profile must be a Base64-encoded image.";
        }

        if (string.IsNullOrWhiteSpace(request.BirthDay))
        {
            errors["birthDay"] = "Birth Day is required.";
        }
        else if (!TryParseBirthDay(request.BirthDay, out _))
        {
            errors["birthDay"] = "Please provide a valid Birth Day";
        }

        if (string.IsNullOrWhiteSpace(request.Occupation))
            errors["occupation"] = "Please selected any Occupation";

        if (string.IsNullOrWhiteSpace(request.Sex))
        {
            errors["sex"] = "Sex is required.";
        }
        else if (!TryParseSex(request.Sex, out _))
        {
            errors["sex"] = "Sex must be Male or Female.";
        }

        return errors;
    }

    public static bool TryParseBirthDay(string value, out DateOnly birthDay)
    {
        var ok = DateOnly.TryParseExact(
            value.Trim(),
            BirthDayFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out birthDay);

        // Reject impossible real-world values like 1899 or far-future dates.
        if (ok && (birthDay.Year < 1900 || birthDay > DateOnly.FromDateTime(DateTime.UtcNow)))
            ok = false;

        return ok;
    }

    public static bool TryParseSex(string value, out Domain.Persons.Sex sex)
    {
        sex = default;
        if (value.Equals("male", StringComparison.OrdinalIgnoreCase))
        {
            sex = Domain.Persons.Sex.Male;
            return true;
        }
        if (value.Equals("female", StringComparison.OrdinalIgnoreCase))
        {
            sex = Domain.Persons.Sex.Female;
            return true;
        }
        return false;
    }

    private static bool IsBase64(string value)
    {
        // Accept both raw Base64 and data URIs ("data:image/png;base64,...").
        var payload = value;
        var commaIndex = value.IndexOf(',');
        if (value.StartsWith("data:", StringComparison.Ordinal) && commaIndex > 0)
            payload = value[(commaIndex + 1)..];

        if (payload.Length == 0 || payload.Length % 4 != 0)
            return false;

        try
        {
            _ = Convert.FromBase64String(payload);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
