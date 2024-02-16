using AutoMapper;
using CloudinaryDotNet;
using Dapper;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Helpers;
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
    public async Task<AppUser> GetUserByUserNameAsync(string userName)
    {
        using var connection = new SqlConnection(_connectionString);

        AppUser user;

        using (var lists = await connection.QueryMultipleAsync("sp_getUserByUserName",
                                    new { userName = userName },
                                    commandType: CommandType.StoredProcedure))
        {
            user = lists.Read<AppUser>().SingleOrDefault();
            if (user is not null)
            {
                if (user.Photos.Count > 0)
                {
                    user.Photos = lists.Read<Photo>().ToList();
                }
            }
        }

        return user;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    ///
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        List<AppUser> users;
        List<Photo> photos;

        using (var lists = await connection.QueryMultipleAsync("sp_getAllUsersAndPhotos",
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



        /*          SIN FOTOS 
        using var connection = new SqlConnection(_connectionString);

        var users = await connection.QueryAsync<AppUser>("sp_getAllUsers",
                                    commandType: CommandType.StoredProcedure);

        return users.ToList();
        */
    }

    //////////////////////////////////////////////////////////////////
    //              PHOTOS

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<int> AddPhotoAsync(Photo photo)
    {
        using var connection = new SqlConnection(_connectionString);

        // si es exitosa me retorna 1 ( la cantidad de cols editadas )
        var parameters = new DynamicParameters();

        parameters.Add("@url", photo.Url);
        parameters.Add("@publicId", photo.PublicId);
        parameters.Add("@appUserId", photo.AppUserId);
        parameters.Add("@isMain", photo.IsMain);

        // retorna SCOPE_IDENTITY
        var res = await connection.QuerySingleAsync<int>("sp_addPhoto",
                                            parameters,
                                            commandType: CommandType.StoredProcedure);
        return res;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<bool> UpdatePhotos(SetMainPhoto setMainPhoto)
    {
        using var connection = new SqlConnection(_connectionString);

        // si es exitosa me retorna 2 ( la cantidad de cols editadas )
        var parameters = new DynamicParameters();

        parameters.Add("@oldMainId", setMainPhoto.oldMainId);
        parameters.Add("@newMainId", setMainPhoto.newMainId);

        var res = await connection.QueryAsync<int>("sp_setMainPhoto",
                                                    parameters,
                                                    commandType: CommandType.StoredProcedure);

        var querySucc = res.FirstOrDefault();

        return querySucc == 2 ? true : false;
    }

    /* ANTIGUO
        public async Task<bool> UpdatePhotos(List<Photo> photos)
        {
            var res = await db.UpdateAsync(photos);

            return res;
        }
    */

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<bool> DeletePhoto(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        var res = await connection.QueryAsync<int>("sp_deletePhoto",
                                            new { photoId = id },
                                            commandType: CommandType.StoredProcedure);

        var querySucc = res.FirstOrDefault(); // devuelve @@ROWCOUNT

        return querySucc == 1 ? true : false;

        /* Dapper Contrib
        var res = await db.DeleteAsync(new Photo() { Id = id });

        return res;
        */
    }
}
