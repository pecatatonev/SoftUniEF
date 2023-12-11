using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trucks.DataProcessor.ExportDto
{
    public class ExportClientDTO
    {
        public string Name { get; set; }

        public ExportTruckDTO[] Trucks { get; set; }
    }
}
