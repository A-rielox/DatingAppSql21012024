using Dapper;
using DatingAppSql21012024.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatingAppSql21012024.Controllers;

public class BuggyController : BaseApiController
{
    private readonly string _connectionString;

    public BuggyController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/buggy/auth
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        // para ver la respuesta de NO autorizado
        return "secret text";
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/buggy/not-found
    [HttpGet("not-found")]
    public async Task<ActionResult<AppUser>> GetNotFound()
    {
        using var connection = new SqlConnection(_connectionString);

        // para respuesta de not found
        var thing = await connection.QuerySingleOrDefaultAsync<AppUser>("sp_getUserById",
                                    new { userId = -1 },
                                    commandType: CommandType.StoredProcedure);

        if (thing == null) return NotFound();

        return Ok(thing);
    }

    //////////////////////////////////////////////// 54
    ///////////////////////////////////////////////////
    // GET: api/buggy/server-error
    [HttpGet("server-error")]
    public async Task<ActionResult<string>> GetServerError()
    {
        // p' error null reference exception

        // me retorna null y al aplicarle un metodo ( .ToString() )
        // da una excepcion ( null reference exception )
        using var connection = new SqlConnection(_connectionString);

        var thing = await connection.QuerySingleOrDefaultAsync<AppUser>("sp_getUserById",
                                    new { userId = -1 },
                                    commandType: CommandType.StoredProcedure);

        var thingToReturn = thing.ToString();

        return thingToReturn;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/buggy/bad-request
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("Bad Request");
    }
}
