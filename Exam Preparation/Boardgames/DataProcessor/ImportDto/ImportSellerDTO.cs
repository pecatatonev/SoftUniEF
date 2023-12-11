using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerDTO
    {
        [Required]
        [MaxLength(20)]
        [MinLength(5)]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        public string Address { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [RegularExpression(@"^www\.[a-zA-Z0-9-]+\.com")]
        public string Website { get; set; }

        [JsonProperty("Boardgames")]
        public int[] BoardgameIds { get; set; }
    }
}

//Name – text with length [5…20] (required)
//•	Address – text with length [2…30] (required)
//•	Country – text (required)
//•	Website – a string (required). First four characters are "www.", followed by upper and lower letters, digits or '-' and the last three characters are ".com".
