using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Models
{
    public class Berichtsheft
    {
        public TraineeInfo Trainee { get; set; }
        public Zeitraum Zeitraum { get; set; }
        public List<BetrieblicheTaetigkeit> Betrieb { get; set; }
        public string Beschreibung { get; set; } = string.Empty;
        public List<SchulTaetigkeit> Schule { get; set; }

        public Signature TraineeSignature { get; set; }
        public Signature AusbilderSignature { get; set; }

        public ReportStatus Status { get; set; }

        // Single total school hours
        public double TotalSchulStunden { get; set; } = 0;
    }
}
