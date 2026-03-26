using Berichthefte_WPF.Models;
using Berichthefte_WPF.ViewModels;
using Berichthefte_WPF.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static Berichthefte_WPF.Models.BetrieblicheTaetigkeit;

namespace Berichthefte_WPF
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;
            
            // Initialize theme toggle state
            ThemeToggleButton.IsChecked = ThemeManager.CurrentTheme == Theme.Dark;
            
            // Update dashboard
            UpdateDashboard();
            UpdateTotals();
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleButton)sender;
            ThemeManager.CurrentTheme = toggle.IsChecked == true ? Theme.Dark : Theme.Light;
        }

        // ================= DASHBOARD UPDATE =================
        private void UpdateDashboard()
        {
            double betrieb = GetBetrieblichTotal();
            double schule = GetSchoolTotal();
            int activities = _vm.CurrentBerichtsheft.Betrieb.Count + _vm.CurrentBerichtsheft.Schule.Count;
            double gesamtStunden = betrieb + schule;

            BetrieblicheStundenValue.Text = betrieb.ToString("F1");
            SchulischeStundenValue.Text = schule.ToString("F1");
            AnzahlAktivitätenValue.Text = activities.ToString();
            GesamtStundenValue.Text = gesamtStunden.ToString("F1");

            BetrieblicheProgress.Value = Math.Min(betrieb / 40.0 * 100, 100);
            SchulischeProgress.Value = Math.Min(schule / 40.0 * 100, 100);
            GesamtProgress.Value = Math.Min(gesamtStunden / 80.0 * 100, 100);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validiere alle erforderlichen Felder
            var (isValid, errorMessage) = ValidationHelper.ValidateTraineeInfo(
                _vm.CurrentBerichtsheft.Trainee?.Name ?? string.Empty,
                _vm.CurrentBerichtsheft.Trainee?.Firma ?? string.Empty,
                _vm.CurrentBerichtsheft.Trainee?.Abteilung ?? string.Empty
            );

            if (!isValid)
            {
                MessageBox.Show($"Fehler bei der Validierung:\n{errorMessage}", "Validierungsfehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validiere Schulstunden
            (isValid, errorMessage) = ValidationHelper.ValidateSchoolHours(_vm.CurrentBerichtsheft.TotalSchulStunden.ToString());
            if (!isValid)
            {
                MessageBox.Show($"Fehler bei den Schulstunden:\n{errorMessage}", "Validierungsfehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _vm.SaveBerichtsheft();
            MessageBox.Show("Berichtsheft erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ================= BETRIEB =================
        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
            // Validiere Eingaben
            var (isValid, errorMessage) = ValidationHelper.ValidateBetriebActivity(
                NewActivityName.Text,
                NewActivityHours.Text
            );

            if (!isValid)
            {
                MessageBox.Show($"Fehler beim Hinzufügen der Aktivität:\n{errorMessage}", "Validierungsfehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(NewActivityHours.Text, out double hours))
                return;

            var type = TaetigkeitTyp.Normal;
            if (NewActivityType.SelectedItem is ComboBoxItem item)
            {
                type = item.Content.ToString() switch
                {
                    "Urlaub" => TaetigkeitTyp.Urlaub,
                    "Krank" => TaetigkeitTyp.Krank,
                    _ => TaetigkeitTyp.Normal
                };
            }

            _vm.CurrentBerichtsheft.Betrieb.Add(new BetrieblicheTaetigkeit
            {
                Aktivitaet = NewActivityName.Text,
                Stunden = hours,
                Typ = type
            });

            ActivitiesListBox.Items.Refresh();
            UpdateTotals();
            UpdateDashboard();

            NewActivityName.Clear();
            NewActivityHours.Clear();
            NewActivityType.SelectedIndex = 0;
        }

        private void DeleteActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is BetrieblicheTaetigkeit t)
            {
                // Bestätigungsdialog
                var result = MessageBox.Show(
                    $"Möchten Sie diese Aktivität wirklich löschen?\n\n\"{t.Aktivitaet}\" ({t.Stunden} Stunden)",
                    "Bestätigung erforderlich",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    _vm.CurrentBerichtsheft.Betrieb.Remove(t);
                    ActivitiesListBox.Items.Refresh();
                    UpdateTotals();
                    UpdateDashboard();
                }
            }
        }

        // ================= SCHULE =================
        private void AddSchoolActivityButton_Click(object sender, RoutedEventArgs e)
        {
            // Validiere Eingaben
            var (isValid, errorMessage) = ValidationHelper.ValidateSchoolActivity(
                NewSchoolFach.Text,
                NewSchoolBeschreibung.Text
            );

            if (!isValid)
            {
                MessageBox.Show($"Fehler beim Hinzufügen der Schulaktivität:\n{errorMessage}", "Validierungsfehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _vm.CurrentBerichtsheft.Schule.Add(new SchulTaetigkeit
            {
                Fach = NewSchoolFach.Text,
                Beschreibung = NewSchoolBeschreibung.Text
            });

            SchoolListBox.Items.Refresh();
            UpdateTotals();
            UpdateDashboard();

            NewSchoolFach.Clear();
            NewSchoolBeschreibung.Clear();
        }

        private void DeleteSchoolActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is SchulTaetigkeit t)
            {
                // Bestätigungsdialog
                var result = MessageBox.Show(
                    $"Möchten Sie diese Schulaktivität wirklich löschen?\n\n\"{t.Fach}\"",
                    "Bestätigung erforderlich",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    _vm.CurrentBerichtsheft.Schule.Remove(t);
                    SchoolListBox.Items.Refresh();
                    UpdateTotals();
                    UpdateDashboard();
                }
            }
        }

        // ================= TOTALS =================
        private double GetBetrieblichTotal()
            => _vm.CurrentBerichtsheft.Betrieb.Sum(b => b.Stunden);

        private double GetSchoolTotal()
            => _vm.CurrentBerichtsheft.TotalSchulStunden;

        private void UpdateTotals()
        {
            double betrieb = GetBetrieblichTotal();
            double schule = GetSchoolTotal();

            TotalBetrieblichHoursTextBlock.Text =
                $"Betriebliche Gesamtstunden: {betrieb}";

            TotalHoursTextBlock.Text =
                $"Gesamtstunden (Betrieb + Schule): {betrieb + schule}";

            // Update Dashboard
            UpdateDashboard();
        }

        private void SchoolTotalHoursTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotals();
        }

        // ================= CLEAR ALL =================
        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Möchten Sie wirklich ALLE Daten löschen?",
                "Warnung",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                // Create a completely new Berichtsheft
                _vm.CurrentBerichtsheft = new Berichtsheft
                {
                    Status = ReportStatus.Draft,
                    Trainee = new TraineeInfo(),
                    Zeitraum = new Zeitraum(),
                    Betrieb = new List<BetrieblicheTaetigkeit>(),
                    Schule = new List<SchulTaetigkeit>(),
                    Beschreibung = string.Empty,
                    TotalSchulStunden = 0
                };

                // Update DataContext to refresh all bindings
                DataContext = null;
                DataContext = _vm;

                // Clear UI Input Fields
                NewActivityName.Clear();
                NewActivityHours.Clear();
                NewActivityType.SelectedIndex = 0;
                NewSchoolFach.Clear();
                NewSchoolBeschreibung.Clear();

                // Update totals
                UpdateTotals();

                MessageBox.Show("Alle Daten wurden gelöscht. Neues Dokument erstellt.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
