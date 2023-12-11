namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlTypes;
    using System.Net;
    using System.Text;
    using Data;
    using Invoices.Extensions;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<ImportDespatchersDTO> despatchersDTOs = xmlString.DeserializeFromXml<List<ImportDespatchersDTO>>("Despatchers");

            List<Despatcher> despatchers = new List<Despatcher>();

            foreach (var despatcherDTO in despatchersDTOs)
            {
                if (!IsValid(despatcherDTO)) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (despatcherDTO.Position == null || despatcherDTO.Position == string.Empty) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var despatcherToAdd = new Despatcher()
                {
                    Name = despatcherDTO.Name,
                    Position = despatcherDTO.Position,
                };

                foreach (var truck in despatcherDTO.Trucks)
                {
                    if (IsValid(truck))
                    {
                        despatcherToAdd.Trucks.Add(new Truck()
                        {
                            RegistrationNumber = truck.RegistrationNumber,
                            VinNumber = truck.VinNumber,
                            TankCapacity = truck.TankCapacity,
                            CargoCapacity = truck.CargoCapacity,
                            CategoryType = (CategoryType)truck.CategoryType,
                            MakeType = (MakeType)truck.MakeType
                        });
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                    }

                }

                    despatchers.Add(despatcherToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcherToAdd.Name, despatcherToAdd.Trucks.Count));
            }

            context.Despatchers.AddRange(despatchers);
            context.SaveChanges();

            return sb.ToString();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            List<ImportClientDTO> clientDTOs = jsonString.DeserializeFromJson<List<ImportClientDTO>>();

            List<Client> clients = new List<Client>();

            int[] trucksIds = context.Trucks.Select(x => x.Id).ToArray();

            foreach (var client in clientDTOs)
            {
                if (!IsValid(client) || client.Type == "usual") 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var clientToAdd = new Client()
                {
                    Name = client.Name,
                    Nationality = client.Nationality,
                    Type = client.Type,
                };

                foreach (var truckId in client.Trucks.Distinct()) 
                {
                    if (trucksIds.Contains(truckId))
                    {
                        clientToAdd.ClientsTrucks.Add(new ClientTruck()
                        {
                            TruckId = truckId
                        });
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                    }
                }

                clients.Add(clientToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedClient, clientToAdd.Name, clientToAdd.ClientsTrucks.Count));
            }

            context.AddRange(clients);
            context.SaveChanges();
            return sb.ToString();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}