using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhishList.Data;
using WhishList.Services;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Wishes
{
    public class EditModel : PageModel
    {
        private readonly IWishService _wishService;
        private readonly IUserService _userService;

        public EditModel(IWishService wishService, IUserService userService)
        {
            _wishService = wishService;
            _userService = userService;
        }

        [BindProperty]
        public Wish Wish { get; set; }
        
        public SelectList UserList { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Wish = _wishService.GetWishById(id.Value);

            if (Wish == null)
            {
                return NotFound();
            }
            
            // Load users for dropdown
            var users = _userService.GetAllUsers();
            UserList = new SelectList(users, "Id", "FullName", Wish.UserId);
            
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Reload users for dropdown
                var users = _userService.GetAllUsers();
                UserList = new SelectList(users, "Id", "FullName", Wish.UserId);
                return Page();
            }

            // Verify the wish exists
            if (!_wishService.WishExists(Wish.Id))
            {
                TempData["ErrorMessage"] = "The wish you're trying to edit no longer exists.";
                return RedirectToPage("./Index");
            }

            // Verify the selected user exists
            if (!_userService.UserExists(Wish.UserId))
            {
                ModelState.AddModelError("Wish.UserId", "Selected user does not exist.");
                var users = _userService.GetAllUsers();
                UserList = new SelectList(users, "Id", "FullName", Wish.UserId);
                return Page();
            }

            try
            {
                _wishService.UpdateWish(Wish);
                TempData["SuccessMessage"] = $"Wish '{Wish.Title}' updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_wishService.WishExists(Wish.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}