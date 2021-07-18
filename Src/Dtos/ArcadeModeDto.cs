using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWArcadeBackend.Dtos
{
    public class ArcadeModeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Players { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
