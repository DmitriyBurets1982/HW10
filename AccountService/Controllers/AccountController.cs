using System.Text.Json;
using AccountService.Services;
using Contracts.AccountService;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("accountservice/[controller]")]
public class AccountController(IAccountService accountService, ILogger<AccountController> logger) : ControllerBase
{
    [HttpPost("create")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    public IActionResult CreateAccount([FromBody] CreateAccountDto dto)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(CreateAccount), JsonSerializer.Serialize(dto));

        var newAccount = accountService.CreateAccount(dto.UserName);
        return Ok(newAccount);
    }

    [HttpPost("deposit")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public IActionResult Deposit(AccountOperationDto dto)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(Deposit), JsonSerializer.Serialize(dto));

        var account = accountService.GetAccount(dto.AccountId);
        if (account == null)
        {
            return NotFound($"Can't get user by [{dto.AccountId}] id");
        }

        try
        {
            accountService.IncreaseBalance(account, dto.Value);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok(account);
    }

    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Withdraw(AccountOperationDto dto)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(Withdraw), JsonSerializer.Serialize(dto));

        var account = accountService.GetAccount(dto.AccountId);
        if (account == null)
        {
            return NotFound($"Can't get user by [{dto.AccountId}] id");
        }

        try
        {
            return Ok(accountService.DecreaseBalance(account, dto.Value));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    public IActionResult GetAccount(int id)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Id}' was called", nameof(GetAccount), id);

        var account = accountService.GetAccount(id);
        if (account == null)
        {
            return NotFound($"Can't get user by [{id}] id");
        }

        return Ok(account);
    }
}
