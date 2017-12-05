using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace WebApplication2.Models
{
    public class Comment
    {
        [DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember]
        public string ownersName { get; set; }
        [DataMember]
        public string text { get; set; }
    }
}
