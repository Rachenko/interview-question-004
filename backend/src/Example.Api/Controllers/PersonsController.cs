using Example.Application.Persons;
using Microsoft.AspNetCore.Mvc;

namespace Example.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>Registers a person and returns the generated database Id.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RegisterPersonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterPersonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _personService.RegisterAsync(request, cancellationToken);
            return Created($"/api/persons/{id}", new RegisterPersonResponse(id));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }
}

public record RegisterPersonResponse(int Id);
