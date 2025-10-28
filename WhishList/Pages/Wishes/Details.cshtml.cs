using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Wishes
{
    public class DetailsModel : PageModel
    {
        private readonly IWishService _wishService;
        private readonly IUserService _userService;

        public DetailsModel(IWishService wishService, IUserService userService)
        {
            _wishService = wishService;
            _userService = userService;
        }

        public Wish Wish { get; set; }
        public IList<Wish> RelatedWishes { get; set; }

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

            // Get other wishes from the same user (excluding current wish)
            var allUserWishes = _wishService.GetWishesByUser(Wish.UserId);
            RelatedWishes = allUserWishes.Where(w => w.Id != id.Value).Take(3).ToList();

            return Page();
        }
    }
}