using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Users;

public class DeleteModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IWishService _wishService;

    public DeleteModel(IUserService userService, IWishService wishService)
    {
        _userService = userService;
        _wishService = wishService;
    }

    [BindProperty]
    public User User { get; set; }
    public int WishCount { get; set; }

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

        // Get count of wishes for warning message
        WishCount = _wishService.GetWishesByUser(id.Value).Count;
            
        return Page();
    }

    public IActionResult OnPost(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        _userService.DeleteUser(id.Value);
        TempData["SuccessMessage"] = "User deleted successfully!";

        return RedirectToPage("./Index");
    }
}