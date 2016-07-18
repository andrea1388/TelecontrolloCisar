using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    class CodiceDtmf
    {
        public CodiceDtmf(String codice)
        {
                        codicedtmf=new byte[tmp.Length ];
            for (int i = 0; i < tmp.Length; i++) {
                TonoDtmf t=new TonoDtmf(tmp.Substring(i,1));
                codicedtmf[i] = tmp;

        }
        public String ToString();
        public bool Confronta(byte[] codice);
        public readonly  byte Length();

    }
}
