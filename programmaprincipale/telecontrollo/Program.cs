using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Raspberry.IO.GeneralPurpose;

/*
    Attende comandi dtmf
    Formato comando
    <codice di sblocco><comando: 0 | 1><linea da 00 a 99 sempre su due cifre>
    0 spegne 1 accende
*/

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
        BufferCircolare buffer;
        Stopwatch duratacomandodtmf = new Stopwatch();
        int tempomassimoduratacomandodtmf = 15000; // tempo max entro il quale completare il comandoi dtmf
        byte stato = 0;
        char comando;
        String linea;
        CodiceDtmf codicedtmf;
        ProcessorPins pins, lettura;
        IGpioConnectionDriver  driver;
        
        public void mainloop(String path2conffile)
        {
            log("Server telecontrollo partito");
            log("By iw3gcb - luglio 2016");
            this.pins = ProcessorPins.Pin17 | ProcessorPins.Pin27 | ProcessorPins.Pin22 | ProcessorPins.Pin23 | ProcessorPins.Pin24 | ProcessorPins.Pin25;

            IniParser parser = new IniParser(path2conffile);
            bool tonodisponibile, squillotelefono;
            TonoDtmf tono;
            int intervallopolling; // tempo tra un polling degli ingressi e il successivo in ms
            int duratasquillo = 0; // durata squillo in ms
            int tempominimoduratasquillo = 250; // tempo minimo durata dello squillo prima di considerarlo valido, in ms
            bool lineaconnessa = false; // indica se la linea è agganciata
            int tempomassimochiamata = 30000; // tempo max durata telefonata
            byte[] indirizzi_pcf; // indirizzi i2c dei pcf

            if(!int.TryParse(parser.GetSetting("ROOT", "intervallopolling"),out intervallopolling)) intervallopolling=10;
            String tmp= parser.GetSetting("ROOT", "codicedtmf");
            if(tmp==null) tmp = "1968*";
            codicedtmf=new CodiceDtmf(tmp);
            buffer = new BufferCircolare((byte)(codicedtmf.Length+10));
            //log("codicedtmf=" + codicedtmf.ToString() );
            log("intervallopolling=" + intervallopolling);
            Stopwatch tempochiamata = new Stopwatch();
            
            // definizione pin i/o
            const ConnectorPin led1Pin = ConnectorPin.P1Pin26;
            const ProcessorPin pin1=ProcessorPin.Pin29;
            const ProcessorPin pin2=ProcessorPin.Pin2;

            var pinTonoDisponibile = ProcessorPin.Pin17;
            pinTonoDisponibile.Input();
            var pinSquillo = ConnectorPin.P1Pin22.ToProcessor();
            var pinAggangioLinea = ConnectorPin.P1Pin19.ToProcessor();
            const pinTono0 = ConnectorPin.P1Pin13.ToProcessor();
            var pinTono1 = ConnectorPin.P1Pin15.ToProcessor();
            var pinTono2 = ConnectorPin.P1Pin16.ToProcessor();
            var pinTono3 = ConnectorPin.P1Pin18.ToProcessor();
             driver = GpioConnectionSettings.DefaultDriver;
            driver.Allocate(pinAggangioLinea, PinDirection.Output);
            driver.Allocate(pinTonoDisponibile, PinDirection.Input);
            driver.Allocate(pinSquillo, PinDirection.Input);
            driver.Allocate(pinTono0, PinDirection.Input);
            driver.Allocate(pinTono1, PinDirection.Input);
            driver.Allocate(pinTono2, PinDirection.Input);
            driver.Allocate(pinTono3, PinDirection.Input);
 
            while (true)
            {
                leggiingressi(out tonodisponibile, out squillotelefono, out tono);
                if (tonodisponibile)
                {
                    //log("ricevuto tono: " + tono.Valore);
                    ElaboraSequenzaToniRicevuti(tono);


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
            lettura = driver.Read(pins);
            Random rnd = new Random();
            tono = new TonoDtmf((byte)rnd.Next(16) );
            tonodisponibile = (lettura & ProcessorPins.Pin17)>0;
            squillotelefono = (lettura & ProcessorPins.Pin17)>0;


        }
        void AgganciaLineaTelefonica()
        {

        }
        void SganciaLineaTelefonica()
        {

        }
        void ElaboraSequenzaToniRicevuti(TonoDtmf tono)
        {
            if (duratacomandodtmf.ElapsedMilliseconds > tempomassimoduratacomandodtmf) stato = 0;
            switch (stato)
            {
                case 0: // codice iniziale non ricevuto
                    buffer.Inserisci(tono.Valore);
                    byte[] array = buffer.EstraiUltimiDati((byte) codicedtmf.Length);
                    if (codicedtmf.Confronta(array))
                    {
                        log("stato1:codice corrisponde");
                        stato = 1;
                        duratacomandodtmf.Start();
                    }
                    break;
                case 1: // codice di sblocco ricevuto. Attendo comando
                    comando=tono.Carattere;
                    if(comando!='0' && comando !='1')
                    {
                        log("comando errato" + comando);
                        stato = 0;
                    }
                    else
                    {
                        log("stato2:comando=" + comando);
                        stato = 2;
                    }
                    break;
                case 2: // numero di linea (decine)
                    linea=tono.Carattere.ToString();
                    log("stato3");
                    stato=3;
                    break;
                case 3: // numero di linea (unità)
                    linea += tono.Carattere.ToString();
                    log("comando completato:linea=" + linea);
                    ComandoRicevuto(comando,linea);
                    stato=0;
                    
                    break;
                    
            }

        }
        void ComandoRicevuto(char comando,String linea)
        {
            byte indirizzo_pcf=indirizzi_pcf[linea / 8];
            switch (comando)
            {
                case '0': //spegni
                    
                    break;
                case '1': //accendi
                    break;
                
            }
        }
      
        static void log(String msg)
        {
            Console.WriteLine(msg);
        }
    }
    }
}
