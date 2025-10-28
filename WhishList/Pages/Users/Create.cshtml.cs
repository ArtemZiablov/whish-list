using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Users;

public class CreateModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IUserService userService, ILogger<CreateModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [BindProperty]
    public User User { get; set; }
    
    public IActionResult OnGet()
    {
        _logger.LogInformation("=== GET: Create User Page Loaded ===");
        return Page();
    }
    
    public IActionResult OnPost()
    {
        _logger.LogInformation("=== POST: Create User Form Submitted ===");
        //_logger.LogInformation($"User Data - FullName: {User?.FullName}, Email: {User?.Email}, Password: {User?.Password != null}");
        
        // Log ModelState
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is INVALID!");
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    _logger.LogError($"Validation Error for {key}: {error.ErrorMessage}");
                }
            }
            return Page();
        }
        
        _logger.LogInformation("ModelState is VALID, attempting to create user...");
        
        try
        {
            // Initialize Wishes collection if it's null
            if (User.Wishes == null)
            {
                User.Wishes = new List<Wish>();
                _logger.LogInformation("Initialized Wishes collection");
            }
            
            bool created = _userService.CreateUser(User);
            _logger.LogInformation($"CreateUser result: {created}");
            
            if (created)
            {
                _logger.LogInformation("User created successfully, redirecting to Index");
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToPage("./Index");
            }
            else
            {
                _logger.LogError("CreateUser returned false");
                ModelState.AddModelError("", "Failed to create user");
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating user");
            _logger.LogError($"Exception details: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }
            
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return Page();
        }
    }
}