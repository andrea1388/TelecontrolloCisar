using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace telecontrollo
{
    class Program
    {
        static void Main(string[] args)
        {
            main m = new main(); 
            //try
            {
                m.mainloop();
            }
            //catch (Exception e)
            {

                //log("Errore " + e.Message );
            }

        }
       
    class main
    {
        BufferCircolare buffer = new BufferCircolare(10);

        public void mainloop()
        {

            bool tonodisponibile, squillotelefono;
            TonoDtmf tono = new TonoDtmf();
            int intervallopolling = 10; // tempo tra un polling degli ingressi e il successivo in ms
            int duratasquillo = 0; // durata squillo in ms
            int tempominimoduratasquillo = 250; // tempo minimo durata dello squillo prima di considerarlo valido, in ms
            bool lineaconnessa = false; // indica se la linea è agganciata
            int tempomassimochiamata = 30000; // tempo max durata telefonata
            Stopwatch tempochiamata = new Stopwatch();
            while (true)
            {
                leggiingressi(out tonodisponibile, out squillotelefono, out tono);
                if (tonodisponibile)
                {
                    buffer.Inserisci(tono.Valore);
                    log("ricevuto tono: " + tono.Valore);
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
                    if (tempochiamata.ElapsedMilliseconds > tempomassimochiamata)
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
            Random rnd = new Random();
            tono = new TonoDtmf((byte)rnd.Next(16) );
            tonodisponibile = true;
            squillotelefono = false;


        }
        void AgganciaLineaTelefonica()
        {

        }
        void SganciaLineaTelefonica()
        {

        }
        void ElaboraSequenzaToniRicevuti(BufferCircolare buffer)
        {
            byte[] array = buffer.EstraiUltimiDati(5);
            String s="";
            for (int i = 0; i < 5; i++) s =s+ array[i].ToString() + " ";

                log(s); 

        }
        static void log(String msg)
        {
            Console.WriteLine(msg);
        }
    }
    }
}
