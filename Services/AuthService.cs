using CrudPark.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CrudPark.API.Services
{
    public class AuthService : IAuthService
    {
        // Se usaría la configuración inyectada, pero para este ejemplo, definimos una clave y emisor.
        // NOTA: En producción, estos valores deben venir del archivo appsettings.json
        private readonly string _key = "EstaEsMiClaveSecretaParaJWT2025-CrudPark"; 
        private readonly string _issuer = "CrudPark.API";
        private readonly string _audience = "CrudPark.VueApp";

        public string GenerateJwtToken(Operator operador)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 1. Definir los Claims (información que llevará el token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, operador.Username), // Sujeto: Nombre de usuario
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID de Token
                new Claim("operatorId", operador.Id.ToString()), // ID del operador
                new Claim(ClaimTypes.Role, "Operator") // Rol (si tuvieras)
            };

            // 2. Crear el Token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(2), // El token expira en 2 horas
                signingCredentials: credentials);

            // 3. Serializar y devolver el token como cadena
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}