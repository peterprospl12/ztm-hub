using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZtmHub.Application.Interfaces;
using ZtmHub.Domain.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ZtmHub.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly string _audience;
    private readonly int _expiryMinutes;
    private readonly string _issuer;
    private readonly string _secretKey;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["JwtSettings:Secret"]
                     ?? throw new InvalidOperationException("JwtSettings:Secret is not configured");
        _issuer = configuration["JwtSettings:Issuer"]
                  ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured");
        _audience = configuration["JwtSettings:Audience"]
                    ?? throw new InvalidOperationException("JwtSettings:Audience is not configured");
        _expiryMinutes = int.TryParse(configuration["JwtSettings:ExpiryMinutes"], out var minutes)
            ? minutes
            : 60;
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value)
        };

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}