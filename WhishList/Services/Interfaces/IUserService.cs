using WhishList.Data;

namespace WhishList.Services.Interfaces;

public interface IUserService
{
    List<User> GetAllUsers();
    User GetUserById(int id);
    bool UserExists(int id);

    bool CreateUser(User user);
    bool UpdateUser(User user);
    bool DeleteUser(int id);
}
