using MyProject.Models;

namespace MyProject.Interfaces
{
    public interface IRepository
    {
        IEnumerable<T> GetAll<T>() where T : class;
        T Get<T>(Func<T, bool> predicate) where T : class, new();
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        (byte[] hash, byte[] salt) GetAdminPassword();
        bool BuyProductForUser(User user, Product product, int quantity);
        bool BuyProductForMarket(Product product);
        IEnumerable<OrderDetailsDto> GetOrderDetails();
        TopSoldProductDto GetTopSoldProduct();
    }
}
