using DatingAppSql21012024.Entities;

namespace DatingAppSql21012024.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}
