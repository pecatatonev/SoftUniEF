using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class ImportDespatchersDTO
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; }
        public string Position { get; set; }
        [XmlArray]
        public ImportTruckDTO[] Trucks { get; set; }
    }
}
//•	Id – integer, Primary Key
//•	Name – text with length [2, 40] (required)
//•	Position – text
//•	Trucks – collection of type Truck
