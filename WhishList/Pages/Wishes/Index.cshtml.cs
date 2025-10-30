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
        private readonly ICurrencyConverter _currencyConverter;

        public IndexModel(IWishService wishService, IUserService userService, UserManager<User> userManager, ICurrencyConverter currencyConverter)
        {
            _wishService = wishService;
            _userService = userService;
            _userManager = userManager;
            _currencyConverter = currencyConverter;
        }

        public IList<Wish> Wishes { get; set; }
        public User FilteredUser { get; set; }
        public string SearchTerm { get; set; }
        public string ViewTitle { get; set; }
        public int CurrentUserId { get; set; }
        public IList<ConvertedWishViewModel> ConvertedWishes { get; set; }
        [BindProperty(SupportsGet = true)] // Bind the currency from the query string
        public string SelectedCurrency { get; set; } = "EUR";
        
        // Model for the converted data sent to the view
        public class ConvertedWishViewModel : Wish 
        {
            public string DisplayPrice { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? userId = null, string searchTerm = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            CurrentUserId = currentUser.Id;
            SearchTerm = searchTerm;
    
            // 1. --- LOGIC TO POPULATE Wishes ---
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
    
            // 2. --- CONSOLIDATED PRICE CONVERSION LOGIC ---
            // This runs on the 'Wishes' list populated above, regardless of the filter.
            ConvertedWishes = new List<ConvertedWishViewModel>();
    
            // You must ensure that the Wish entity now includes PriceAmount and PriceCurrencyCode 
            // for this loop to work correctly.
            foreach (var wish in Wishes)
            {
                // Convert the price using the service
                // We should add a null/default check here to handle wishes created before the price field was added
                decimal amountToConvert = wish.PriceAmount; 
                string fromCurrency = string.IsNullOrEmpty(wish.PriceCurrencyCode) ? SelectedCurrency : wish.PriceCurrencyCode;

                decimal convertedAmount = await _currencyConverter.ConvertAsync(
                    amountToConvert, 
                    fromCurrency, 
                    SelectedCurrency);

                // Create the view model and format the price string
                // NOTE: In a real app, you might use a library like AutoMapper to simplify object mapping.
                ConvertedWishes.Add(new ConvertedWishViewModel 
                {
                    Id = wish.Id,
                    Title = wish.Title,
                    Description = wish.Description,
                    Image = wish.Image,
                    Url = wish.Url,
                    UserId = wish.UserId,
                    User = wish.User,
            
                    // The PriceAmount and PriceCurrencyCode properties must be added to the Wish entity 
                    // for these lines to compile and work as expected!
                    PriceAmount = wish.PriceAmount, 
                    PriceCurrencyCode = wish.PriceCurrencyCode,

                    DisplayPrice = $"{SelectedCurrency} {convertedAmount:N2}" 
                });
            }

            return Page();
        }        
    }
}