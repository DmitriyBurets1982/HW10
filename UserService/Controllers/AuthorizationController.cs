using System.IdentityModel.Tokens.Jwt;
using Contracts.UserService;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers;

namespace UserService.Controllers
{
    [ApiController]
    [Route("userservice/auth")]
    public class AuthorizationController(SessionStorage sessionStorage) : ControllerBase
    {
        private const string TokenHeaderName = "Authorization";
        private const string SessionIdCookieName = "sid";

        private readonly SessionStorage _sessionStorage = sessionStorage;

        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] UserCredentials credentials)
        {
            var user = UserStorage.GetByUserName(credentials.Username);
            if (user == null)
            {
                return Unauthorized(new { errorText = "Invalid username or password." });
            }

            var session = _sessionStorage.AddSession(user);

            HttpContext.Response.Cookies.Append(SessionIdCookieName, session.ToString());

            return Ok($"Hello, {user.FirstName} {user.LastName}!");
        }

        [HttpGet("check")]
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

            if (!_sessionStorage.TryGetSessionToken(sessionGuid, out var jwt))
            {
                return Unauthorized(new { errorText = "Your session is expired or was logout." });
            }

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            HttpContext.Response.Headers.Append(TokenHeaderName, $"Bearer {encodedJwt}");

            return Ok();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (!HttpContext.Request.Cookies.TryGetValue(SessionIdCookieName, out var sessionId))
            {
                return Unauthorized(new { errorText = "Should login first." });
            }

            if (!Guid.TryParse(sessionId, out var sessionGuid))
            {
                return BadRequest(new { errorText = "Session id cookie has wrong format." });
            }

            _sessionStorage.RemoveSessionToken(sessionGuid);

            HttpContext.Response.Cookies.Delete(SessionIdCookieName);

            return Ok();
        }
    }
}
