using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngieCCComputer
{
    public class LoadResponse
    {
        public string name { get; set; }
        public double p { get; set; }
        public override string ToString() => name + ": " + String.Format("{0:0.0}", p) + " MW";
    }
 }
