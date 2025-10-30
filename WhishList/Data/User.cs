using Microsoft.AspNetCore.Identity;

namespace WhishList.Data;


public class User : IdentityUser<int>  // Using int as primary key
{
    public string FullName { get; set; }
    public List<Wish> Wishes { get; set; } = new List<Wish>();
    
    // Friend requests sent by this user
    public List<Friend> SentFriendRequests { get; set; } = new List<Friend>();
    
    // Friend requests received by this user
    public List<Friend> ReceivedFriendRequests { get; set; } = new List<Friend>();
}