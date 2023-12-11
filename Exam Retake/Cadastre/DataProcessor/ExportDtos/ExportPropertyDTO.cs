using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ExportDtos
{
    [XmlType("Property")]
    public class ExportPropertyDTO
    {
        [XmlAttribute("postal-code")]
        public string PostalCode { get; set; }
        [XmlElement]
        public string PropertyIdentifier { get; set; }
        [XmlElement]
        public int Area { get; set; }
        [XmlElement]
        public string DateOfAcquisition { get; set; }
    }
}
