using Berichthefte_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Services.Interfaces
{
    public interface IPdfExportService
    {
        void ExportToPdf(Berichtsheft berichtsheft, string outputPath);
    }
}
