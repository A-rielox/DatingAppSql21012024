using AutoMapper;
using Dapper;
using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatingAppSql21012024.Data;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public UserRepository(IConfiguration configuration, IMapper mapper)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _mapper = mapper;
    }



    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    /// LA OCUPO EN AppUserStore
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
    public async Task<bool> UpdateUserAsync(AppUser user)
    {
        // si es exitosa me retorna 1 ( la cantidad de cols editadas )
        var parameters = new DynamicParameters();

        parameters.Add("@userId", user.Id);
        parameters.Add("@introduction", user.Introduction);
        parameters.Add("@lookingFor", user.LookingFor);
        parameters.Add("@interests", user.Interests);
        parameters.Add("@city", user.City);
        parameters.Add("@country", user.Country);
        parameters.Add("@lastActive", user.LastActive);

        using var connection = new SqlConnection(_connectionString);

        var res = await connection.QuerySingleAsync<int>("sp_updateUser",
                                            parameters,
                                            commandType: CommandType.StoredProcedure);

        return res == 1 ? true : false;


        /* con Dapper Contrib
         var res = await db.UpdateAsync(user);
         return res; */
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///
    public async Task<AppUser> GetUserByIdAsync(int id)
    {

        using var connection = new SqlConnection(_connectionString);

        var user = await connection.QuerySingleAsync<AppUser>("sp_getUserById",
                                    new { userId = id },
                                    commandType: CommandType.StoredProcedure);

        return user;
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


    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///     LA OCUPO EN AppUserStore - BuscarUsuarioPorEmail
    public async Task<AppUser> GetUserByUserNameStoreAsync(string userName)
    {
        
        using var connection = new SqlConnection(_connectionString);

        var user = await connection.QuerySingleOrDefaultAsync<AppUser>("sp_getUserByUserNameStore",
                                                  new { userName = userName },
                                                  commandType: CommandType.StoredProcedure);

        return user;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///
    public async Task<MemberDto> GetUserByUserNameAsync(string userName)
    {
        using var connection = new SqlConnection(_connectionString);

        MemberDto member;

        using (var lists = await connection.QueryMultipleAsync("sp_getUserByUserName",
                                    new { userName = userName },
                                    commandType: CommandType.StoredProcedure))
        {
            member = _mapper.Map<MemberDto>( lists.Read<AppUser>().SingleOrDefault() );
            member.Photos = _mapper.Map<List<PhotoDto>>( lists.Read<Photo>().ToList() );
        }

        return member;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///
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

        /*          SIN FOTOS */
        using var connection = new SqlConnection(_connectionString);

        var users = await connection.QueryAsync<AppUser>("sp_getAllUsers",
                                    commandType: CommandType.StoredProcedure);

        return users.ToList();        
    }
}
