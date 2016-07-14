using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace telecontrollo
{
    class Program
    {
        void Main(string[] args)
        {
            try
            {
                mainloop();
            }
            catch (Exception e)
            {

                log("Errore " + e.Message );
            }

        }
         void mainloop()
        {
            BufferCircolare buffer=new BufferCircolare(1,1);
            bool tonodisponibile, squillotelefono;
            TonoDtmf tono=new TonoDtmf ();
            int intervallopolling=10; // tempo tra un polling degli ingressi e il successivo in ms
            int duratasquillo=0; // durata squillo in ms
            int tempominimoduratasquillo=250; // tempo minimo durata dello squillo prima di considerarlo valido, in ms
            bool lineaconnessa = false; // indica se la linea è agganciata
            int tempomassimochiamata=30000; // tempo max durata telefonata
            Stopwatch tempochiamata=new Stopwatch();
            while (true)
            {
                leggiingressi(out tonodisponibile, out squillotelefono, out tono);
                if (tonodisponibile)
                {
                    buffer.Inserisci(tono.Valore);
                    log("ricevuto tono " + tono);
                    ElaboraSequenzaToniRicevuti(buffer);


                }
                if (squillotelefono) duratasquillo += intervallopolling;
                else duratasquillo = 0;
                if (duratasquillo > tempominimoduratasquillo)
                {
                    if (!lineaconnessa)
                    {
                        lineaconnessa = true;
                        AgganciaLineaTelefonica();
                        tempochiamata.Start();
                        duratasquillo = 0;
                        log("linea agganciata");
                    }
                }
                if (lineaconnessa)
                {
                    if (tempochiamata.ElapsedMilliseconds  > tempomassimochiamata)
                    {
                        SganciaLineaTelefonica();
                        tempochiamata.Reset();
                        lineaconnessa = false;
                        log("linea sganciata per limite temporale");
                    }
                }
                Thread.Sleep(intervallopolling);
            }


        }
         void leggiingressi(out bool tonodisponibile, out bool squillotelefono, out TonoDtmf tono) 
        {
            tono=new TonoDtmf(1);
            tonodisponibile=false;
            squillotelefono=false;
           

        }
        void AgganciaLineaTelefonica()
        {
            
        }
        void SganciaLineaTelefonica()
        {
            
        }
        void ElaboraSequenzaToniRicevuti(BufferCircolare buffer)
        {
            
        }
        static void log(String msg)
        {
            Console.WriteLine(msg);
        }
    }
}
