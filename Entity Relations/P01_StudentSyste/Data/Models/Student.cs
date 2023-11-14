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
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [MaxLength(100)]
        [Unicode]
        public string Name { get; set; }

        [StringLength(10)]
        public string? PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public int HomeworkId { get; set; }
        [ForeignKey(nameof(HomeworkId))]
        public ICollection<Homework> Homeworks { get; set; }
        
        public ICollection<Course> Course { get; set; }
        //maybe studentCourses
    }
}
