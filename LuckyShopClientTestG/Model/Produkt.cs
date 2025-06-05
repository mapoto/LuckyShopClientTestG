using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyShopClientTestG.Model
{
    public class Produkt
    {
        public  string ProduktID { get; set; }
        public decimal Preis { get; set; }
        public string Bezeichnung { get; set; }

        public Produkt(string ProduktID, decimal Preis, string Bezeichnung)
        {
            this.Preis = Decimal.Round(Preis, 2);
            this.Bezeichnung = Bezeichnung;
            this.ProduktID = ProduktID; 
            
        }
    }
}
