using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        // Analizar si se puede cambiar por un Guid
        public int Id { get; set; } // usa exactamente el nombre Id, para despues poder usar Entity Framework
        public string UserName { get; set; } // Usa exactamente el nombre UserName para despues poder usar ASP net Core Identity
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }

        // Directamente lo creo proyectando desde el repositorios
        // public int GetAge()
        // {
        //     return DateOfBirth.CalculateAge();
        // }
    }
}