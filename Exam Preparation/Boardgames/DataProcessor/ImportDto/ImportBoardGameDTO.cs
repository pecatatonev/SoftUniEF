using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Boardgame")]
    public class ImportBoardGameDTO
    {
        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        [XmlElement("Name")]
        public string Name { get; set; }
        [Required]
        [Range(1.00, 10.00)]
        [XmlElement("Rating")]
        public double Rating { get; set; }
        [Required]
        [Range(2018, 2023)]
        [XmlElement("YearPublished")]
        public int YearPublished { get; set; }
        [Required]
        [XmlElement("CategoryType")]
        public int CategoryType { get; set; }
        [Required]
        [XmlElement("Mechanics")]
        public string Mechanics { get; set; }
    }
}

//•	Name – text with length [10…20] (required)
//•	Rating – double in range [1…10.00] (required)
//•	YearPublished – integer in range [2018…2023] (required)
//•	CategoryType – enumeration of type CategoryType, with possible values (Abstract, Children, Family, Party, Strategy) (required)
//•	Mechanics – text (string, not an array) (required)
