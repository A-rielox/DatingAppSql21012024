using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Entities;

namespace DatingAppSql21012024.Interfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
    Task<bool> AddLike(int sourceUserId, int targetUserId);

    //Task<LikesPagedList> GetUserLikes(LikesParams likesParams);
    Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);


    //Task<AppUser> GetUserWithLikes(int userId); no lo voy a coupar
}

