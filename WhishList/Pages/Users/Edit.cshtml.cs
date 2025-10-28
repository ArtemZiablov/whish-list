using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Users;

public class EditModel : PageModel
{
    private readonly IUserService _userService;

    public EditModel(IUserService userService)
    {
        _userService = userService;
    }
    
    [BindProperty]
    public User? User { get; set; }
    
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

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool updated = _userService.UpdateUser(User);
        if (!updated) return BadRequest();
        TempData["SuccessMessage"] = "User updated successfully!";
        return RedirectToPage("./Index");
    }
}
