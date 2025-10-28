using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IWishService _wishService;

    public DetailsModel(IUserService userService, IWishService wishService)
    {
        _userService = userService;
        _wishService = wishService;
    }

    public User User { get; set; }
    public IList<Wish> UserWishes { get; set; }

    public IActionResult OnGet(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        User = _userService.GetUserById(id.Value);

        if (User == null)
        {
            return NotFound();
        }

        UserWishes = _wishService.GetWishesByUser(id.Value);
            
        return Page();
    }
}