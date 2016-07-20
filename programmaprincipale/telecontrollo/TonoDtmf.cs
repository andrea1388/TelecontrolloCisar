using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    
    class TonoDtmf
    {
        private byte valore; // valori da 0 a 15
        private char carattere; // 0,.. 9, ABCD*#
        public TonoDtmf(byte v) 
        {
            this.valore=v;
            this.carattere = DaValoreACarattere(v);
        }
        public TonoDtmf(Char carattere)
        {
            this.carattere  = carattere;
            this.valore=DaCarattereAValore(carattere );

        }
        /*
        public TonoDtmf()
        {
            this.tono = 0;
        }
        */
        public byte Valore
        {
            get {return valore;}
            set {valore = value; carattere=DaValoreACarattere(valore);}
        }
         public char Carattere
        {
            get {return carattere;}
            set {carattere = value;valore=DaCarattereAValore(carattere);}
        }
         Byte DaCarattereAValore(char c)
         {
             if (c >= '1' && c <= '9') return (byte)(c - '0');
             switch (c)
             {
                 case 'D':
                     return 0;
                 case '0':
                     return 10;
                 case '*':
                     return 11;
                 case '#':
                     return 12;
                 case 'A':
                     return 13;
                 case 'B':
                     return 14;
                 case 'C':
                     return 15;
             }
             throw new Exception("DaCarattereAValore:bad char " + c.ToString() );
         }
        Char DaValoreACarattere(byte t) 
        {
            char chr;
            if (t >0 && t <= 9)
            {
                chr=(char)('0' + t);
                return chr;
            }
            switch (t)
            {
                case 10:
                    return '0';
                case 11:
                    return '*';
                case 12:
                    return '#';
                case 13:
                    return 'A';
                case 14:
                    return 'B';
                case 15:
                    return 'C';
                case 0:
                    return 'D';

            }
            throw new Exception("codice >15");
        }

    }
}
