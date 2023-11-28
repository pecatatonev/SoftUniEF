using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            string inputSupplierXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            string inputPartsXml = File.ReadAllText("../../../Datasets/parts.xml");
            string inputCarsXml = File.ReadAllText("../../../Datasets/cars.xml");
            string inputCustomersXml = File.ReadAllText("../../../Datasets/customers.xml");
            string inputSalesXml = File.ReadAllText("../../../Datasets/sales.xml");

            //ImportData
            //Console.WriteLine(ImportSuppliers(context,inputSupplierXml));
            //Console.WriteLine(ImportParts(context, inputPartsXml));
            //Console.WriteLine(ImportCars(context, inputCarsXml));
            //Console.WriteLine(ImportCustomers(context, inputCustomersXml));
            //Console.WriteLine(ImportSales(context, inputSalesXml));

            //ExportData
            //Console.WriteLine(GetCarsWithDistance(context));
            //Console.WriteLine(GetCarsFromMakeBmw(context));
            //Console.WriteLine(GetLocalSuppliers(context));
            //Console.WriteLine(GetCarsWithTheirListOfParts(context)); doesnt work
            //Console.WriteLine(GetTotalSalesByCustomer(context));
            //Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        private static Mapper GetMapper() {
            var cfg = new MapperConfiguration(c => c.AddProfile<CarDealerProfile>());
            return new Mapper(cfg);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            //1. Create serializer
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDTO[]), new XmlRootAttribute("Suppliers"));

            //2.Create Reader and deserialize objects
            using var reader = new StringReader(inputXml);
            ImportSupplierDTO[] importSupplierDTOs = (ImportSupplierDTO[])xmlSerializer.Deserialize(reader);

            //3. Map to suplliers
            var mapper = GetMapper();
            Supplier[] suppliers = mapper.Map<Supplier[]>(importSupplierDTOs);

            //4. Add to EFcontext
            context.AddRange(suppliers);

            //5.Commit changes
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartsDTO[]), new XmlRootAttribute("Parts"));
            
            using var reader = new StringReader(inputXml);
            ImportPartsDTO[] importPartsDTOs = (ImportPartsDTO[])xmlSerializer.Deserialize(reader);

            var supplierIds = context.Suppliers.Select(x => x.Id).ToArray();

            var mapper = GetMapper();
            Part[] parts = mapper.Map<Part[]>(importPartsDTOs
                .Where(p => supplierIds.Contains(p.SupplierId)));

            context.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Length}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDTO[]), new XmlRootAttribute("Cars"));
            using StringReader stringReader = new StringReader(inputXml);

            ImportCarDTO[] importCarDTOs = (ImportCarDTO[])xmlSerializer.Deserialize(stringReader);

            var mapper = GetMapper();
            List<Car> cars = new List<Car>();

            foreach (var carDTO in importCarDTOs) { 
                Car car = mapper.Map<Car>(carDTO);

                int[] carPartIds = carDTO.PartsIds
                    .Select(x => x.Id)
                    .Distinct()
                    .ToArray();

                var carParts = new List<PartCar>();
                foreach (var id in carPartIds) 
                {
                    carParts.Add(new PartCar
                    {
                        Car = car,
                        PartId = id
                    });
                }

                car.PartsCars = carParts;
                cars.Add(car);
            }

            context.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDTO[]), new XmlRootAttribute("Customers"));
            using StringReader stringReader = new StringReader(inputXml);

            ImportCustomerDTO[] importCustomerDTOs = (ImportCustomerDTO[])xmlSerializer.Deserialize(stringReader);

            var mapper = GetMapper();
            Customer[] customers = mapper.Map<Customer[]>(importCustomerDTOs);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSaleDTO[]), new XmlRootAttribute("Sales"));
            using StringReader stringReader = new StringReader(inputXml);

            ImportSaleDTO[] importSaleDTOs = (ImportSaleDTO[])xmlSerializer.Deserialize(stringReader);

            int[] carIds = context.Cars
                .Select(car => car.Id).ToArray();

            var mapper = GetMapper();
            Sale[] sales = mapper.Map<Sale[]>(importSaleDTOs)
                .Where(s => carIds.Contains(s.CarId)).ToArray();

            context.AddRange(sales);
            context.SaveChanges();
            
            return $"Successfully imported {sales.Length}"; ;
        }

        public static string GetCarsWithDistance(CarDealerContext context) 
        {
            //in Attributes
            var mapper = GetMapper();

            var carsWithDistance = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarWithAttrDTO>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarWithAttrDTO[]), new XmlRootAttribute("cars"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, carsWithDistance, xsn);
            };
            return stringBuilder.ToString().Trim();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context) {
            var mapper = GetMapper();

            var carFromBmw = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportCarsFromMakeBmwDTO>(mapper.ConfigurationProvider)
                .ToArray();

            return SerializeToXml<ExportCarsFromMakeBmwDTO[]>(carFromBmw, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context) {
            var mapper = GetMapper();

            var localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportLocalSuppliersDTO { 
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();


            return SerializeToXml<ExportLocalSuppliersDTO[]>(localSuppliers, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context) 
        {
            //doesnt work
            var mapper = GetMapper();

            //var cars = context.Cars
            //    .Select(c => new ExportCarsWithTheirPartsList
            //    {
            //        Make = c.Make,
            //        Model = c.Model,
            //        TraveledDistance = c.TraveledDistance,
            //        Parts = new ExportPartsDTO(){ 
            //            Name = c.PartsCars.Select(p => p.Part.Name).ToString(),
            //            Price = decimal.Parse(c.PartsCars.Select(p => p.Part.Price).ToString())
            //        }
            //    })
            //    .OrderByDescending(c => c.Parts.Price)
            //    .OrderByDescending(c => c.TraveledDistance)
            //    .ThenBy(c => c.Model)
            //    .Take(5)
            //    .ToArray();

           // var carsWithParts = context.Cars
           //.AsNoTracking()
           //.OrderByDescending(c => c.TraveledDistance)
           //.ThenBy(c => c.Model)
           //.Take(5)
           //.ProjectTo<ExportCarsWithTheirPartsList>(mapper.ConfigurationProvider)
           //.ToArray();

            return SerializeToXml<ExportCarsWithTheirPartsList[]>(carsWithParts, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var mapper = GetMapper();

            var totalSales = context.Customers
               .Where(c => c.Sales.Any())
               .Select(c => new ExportSalesByCustomerDTO
               {
                   FullName = c.Name,
                   BoughtCars = c.Sales.Count,
                   SpentMoney = c.Sales.Sum(s =>
                       s.Car.PartsCars.Sum(pc =>
                           Math.Round(c.IsYoungDriver ? pc.Part.Price * 0.95m : pc.Part.Price, 2)
                       )
                   )
               })
               .OrderByDescending(s => s.SpentMoney)
               .ToArray();

            return SerializeToXml<ExportSalesByCustomerDTO[]>(totalSales, "customers");
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            ExportSaleAppliedDiscountDTO[] sales = context.Sales
                .Select(s => new ExportSaleAppliedDiscountDTO()
                {
                    Car = new ExportCarWithAttrDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(p => p.Part.Price),
                    PriceWithDiscount =
                        Math.Round((double)(s.Car.PartsCars
                            .Sum(p => p.Part.Price) * (1 - (s.Discount / 100))), 4)
                })
                .ToArray();

            return SerializeToXml<ExportSaleAppliedDiscountDTO[]>(sales, "sales");
        }

        //Generic method ro serialize DTOs to XML
        private static string SerializeToXml<T>(T dto, string xmlRootAttribute) { 
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttribute));

            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture)) {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);

                try
                {
                    xmlSerializer.Serialize(stringWriter, dto, xmlSerializerNamespaces);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return stringBuilder.ToString();
        }
    }
}