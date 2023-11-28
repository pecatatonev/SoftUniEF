using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            string userJson = File.ReadAllText("../../../Datasets/users.json");
            string productsJson = File.ReadAllText("../../../Datasets/products.json");
            string categoryJson = File.ReadAllText("../../../Datasets/categories.json");
            string categoryProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");

            //Import Exercises
            //Console.WriteLine(ImportUsers(context, userJson));
            //Console.WriteLine(ImportProducts(context, productsJson));
            //Console.WriteLine(ImportCategories(context, categoryJson));
            //Console.WriteLine(ImportCategoryProducts(context, categoryProductsJson));

            //Export Exercises
            //Console.WriteLine(GetProductsInRange(context));
            Console.WriteLine(GetSoldProducts(context));
            //Console.WriteLine(GetCategoriesByProductsCount(context));
            //Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson){
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson) { 
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Length}";

        }

        public static string ImportCategories(ProductShopContext context, string inputJson) { 
            var allCategories = JsonConvert.DeserializeObject<Category[]>(inputJson);
            var validCategories = allCategories?.Where(c => c.Name is not null).ToArray();

            if (validCategories != null)
            {
                context.Categories.AddRange(validCategories);
                context.SaveChanges();
                return $"Successfully imported {validCategories.Length}";
            }
            return $"Successfully imported 0";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson) {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            if (categoryProducts != null)
            { 
            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Length}";
            }

            return $"Successfully imported 0";
        }

        public static string GetProductsInRange(ProductShopContext context) {
            var productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new {
                    name = p.Name,
                    price = p.Price,
                    seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .OrderBy(p => p.price)
                .ToArray();

            var productsInRangeJson = JsonConvert.SerializeObject(productsInRange, Formatting.Indented);
            return productsInRangeJson;
        }

        public static string GetSoldProducts(ProductShopContext context) {
            var userWithSoldProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                .Select(u => new { 
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                        .Select(p => new { 
                            name = p.Name,
                            price = p.Price,
                            buyerFirstName = p.Buyer.FirstName,
                            buyerLastName = p.Buyer.LastName
                        }).ToArray()
                })
                .ToArray();

            var usersInJson = JsonConvert.SerializeObject(userWithSoldProducts, Formatting.Indented);
            return usersInJson;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context) {
            var categoriesByProductCount = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = c.CategoriesProducts.Average(p => p.Product.Price).ToString("0.00"),
                    totalRevenue = c.CategoriesProducts.Sum(p => p.Product.Price).ToString("0.00")
                })
                .ToArray();

            var categoriesJson = JsonConvert.SerializeObject(categoriesByProductCount, Formatting.Indented);
            return categoriesJson;
        }

        public static string GetUsersWithProducts(ProductShopContext context) {
            var usersWithProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    firstName = u.FirstName, 
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = u.ProductsSold
                    .Where(p => p.BuyerId != null)
                    .Select(ps => new { 
                        name = ps.Name,
                        price = ps.Price,
                    }).ToArray()
                })
                .OrderByDescending(u => u.soldProducts.Count())
                .ToArray();


            var output = new
            {
                usersCount = usersWithProducts.Count(),
                users = usersWithProducts.Select(u => new
                {
                    u.firstName,
                    u.lastName,
                    u.age,
                    soldProducts = new 
                    { 
                    count = u.soldProducts.Count(),
                    products = u.soldProducts
                    }
                })
            };

            string usersWithProductsJSONOutput = JsonConvert.SerializeObject(output, new JsonSerializerSettings
            { 
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            });
            return usersWithProductsJSONOutput;
        }
    }
}