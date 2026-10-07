using Example.Application.Persons;
using Xunit;

namespace Example.Application.Tests;

public class RegisterPersonValidatorTests
{
    private static RegisterPersonRequest ValidRequest() => new()
    {
        FirstName = "Somchai",
        LastName = "Jaidee",
        Email = "somchai@example.com",
        Phone = "081-234-5678",
        Profile = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
        BirthDay = "15/08/1995",
        OccupationId = 1,
        Sex = "male"
    };

    [Fact]
    public void Valid_request_passes()
    {
        var errors = RegisterPersonValidator.Validate(ValidRequest());
        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@domain")]
    [InlineData("@example.com")]
    public void Invalid_email_fails(string email)
    {
        var request = ValidRequest();
        request.Email = email;

        var errors = RegisterPersonValidator.Validate(request);

        Assert.Equal("Please provide a valid Email", errors["email"]);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("phone number here")]
    public void Invalid_phone_fails(string phone)
    {
        var request = ValidRequest();
        request.Phone = phone;

        var errors = RegisterPersonValidator.Validate(request);

        Assert.Equal("Please provide a valid Phone", errors["phone"]);
    }

    [Theory]
    [InlineData("1995-08-15")] // wrong format (yyyy-MM-dd)
    [InlineData("32/01/1995")] // impossible day
    [InlineData("15/13/1995")] // impossible month
    [InlineData("15/08/1899")] // too far in the past
    public void Invalid_birthday_fails(string birthDay)
    {
        var request = ValidRequest();
        request.BirthDay = birthDay;

        var errors = RegisterPersonValidator.Validate(request);

        Assert.Equal("Please provide a valid Birth Day", errors["birthDay"]);
    }

    [Fact]
    public void BirthDay_accepts_day_month_year_format()
    {
        Assert.True(RegisterPersonValidator.TryParseBirthDay("05/02/2000", out var date));
        Assert.Equal(new DateOnly(2000, 2, 5), date);
    }

    [Fact]
    public void Missing_profile_fails()
    {
        var request = ValidRequest();
        request.Profile = null;

        var errors = RegisterPersonValidator.Validate(request);

        Assert.Equal("Please selected any profile", errors["profile"]);
    }

    [Fact]
    public void Non_base64_profile_fails()
    {
        var request = ValidRequest();
        request.Profile = "this is not base64!!!";

        var errors = RegisterPersonValidator.Validate(request);

        Assert.Contains("Base64", errors["profile"]);
    }

    [Fact]
    public void Missing_occupation_fails()
    {
        var request = ValidRequest();
        request.OccupationId = null;

        var errors = RegisterPersonValidator.Validate(request);

        Assert.Equal("Please selected any Occupation", errors["occupation"]);
    }

    [Fact]
    public void Every_required_field_must_be_present()
    {
        var errors = RegisterPersonValidator.Validate(new RegisterPersonRequest());

        Assert.Equal(
            new[] { "birthDay", "email", "firstName", "lastName", "occupation", "phone", "profile", "sex" },
            errors.Keys.OrderBy(k => k));
    }
}
