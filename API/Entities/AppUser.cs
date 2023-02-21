using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } // usa exactamente el nombre Id, para despues poder usar Entity Framework
        public string UserName { get; set; } // Usa exactamente el nombre UserName para despues poder usar ASP net Core Identity
    }
}
