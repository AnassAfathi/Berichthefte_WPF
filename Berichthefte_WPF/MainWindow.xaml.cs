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

        /// <summary>
        /// THEME TOGGLE - Benutzer wechselt Dark/Light Mode
        /// RESULTAT:  Dark/Light Mode wechsel erfolgreich!
        /// </summary>
        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleButton)sender;
            ThemeManager.CurrentTheme = toggle.IsChecked == true ? Theme.Dark : Theme.Light;
        }

        
        /// <summary>
        /// UPDATEDASHBOARD - Aktualisiere die 4 großen Stat-Cards
        /// BERECHNUNG:
        /// 1. GetBetrieblichTotal() → Summe betrieblicher Stunden
        /// 2. GetSchoolTotal() → Summe schulischer Stunden
        /// 3. Activities.Count → Anzahl der Einträge
        /// 4. Gesamtstunden = betrieb + schule
        /// 
        /// RESULTAT: Dashboard zeigt aktuelle Statistiken!
        /// </summary>
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

        /// <summary>
        /// SAVEBUTTON_CLICK - Benutzer klickt "Save" Button
        /// 
        /// FLOW:
        /// 1. Validiere Trainee-Info (Name, Firma, Abteilung erforderlich)
        /// 2. Falls Fehler → Zeige Error-Dialog und beende
        /// 3. Validiere Schulstunden (müssen gültige Zahl sein)
        /// 4. Falls Fehler → Zeige Error-Dialog und beende
        /// 5. Falls alles OK → Speichere mit _vm.SaveBerichtsheft()
        /// 6. Zeige Erfolgs-Meldung
        /// 
        /// RESULTAT: Daten sind in JSON-Datei gespeichert!
        /// </summary>
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

        /// <summary>
        /// ADDACTIVITYBUTTON_CLICK - Benutzer klickt "Add" für betriebliche Tätigkeit
        /// 
        /// FLOW - 7 SCHRITTE:
        /// 1. Validiere Eingaben (Aktivitätsbeschreibung + Stunden)
        /// 2. Falls Fehler → Zeige Error und beende
        /// 3. Konvertiere Stunden zu Double
        /// 4. Lese Typ aus ComboBox (Normal/Urlaub/Krank)
        /// 5. Erstelle neues BetrieblicheTaetigkeit Objekt
        /// 6. Füge zur Betrieb-Liste hinzu
        /// 7. Aktualisiere UI (ListBox, Totals, Dashboard)
        /// 8. Lösche Eingabefelder (Clear für nächsten Eintrag)
        /// 
        /// RESULTAT:  Betriebliche Tätigkeit hinzugefügt!
        /// </summary>
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

        /// <summary>
        /// DELETEACTIVITYBUTTON_CLICK - Benutzer klickt × neben betrieblicher Tätigkeit
        /// 
        /// FLOW:
        /// 1. Ermittle welche Tätigkeit gelöscht werden soll (aus Button.DataContext)
        /// 2. Zeige Bestätigungsdialog mit Aktivitätsdetails
        /// 3. Falls Benutzer "Nein" klickt → Beende (nichts löschen)
        /// 4. Falls Benutzer "Ja" klickt:
        ///    a. Entferne Tätigkeit aus Liste
        ///    b. Aktualisiere ListBox
        ///    c. Berechne neue Totals
        ///    d. Aktualisiere Dashboard
        ///  
        /// RESULTAT: Betriebliche Tätigkeit gelöscht!
        /// </summary>
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

        /// <summary>
        /// ADDSCHOOLACTIVITYBUTTON_CLICK - Benutzer klickt "Add" für Schulaktivität
        /// 
        /// FLOW:
        /// 1. Validiere Eingaben (Fach + Beschreibung)
        /// 2. Falls Fehler → Zeige Error und beende
        /// 3. Erstelle neues SchulTaetigkeit Objekt
        /// 4. Füge zur Schule-Liste hinzu
        /// 5. Aktualisiere UI (ListBox, Totals, Dashboard)
        /// 6. Lösche Eingabefelder
        /// RESULTAT: Schulaktivität hinzugefügt!
        /// </summary>
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

        /// <summary>
        /// DELETESCHOOLACTIVITYBUTTON_CLICK - Benutzer klickt × neben Schulaktivität
        /// 
        /// FLOW:
        /// 1. Ermittle welche Schulaktivität gelöscht werden soll
        /// 2. Zeige Bestätigungsdialog
        /// 3. Falls "Ja": Lösche + aktualisiere UI
        /// 4. Falls "Nein": Beende (nichts löschen)
        /// 
        /// RESULTAT: Schulaktivität gelöscht!
        /// </summary>
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

        /// <summary>
        /// GETBETRIEBLICHTOTAL - Berechne Summe aller betrieblichen Stunden
        /// 
        /// FUNKTION:
        /// • Durchsucht alle Einträge in der Betrieb-Liste
        /// • Addiert alle "Stunden" Werte
        /// • Gibt Gesamtsumme zurück
        /// 
        /// RESULTAT:  Betriebliche Stundensumme berechnet!
        /// </summary>
        private double GetBetrieblichTotal()
            => _vm.CurrentBerichtsheft.Betrieb.Sum(b => b.Stunden);

        /// <summary>
        /// GETSCHOOLTOTAL - Gib Summe schulischer Stunden zurück
        /// 
        /// WARUM?
        /// • Schulaktivitäten haben KEINE Stundenangabe (nur Fach+Beschreibung)
        /// • Schulstunden werden SEPARAT in einem Textfeld eingegeben
        /// • TotalSchulStunden = Manuell eingetragene Schulstunden
        /// 
        /// RESULTAT: Schulstundensumme zurückgegeben!
        /// </summary>
        private double GetSchoolTotal()
            => _vm.CurrentBerichtsheft.TotalSchulStunden;

        /// <summary>
        /// UPDATETOTALS - Berechne und zeige alle Summen
        /// 
        /// FLOW:
        /// 1. Berechne betriebliche Gesamtstunden (GetBetrieblichTotal)
        /// 2. Berechne schulische Gesamtstunden (GetSchoolTotal)
        /// 3. Aktualisiere TextBlocks mit den Summen
        /// 4. Rufe UpdateDashboard auf (aktualisiert Statistiken)
        /// 
        /// 
        /// RESULTAT:  Alle Totals aktualisiert!
        /// </summary>
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

        /// <summary>
        /// SCHOOLTOTALHOURS_TEXTCHANGED - Echtzeit-Update wenn Schulstunden ändern
        ///
        /// 
        /// FLOW:
        /// 1. Event wird triggered
        /// 2. UpdateTotals() wird aufgerufen
        /// 3. Dashboard wird aktualisiert
        /// 4. Benutzer sieht neue Werte in ECHTZEIT!
        /// 
        /// 
        /// RESULTAT: Dashboard aktualisiert sich in Echtzeit!
        /// </summary>
        private void SchoolTotalHoursTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotals();
        }

        /// <summary>
        /// CLEARALLBUTTON_CLICK - Benutzer klickt "Clear All" Button
        /// 1. Zeige Bestätigungsdialog mit WARNING-Icon
        /// 2. Falls Benutzer "Nein" klickt → Beende (nichts löschen)
        /// 3. Falls Benutzer "Ja" klickt:
        ///    a. Erstelle völlig NEUES, leeres Berichtsheft-Objekt
        ///    b. Aktualisiere Binding: DataContext = null; DataContext = _vm
        ///    c. Leere alle UI Eingabefelder (TextBoxes, ComboBoxes)
        ///    d. Berechne neue Totals
        ///    e. Zeige Erfolgs-Meldung
        /// 
        /// RESULTAT: Alles ist gelöscht! Neues Berichtsheft erstellt!
        /// </summary>
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
