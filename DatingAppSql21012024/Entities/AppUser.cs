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


    public string Email => UserName; // el mismo UserName
    public string NormalizedEmail { get; set; } // el mismo UserName Normalized


    //public List<Photo> Photos { get; set; } = new();
}
