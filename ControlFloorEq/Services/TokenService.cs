using Abixe_Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControlFloor.Services
{
   
    public class TokenService
    {
        private readonly string _secretKey;
        private readonly int _minutesToExpiration;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _message;

        public TokenService(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            _secretKey = jwtSettings.GetValue<string>("Key");
            _minutesToExpiration = jwtSettings.GetValue<int>("MinutesToExpiration");
            _issuer = jwtSettings.GetValue<string>("Issuer");
            _audience = jwtSettings.GetValue<string>("Audience");
            _message = jwtSettings.GetValue<string>("Message");
        }

        public Task<AuthResponse> GenerarToken(AbxAuthorization credencialesUsuario)
        {
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // Creación de claims usando inicialización directa
            var claims = new List<Claim>
        {
            new Claim("UserId", credencialesUsuario.IdUsuario.ToString()),
            new Claim("DBCompany", credencialesUsuario.Company),
            new Claim("IdRole", credencialesUsuario.IdRole)
        };

            // Creación del token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_minutesToExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            var expiracion = DateTime.UtcNow.AddMinutes(_minutesToExpiration);

            // Uso de Task.FromResult ya que no hay operaciones asíncronas reales
            return Task.FromResult(new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion,
                Message = _message
            });
        }
    }

}
