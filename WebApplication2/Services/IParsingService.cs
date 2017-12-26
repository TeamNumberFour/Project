using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IParsingService
    {
         
         Task<Post[]> Common(string query, string Un, string Fa);
        string getstr(string str); // new
    }
}
