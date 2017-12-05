using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IParsingService
    {
         Task Gazeta(string from, string to);
        Task<Post[]> Common(string query);

    }
}
