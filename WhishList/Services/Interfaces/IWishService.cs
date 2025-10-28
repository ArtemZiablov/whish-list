using WhishList.Data;

namespace WhishList.Services.Interfaces;

public interface IWishService
{
    // Read operations
    List<Wish> GetAllWishes();
    List<Wish> GetWishesByUser(int userId);
    List<Wish> SearchWishes(string searchString);
    Wish GetWishById(int id);
    Wish GetWishWithUserById(int id);
    
    // Write operations
    void CreateWish(Wish wish);
    void DeleteWish(Wish wish);
    bool WishExists(int id);

    void UpdateWish(Wish wish);
}