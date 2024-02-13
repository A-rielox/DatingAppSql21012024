using AutoMapper;
using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppSql21012024.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public AccountController(UserManager<AppUser> userManager
                             , IMapper mapper
                             , ITokenService tokenService
        )
    {
        this._userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }


    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    // POST: api/Account/register
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.UserName.ToLower();

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        //var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        //if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

        var userDto = new UserDto
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            Token = _tokenService.CreateToken(user)
        };

        return Ok(userDto);
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Account/login
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //var user = await _userRepository.GetUserByUsernameAsync(loginDto.UserName);
        var user = await _userManager.FindByNameAsync(loginDto.UserName); // .FindByNameAsync est[a en AppUserStore

        if (user == null) return Unauthorized("Invalid Username.");

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result) return Unauthorized("Invalid Password.");



        var userDto = new UserDto
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain == 1)?.Url,
            Token = _tokenService.CreateToken(user)
        };

        return Ok(userDto);
    }




    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    private async Task<bool> UserExists(string username)
    {
        var userInDb = await _userManager.FindByNameAsync(username.ToLower());
        
        return userInDb is not null;
    }
}
