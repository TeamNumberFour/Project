using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.Requests
{
    public class Choose     
    {
        [Required]
        public string University { get; set; }
        public string Faculty { get; set; }
        public ICollection<University> Universities { get; set; }
        public ICollection<Faculty> Faculties { get; set; }
    }
}
