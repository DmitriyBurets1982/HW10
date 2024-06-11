using System.Net;
using System.Text;
using System.Text.Json;
using Contracts.AccountService;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(HttpClient httpClient, IConfiguration configuration, ILogger<AccountController> logger) : ControllerBase
{
    [HttpPost("create")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    public async Task<ActionResult<Account>> CreateAccount([FromBody] CreateAccountDto dto)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(CreateAccount), JsonSerializer.Serialize(dto));

        var httpContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var url = $"{configuration["AccountService"]}/accountservice/account/create";
        logger.LogInformation("AccountService url: '{Url}'", url);

        using var response = await httpClient.PostAsync(url, httpContent);
        var account = await response.Content.ReadFromJsonAsync<Account>();

        return Ok(account);
    }

    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<Account>> Deposit([FromBody] AccountOperationDto dto)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(Deposit), JsonSerializer.Serialize(dto));

        var httpContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var url = $"{configuration["AccountService"]}/accountservice/account/deposit";
        logger.LogInformation("AccountService url: '{Url}'", url);

        using var response = await httpClient.PostAsync(url, httpContent);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(await response.Content.ReadAsStringAsync());
        }

        var account = await response.Content.ReadFromJsonAsync<Account>();
        return Ok(account);
    }

    [HttpPost("withdraw")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<bool>> Withdraw([FromBody] AccountOperationDto dto)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(Withdraw), JsonSerializer.Serialize(dto));

        var httpContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var url = $"{configuration["AccountService"]}/accountservice/account/withdraw";
        logger.LogInformation("AccountService url: '{Url}'", url);

        using var response = await httpClient.PostAsync(url, httpContent);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(await response.Content.ReadAsStringAsync());
        }

        var result = await response.Content.ReadFromJsonAsync<bool>();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    public async Task<ActionResult<Account>> GetAccount(int id)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Id}' was called", nameof(GetAccount), id);

        var url = $"{configuration["AccountService"]}/accountservice/account/{id}";
        logger.LogInformation("AccountService url: '{Url}'", url);

        using var response = await httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(await response.Content.ReadAsStringAsync());
        }

        var account = await response.Content.ReadFromJsonAsync<Account>();
        return Ok(account);
    }
}
