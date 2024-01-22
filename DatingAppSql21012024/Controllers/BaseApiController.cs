using Microsoft.AspNetCore.Mvc;

namespace DatingAppSql21012024.Controllers;

//[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
}
