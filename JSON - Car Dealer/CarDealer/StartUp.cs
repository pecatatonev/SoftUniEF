using CarDealer.Data;
using CarDealer.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            string supplierJson = File.ReadAllText("../../../Datasets/suppliers.json");
            string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            string salesJson = File.ReadAllText("../../../Datasets/sales.json");

            //Import Exercises
            //Console.WriteLine(ImportSuppliers(context, supplierJson));
            //Console.WriteLine(ImportParts(context, partsJson));
            //Console.WriteLine(ImportCars(context, carsJson));
            //Console.WriteLine(ImportCustomers(context, customersJson));
            //Console.WriteLine(ImportSales(context, salesJson));

            //Export Exercises
            //Console.WriteLine(GetOrderedCustomers(context));
            //Console.WriteLine(GetCarsFromMakeToyota(context));
            //Console.WriteLine(GetLocalSuppliers(context));
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));
            //Console.WriteLine(GetTotalSalesByCustomer(context));
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson) {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            if (suppliers != null)
            {
                context.Suppliers.AddRange(suppliers);
                context.SaveChanges();
                return $"Successfully imported {suppliers.Count()}.";
            }
            return $"Successfully imported 0.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson) { 
            var allParts = JsonConvert.DeserializeObject<Part[]>(inputJson);

            int[] suplliersId = context.Suppliers.Select(x => x.Id).ToArray();
            var validParts = allParts.Where(p => suplliersId.Contains(p.SupplierId)).ToArray();

            if (validParts != null)
            {
                context.Parts.AddRange(validParts);
                context.SaveChanges();
                return $"Successfully imported {validParts.Length}.";
            }

            return $"Successfully imported 0.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson) {
            var cars = JsonConvert.DeserializeObject<Car[]>(inputJson);

            if(cars != null) {
                context.Cars.AddRange(cars);
                context.SaveChanges();
                return $"Successfully imported {cars.Count()}.";
            }
            return $"Successfully imported 0.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson) {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            if(customers != null)
            {
                context.Customers.AddRange(customers);
                context.SaveChanges();
                return $"Successfully imported {customers.Length}.";
            }
            return $"Successfully imported 0.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson) {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            if(sales != null)
            {
                context.Sales.AddRange(sales);
                context.SaveChanges();
                return $"Successfully imported {sales.Length}.";
            }

            return $"Successfully imported 0.";
        }

        public static string GetOrderedCustomers(CarDealerContext context) {
            var customersOrder = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver ? 1 : 0)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    c.IsYoungDriver
                }).ToArray();

            string json = JsonConvert.SerializeObject(customersOrder, Formatting.Indented);
            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context) {
            var carsFromToyota = context.Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance);

            var json = JsonConvert.SerializeObject(carsFromToyota, Formatting.Indented);
            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context) {
            var localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new { 
                    s.Id, 
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var json = JsonConvert.SerializeObject (localSuppliers, Formatting.Indented);
            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars
                    .Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = $"{pc.Part.Price:F2}"
                    }).ToArray()
                }).ToArray();
            var jsonFile = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return jsonFile;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context) {
            var totalSales = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new { 
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(x => x.Part.Price))
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToArray();

            var json = JsonConvert.SerializeObject(totalSales, Formatting.Indented);
            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context) {
            var salesWithDiscount = context.Sales
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TraveledDistance
                    },
                        customerName = s.Customer.Name,
                        discount = s.Discount,
                        price = s.Car.PartsCars.Sum(x => x.Part.Price),
                        priceWithDiscount = s.Car.PartsCars.Sum(x => x.Part.Price) - s.Discount
                }).Take(10).ToArray();

            var json = JsonConvert.SerializeObject (salesWithDiscount, Formatting.Indented);
            return json;
            //doesnt work
        }
    }
}