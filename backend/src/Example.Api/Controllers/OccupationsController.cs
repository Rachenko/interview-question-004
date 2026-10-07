using Example.Application.Occupations;
using Microsoft.AspNetCore.Mvc;

namespace Example.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OccupationsController : ControllerBase
{
    private readonly IOccupationService _occupationService;

    public OccupationsController(IOccupationService occupationService)
    {
        _occupationService = occupationService;
    }

    /// <summary>Lists all occupations for the registration form dropdown.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OccupationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var occupations = await _occupationService.ListAsync(cancellationToken);
        return Ok(occupations.Select(o => new OccupationResponse(o.Id, o.Name)).ToList());
    }
}

public record OccupationResponse(int Id, string Name);
