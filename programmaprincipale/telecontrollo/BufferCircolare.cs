using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace telecontrollo
{
    class BufferCircolare
    {
        byte lunghezzabuffer;
        private byte  primo=0, ultimo=0;
        byte[] array;
        public BufferCircolare(byte lunghezzabuffer)
        {
            this.lunghezzabuffer = lunghezzabuffer;
            array=new byte[lunghezzabuffer];
            for (int i = 0; i < lunghezzabuffer; i++) array[i] = 0xFF;
        }

       
        public void Inserisci(byte tono)
        {
            array[ultimo++] = tono;
            if (ultimo == primo)
            {
                primo++;
                if (primo >= lunghezzabuffer) primo = 0;
            }
            if (ultimo >= lunghezzabuffer)
            {
                ultimo = 0;
            }
        }
        public byte[] EstraiUltimiDati(byte quantita)
        {
            byte[] valori = new byte[quantita];
            short  index=(short)(quantita - 1);
            short ptr = (short)(ultimo - 1);
            while(true )
            {
                if (ptr < 0) ptr = (byte)(lunghezzabuffer - 1);
                valori[index] = array[ptr];
                index--;
                ptr--;
                if (index ==-1) return valori;

            }
            
        }


    }
}
