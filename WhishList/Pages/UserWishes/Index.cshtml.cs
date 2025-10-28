using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.UserWishes;

public class IndexModel : PageModel
{
    private readonly IWishService _wishService;
    private readonly IUserService _userService;

    public IndexModel(IWishService wishService, IUserService userService)
    {
        _wishService = wishService;
        _userService = userService;
    }

    public User User { get; set; }
    public IList<Wish> Wishes { get; set; }

    public IActionResult OnGet(int userId)
    {
        User = _userService.GetUserById(userId);
            
        if (User == null)
        {
            return NotFound();
        }

        Wishes = _wishService.GetWishesByUser(userId);
        return Page();
    }
}