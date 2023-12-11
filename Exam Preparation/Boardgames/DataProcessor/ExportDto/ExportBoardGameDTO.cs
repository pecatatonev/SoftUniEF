using Boardgames.Data.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    public class ExportBoardGameDTO
    {
        public string Name { get; set; }
        public double Rating { get; set; }
        public string Mechanics { get; set; }
        [JsonProperty("Category")]
        public CategoryType Category { get; set; }
    }
}
