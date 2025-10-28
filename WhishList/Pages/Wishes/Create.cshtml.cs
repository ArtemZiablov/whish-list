using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhishList.Data;
using WhishList.Services;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Wishes
{
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
        
        public SelectList UserList { get; set; }
        public User SelectedUser { get; set; }
        public bool IsUserPreselected { get; set; }

        public IActionResult OnGet(int? userId = null)
        {
            if (userId.HasValue)
            {
                // Creating wish for specific user
                SelectedUser = _userService.GetUserById(userId.Value);
                if (SelectedUser == null)
                {
                    TempData["ErrorMessage"] = $"User with ID {userId} not found.";
                    return RedirectToPage("./Index");
                }
                
                // Pre-populate the UserId
                Wish = new Wish { UserId = userId.Value };
                IsUserPreselected = true;
            }
            
            // Load users for dropdown
            var users = _userService.GetAllUsers();
            UserList = new SelectList(users, "Id", "FullName", userId);
            
            return Page();
        }

        public IActionResult OnPost(int? userId = null)
        {
            // If userId was provided in route, enforce it
            if (userId.HasValue)
            {
                Wish.UserId = userId.Value;
                SelectedUser = _userService.GetUserById(userId.Value);
                IsUserPreselected = true;
            }

            if (!ModelState.IsValid)
            {
                // Reload data for the form
                var users = _userService.GetAllUsers();
                UserList = new SelectList(users, "Id", "FullName", Wish.UserId);
                return Page();
            }

            // Verify the selected user exists
            if (!_userService.UserExists(Wish.UserId))
            {
                ModelState.AddModelError("Wish.UserId", "Selected user does not exist.");
                var users = _userService.GetAllUsers();
                UserList = new SelectList(users, "Id", "FullName", Wish.UserId);
                return Page();
            }

            _wishService.CreateWish(Wish);
            TempData["SuccessMessage"] = $"Wish '{Wish.Title}' created successfully!";

            // Redirect appropriately
            if (userId.HasValue)
            {
                return RedirectToPage("./Index", new { userId = userId });
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
    }
}