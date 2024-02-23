namespace DatingAppSql21012024.Entities;

public class UserLike
{
    public int SourceUserId { get; set; } // id del q da le like

    public int TargetUserId { get; set; } // id de al q se le da like
}


// on delete: cascade, para q SI se borre si se borra el usuario
