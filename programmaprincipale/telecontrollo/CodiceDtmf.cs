using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    class CodiceDtmf
    {
        byte[] codice;
        public CodiceDtmf(String c)
        {
            codice=new byte[c.Length];
            for (int i = 0; i < c.Length; i++)
            {
                TonoDtmf t = new TonoDtmf(c[i]);
                codice[i] = t.Valore;
            }

        }
        //public String ToString();
        public bool Confronta(byte[] cod)
        {
            for (int i = 0; i < codice.Length; i++)
                if (codice[i] != cod[i]) return false;
            return true;
        }
        public int Length
        {
            get {return codice.Length;}
        }

    }
}
