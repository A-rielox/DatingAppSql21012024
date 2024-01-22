using System.ComponentModel.DataAnnotations;

namespace DatingAppSql21012024.DTOs;

public class RegisterDto
{
    [Required] 
    public string UserName { get; set; }

    [Required] 
    public string KnownAs { get; set; }

    [Required] 
    public string Gender { get; set; }


    // es optional p' q' funcione en [Required]
    [Required] 
    public DateTime? DateOfBirth { get; set; }

    [Required] 
    public string City { get; set; }

    [Required] 
    public string Country { get; set; }


    [Required]
    [StringLength(12, MinimumLength = 4)]
    public string Password { get; set; }
}
