using WhishList.Data;

namespace WhishList.Services.Interfaces;

public interface IFriendService
{
    // Send friend request
    Task<bool> SendFriendRequestAsync(int userId, string friendEmail);
    
    // ADDED: Overload to match what the PageModel needs
    Task<bool> SendFriendRequestAsync(int userId, int friendId); 

    // Manage friend requests
    Task<bool> AcceptFriendRequestAsync(int friendRequestId, int currentUserId);
    Task<bool> RejectFriendRequestAsync(int friendRequestId, int currentUserId);
    Task<bool> RemoveFriendAsync(int userId, int friendId);
    
    // Get friends and requests
    List<User> GetFriends(int userId);
    List<Friend> GetPendingFriendRequests(int userId);
    List<Friend> GetSentFriendRequests(int userId);
    
    // Check friendship status
    bool AreFriends(int userId, int friendId);
    bool FriendRequestExists(int userId, int friendId);
    Friend GetFriendRequest(int friendRequestId);
    
    // ADDED: Method to get a friendship by user IDs, as expected by SearchModel
    Friend? GetFriendship(int userId, int friendId);
}