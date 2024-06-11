using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Contracts.UserService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Helpers;

public sealed class SessionStorage
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    public readonly TimeSpan _tokenLifetime;

    private readonly AuthJwtManager _authJwtManager;
    private readonly TokenOptions _authOptions;

    public SessionStorage(AuthJwtManager authJwtManager, IOptions<TokenOptions> authOptions)
    {
        _authJwtManager = authJwtManager;
        _authOptions = authOptions.Value;
        _tokenLifetime = TimeSpan.FromDays(_authOptions.TokenLifetimeInMinutes);
    }

    public Guid AddSession(User user)
    {
        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                notBefore: now,
                claims: GetClaims(user),
                expires: now.Add(TimeSpan.FromDays(_authOptions.TokenLifetimeInMinutes)),
                signingCredentials: new SigningCredentials(_authJwtManager.GetPrivateSecurityKey(), SecurityAlgorithms.RsaSha512));

        Guid newSessionId;
        do
        {
            newSessionId = Guid.NewGuid();
        }
        while (_cache.TryGetValue(newSessionId, out _));

        _cache.Set(newSessionId, jwt, _tokenLifetime);
        return newSessionId;
    }

    public bool TryGetSessionToken(Guid sessionId, out JwtSecurityToken? token)
    {
        return _cache.TryGetValue(sessionId, out token);
    }

    public void RemoveSessionToken(Guid sessionId)
    {
        _cache.Remove(sessionId);
    }

    private static Claim[] GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.UserName)
        };

        ClaimsIdentity claimsIdentity = new(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity.Claims.ToArray();
    }
}
