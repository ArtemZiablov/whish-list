using Microsoft.EntityFrameworkCore;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Services.Implementations;

public class FriendService : IFriendService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FriendService> _logger;

    public FriendService(AppDbContext context, ILogger<FriendService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SendFriendRequestAsync(int userId, string friendEmail)
    {
        try
        {
            // Find the friend by email
            var friendUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == friendEmail);

            if (friendUser == null)
            {
                _logger.LogWarning($"User with email {friendEmail} not found");
                return false;
            }
            
            // Delegate to the ID-based method
            return await SendFriendRequestAsync(userId, friendUser.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending friend request by email");
            return false;
        }
    }

    // ADDED: Implementation for the new overload
    public async Task<bool> SendFriendRequestAsync(int userId, int friendId)
    {
        try
        {
            // Can't send friend request to yourself
            if (friendId == userId)
            {
                _logger.LogWarning("User attempted to send friend request to themselves");
                return false;
            }

            // Check if they're already friends or request exists
            if (FriendRequestExists(userId, friendId))
            {
                _logger.LogWarning("Friend request already exists");
                return false;
            }

            // We only need to check if the friendId exists.
            // A full user object check isn't strictly needed 
            // if we trust the friendId, but this is safer.
            var friendExists = await _context.Users.AnyAsync(u => u.Id == friendId);
            if (!friendExists)
            {
                _logger.LogWarning($"User with ID {friendId} not found");
                return false;
            }

            var friendRequest = new Friend
            {
                UserId = userId,
                FriendUserId = friendId,
                Status = FriendRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            _context.Friends.Add(friendRequest);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending friend request by ID");
            return false;
        }
    }

    public async Task<bool> AcceptFriendRequestAsync(int friendRequestId, int currentUserId)
    {
        try
        {
            var friendRequest = await _context.Friends
                .FirstOrDefaultAsync(f => f.Id == friendRequestId 
                                         && f.FriendUserId == currentUserId 
                                         && f.Status == FriendRequestStatus.Pending);

            if (friendRequest == null)
            {
                return false;
            }

            friendRequest.Status = FriendRequestStatus.Accepted;
            friendRequest.AcceptedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting friend request");
            return false;
        }
    }

    public async Task<bool> RejectFriendRequestAsync(int friendRequestId, int currentUserId)
    {
        try
        {
            var friendRequest = await _context.Friends
                .FirstOrDefaultAsync(f => f.Id == friendRequestId 
                                         && f.FriendUserId == currentUserId 
                                         && f.Status == FriendRequestStatus.Pending);

            if (friendRequest == null)
            {
                return false;
            }

            friendRequest.Status = FriendRequestStatus.Rejected;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting friend request");
            return false;
        }
    }

    public async Task<bool> RemoveFriendAsync(int userId, int friendId)
    {
        try
        {
            var friendship = await _context.Friends
                .FirstOrDefaultAsync(f => 
                    ((f.UserId == userId && f.FriendUserId == friendId) ||
                     (f.UserId == friendId && f.FriendUserId == userId)) &&
                    f.Status == FriendRequestStatus.Accepted);

            if (friendship == null)
            {
                return false;
            }

            _context.Friends.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing friend");
            return false;
        }
    }

    public List<User> GetFriends(int userId)
    {
        var friends = _context.Friends
            .Include(f => f.User)
            .Include(f => f.FriendUser)
            .Where(f => (f.UserId == userId || f.FriendUserId == userId) 
                        && f.Status == FriendRequestStatus.Accepted)
            .ToList();

        var friendsList = new List<User>();
        foreach (var friendship in friends)
        {
            if (friendship.UserId == userId)
            {
                friendsList.Add(friendship.FriendUser);
            }
            else
            {
                friendsList.Add(friendship.User);
            }
        }

        return friendsList;
    }

    public List<Friend> GetPendingFriendRequests(int userId)
    {
        return _context.Friends
            .Include(f => f.User)
            .Include(f => f.FriendUser)
            .Where(f => f.FriendUserId == userId && f.Status == FriendRequestStatus.Pending)
            .OrderByDescending(f => f.RequestedAt)
            .ToList();
    }

    public List<Friend> GetSentFriendRequests(int userId)
    {
        return _context.Friends
            .Include(f => f.User)
            .Include(f => f.FriendUser)
            .Where(f => f.UserId == userId && f.Status == FriendRequestStatus.Pending)
            .OrderByDescending(f => f.RequestedAt)
            .ToList();
    }

    public bool AreFriends(int userId, int friendId)
    {
        return _context.Friends.Any(f => 
            ((f.UserId == userId && f.FriendUserId == friendId) ||
             (f.UserId == friendId && f.FriendUserId == userId)) &&
            f.Status == FriendRequestStatus.Accepted);
    }

    public bool FriendRequestExists(int userId, int friendId)
    {
        return _context.Friends.Any(f => 
            ((f.UserId == userId && f.FriendUserId == friendId) ||
             (f.UserId == friendId && f.FriendUserId == userId)) &&
            (f.Status == FriendRequestStatus.Pending || f.Status == FriendRequestStatus.Accepted));
    }

    public Friend GetFriendRequest(int friendRequestId)
    {
        return _context.Friends
            .Include(f => f.User)
            .Include(f => f.FriendUser)
            .FirstOrDefault(f => f.Id == friendRequestId);
    }
    
    // ADDED: Implementation for the new method
    public Friend? GetFriendship(int userId, int friendId)
    {
        return _context.Friends
            .FirstOrDefault(f =>
                (f.UserId == userId && f.FriendUserId == friendId) ||
                (f.UserId == friendId && f.FriendUserId == userId));
    }
}