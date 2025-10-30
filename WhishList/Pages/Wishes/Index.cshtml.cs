using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Wishes
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IWishService _wishService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public IndexModel(IWishService wishService, IUserService userService, UserManager<User> userManager)
        {
            _wishService = wishService;
            _userService = userService;
            _userManager = userManager;
        }

        public IList<Wish> Wishes { get; set; }
        public User FilteredUser { get; set; }
        public string SearchTerm { get; set; }
        public string ViewTitle { get; set; }
        public int CurrentUserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? userId = null, string searchTerm = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            CurrentUserId = currentUser.Id;
            SearchTerm = searchTerm;

            if (userId.HasValue)
            {
                // Filter by specific user (viewing a friend's wishes)
                FilteredUser = _userService.GetUserById(userId.Value);
                if (FilteredUser == null)
                {
                    TempData["ErrorMessage"] = $"User with ID {userId} not found.";
                    return RedirectToPage("/Index");
                }

                Wishes = _wishService.GetWishesByUser(userId.Value);
                ViewTitle = $"Wishes for {FilteredUser.FullName}";
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Search only in current user's wishes
                var allUserWishes = _wishService.GetWishesByUser(currentUser.Id);
                Wishes = allUserWishes.Where(w => 
                    w.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                    w.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewTitle = $"Search Results for '{searchTerm}'";
            }
            else
            {
                // Show only current user's wishes by default
                Wishes = _wishService.GetWishesByUser(currentUser.Id);
                ViewTitle = "My Wishes";
            }

            return Page();
        }
    }
}