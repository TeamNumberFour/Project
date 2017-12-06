using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Runtime.Serialization;


namespace WebApplication2.Models
{
    [DataContract]
    //[KnownType(typeof(Comment))]
    public class Post
    {
        [DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember]
        public string ownersName { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public Comment[] comments { get; set; }
        [DataMember]
        public int emo { get; set; }
        public string source { get; set; }
        public bool pass { get; set; }

    }
}
