[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _service;
    private readonly IHttpContextAccessor _http;

    public UserProfileController(IUserProfileService service, IHttpContextAccessor http)
    {
        _service = service;
        _http = http;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var profile = await _service.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UserProfileDto dto)
    {
        var userId = int.Parse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        if (dto.Id != userId)
            return Forbid();

        var updated = await _service.UpdateProfileAsync(dto);
        return Ok(updated);
    }
}
