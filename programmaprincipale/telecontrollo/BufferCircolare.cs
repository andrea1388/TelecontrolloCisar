using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    class BufferCircolare
    {
        byte lunghezzabuffer,lunghezzacodice;
        private byte  primo=0, ultimo=0;
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
            array[ultimo++] = tono;
            if (ultimo >= lunghezzabuffer)
            {
                ultimo = 0;
            }
            if (ultimo == primo)
            {
                primo++;
                if (primo >= lunghezzabuffer) primo = 0;
            }
        }
        byte[] EstraiUltimiDati(byte quantita)
        {
            byte[] valori = new byte[quantita];
            byte index=quantita-1,ptr = ultimo ;
            valori[index] = array[ptr];
            index--;
            ptr--;
            if (ptr < 0) ptr = lunghezzabuffer - 1;
            if (index == 0) return valori;
            
        }


    }
}
