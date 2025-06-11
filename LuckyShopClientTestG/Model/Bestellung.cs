using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace LuckyShopClientTestG.Model
{
    public class Bestellung
    {
        public string BestellID { get; set; }
        public List<string> Produkte { get; set; }
        public decimal Gesamtsumme { get; set; }
        public string KundenID { get; set; }

        // TODO: needs to have Versandadresse

        public Bestellung(string BestellID, List<string> Produkte, decimal Gesamtsumme, string KundenID)
        {
            this.BestellID = BestellID;
            this.Produkte = Produkte;
            this.Gesamtsumme = Gesamtsumme;
            this.KundenID = KundenID;
        }

        public override bool Equals(Object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Bestellung a = (Bestellung)obj;

            if (a.BestellID != this.BestellID)
            {
                return false;
            }

            return true;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return BestellID.GetHashCode();
        }

    }
}
