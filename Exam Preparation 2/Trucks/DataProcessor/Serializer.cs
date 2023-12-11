namespace Trucks.DataProcessor
{
    using Data;
    using Invoices.Extensions;
    using Trucks.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despatchers = context.Despatchers
                .Where(d => d.Trucks.Count() > 0)
                .Select(d => new ExportDespatcherDTO() 
                {
                    Name = d.Name,
                    TrucksCount = d.Trucks.Count(),
                    Trucks = d.Trucks.Select(t => new ExportTruckRegNumAndMakeDTO() 
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType
                    })
                    .OrderBy(t => t.RegistrationNumber)
                    .ToArray()
                })
                .OrderByDescending(d => d.TrucksCount)
                .ThenBy(d => d.Name)
                .ToArray();

            return despatchers.SerializeToXml<ExportDespatcherDTO[]>("Despatchers");
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Where(c => c.ClientsTrucks.Any(t => t.Truck.TankCapacity >= capacity))
                .Select(c => new ExportClientDTO()
                {
                    Name = c.Name,
                    Trucks = c.ClientsTrucks
                    .Where(t => t.Truck.TankCapacity >= capacity)
                    .Select(t => new ExportTruckDTO()
                    {
                        TruckRegistrationNumber = t.Truck.RegistrationNumber,
                        VinNumber = t.Truck.VinNumber,
                        TankCapacity = t.Truck.TankCapacity,
                        CargoCapacity = t.Truck.CargoCapacity,
                        CategoryType = t.Truck.CategoryType,
                        MakeType = t.Truck.MakeType,

                    })
                    .OrderBy(t => t.MakeType)
                    .ThenByDescending(t => t.CargoCapacity)
                    .ToArray()
                })
                .OrderByDescending(c => c.Trucks.Length)
                .ThenBy(c => c.Name)
                .Take(10)
                .ToArray();

            return clients.SerializeToJson<ExportClientDTO[]>();
        }
    }
}
