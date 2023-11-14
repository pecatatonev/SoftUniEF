using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [MaxLength(80)]
        [Unicode]
        public string Name { get; set; }

        [Unicode]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public int ResourceId { get; set; }
        [ForeignKey(nameof(ResourceId))]
        public ICollection<Resource> Resources { get; set; }

        public int HomeworkId { get; set; }
        [ForeignKey(nameof(HomeworkId))]
        public ICollection<Homework> Homeworks { get; set; }
        public ICollection<Student> Student { get;}
        //maybe student courses
    }
}
