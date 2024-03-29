﻿using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Extensions;
using DatingAppSql21012024.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppSql21012024.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepository userRepository,
                           ILikesRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    [HttpPost("{username}")] // a quien se le da el like
    public async Task<ActionResult> AddLike(string username)
    {
        var likedUser = await _userRepository.GetUserByUserNameAsync(username); // me trae fotos, ver
            // si puedo cambiar el en front q mande el id en lugar del name p' ocupar GetUserByIdAsync
            // q no trae fotos

        var sourceUserId = User.GetUserId(); // el que da el like
        var sourceUser = await _userRepository.GetUserByIdAsync(sourceUserId); // sin fotos

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself.");

        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already like this user.");

        if (await _likesRepository.AddLike(sourceUserId, likedUser.Id)) return Ok();

        return BadRequest("Failed to like user.");
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
    {
        var userId = User.GetUserId();

        var users = await _likesRepository.GetUserLikes(predicate, userId);


        return Ok(users);
    }

    //public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    //{
    //    likesParams.UserId = User.GetUserId();

    //    var pagedUsers = await _uow.LikesRepository.GetUserLikes(likesParams);

    //    Response.AddPaginationHeader(new PaginationHeader(pagedUsers.CurrentPage,
    //        pagedUsers.PageSize, pagedUsers.TotalCount, pagedUsers.TotalPages));

    //    return Ok(pagedUsers);
    //}
}
