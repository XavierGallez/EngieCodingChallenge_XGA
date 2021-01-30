using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngieCCComputer
{
    public class Payload
    {
        public double load { get; set; }
        public Dictionary<string, double> fuels { get; set; }
        public List<PowerPlant> powerplants { get; set; }
    }
}
