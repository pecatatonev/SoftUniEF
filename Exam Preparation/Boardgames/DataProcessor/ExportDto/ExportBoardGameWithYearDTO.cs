﻿using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Boardgame")]
    public class ExportBoardGameWithYearDTO
    {
        [XmlElement("BoardgameName")]
        public string Name { get; set; }
        [XmlElement("BoardgameYearPublished")]
        public int YearPublished { get; set; }
    }
}