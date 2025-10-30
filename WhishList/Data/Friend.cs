namespace WhishList.Data;

// Friendship.cs - This is what exists
public class Friend
{
    public int Id { get; set; }
    
    // User who sent the friend request
    public int UserId { get; set; }
    public User User { get; set; }
    
    // User who received the friend request
    public int FriendUserId { get; set; }
    public User FriendUser { get; set; }
    
    // Status of the friend request
    public FriendRequestStatus Status { get; set; }
    
    // When the request was sent
    public DateTime RequestedAt { get; set; }
    
    // When the request was accepted (nullable)
    public DateTime? AcceptedAt { get; set; }
}

public enum FriendRequestStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
