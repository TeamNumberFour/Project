using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class University
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ICollection<Faculty> Faculties { get; set; }
        [Required]
        public String Title { get; set; }
    }
}
