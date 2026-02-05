using Berichthefte_WPF.Models;
using Berichthefte_WPF.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            UpdateTotals();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.SaveBerichtsheft();
            MessageBox.Show("Berichtsheft saved!");
        }

        // ================= BETRIEB =================
        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
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

            NewActivityName.Clear();
            NewActivityHours.Clear();
        }

        private void DeleteActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is BetrieblicheTaetigkeit t)
            {
                _vm.CurrentBerichtsheft.Betrieb.Remove(t);
                ActivitiesListBox.Items.Refresh();
                UpdateTotals();
            }
        }

        // ================= SCHULE =================
        private void AddSchoolActivityButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentBerichtsheft.Schule.Add(new SchulTaetigkeit
            {
                Fach = NewSchoolFach.Text,
                Beschreibung = NewSchoolBeschreibung.Text
            });

            SchoolListBox.Items.Refresh();
            UpdateTotals();

            NewSchoolFach.Clear();
            NewSchoolBeschreibung.Clear();
        }

        private void DeleteSchoolActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is SchulTaetigkeit t)
            {
                _vm.CurrentBerichtsheft.Schule.Remove(t);
                SchoolListBox.Items.Refresh();
                UpdateTotals();
            }
        }

        // ================= TOTALS =================
        private double GetBetrieblichTotal()
            => _vm.CurrentBerichtsheft.Betrieb.Sum(b => b.Stunden);

        private double GetSchoolTotal()
            => double.TryParse(SchoolTotalHoursTextBox.Text, out var h) ? h : 0;

        private void UpdateTotals()
        {
            double betrieb = GetBetrieblichTotal();
            double schule = GetSchoolTotal();

            TotalBetrieblichHoursTextBlock.Text =
                $"Betriebliche Gesamtstunden: {betrieb}";

            TotalHoursTextBlock.Text =
                $"Gesamtstunden (Betrieb + Schule): {betrieb + schule}";
        }

        private void SchoolTotalHoursTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotals();
        }
    }
}
