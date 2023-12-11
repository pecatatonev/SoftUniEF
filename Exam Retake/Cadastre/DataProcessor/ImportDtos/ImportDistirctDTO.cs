using Cadastre.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    [XmlType("District")]
    public class ImportDistirctDTO
    {
        [Required]
        [XmlAttribute("Region")]
        public string Region { get; set; }
        [Required]
        [MaxLength(80)]
        [MinLength(2)]
        [XmlElement("Name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(8)]
        [MinLength(8)]
        [RegularExpression(@"[A-Z]{2}-\d{5}")]
        [XmlElement("PostalCode")]
        public string PostalCode { get; set; }
        [XmlArray("Properties")]
        [XmlArrayItem("Property")]
        public ImportPropertyDTO[] Properties { get; set; }
    }
}

//•	Id – integer, Primary Key
//•	Name – text with length [2, 80] (required)
//•	PostalCode – text with length 8. All postal codes must have the following structure:starting with two capital letters, followed by e dash '-', followed by five digits. Example: SF - 10000(required)
//•	Region – Region enum (SouthEast = 0, SouthWest, NorthEast, NorthWest)(required)
//•	Properties - collection of type Property