namespace WhishList.Data;

public class Wish
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Url { get; set; }  
    // Foreign key
    public int UserId { get; set; }
    public User? User { get; set; }
}