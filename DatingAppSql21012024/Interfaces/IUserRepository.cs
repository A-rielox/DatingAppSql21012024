using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Entities;

namespace DatingAppSql21012024.Interfaces;

public interface IUserRepository
{
    Task<bool> UpdateUserAsync(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUserNameAsync(string username); 
    Task<AppUser> GetUserByUserNameStoreAsync(string username);  //  LA OCUPO EN AppUserStore - BuscarUsuarioPorEmail


    Task<int> CreateUser(AppUser usuario); // CrearUsuario


    //Task<AppUser> BuscarUsuarioPorEmail(string emailNormalizado); // BuscarUsuarioPorEmail

    //Task<AppUserPagedList> GetPagedUsersAsync(UserParams userParams); // reemplaza a la de arriba

    ////Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    ////Task<MemberDto> GetMemberAsync(string username);

    //Task<int> AddPhotoAsync(Photo photo);
    //Task<bool> UpdatePhotos(SetMainPhoto setMainPhoto);
    //Task<bool> DeletePhoto(int id);
}

// ANTIGUO
//Task<bool> UpdatePhotos(List<Photo> photos);
