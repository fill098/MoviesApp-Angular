using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ApiControllerBase : ControllerBase
{
}
