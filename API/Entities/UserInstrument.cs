using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class UserInstrument
    {
        public int Id { get; set; }
        public string InstrumentName { get; set; }
        public string PublicId { get; set; }
        public AppUser AppUser { get; set; }
    }
}