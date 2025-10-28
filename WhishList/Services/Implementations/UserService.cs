using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<User> userManager, AppDbContext context, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }
    
    public List<User> GetAllUsers()
    {
        return _userManager.Users
            .Include(u => u.Wishes)
            .ToList();
    }

    public User GetUserById(int id)
    {
        return _userManager.Users
            .Include(u => u.Wishes)
            .FirstOrDefault(u => u.Id == id);
    }

    public bool UserExists(int id)
    {
        return _userManager.Users.Any(u => u.Id == id);
    }

    // Synchronous version (keeping your interface compatibility)
    public bool CreateUser(User user)
    {
        // Note: Password should be passed separately with UserManager
        // This is a wrapper for compatibility with your existing interface
        var result = Task.Run(async () => 
        {
            // Generate a temporary password or require it to be set
            var tempPassword = "TempPass123!";
            return await _userManager.CreateAsync(user, tempPassword);
        }).GetAwaiter().GetResult();

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError($"User creation error: {error.Description}");
            }
        }

        return result.Succeeded;
    }

    public bool UpdateUser(User user)
    {
        var result = Task.Run(async () => 
            await _userManager.UpdateAsync(user)
        ).GetAwaiter().GetResult();

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError($"User update error: {error.Description}");
            }
        }

        return result.Succeeded;
    }

    public bool DeleteUser(int id)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        var result = Task.Run(async () => 
            await _userManager.DeleteAsync(user)
        ).GetAwaiter().GetResult();

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError($"User deletion error: {error.Description}");
            }
        }

        return result.Succeeded;
    }

    // Additional helpful methods when using UserManager
    public async Task<bool> CreateUserWithPasswordAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError($"User creation error: {error.Description}");
            }
        }

        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ResetPasswordAsync(int userId, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
}