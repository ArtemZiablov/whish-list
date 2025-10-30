using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Friends;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IWishService _wishService;
    private readonly IFriendService _friendService;
    private readonly UserManager<User> _userManager;

    public DetailsModel(
        IUserService userService,
        IWishService wishService,
        IFriendService friendService,
        UserManager<User> userManager)
    {
        _userService = userService;
        _wishService = wishService;
        _friendService = friendService;
        _userManager = userManager;
    }

    public User Friend { get; set; }
    public List<Wish> FriendWishes { get; set; }
    public bool IsFriend { get; set; }
    public bool IsCurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        Friend = _userService.GetUserById(id.Value);
        if (Friend == null)
            return NotFound();

        IsCurrentUser = Friend.Id == currentUser.Id;
        IsFriend = _friendService.AreFriends(currentUser.Id, Friend.Id);

        // Only show wishes if they're friends or it's the current user
        if (IsFriend || IsCurrentUser)
        {
            FriendWishes = _wishService.GetWishesByUser(Friend.Id);
        }
        else
        {
            FriendWishes = new List<Wish>();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRemoveFriendAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        var success = await _friendService.RemoveFriendAsync(currentUser.Id, id);
        
        if (success)
            TempData["SuccessMessage"] = "Friend removed successfully.";
        else
            TempData["ErrorMessage"] = "Could not remove friend.";

        return RedirectToPage("./Index");
    }
}
