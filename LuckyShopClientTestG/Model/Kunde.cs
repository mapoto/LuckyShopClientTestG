using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyShopClientTestG.Model
{
    public class Kunde
    {
        /*Hashed Email Adress*/
        public string KundenID { get; set; }       

        public string Name { get; set; }
        public string Adresse { get; set; }

        public Kunde(string KundenID, string Name, string Adresse)
        {
            this.KundenID = KundenID;
            this.Name = Name;   
            this.Adresse = Adresse; 
        }

        public override bool Equals(Object obj)
        {
 

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Kunde a = (Kunde)obj;

            if (a.KundenID != this.KundenID)
            {
                return false;
            }

            return true;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return KundenID.GetHashCode();
        }

    }
}
