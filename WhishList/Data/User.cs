using Microsoft.AspNetCore.Identity;

namespace WhishList.Data;

public class User : IdentityUser<int>  // Using int as primary key
{
    public string FullName { get; set; }
    public List<Wish> Wishes { get; set; } = new List<Wish>();
}