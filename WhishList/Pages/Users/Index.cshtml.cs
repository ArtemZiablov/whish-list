using Microsoft.AspNetCore.Mvc.RazorPages;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService)
    {
        _userService = userService;
    }
    
    public IList<User> Users { get; set; }
    
    public void OnGet()
    {
        Users = _userService.GetAllUsers();
    }
}