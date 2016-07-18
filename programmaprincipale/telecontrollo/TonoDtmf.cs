using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    
    class TonoDtmf
    {
        private byte tono; // valori da 0 a 15
        private char codiceascii; // 0,.. 9, ABCD*#
        public TonoDtmf(byte tono) 
        {
            this.tono=tono;
            switch (tono)
            {
                case 0:
                    codiceascii = 'D';
                    break;
                case 11:
                    codiceascii = '*';
                    break;
                case 12:
                    codiceascii = '#';
                    break;
                case 13:
                    codiceascii = 'A';
                    break;
                case 14:
                    codiceascii = 'B';
                    break;
                case 15:
                    codiceascii = 'C';
                    break;
                case 10:
                    codiceascii = '0';
                    break;
                default:
                    codiceascii = (char)('0' + (char)tono);
                    break;

            }
        }
        public TonoDtmf(Char tono)
        {
            this.codiceascii = tono;
            this.tono=to

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
        Byte CharToTono()
        {
               switch (tono)
            {
                case 0:
                    codiceascii = 'D';
                    break;
                case 11:
                    codiceascii = '*';
                    break;
                case 12:
                    codiceascii = '#';
                    break;
                case 13:
                    codiceascii = 'A';
                    break;
                case 14:
                    codiceascii = 'B';
                    break;
                case 15:
                    codiceascii = 'C';
                    break;
                case 10:
                    codiceascii = '0';
                    break;
                default:
                    codiceascii = (char)('0' + (char)tono);
                    break;
        }
        Char TonoToChar(byte t) 
        {
            char chr;
            if (t >0 && t <= 9)
            {
                chr=(char)('0' + tono);
                return chr;
            }
            switch (tono)
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
