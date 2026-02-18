using MyProject.Interfaces;
using MyProject.Models;
using MyProject.Repositories;
using MyProject.Services;
using MyProject.Enums;

namespace MyProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User user = new User();
            Product product = new Product();
            IRepository repository = new Repository();
            IMarketService marketService = new MarketService(repository);
            IUserService userService = new UserService(repository);

            Console.WriteLine($"Welcome {marketService.GetMarketName()}!");

            while (true)
            {
                try
                {
                    string name;

                    while (true)
                    {
                        Console.Write("Name: ");
                        name = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Name is required! Please try again.");
                            continue;
                        }

                        if(!userService.IsNameStartedWithUpperCase(name))
                        {
                            Console.WriteLine("Name is required to start with upper case!");
                            continue;
                        }

                        bool containsNumber = false;

                        foreach (char c in name)
                        {
                            if (char.IsDigit(c))
                            {
                                containsNumber = true;
                                break;
                            }
                        }

                        if (containsNumber)
                        {
                            Console.WriteLine("Name must not contain numbers. Please try again.");
                            continue;
                        }

                        break;
                    }

                    if (name == "admin" || name == "Admin")
                    {
                        Console.Write("Password: ");
                        string password = Console.ReadLine();

                        while (true)
                        {
                            if (marketService.IsAdmin(password))
                            {
                                Console.WriteLine(
                                    "1.Buy product | 2.Check balance | 3.Get all products | 4.Update product price | 5.Delete product" +
                                    "                        6.Get all products quantity | 7.Get products sell income | 8.Get all orders details | 9.Get top sold product |0.Exit");

                                Console.Write("Select: ");
                                if (!int.TryParse(Console.ReadLine(), out int input))
                                {
                                    continue;
                                }

                                AdminMenu option = (AdminMenu)input;

                                if (option == 0)
                                {
                                    break;
                                }

                                switch (option)
                                {
                                    case AdminMenu.BuyProduct:
                                        Console.Write("Product name: ");
                                        product.Name = Console.ReadLine();

                                        Console.Write("Product quantity: ");
                                        if (!int.TryParse(Console.ReadLine(), out int productQuantity))
                                        {
                                            continue;
                                        }
                                        product.Quantity = productQuantity;

                                        Console.Write("Price: ");
                                        if (!decimal.TryParse(Console.ReadLine(), out decimal productPrice))
                                        {
                                            continue;
                                        }
                                        product.Price = productPrice;

                                        if (marketService.BuyProductForMarket(product))
                                        {
                                            Console.WriteLine("Product purchased succsesfully!");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Product purchased failed.");
                                        }
                                        break;

                                    case AdminMenu.CheckBalance:
                                        decimal balance = marketService.GetMarketBalance();
                                        Console.WriteLine($"Balance: {balance}");
                                        break;

                                    case AdminMenu.GetAllProducts:
                                        var products = marketService.GetAllProducts();

                                        foreach (var currentProduct in products)
                                        {
                                            Console.WriteLine($"{currentProduct.Name} | {currentProduct.Price}$ | {currentProduct.Quantity}");
                                        }
                                        break;

                                    case AdminMenu.UpdateProductPrice:
                                        Console.Write("New price: ");
                                        if (!decimal.TryParse(Console.ReadLine(), out decimal newPrice))
                                        {
                                            continue;
                                        }

                                        Console.Write("Product name: ");
                                        string productName = Console.ReadLine();

                                        if (marketService.UpdateProductPrice(newPrice, productName))
                                        {
                                            Console.WriteLine("Product price updated!");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Product price updating failed.");
                                        }
                                        break;

                                    case AdminMenu.DeleteProduct:
                                        Console.Write("Product name: ");
                                        string productNameForDelete = Console.ReadLine();

                                        if (marketService.DeleteProduct(productNameForDelete))
                                        {
                                            Console.WriteLine($"Product {productNameForDelete} deleted sucsessfully!");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Product deleting failed.");
                                        }
                                        break;

                                    case AdminMenu.GetAllProductsQuantity:
                                        int productsQuantity = marketService.GetAllProductsQuantity();
                                        Console.WriteLine($"All products quantity: {productsQuantity}");
                                        break;

                                    case AdminMenu.GetProductsSellIncome:
                                        decimal income = marketService.GetProductsSellIncome();
                                        Console.WriteLine($"All products sell income: {income}");
                                        break;

                                    case AdminMenu.GetAllOrderDetails:
                                        var allOrderDetails = marketService.GetAllOrderDetails();

                                        foreach (var currentOrder in allOrderDetails)
                                        {
                                            Console.WriteLine($"{currentOrder.ProductName} | {currentOrder.Quantity} | {currentOrder.Price} |" +
                                                $"{currentOrder.TotalPrice} | {currentOrder.UserName} | {currentOrder.Email} | {currentOrder.CreatedAt}");
                                        }
                                        break;

                                    case AdminMenu.GetMostSoldProduct:
                                        var topProduct = marketService.GetMostSoldProduct();
                                        Console.WriteLine($"Most sold product: {topProduct.ProductName}\nQuantity: {topProduct.TotalSoldQuantity}");
                                        break;
                                }
                            }
                            else
                            {
                                throw new Exception("Wrong password for admin.");
                            }
                        }
                    }
                    else
                    {
                        Console.Write("Email: ");
                        string email = Console.ReadLine();

                        while (true)
                        {
                            if (!userService.IsCorrectEmailFormat(email))
                            {
                                throw new Exception("Incorrect email format!");
                            }

                            Console.WriteLine("1.Add user | 2.Get all products | 3.Buy product | 4.Add balance | 5.Get balance | 0.Exit");

                            Console.Write("Select: ");
                            if (!int.TryParse(Console.ReadLine(), out int input))
                            {
                                continue;
                            }

                            UserMenu option = (UserMenu)input;

                            if (option == 0)
                            {
                                break;
                            }

                            switch (option)
                            {
                                case UserMenu.AddUser:
                                    user.Name = name;
                                    user.Email = email;

                                    Console.Write("Your balance: ");
                                    if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
                                    {
                                        continue;
                                    }
                                    user.Balance = balance;

                                    if (userService.AddUser(user))
                                    {
                                        Console.WriteLine($"Welcome {user.Name}!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("User adding failed.");
                                    }
                                    break;

                                case UserMenu.GetAllProducts:
                                    var products = marketService.GetAllProducts();

                                    foreach (var currentProduct in products)
                                    {
                                        Console.WriteLine($"{currentProduct.Name} | {currentProduct.Price}$ | {currentProduct.Quantity}");
                                    }
                                    break;

                                case UserMenu.BuyProduct:
                                    user.Email = email;

                                    Console.Write("Product name: ");
                                    product.Name = Console.ReadLine();

                                    Console.Write("Quantity: ");
                                    if (!int.TryParse(Console.ReadLine(), out int quantity))
                                    {
                                        continue;
                                    }

                                    if (userService.BuyProductForUser(user, product, quantity))
                                    {
                                        Console.WriteLine("Product purchased succsesfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Product purchased failed.");
                                    }
                                    break;

                                case UserMenu.AddBalance:
                                    user.Email = email;

                                    Console.Write("Adding price: ");
                                    if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                                    {
                                        continue;
                                    }

                                    if (userService.AddUserBalance(price, user))
                                    {
                                        Console.WriteLine("Balance updated sucsessfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Balance updating failed.");
                                    }
                                    break;

                                case UserMenu.GetBalance:
                                    user.Email = email;

                                    decimal userBalance = userService.GetUserBalance(user);
                                    Console.WriteLine($"Balance: {userBalance}");
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


        }
    }
}
