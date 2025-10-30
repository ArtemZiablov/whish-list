using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Wishes;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IWishService _wishService;
    private readonly UserManager<User> _userManager;

    public CreateModel(IWishService wishService, UserManager<User> userManager)
    {
        _wishService = wishService;
        _userManager = userManager;
    }

    [BindProperty]
    public Wish Wish { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        Wish = new Wish { UserId = currentUser.Id };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Challenge();

        // Ensure the wish is created for the current user
        Wish.UserId = currentUser.Id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _wishService.CreateWish(Wish);
        TempData["SuccessMessage"] = $"Wish '{Wish.Title}' created successfully!";

        return RedirectToPage("./Index");
    }
}