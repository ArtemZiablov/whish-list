using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Friends;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IFriendService _friendService;
    private readonly UserManager<User> _userManager;
    private readonly IWishService _wishService;

    public IndexModel(
        IFriendService friendService, 
        UserManager<User> userManager,
        IWishService wishService)
    {
        _friendService = friendService;
        _userManager = userManager;
        _wishService = wishService;
    }

    public List<User> Friends { get; set; } = new();
    public List<Friend> PendingRequests { get; set; } = new();
    public List<Friend> SentRequests { get; set; } = new();
    public Dictionary<int, int> FriendWishCounts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        // Get all friends
        Friends = _friendService.GetFriends(currentUser.Id);
        
        // Get wish counts for each friend
        foreach (var friend in Friends)
        {
            var wishCount = _wishService.GetWishesByUser(friend.Id).Count;
            FriendWishCounts[friend.Id] = wishCount;
        }

        // Get pending friend requests (received)
        PendingRequests = _friendService.GetPendingFriendRequests(currentUser.Id);

        // Get sent friend requests
        SentRequests = _friendService.GetSentFriendRequests(currentUser.Id);

        return Page();
    }

    public async Task<IActionResult> OnPostAcceptRequestAsync(int requestId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        var success = await _friendService.AcceptFriendRequestAsync(requestId, currentUser.Id);
        
        if (success)
            TempData["SuccessMessage"] = "Friend request accepted!";
        else
            TempData["ErrorMessage"] = "Could not accept friend request.";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectRequestAsync(int requestId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        var success = await _friendService.RejectFriendRequestAsync(requestId, currentUser.Id);
        
        if (success)
            TempData["SuccessMessage"] = "Friend request rejected.";
        else
            TempData["ErrorMessage"] = "Could not reject friend request.";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveFriendAsync(int friendId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        var success = await _friendService.RemoveFriendAsync(currentUser.Id, friendId);
        
        if (success)
            TempData["SuccessMessage"] = "Friend removed successfully.";
        else
            TempData["ErrorMessage"] = "Could not remove friend.";

        return RedirectToPage();
    }
}
