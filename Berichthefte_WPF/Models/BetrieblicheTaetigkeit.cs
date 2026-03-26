using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Models
{
    public class BetrieblicheTaetigkeit
    {
        public enum TaetigkeitTyp
        {
            Normal,
            Urlaub,
            Krank
        }
        public string? Aktivitaet { get; set; }
        public double Stunden { get; set; }
        public TaetigkeitTyp Typ { get; set; }
    }
}
