using Dapper;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatingAppSql21012024.Data;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }



    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///
    public async Task<int> CreateUser(AppUser usuario)  // CrearUsuario
    {
        using var connection = new SqlConnection(_connectionString);

        var usuarioId = await connection.QuerySingleAsync<int>(@"
                                        INSERT INTO dbo.AppUsers 
	                                        (
		                                        userName, knownAs, gender, 
		                                        dateOfBirth, city, country, 
		                                        passwordHash, Email, NormalizedEmail
	                                        )
	                                        VALUES
	                                        (
		                                        @userName, @knownAs, @gender,
		                                        @dateOfBirth, @city, @country,
		                                        @passwordHash, @Email, @NormalizedEmail
	                                        );
                                        SELECT SCOPE_IDENTITY();
                                        ", usuario);

        //await connection.ExecuteAsync("CrearDatosUsuarioNuevo"
        //                                , new { usuarioId }
        //                                , commandType: System.Data.CommandType.StoredProcedure
        //                             );

        return usuarioId;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///
    //public async Task<AppUser> BuscarUsuarioPorEmail(string emailNormalizado) // BuscarUsuarioPorEmail
    //{
    //    using var connection = new SqlConnection(_connectionString);

    //    var usuario = await connection.QuerySingleOrDefaultAsync<AppUser>(
    //                                            @"SELECT * FROM Usuarios
    //                                              Where EmailNormalizado = @emailNormalizado",
    //                                            new { emailNormalizado });

    //    return usuario;
    //}

    public async Task<AppUser> GetUserByUserNameAsync(string userName)  // BuscarUsuarioPorEmail
    {
        //AppUser user;

        //using (var lists = await db.QueryMultipleAsync("sp_getUserByUserName",
        //                            new { userName = username },
        //                            commandType: CommandType.StoredProcedure))
        //{
        //    user = lists.Read<AppUser>().SingleOrDefault();
        //    user.Photos = lists.Read<Photo>().ToList();
        //}

        //return user;

        using var connection = new SqlConnection(_connectionString);

        var user = await connection.QuerySingleOrDefaultAsync<AppUser>("sp_getUserByUserName",
                                                  new { userName = userName },
                                                  commandType: CommandType.StoredProcedure);

        return user;
        
    }



    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///
    ///
    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        /*
        List<AppUser> users;
        List<Photo> photos;

        using (var lists = await db.QueryMultipleAsync("sp_getAllUsersAndPhotos",
                                    commandType: CommandType.StoredProcedure))
        {
            users = lists.Read<AppUser>().ToList();
            photos = lists.Read<Photo>().ToList();
        }

        users.ForEach(u =>
        {
            u.Photos = photos.Where(p => p.AppUserId == u.Id)
                             .ToList();
        });

        return users;
        */
        using var connection = new SqlConnection(_connectionString);

        var users = await connection.QueryAsync<AppUser>("sp_getAllUsers",
                                    commandType: CommandType.StoredProcedure);

        return users.ToList();
        
    }

}
