using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LuckyShopClientTestG.Model;
using LuckyShopClientTestG.MVVM;
using LuckyShopClientTestG.View;


namespace LuckyShopClientTestG.ViewModel
{
    internal class ContentViewModel : ViewModelBase
    {
        private const string Json = "Resources\\login.json";

        public ContentViewModel() {

            Auth auth = new Auth();

            ConnectorSQL = new ConnectAPI(auth);

            ConnectButtonLabel = "Connect";

            ConnectorSQL.connection.StateChange += Connection_StateChange;

        }

        private Bestellung currentBestellung;

        private ConnectAPI ConnectorSQL;

        #region Bound Properties
        private ObservableCollection<Produkt> produktListe;

        public ObservableCollection<Produkt> ProduktListe
        {
            get => produktListe;
            set { produktListe = value; OnPropertyChanged(); }
        }

        private Produkt selectedProdukt;

        public Produkt SelectedProdukt
        {
            get { return selectedProdukt; }
            set {
                selectedProdukt = value;
                OnPropertyChanged();
            }
        }

        private string currentName;
        public string CurrentName
        {
            get => currentName;
            set { currentName = value; 
                OnPropertyChanged();
            }
        }

        private string currentAdresse;
        public string CurrentAdresse
        {
            get => currentAdresse;
            set {
                currentAdresse = value; 
                OnPropertyChanged();
            }
        }

        private string currentEmail;
        public string CurrentEmail
        {
            get => currentEmail;
            set {
                currentEmail = value; 
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Produkt> warenkorb = new ObservableCollection<Produkt>();
        public ObservableCollection<Produkt> Warenkorb
        {
            get => warenkorb;
            set { warenkorb = value; 
                OnPropertyChanged();
            }
        }

        private decimal warenKorbSumme = 0;
        public decimal WarenkorbSumme
        {
            get => warenKorbSumme;
            set { warenKorbSumme = value; OnPropertyChanged(); }
        }

        private Produkt selectedArtikel;
        public Produkt SelectedArtikel
         {
            get => selectedArtikel;
            set { selectedArtikel = value; OnPropertyChanged(); }
        }


        private int selectedArtikelIndex;
        public int SelectedArtikelIndex
        {
            get => selectedArtikelIndex;
            set { selectedArtikelIndex = value; OnPropertyChanged(); }
        }



        private string connectButtonLabel;

        public string ConnectButtonLabel
        {
            get => connectButtonLabel;
            set { connectButtonLabel = value; OnPropertyChanged(); }
        }


        private string statusMessage;

        public string StatusMessage {
            get => statusMessage;
            set { statusMessage = value; OnPropertyChanged(); }
        }


        #endregion




        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            StatusMessage = ConnectorSQL.statusMessage;
        }


        #region Relay Commands

        public RelayCommand EstablishConnectionCommand => new RelayCommand(
            _execute => EstablishConnection(),
            _canExecuteFunc => ConnectorSQL != null);
        private void EstablishConnection()
        {

            if (ConnectorSQL.isConnected)
            {
                ConnectorSQL.Close();
            }

            else
            {
                ConnectorSQL.Connect();
            }

            RefreshUIElements();
        }


        private void RefreshUIElements()
        {

            if (ConnectorSQL.isConnected)
            {
                ConnectButtonLabel = "Disconnect";
                ProduktListe = ConnectorSQL.ProduktListe;
            }

            else
            {
                ConnectButtonLabel = "Connect";
                ProduktListe.Clear();
            }
        }



        public RelayCommand InWarenkorbCommand => new RelayCommand(
          _execute => InWarenkorb(),
          _canExecuteFunc => ConnectorSQL != null);
        private void InWarenkorb()
        {
            Warenkorb.Add(SelectedProdukt);
            WarenkorbSumme = Decimal.Round(Warenkorb.Sum(x => x.Preis),2);
        }

        public RelayCommand DeleteArtikelCommand => new RelayCommand(
          _execute => DeleteArtikel(),
          _canExecuteFunc => ConnectorSQL != null);

        private void DeleteArtikel()
        {
            Warenkorb.Remove(SelectedArtikel);
            WarenkorbSumme = Decimal.Round(Warenkorb.Sum(x => x.Preis),2);

        }

        public RelayCommand BestellenCommand => new RelayCommand(
          _execute => Bestellen(),
          _canExecuteFunc => ConnectorSQL != null);

        private void Bestellen()
        {
            currentBestellung = new Bestellung(BestellID: GenerateOrderId(), Produkte: Warenkorb.Select(x=>x.ProduktID).ToList(), Gesamtsumme: WarenkorbSumme);
            ConnectorSQL.BestellungAufnehmen(currentBestellung);
        }

        private string GenerateOrderId()
        {
            // Use a short hash of the customer ID for privacy and length
            string customerHash = CurrentEmail.GetHashCode().ToString("X");
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string random = Guid.NewGuid().ToString("N").Substring(0, 6); // 6-char random string

            return $"{customerHash}-{timestamp}-{random}";
        }

        #endregion
    }
}

