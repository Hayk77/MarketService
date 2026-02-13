using MyProject.Models;

namespace MyProject.Interfaces
{
    public interface IUserService
    {
        bool AddUser(User user);
        bool IsNameStartedWithUpperCase(string name);
        bool IsCorrectEmailFormat(string email);
        bool IsUnrepetableEmail(string email);
        bool BuyProductForUser(User user, Product product, int quantity);
        bool AddUserBalance(decimal newPrice, User user);
        decimal GetUserBalance(User user);
    }
}
