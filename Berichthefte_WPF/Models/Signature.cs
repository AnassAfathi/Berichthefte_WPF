using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Models
{
    public class Signature
    {
        public byte[] ImageData { get; set; }
        public DateTime SignedAt { get; set; }
    }
}
