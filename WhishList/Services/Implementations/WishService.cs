using Microsoft.EntityFrameworkCore;
using WhishList.Data;
using WhishList.Services.Interfaces;

namespace WhishList.Services.Implementations;

public class WishService : IWishService
{
    private readonly AppDbContext _context;

    public WishService(AppDbContext context)
    {
        _context = context;
    }
    
    public List<Wish> GetAllWishes()
    {
        return _context.Wishes
            .Include(w => w.User)
            .ToList();
    }

    public List<Wish> GetWishesByUser(int userId)
    {
        return _context.Wishes
            .Include(w => w.User)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.Id)
            .ToList();
    }

    public List<Wish> SearchWishes(string searchString)
    {
        // If search term is empty, return all wishes
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return GetAllWishes();
        }

        searchString = searchString.ToLower();
        
        return _context.Wishes
            .Include(w => w.User)
            .Where(w => w.Title.ToLower().Contains(searchString) || 
                        w.Description.ToLower().Contains(searchString))
            .ToList();
    }

    public Wish GetWishById(int id)
    {
        return _context.Wishes.Find(id)!;
    }

    public Wish GetWishWithUserById(int id)
    {
        return _context.Wishes
            .Include(w => w.User)
            .FirstOrDefault(w => w.Id == id)!;
    }

    public void CreateWish(Wish wish)
    {
        _context.Wishes.Add(wish);
        _context.SaveChanges();
    }

    public void UpdateWish(Wish wish)
    {
        // Get existing entity
        var existingWish = _context.Wishes.Find(wish.Id);
    
        if (existingWish == null)
            throw new ArgumentException("Wish not found");
    
        // Update only specific properties
        existingWish.Title = wish.Title;
        existingWish.Description = wish.Description;
        existingWish.Image = wish.Image;
        existingWish.Url = wish.Url;
        existingWish.UserId = wish.UserId;
    
        // SaveChanges() will only update changed fields
        _context.SaveChanges();
    }

    public void DeleteWish(Wish wish)
    {
        var wishToDelete = _context.Wishes.Find(wish.Id);

        if (wishToDelete != null)
        {
            _context.Wishes.Remove(wishToDelete);
            _context.SaveChanges();
        }
        else
        {
            throw new ArgumentException($"Wish with ID = {wish.Id} was not found");
        }
        
    }

    public bool WishExists(int id)
    {
        return _context.Wishes.Any(w => w.Id == id);
    }
}