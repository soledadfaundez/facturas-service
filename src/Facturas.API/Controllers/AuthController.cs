using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// EFSC: Este controlador es solo para generar un token JWT de prueba. En un escenario real, se debería implementar un proceso de autenticación adecuado (por ejemplo, validando credenciales contra una base de datos) antes de emitir un token.
namespace Facturas.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "enid"),
            new Claim(ClaimTypes.Email, "enid@test.com"),
            new Claim(ClaimTypes.Role, "AUDITOR")
        };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

        var jwt =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return Ok(new
        {
            access_token = jwt
        });
    }
}