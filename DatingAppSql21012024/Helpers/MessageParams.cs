namespace DatingAppSql21012024.Helpers;

public class MessageParams : PaginationParams
{
    public string Username { get; set; } // el user logeado
    public string Container { get; set; } = "Unread";
}
