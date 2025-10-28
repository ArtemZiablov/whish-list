using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Wishes
{
    public class IndexModel : PageModel
    {
        private readonly IWishService _wishService;
        private readonly IUserService _userService;

        public IndexModel(IWishService wishService, IUserService userService)
        {
            _wishService = wishService;
            _userService = userService;
        }

        public IList<Wish> Wishes { get; set; }
        public User FilteredUser { get; set; }
        public string SearchTerm { get; set; }
        public string ViewTitle { get; set; }

        public IActionResult OnGet(int? userId = null, string searchTerm = null)
        {
            SearchTerm = searchTerm;

            if (userId.HasValue)
            {
                // Filter by specific user
                FilteredUser = _userService.GetUserById(userId.Value);
                if (FilteredUser == null)
                {
                    TempData["ErrorMessage"] = $"User with ID {userId} not found.";
                    return RedirectToPage("/Users/Index");
                }

                Wishes = _wishService.GetWishesByUser(userId.Value);
                ViewTitle = $"Wishes for {FilteredUser.FullName}";
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Search across all wishes
                Wishes = _wishService.SearchWishes(searchTerm);
                ViewTitle = $"Search Results for '{searchTerm}'";
            }
            else
            {
                // Show all wishes
                Wishes = _wishService.GetAllWishes();
                ViewTitle = "All Wishes";
            }

            return Page();
        }
    }
}