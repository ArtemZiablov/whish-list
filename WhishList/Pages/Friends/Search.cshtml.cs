using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // <-- ADDED THIS
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Friends;

[Authorize]
public class SearchModel : PageModel
{
    private readonly IFriendService _friendshipService;
    private readonly UserManager<User> _userManager;

    public SearchModel(IFriendService friendshipService, UserManager<User> userManager)
    {
        _friendshipService = friendshipService;
        _userManager = userManager;
    }

    public List<UserSearchResult> SearchResults { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public string SearchTerm { get; set; }

    // ADDED: To safely check user in the view
    public int CurrentUserId { get; set; } 

    public async Task<IActionResult> OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        CurrentUserId = currentUser.Id; // <-- ADDED THIS

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            // FIXED: Search logic now uses UserManager, not the friend service
            var users = await _userManager.Users
                .Where(u => u.Id != currentUser.Id &&
                            (u.FullName.Contains(SearchTerm) || u.Email.Contains(SearchTerm)))
                .ToListAsync();
            
            SearchResults = users.Select(u => new UserSearchResult
            {
                User = u,
                IsFriend = _friendshipService.AreFriends(currentUser.Id, u.Id),
                // This now relies on a new GetFriendship method we will add to the service
                Friendship = _friendshipService.GetFriendship(currentUser.Id, u.Id) 
            }).ToList();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSendRequestAsync(int friendId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        // FIXED: Renamed _friendService to _friendshipService
        // FIXED: Calling new overload SendFriendRequestAsync(int, int)
        var success = await _friendshipService.SendFriendRequestAsync(currentUser.Id, friendId);
        
        if (success)
            TempData["SuccessMessage"] = "Friend request sent!";
        else
            TempData["ErrorMessage"] = "Could not send friend request. Perhaps a request already exists?";

        return RedirectToPage(new { SearchTerm });
    }

    public class UserSearchResult
    {
        public User User { get; set; }
        public bool IsFriend { get; set; }
        public Friend? Friendship { get; set; }
    }
}