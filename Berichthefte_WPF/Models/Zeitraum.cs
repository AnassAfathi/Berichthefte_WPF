using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Models
{
    public class Zeitraum
    {
        public DateTime Von { get; set; }
        public DateTime Bis { get; set; }
        public int KalenderWoche { get; set; }
        public int AusbildungsnachweisNr { get; set; }
    }
}
