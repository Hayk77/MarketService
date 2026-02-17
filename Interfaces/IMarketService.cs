using MyProject.Models;

namespace MyProject.Interfaces
{
    public interface IMarketService
    {
        bool IsAdmin(string password);
        string GetMarketName();
        int GetAllProductsQuantity();
        decimal GetMarketBalance();
        decimal GetProductsSellIncome();
        IEnumerable<Product> GetAllProducts();
        bool UpdateProductPrice(decimal newPrice, string productName);
        bool DeleteProduct(string productName);
        bool BuyProductForMarket(Product product);
        IEnumerable<OrderDetailsDto> GetAllOrderDetails();
        TopSoldProductDto GetMostSoldProduct();
    }
}
