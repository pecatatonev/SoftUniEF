using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastre.Data.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string PropertyIdentifier { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Area { get; set; }
        [MaxLength(500)]
        public string Details { get; set; }
        [Required]
        [MaxLength(200)]
        public string Address { get; set; }
        [Required]
        public DateTime DateOfAcquisition { get; set; }
        [Required]
        public int DistrictId { get; set; }
        [ForeignKey(nameof(DistrictId))]
        public District District { get; set; }
        public ICollection<PropertyCitizen> PropertiesCitizens { get; set; } = new HashSet<PropertyCitizen>();
    }
}

//•	Id – integer, Primary Key
//•	PropertyIdentifier – text with length [16, 20] (required)
//•	Area – int not negative (required)
//•	Details - text with length[5, 500] (not required)
//•	Address – text with length [5, 200] (required)
//•	DateOfAcquisition – DateTime (required)
//•	DistrictId – integer, foreign key (required)
//•	District – District
//•	PropertiesCitizens - collection of type PropertyCitizen
