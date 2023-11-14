using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models
{
    public class Writer
    {
        public int Id { get; set; }
        [MaxLength(20)]
        [Required]
        public string Name { get; set; }
        public string? Pseudonym { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
    }
}