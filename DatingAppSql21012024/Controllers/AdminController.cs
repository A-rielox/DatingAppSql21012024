using Dapper;
using DatingAppSql21012024.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatingAppSql21012024.Controllers;

public class AdminController : BaseApiController
{
    private IDbConnection db;

    public AdminController(IConfiguration configuration)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    ////////////////////////////////////////////////////////
    // GET: admin/users-with-roles
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult<List<FromDbUserForRoles>>> GetUsersWithRoles()
    {
        List<FromDbUserForRoles> users;
        List<FromDbRoleForRoles> roles;

        using (var lists = await db.QueryMultipleAsync("sp_getUsersWithRoles",
                                    commandType: CommandType.StoredProcedure))
        {
            users = lists.Read<FromDbUserForRoles>().ToList();
            roles = lists.Read<FromDbRoleForRoles>().ToList();
        }

        users.ForEach(u =>
        {
            u.Roles = roles.Where(r => r.UserId == u.Id).Select(r => r.name).ToList();
        });

        return users;
    }

    ////////////////////////////////////////////////////////
    // POST: admin/edit-roles/{username}            CAMBIAR A Q SEA CON USERID EN LUGAR DE USERNAME
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")] // deberia ser PUT xq se esta actualizando
    public async Task<ActionResult<List<string>>> EditRoles(string username, [FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role.");

        var newRoles = await db.QueryAsync<string>("sp_editRoles",
                                    new { userName = username, rolesList = roles },
                                    commandType: CommandType.StoredProcedure);

        return newRoles.ToList();
    }

    ////////////////////////////////////////////////////////
    // GET: admin/photos-to-mederate
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Solo Admins o Moderators.");
    }
}

//      Las Policy las configuro en IdentityServiceExtensions
