using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Models
{
    public class TraineeInfo
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Firma { get; set; }
        [Required]
        public string Abteilung { get; set; }
        [Range(1, 4)]
        public int Ausbildungsjahr { get; set; }
    }
}
