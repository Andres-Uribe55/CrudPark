using CrudPark.API.Models;

namespace CrudPark.API.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(Operator operador);
    }
}