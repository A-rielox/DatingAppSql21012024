namespace DatingAppSql21012024.Entities;

[System.ComponentModel.DataAnnotations.Schema.Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int IsMain { get; set; }
    public string PublicId { get; set; }
    public int AppUserId { get; set; }
}
