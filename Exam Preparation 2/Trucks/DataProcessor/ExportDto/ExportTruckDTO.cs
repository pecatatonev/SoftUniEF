using System.ComponentModel.DataAnnotations;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ExportDto
{
    public class ExportTruckDTO
    {
        public string TruckRegistrationNumber { get; set; }
        public string VinNumber { get; set; }
        public int TankCapacity { get; set; }
        public int CargoCapacity { get; set; }
        public CategoryType CategoryType { get; set; }
        public MakeType MakeType { get; set; }
    }
}