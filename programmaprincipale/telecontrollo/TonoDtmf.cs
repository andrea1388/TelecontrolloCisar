using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    
    class TonoDtmf
    {
        private byte tono;
        public TonoDtmf(byte tono) 
        {
            this.tono=tono;
        }
        public TonoDtmf()
        {
            this.tono = 0;
        }
        public byte Valore
        {
            get {return tono;}
            set {tono = value;}
        }
        public string ToString() 
        {
            char chr;
            if (tono >0 && tono <= 9)
            {
                chr=(char)('0' + tono);
                return chr.ToString();
            }
            switch (tono)
            {
                case 10:
                    return "0";
                case 11:
                    return "*";
                case 12:
                    return "#";
                case 13:
                    return "A";
                case 14:
                    return "B";
                case 15:
                    return "C";
                case 0:
                    return "D";

            }
            throw new Exception("codice >15");
        }

    }
}
