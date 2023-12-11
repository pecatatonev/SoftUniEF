using Cadastre.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    [XmlType(nameof(Property))]
    public class ImportPropertyDTO
    {
        [Required]
        [MaxLength(20)]
        [MinLength(16)]
        [XmlElement("PropertyIdentifier")]
        public string PropertyIdentifier { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        [XmlElement("Area")]
        public int Area { get; set; }
        [MaxLength(500)]
        [MinLength(5)]
        [XmlElement("Details")]
        public string Details { get; set; }
        [Required]
        [MaxLength(200)]
        [MinLength(5)]
        [XmlElement("Address")]
        public string Address { get; set; }
        [Required]
        [XmlElement("DateOfAcquisition")]
        public string DateOfAcquisition { get; set; }
    }
}
//•	PropertyIdentifier – text with length [16, 20] (required)
//•	Area – int not negative (required)
//•	Details - text with length[5, 500] (not required)
//•	Address – text with length [5, 200] (required)
//•	DateOfAcquisition – DateTime (required)