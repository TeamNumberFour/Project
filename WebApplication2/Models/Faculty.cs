using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Faculty
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UniversityId { get; set; }

        public University University { get; set; }
        [Required]
        public String Title { get; set; }
    }
}
