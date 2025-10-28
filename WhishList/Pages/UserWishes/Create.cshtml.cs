using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.UserWishes;

public class CreateModel : PageModel
{
    private readonly IWishService _wishService;
    private readonly IUserService _userService;

    public CreateModel(IWishService wishService, IUserService userService)
    {
        _wishService = wishService;
        _userService = userService;
    }

    [BindProperty]
    public Wish Wish { get; set; }
        
    public User User { get; set; }

    public IActionResult OnGet(int userId)
    {
        User = _userService.GetUserById(userId);
            
        if (User == null)
        {
            return NotFound();
        }

        // Pre-populate the UserId
        Wish = new Wish { UserId = userId };
            
        return Page();
    }

    public IActionResult OnPost(int userId)
    {
        User = _userService.GetUserById(userId);
            
        if (User == null)
        {
            return NotFound();
        }

        // Ensure the wish is created for the correct user
        Wish.UserId = userId;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _wishService.CreateWish(Wish);
        TempData["SuccessMessage"] = $"Wish '{Wish.Title}' created for {User.FullName}!";

        // Redirect back to user's wishes
        return RedirectToPage("./Index", new { userId = userId });
    }
}