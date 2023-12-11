using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Trucks.Data.Models.Enums;
using Trucks.Data.Models;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTruckDTO
    {
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression(@"[A-Z]{2}[0-9]{4}[A-Z]{2}")]
        public string RegistrationNumber { get; set; }
        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string VinNumber { get; set; }
        [Range(950, 1420)]
        public int TankCapacity { get; set; }
        [Range(5000,29000)]
        public int CargoCapacity { get; set; }
        [Required]
        public int CategoryType { get; set; }
        [Required]
        public int MakeType { get; set; }
    }
}

//•	RegistrationNumber – text with length 8. First two characters are upper letters [A-Z], followed by four digits and the last two characters are upper letters [A-Z] again.
//•	VinNumber – text with length 17 (required)
//•	TankCapacity – integer in range [950…1420]
//•	CargoCapacity – integer in range [5000…29000]
//•	CategoryType – enumeration of type CategoryType, with possible values (Flatbed, Jumbo, Refrigerated, Semi) (required)
//•	MakeType – enumeration of type MakeType, with possible values (Daf, Man, Mercedes, Scania, Volvo) (required)