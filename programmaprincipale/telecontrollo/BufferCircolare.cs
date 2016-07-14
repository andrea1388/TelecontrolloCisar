using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    class BufferCircolare
    {
        byte lunghezzabuffer,lunghezzacodice;
        private int p1;
        private int p2;
        byte[] array;
        public BufferCircolare(byte lunghezzabuffer, byte lunghezzacodice)
        {
            this.lunghezzabuffer = lunghezzabuffer;
            this.lunghezzacodice = lunghezzacodice;
            if (lunghezzacodice > lunghezzabuffer) throw new Exception("Buffer insufficiente");
            array=new byte[lunghezzabuffer];
        }

       
        public void Inserisci(byte tono)
        {
            
        }
        String EstraiUltimiDati(byte quantita)
        {

        }


    }
}
