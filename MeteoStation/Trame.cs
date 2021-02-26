using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoStation
{
    class Trame
    {
        public int id { get; set; }
        public int cptOctet { get; set; }
        public int Type { get; set; }
        public int checksum { get; set; }
        public String data { get; set; }
    }
}
