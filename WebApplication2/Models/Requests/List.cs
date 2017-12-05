using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.Requests
{
    public class List: Choose
    {
        public string Query { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
