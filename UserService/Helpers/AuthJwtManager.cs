using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Helpers;

public sealed class AuthJwtManager
{
    private readonly RsaSecurityKey _privateKey;

    public AuthJwtManager(IOptions<TokenOptions> authOptions)
    {
        var authOptionsValue = authOptions.Value;

        var privateKeyBytes = Convert.FromBase64String(authOptionsValue.PrivateKey);
        var privateCrypto = new RSACryptoServiceProvider(1024);
        privateCrypto.ImportRSAPrivateKey(privateKeyBytes, out _);
        _privateKey = new RsaSecurityKey(privateCrypto);
    }

    public SecurityKey GetPrivateSecurityKey()
    {
        return _privateKey;
    }
}
