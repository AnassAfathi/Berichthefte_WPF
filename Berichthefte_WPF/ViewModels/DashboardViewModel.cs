using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Berichthefte_WPF.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private double _betrieblicheStunden;
        private double _schulischeStunden;
        private double _gesamtStunden;
        private int _anzahlAktivitäten;
        private int _urlaubsTage;
        private int _kranktage;

        public double BetrieblicheStunden
        {
            get => _betrieblicheStunden;
            set { _betrieblicheStunden = value; OnPropertyChanged(); }
        }

        public double SchulischeStunden
        {
            get => _schulischeStunden;
            set { _schulischeStunden = value; OnPropertyChanged(); }
        }

        public double GesamtStunden
        {
            get => _gesamtStunden;
            set { _gesamtStunden = value; OnPropertyChanged(); }
        }

        public int AnzahlAktivitäten
        {
            get => _anzahlAktivitäten;
            set { _anzahlAktivitäten = value; OnPropertyChanged(); }
        }

        public int UrlaubsTage
        {
            get => _urlaubsTage;
            set { _urlaubsTage = value; OnPropertyChanged(); }
        }

        public int Kranktage
        {
            get => _kranktage;
            set { _kranktage = value; OnPropertyChanged(); }
        }

        public double BetrieblicheProgress => BetrieblicheStunden / 40.0 * 100; // 40h pro Woche
        public double SchulischeProgress => SchulischeStunden / 40.0 * 100;
        public double GesamtProgress => GesamtStunden / 80.0 * 100; // 80h total target

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name == nameof(BetrieblicheStunden) || name == nameof(SchulischeStunden))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BetrieblicheProgress)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SchulischeProgress)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GesamtProgress)));
            }
        }
    }
}
