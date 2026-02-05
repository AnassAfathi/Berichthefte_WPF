using Berichthefte_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Services.Interfaces
{
    public interface IBerichtsheftStorage
    {
        void SaveBerichtsheft(Berichtsheft berichtsheft, string filePath);
        Berichtsheft LoadBerichtsheft(string filePath);
    }
}
