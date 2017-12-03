using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IParsingService
    {
         void Gazeta(string req, string from, string to);

    }
}
