using Microsoft.AspNetCore.Mvc;
using MediatR;
using Liup.Authorization.Application.Authorization.Manager.Requests;

namespace Liup.Authorization.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediatr)
    {
        _logger = logger;
        _mediatr = mediatr;
    }

    [HttpGet("SignIn")]
    public async Task<IActionResult> Get()
    {
        var mediator = await _mediatr.Send(new AuthenticateUserRequest("Wiobacim", "T123", "Anan", "Baban", "anan@gmail.com", "OrospuCocugu123"));
        return Ok(mediator);
    }
}
