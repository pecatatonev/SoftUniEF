

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        [Key]
        public int Id { get; set; }
        [StringLength(8)]
        public string RegistrationNumber { get; set; }
        [Required]
        [StringLength(17)]
        public string VinNumber { get; set; }
        public int TankCapacity { get; set; }
        public int CargoCapacity { get; set; }
        [Required]
        public CategoryType CategoryType { get; set; }
        [Required]
        public MakeType MakeType { get; set; }
        [Required]
        public int DespatcherId { get; set; }
        [ForeignKey(nameof(DespatcherId))]
        public Despatcher Despatcher { get; set; }
        public ICollection<ClientTruck> ClientsTrucks { get; set; } = new HashSet<ClientTruck>();
    }
}

//•	Id – integer, Primary Key
//•	RegistrationNumber – text with length 8. First two characters are upper letters [A-Z], followed by four digits and the last two characters are upper letters [A-Z] again.
//•	VinNumber – text with length 17 (required)
//•	TankCapacity – integer in range [950…1420]
//•	CargoCapacity – integer in range [5000…29000]
//•	CategoryType – enumeration of type CategoryType, with possible values (Flatbed, Jumbo, Refrigerated, Semi) (required)
//•	MakeType – enumeration of type MakeType, with possible values (Daf, Man, Mercedes, Scania, Volvo) (required)
//•	DespatcherId – integer, foreign key (required)
//•	Despatcher – Despatcher 
//•	ClientsTrucks – collection of type ClientTruck

