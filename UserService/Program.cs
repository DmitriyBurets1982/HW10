using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddOptions<TokenOptions>()
                .Bind(builder.Configuration.GetSection(TokenOptions.SettingsSectionName), options => options.BindNonPublicProperties = true)
                .ValidateDataAnnotations();
builder.Services.AddSingleton<AuthJwtManager, AuthJwtManager>();
builder.Services.AddSingleton<SessionStorage, SessionStorage>();

ConfigureAuthentification(builder.Services, builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void ConfigureAuthentification(IServiceCollection services, ConfigurationManager configuration)
{
    var tokenOptions = configuration
        .GetRequiredSection(UserTokenOptions.SettingsSectionName)
        .Get<UserTokenOptions>(options => options.BindNonPublicProperties = true);

    if (tokenOptions is null)
    {
        throw new InvalidOperationException("Can't get TokenOptions from services");
    }

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = tokenOptions.Issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = RsaKeyCreator.GetPublicSecurityKey(tokenOptions.PublicKey),
                ValidateIssuerSigningKey = true,
            };
        });
}
