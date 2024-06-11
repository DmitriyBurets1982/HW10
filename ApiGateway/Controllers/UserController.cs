using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;
using Contracts.UserService;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(HttpClient httpClient, IConfiguration configuration, ILogger<UserController> logger) : ControllerBase
{
    private static readonly ConcurrentDictionary<int, LoginDto> _userTokens = new();

    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<User>> RegisterUser(RegisterUserDto dto)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var url = $"{configuration["UserService"]}/userservice/user/register";
        logger.LogInformation("UserService url: '{Url}'", url);

        using var response = await httpClient.PostAsync(url, httpContent);
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return Conflict(await response.Content.ReadAsStringAsync());
        }

        var user = await response.Content.ReadFromJsonAsync<User>();

        return Ok(user);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginDto>> Login([FromBody] UserCredentials credentials)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");
        var url = $"{configuration["UserService"]}/userservice/user/login";
        logger.LogInformation("UserService url: '{Url}'", url);

        using var response = await httpClient.PostAsync(url, httpContent);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Forbid(await response.Content.ReadAsStringAsync());
        }

        var dto = await response.Content.ReadFromJsonAsync<LoginDto>();
        _userTokens.AddOrUpdate(dto!.UserId, dto, (key, value) => dto);

        return Ok(dto);
    }

    //[HttpPost("logout")]
    //public async Task<IActionResult> Logout()
    //{
    //    var url = $"{configuration["UserService"]}/userservice/user/logout";
    //    logger.LogInformation("UserService url: '{Url}'", url);

    //    await httpClient.PostAsync(url, null);
    //    return Ok();
    //}

    [HttpPut]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<User>> UpdateUser(int userId, UpdateUserDto dto)
    {
        if (!_userTokens.TryGetValue(userId, out LoginDto? loginDto))
        {
            return Unauthorized();
        }

        httpClient.DefaultRequestHeaders.Add("Authorization", loginDto.Token);
        var httpContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var url = $"{configuration["UserService"]}/userservice/user/{userId}";
        logger.LogInformation("UserService url: '{Url}'", url);

        using var response = await httpClient.PutAsync(url, httpContent);
        var user = await response.Content.ReadFromJsonAsync<User>();

        return Ok(user);
    }
}
