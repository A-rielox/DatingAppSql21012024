namespace DatingAppSql21012024.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public DateTime DateOfBirth { get; set; } = new DateTime(1901, 01, 01, 00, 00, 00);
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    ////////////////////////////
    /// p' Identity
    public string Email => UserName; // el mismo UserName
    public string NormalizedEmail { get; set; } // el mismo UserName Normalized


    ////////////////////////////
    //[Write(false)]
    // para llenar la lista de photos de un user que mando al front
    public List<Photo> Photos { get; set; } = new();

    ////////////////////////////
    //public List<AppUser> LikedByUsers { get; set; } // los q te dan like
    //public List<AppUser> LikedUsers { get; set; } // a quienes les doy like

    /*
    ////////////////////////////
    [Write(false)] // x register user ( cambiarlo a sp )
    public List<Message> MessagesSent { get; set; }
    [Write(false)] // x register user ( cambiarlo a sp )
    public List<Message> MessagesReceived { get; set; }

    ////////////////////////////
    // es la misma navigation-property hacia la join-table en AppUser.cs y AppRole.cs
    //public ICollection<AppUserRole> UserRoles { get; set; }
    */
}
