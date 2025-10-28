using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Wishes
{
    public class DeleteModel : PageModel
    {
        private readonly IWishService _wishService;

        public DeleteModel(IWishService wishService)
        {
            _wishService = wishService;
        }

        [BindProperty]
        public Wish Wish { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Wish = _wishService.GetWishWithUserById(id.Value);

            if (Wish == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the wish to find the userId before deletion
            var wish = _wishService.GetWishById(id.Value);
            
            if (wish != null)
            {
                var userId = wish.UserId;
                var title = wish.Title;
                
                _wishService.DeleteWish(wish);
                TempData["SuccessMessage"] = $"Wish '{title}' has been deleted successfully!";
                
                // Check if there's a return URL
                if (!string.IsNullOrEmpty(Request.Query["returnUrl"]))
                {
                    return LocalRedirect(Request.Query["returnUrl"]);
                }
                
                // Otherwise, redirect to the user's wishes if we have a userId
                if (userId > 0)
                {
                    return RedirectToPage("./Index", new { userId = userId });
                }
            }

            return RedirectToPage("./Index");
        }
    }
}