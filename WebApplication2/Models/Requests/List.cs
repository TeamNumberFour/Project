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
        public bool filtered { get; set; }
        public bool employee { get; set; }
        public bool student { get; set; }
        public bool abitur { get; set; }
        public bool grad { get; set; }
        public bool employer { get; set; }
        public string nickname { get; set; }
        public bool positive { get; set; }
        public bool negative { get; set; }
        public bool neutral { get; set; }
        public bool uncertain { get; set; }
        public string key { get; set; }
        public DateTime data1 { get; set; }
        public DateTime data2 { get; set; }
    }
}
