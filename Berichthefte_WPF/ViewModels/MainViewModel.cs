using Berichthefte_WPF.Models;
using Berichthefte_WPF.Services.Interfaces;
using Berichthefte_WPF.Services.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Berichthefte_WPF.ViewModels
{
    public class MainViewModel
    {
        private readonly IBerichtsheftStorage _storage;

        private readonly string _filePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "..", "..", "..", "Data", "Reports", "CurrentBerichtsheft.json"
        );
        public Berichtsheft CurrentBerichtsheft { get; set; }

        public MainViewModel()
        {
            _storage = new JsonBerichtsheftStorage();
            LoadBerichtsheft();
        }

        public void LoadBerichtsheft()
        {
            if (File.Exists(_filePath))
            {
                CurrentBerichtsheft = _storage.LoadBerichtsheft(_filePath);
            }
            else
            {
                CurrentBerichtsheft = new Berichtsheft
                {
                    Status = ReportStatus.Draft,
                    Trainee = new TraineeInfo(),
                    Betrieb = new List<BetrieblicheTaetigkeit>(),
                    Schule = new List<SchulTaetigkeit>()
                };
            }
        }

        public void SaveBerichtsheft()
        {
            // ✅ Ensure folder exists before saving
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            _storage.SaveBerichtsheft(CurrentBerichtsheft, _filePath);
        }
    }
}
