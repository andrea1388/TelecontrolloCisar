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
            if (args.Length == 1) m.mainloop(args[0]); else m.mainloop("telecontrollo.conf");
            //try
            {
                
            }
            //catch (Exception e)
            {

                //log("Errore " + e.Message );
            }

        }
       
    class main
    {
        BufferCircolare buffer = new BufferCircolare(10);

        public void mainloop(String path2conffile)
        {
            log("Server telecontrollo partito");
            log("By iw3gcb - luglio 2016");

            IniParser parser = new IniParser(path2conffile);
            bool tonodisponibile, squillotelefono;
            TonoDtmf tono = new TonoDtmf();
            int intervallopolling; // tempo tra un polling degli ingressi e il successivo in ms
            int duratasquillo = 0; // durata squillo in ms
            int tempominimoduratasquillo = 250; // tempo minimo durata dello squillo prima di considerarlo valido, in ms
            bool lineaconnessa = false; // indica se la linea è agganciata
            int tempomassimochiamata = 30000; // tempo max durata telefonata

            if(!int.TryParse(parser.GetSetting("ROOT", "intervallopolling"),out intervallopolling)) intervallopolling=10;
            String tmp= parser.GetSetting("ROOT", "codicedtmf");
            if(tmp==null) tmp = "1968*";
            CodiceDtmf codicedtmf=new CodiceDtmf(tmp);
            log("codicedtmf=" + codicedtmf.ToString() );
            log("intervallopolling=" + intervallopolling);
            Stopwatch tempochiamata = new Stopwatch();
            while (true)
            {
                leggiingressi(out tonodisponibile, out squillotelefono, out tono);
                if (tonodisponibile)
                {
                    buffer.Inserisci(tono.Valore);
                    log("ricevuto tono: " + tono.Valore);
                    ElaboraSequenzaToniRicevuti();


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
        void ElaboraSequenzaToniRicevuti()
        {
            byte[] array = buffer.EstraiUltimiDati((byte) codicedtmf.Length);
            if ( codicedtmf.Confronta(array))
            {
                log("match");
            }
            log(s); 

        }
        bool CodiceEsatto(byte[] sequenza)
        {
            for(int i=0;i<sequenza.Length;i++) 
        }
        static void log(String msg)
        {
            Console.WriteLine(msg);
        }
    }
    }
}
