﻿using AutoMapper;
using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Extensions;
using DatingAppSql21012024.Helpers;
using DatingAppSql21012024.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingAppSql21012024.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository userRepository,
                           IMapper mapper
                           , IPhotoService photoService
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
    }



    //////////////////////////////////////////
    /////////////////////////////////////////////
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    //{// el está usando getMembers
    //    var users = await _userRepository.GetUsersAsync();
    //    var members = _mapper.Map<IEnumerable<MemberDto>>(users);

    //    return Ok(members);
    //}

    // CON PAGINACION
    //[Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<AppUserPagedList>> GetUsers([FromQuery] UserParams userParams)
    {// el está usando getMembers
        var currentUser = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

        // p'q no me mande a mi en la lista de usuarios
        userParams.CurrentUsername = currentUser.UserName;

        // p'q xdefault me mande el sexo opuesto
        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
        }

        AppUserPagedList users = await _userRepository.GetPagedUsersAsync(userParams);

        var members = _mapper.Map<IEnumerable<MemberDto>>(users);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,
                                                          users.TotalCount, users.TotalPages));

        return Ok(members);
    }


    //////////////////////////////////////////
    /////////////////////////////////////////////
    //[Authorize(Roles = "Member")]
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {// el está usando getMember
        var user = await _userRepository.GetUserByUserNameAsync(username);

        // si no hay con este nombre tengo null
        if (user is null) return Ok("Este usuario no existe.");

        var member = _mapper.Map<MemberDto>(user);

        return Ok(member);
    }


    //////////////////////////////////////////////
    /////////////////////////////////////////////////
    // PUT api/Users
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        //var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var username = User.GetUsername();

        var user = await _userRepository.GetUserByUserNameAsync(username);

        if (user == null) return NotFound();

        // lo q esta em memberUpdateDto lo mete a user
        //                |---------->
        _mapper.Map(memberUpdateDto, user);

        // aùn y si no hay cambios me sobreescribe todo
        if (await _userRepository.UpdateUserAsync(user)) return NoContent();

        return BadRequest("Failed to update user.");
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Users/add-photo
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        // al probar en postman la "key" q mando en el body se debe llamar como le pongo aca
        // el paramatro " File "

        var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
        if (user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            AppUserId = user.Id
        };

        // si es su primera foto => la pongo como main
        // como estoy checando "user.Photos.Count" tengo que cargar las fotos con "GetUserByUsernameAsync"
        if (user.Photos.Count == 0)
        {
            photo.IsMain = 1;
        }

        var photoId = await _userRepository.AddPhotoAsync(photo);

        if (photoId > 0)
        {
            var newPhoto = _mapper.Map<PhotoDto>(photo);
            newPhoto.Id = photoId;

            //return _mapper.Map<PhotoDto>(photo);
            return CreatedAtAction(nameof(GetUser),
                        new { username = user.UserName }, newPhoto);
        }

        return BadRequest("Problem adding the photo.");
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // PUT: api/Users/set-main-photo/{photoId}
    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain == 1) return BadRequest("This is already your main photo.");

        var currentMain = user.Photos.FirstOrDefault(p => p.IsMain == 1);

        var obj = new SetMainPhoto(currentMain.Id, photoId);

        if (await _userRepository.UpdatePhotos(obj)) return NoContent();

        return BadRequest("Problem setting the main photo");
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // DELETE: api/Users/delete-photo/{photoId}
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        // user con fotos
        var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain == 1) return BadRequest("You can not delete your main photo.");

        // si esta en cloudinary => tiene una publicId, las otras son del seed inicial
        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        if (await _userRepository.DeletePhoto(photo.Id)) return Ok();

        return BadRequest("Failed to delete the photo.");
    }
}
