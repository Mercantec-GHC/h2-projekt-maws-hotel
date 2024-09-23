using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels
{
    public class Room
    {
        public int Id { get; set; }
        public bool CurrentlyBooked { get; set; }
        public float Price { get; set; }
        public int DigitalKey { get; set; }
        public int Type { get; set; }
        public string Photos { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
