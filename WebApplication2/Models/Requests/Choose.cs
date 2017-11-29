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
        public String University { get; set; }
        public String Faculty { get; set; }
    }
}
