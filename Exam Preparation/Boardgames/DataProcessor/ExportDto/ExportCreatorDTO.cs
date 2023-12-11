using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Creator")]
    public class ExportCreatorDTO
    {
        [XmlAttribute("BoardgamesCount")]
        public int BoardgamesCount { get; set; }
        public string CreatorName { get; set; }
        [XmlArray]
        public ExportBoardGameWithYearDTO[] Boardgames { get; set; }
    }
}
