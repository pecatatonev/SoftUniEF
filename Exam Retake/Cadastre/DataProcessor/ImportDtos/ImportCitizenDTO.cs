using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cadastre.DataProcessor.ImportDtos
{
    public class ImportCitizenDTO
    {
        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        [JsonProperty("LastName")]
        public string LastName { get; set; }
        [Required]
        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; }
        [Required]
        [JsonProperty("MaritalStatus")]
        public string MaritalStatus { get; set; }
        [Required]
        [JsonProperty("Properties")]
        public int[] Properties { get; set; }
    }
}
//•	FirstName – text with length [2, 30] (required)
//•	LastName – text with length [2, 30] (required)
//•	BirthDate – DateTime (required)
//•	MaritalStatus - MaritalStatus enum (Unmarried = 0, Married, Divorced, Widowed)(required)