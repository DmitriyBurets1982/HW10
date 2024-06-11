using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Contracts.AccountService;
using Contracts.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers;

namespace UserService.Controllers;

[Authorize]
[ApiController]
[Route("userservice/[controller]")]
public class UserController(HttpClient httpClient, SessionStorage sessionStorage, IConfiguration configuration, ILogger<UserController> logger) : ControllerBase
{
    private const string SessionIdCookieName = "sid";
    private const string TokenHeaderName = "Authorization";

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<User>> RegisterUser(RegisterUserDto dto)
    {
        var user = UserStorage.GetByUserName(dto.UserName);
        if (user != null)
        {
            return Conflict($"User with '{dto.UserName}' username already exist. (Id = {user.Id})");
        }

        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(RegisterUser), JsonSerializer.Serialize(dto));

        var httpContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var url = $"{configuration["AccountService"]}/accountservice/account/create";
        logger.LogInformation("AccountService url: '{Url}'", url);

        using var response = await httpClient.PostAsync(url, httpContent);
        var account = await response.Content.ReadFromJsonAsync<Account>();

        user = UserStorage.Register(account!.Id, dto.FirstName, dto.LastName, dto.UserName);

        return Ok(user);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult UpdateUser(int id, [FromBody] UpdateUserDto value)
    {
        var existingUser = UserStorage.GetValueOrDefault(id);
        if (existingUser == null)
        {
            return NotFound($"Can't get user by [{id}] id");
        }

        if (!IsRequestedUserAutorizedOrAdmin(existingUser))
        {
            return Forbid();
        }

        if (value.FirstName != null)
        {
            existingUser.FirstName = value.FirstName;
        }

        if (value.LastName != null)
        {
            existingUser.LastName = value.LastName;
        }

        return Ok(existingUser);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<LoginDto> Login([FromBody] UserCredentials credentials)
    {
        var user = UserStorage.GetByUserName(credentials.Username);
        if (user == null)
        {
            return Unauthorized(new { errorText = "Invalid username or password." });
        }

        var sessionId = sessionStorage.AddSession(user);
        HttpContext.Response.Cookies.Append(SessionIdCookieName, sessionId.ToString());

        sessionStorage.TryGetSessionToken(sessionId, out var jwt);
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return Ok(new LoginDto { UserId = user.Id, Token = $"Bearer {encodedJwt}"});
        //return Ok($"Hello, {user.FirstName} {user.LastName}!");
    }

    [HttpGet("check")]
    [AllowAnonymous]
    // Возвращает токен залогиненного пользователя - необходим для тестов Postman
    public IActionResult Check()
    {
        if (!HttpContext.Request.Cookies.TryGetValue(SessionIdCookieName, out var sessionId))
        {
            return Unauthorized(new { errorText = "Should login first." });
        }

        if (!Guid.TryParse(sessionId, out var sessionGuid))
        {
            return BadRequest(new { errorText = "Session id cookie has wrong format." });
        }

        if (!sessionStorage.TryGetSessionToken(sessionGuid, out var jwt))
        {
            return Unauthorized(new { errorText = "Your session is expired or was logout." });
        }

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        HttpContext.Response.Headers.Append(TokenHeaderName, $"Bearer {encodedJwt}");

        return Ok();
    }

    private bool IsRequestedUserAutorizedOrAdmin(User user)
    {
        if (User.Identity?.Name is null)
        {
            return false;
        }

        return user.UserName.Equals(User.Identity.Name, StringComparison.InvariantCultureIgnoreCase);
    }
}
