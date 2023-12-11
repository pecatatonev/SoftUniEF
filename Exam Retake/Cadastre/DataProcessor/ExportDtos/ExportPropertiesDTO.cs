using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.DataProcessor.ExportDtos
{
    public class ExportPropertiesDTO
    {
        public string PropertyIdentifier { get; set; }
        public int Area { get; set; }
        public string Address { get; set; }
        public string DateOfAcquisition { get; set; }
        public ExportOwnerDTO[] Owners { get; set; }
    }
}
