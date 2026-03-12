using Berichthefte_WPF.Models;
using Berichthefte_WPF.Services.Interfaces;
using Berichthefte_WPF.Services.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Berichthefte_WPF.ViewModels
{
    public class MainViewModel
    {
        private readonly IBerichtsheftStorage _storage;

        private readonly IPdfExportService _pdfExportService;
        public ICommand ExportPdfCommand { get; set; }
      

        private readonly string _filePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "..", "..", "..", "Data", "Reports", "CurrentBerichtsheft.json"
        );
        public Berichtsheft CurrentBerichtsheft { get; set; }

        public MainViewModel()
        {
            _storage = new JsonBerichtsheftStorage();

            _pdfExportService = new BerichtsheftPdfExportService();
            ExportPdfCommand = new RelayCommand(ExportPdf);
            if (File.Exists(_filePath))
                File.Delete(_filePath);
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
                    Zeitraum = new Zeitraum(),
                    Betrieb = new List<BetrieblicheTaetigkeit>(),
                    Schule = new List<SchulTaetigkeit>(),
                    Beschreibung = "",
                    TotalSchulStunden = 0

                };
            }
            if (CurrentBerichtsheft.Zeitraum == null)
                CurrentBerichtsheft.Zeitraum = new Zeitraum();

            if (CurrentBerichtsheft.Trainee == null)
                CurrentBerichtsheft.Trainee = new TraineeInfo();

            if (CurrentBerichtsheft.Betrieb == null)
                CurrentBerichtsheft.Betrieb = new List<BetrieblicheTaetigkeit>();

            if (CurrentBerichtsheft.Schule == null)
                CurrentBerichtsheft.Schule = new List<SchulTaetigkeit>();
        }

        public void SaveBerichtsheft()
        {
            // ✅ Ensure folder exists before saving
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            _storage.SaveBerichtsheft(CurrentBerichtsheft, _filePath);
        }

        private void ExportPdf()
        {
            if (CurrentBerichtsheft == null)
            {
                MessageBox.Show("Kein Berichtsheft vorhanden.");
                return;
            }

            try
            {
                _storage.SaveBerichtsheft(CurrentBerichtsheft, _filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern vor dem Export: {ex.Message}");
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"Berichtsheft_{CurrentBerichtsheft.Zeitraum?.AusbildungsnachweisNr ?? 0}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfExportService.ExportToPdf(CurrentBerichtsheft, saveFileDialog.FileName);
                    MessageBox.Show("PDF erfolgreich erstellt.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Export: {ex.Message}");
                }
            }
        }
    }
}
