using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Domain
{
    public class AppUser
    {
        // Analizar si se puede cambiar por un Guid
        public int Id { get; set; } // usa exactamente el nombre Id, para despues poder usar Entity Framework
        public string UserName { get; set; } // Usa exactamente el nombre UserName para despues poder usar ASP net Core Identity
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
