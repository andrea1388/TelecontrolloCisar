using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Raspberry.IO.Components.Expanders.Pcf8574;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;


/*
    Attende comandi dtmf
    Formato comando
    <codice di sblocco><comando: 0 | 1><linea da 00 a 99 sempre su due cifre>
    0 spegne 1 accende
*/


namespace telecontrollo
{
    enum tipoComando { on,off,onoff,offon};
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
        const ProcessorPin pinAggangioLinea=ProcessorPin.Pin10;
        const ProcessorPin pinAggangioPTT = ProcessorPin.Pin18;
        Pcf scheda;
        bool lineaconnessa = false; // indica se la linea è agganciata
        String parametriE2speak;
        Semaphore sem = new Semaphore(1, 1);

        public void mainloop(String path2conffile)
        {
            // variabili locali
            IniParser parser;
            ushort I2cClockDiv = 2500;
            try
            {
                parser = new IniParser(path2conffile);
            }
            catch (Exception e)
            {

                log("Errore " + e.Message );
                return;
            }
            
            bool tonodisponibile, squillotelefono;
            TonoDtmf tono;
            int intervallopolling; // tempo tra un polling degli ingressi e il successivo in ms
            int duratasquillo = 0; // durata squillo in ms
            int tempominimoduratasquillo = 250; // tempo minimo durata dello squillo prima di considerarlo valido, in ms
            int tempomassimochiamata = 6000; // tempo max durata telefonata
            bool statoPrecedenteTonoDiponibile = false;


            log("Server telecontrollo - By iw3gcb - luglio 2016");
            log("File di configurazione=" + path2conffile);
            // carica impostazioni
            if(!int.TryParse(parser.GetSetting("ROOT", "intervallopolling"),out intervallopolling)) intervallopolling=10;
            if (!int.TryParse(parser.GetSetting("ROOT", "tempomassimochiamata"), out tempomassimochiamata)) tempomassimochiamata = 30000;
            String tmp = parser.GetSetting("ROOT", "codicedtmf");
            if(tmp==null) tmp = "1968*";
            codicedtmf=new CodiceDtmf(tmp);
            buffer = new BufferCircolare((byte)(codicedtmf.Length+10));
            //log("codicedtmf=" + codicedtmf.ToString() );
            tmp = parser.GetSetting("ROOT", "indirizziPcf");
            log("indirizzi pcf=" + tmp);
            scheda = new Pcf(tmp);
            parametriE2speak = parser.GetSetting("ROOT", "parametriE2speak");
            if (parametriE2speak == null) parametriE2speak = "-v it -p 70 -s 155 2>/dev/null";
            if (!ushort.TryParse(parser.GetSetting("ROOT", "I2cClockDiv"), out I2cClockDiv)) I2cClockDiv = 2500;
            scheda.i2cClockDiv = I2cClockDiv;
            log("I2cClockDiv: " + scheda.i2cClockDiv.ToString());
            log("intervallopolling=" + intervallopolling);
            log("tempomassimochiamata=" + tempomassimochiamata.ToString());
            log("parametriE2speak=" + parametriE2speak);
            log("Server telecontrollo partito");
            #if DEBUG
            log("DEBUG version - Not workin' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            #endif
            Stopwatch tempochiamata = new Stopwatch();
            
            // definizione pin i/o
            // pin da leggere con una sola lettura contemporanea
            // 17= tonodisponibile, 21=bit0 tono, 22=bit1, 23= bit3, 24=bit3, 25=squillo
            #if !DEBUG
            pins = ProcessorPins.Pin17 | ProcessorPins.Pin21 | ProcessorPins.Pin22 | ProcessorPins.Pin23 | ProcessorPins.Pin24 | ProcessorPins.Pin25;
            driver = GpioConnectionSettings.DefaultDriver;
            driver.Allocate(pinAggangioLinea, PinDirection.Output);
            driver.Allocate(pinAggangioPTT, PinDirection.Output);
            driver.Allocate(ProcessorPin.Pin17, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin21, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin22, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin23, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin24, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin25, PinDirection.Input);
            #endif
            SganciaLineaTelefonica();
            SgancioPTT();
            while (true)
            {
                leggiingressi(out tonodisponibile, out squillotelefono, out tono);
                if (tonodisponibile)
                {
                    if (tonodisponibile && !statoPrecedenteTonoDiponibile)
                    {
                        statoPrecedenteTonoDiponibile = true;
                        log("ricevuto tono: " + tono.Carattere);
                        ElaboraSequenzaToniRicevuti(tono);
                        if (lineaconnessa) tempochiamata.Restart();
                    }

                }
                else statoPrecedenteTonoDiponibile = false;
                if (squillotelefono) duratasquillo += intervallopolling;
                else duratasquillo = 0;
                if (duratasquillo > tempominimoduratasquillo)
                {
                    if (!lineaconnessa)
                    {
                        lineaconnessa = true;
                        AgganciaLineaTelefonica();
                        tempochiamata.Restart();
                        duratasquillo = 0;
                        log("linea agganciata");
                    }
                }
                if (lineaconnessa)
                {
                    if (tempochiamata.ElapsedMilliseconds > tempomassimochiamata)
                    {
                        SganciaLineaTelefonica();
                        tempochiamata.Stop();
                        lineaconnessa = false;
                        log("linea sganciata per limite temporale");
                    }
                }
                Thread.Sleep(intervallopolling);
            }


        }
        void leggiingressi(out bool tonodisponibile, out bool squillotelefono, out TonoDtmf tono)
        {
            #if !DEBUG
            lettura = driver.Read(pins);
            tonodisponibile = (lettura & ProcessorPins.Pin17)>0;
            squillotelefono = (lettura & ProcessorPins.Pin25)==0; // è attivo basso
            byte t;
            t = (byte)(((uint)lettura >> 21) & 0x0f);
            tono = new TonoDtmf(t);
            #else
            Random rnd = new Random();
            uint lettura = (uint)rnd.Next(0xfffffff);
            //tono = new TonoDtmf((byte)rnd.Next(16));
            byte t;
            t = (byte)(((uint)lettura >> 21) & 0x0f);
            tono = new TonoDtmf(t);
            tonodisponibile = true;
            squillotelefono = false;
            #endif

        }
        void AgganciaLineaTelefonica()
        {
            #if !DEBUG
            driver.Write(pinAggangioLinea, true);
            #endif
            log("Linea telefonica agganciata");
        }
        void SganciaLineaTelefonica()
        {
            #if !DEBUG
            driver.Write(pinAggangioLinea, false);
            #endif
            log("Linea telefonica sganciata");
        }
        void AggancioPTT()
        {
            #if !DEBUG
            driver.Write(pinAggangioPTT, true);
            #endif
            log("PTT agganciato");
        }
        void SgancioPTT()
        {
            #if !DEBUG
            driver.Write(pinAggangioPTT, false);
            #endif
            log("PTT sganciato");
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
                        duratacomandodtmf.Restart();
                    }
                    break;
                case 1: // codice di sblocco ricevuto. Attendo comando
                    comando=tono.Carattere;
                    if(comando!='0' && comando !='1')
                    {
                        log("comando errato: " + comando);
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
        void ComandoRicevuto(char comando,String l)
        {
            int linea;
            if(!int.TryParse(l,out linea))
            {
                log("ComandoRicevuto:Numero linea errata:" + l);
                return;
            }
            if (linea < 1 || linea > scheda.NumeroLineeIO )
            {
                log("ComandoRicevuto:Numero linea errata:" + linea);
                return;
            }
            bool ok=true;
            Thread newThread = new Thread(this.Azione);
            CComando cmd=new CComando();
            cmd.linea = linea;
            switch (comando)
            {
                case '0': //spegni
                    cmd.comando = tipoComando.off;
                    break;
                case '1': //accendi
                    cmd.comando = tipoComando.on;
                    break;
                default:
                    ok = false;
                    break;

            }
                    
            String msg;
            if (ok)
            {
                newThread.Start(cmd);
                msg = "Linea_" + linea.ToString();
                if (comando == '0') msg += "_spenta"; else msg += "_accesa";
            }
            else
            {
                msg = "Comando_non_riuscito";
            }
            if(!lineaconnessa)
            {
                Thread.Sleep(1000);
                AggancioPTT();
                Thread.Sleep(300);
            }
            Parla(msg);
            if (!lineaconnessa) SgancioPTT();
        }
        void Parla(String msg )
        {
            #if !DEBUG
            var info = new ProcessStartInfo();
            info.FileName="espeak";
            info.Arguments = msg + " " + parametriE2speak;
            Process p=Process.Start(info);
            p.WaitForExit();
            #endif
        }
        static void log(String msg)
        {
            DateTime t = DateTime.Now;
            String dt = t.ToString("d/M/y HH:mm:ss:FFF ");
            Console.WriteLine(dt + msg);
        }
        public void Azione(object data)
        {
            CComando cmd = (CComando)data;
            switch(cmd.comando )
            {
                case tipoComando.on:
                    sem.WaitOne();
                    scheda.AccendiLinea(cmd.linea);
                    sem.Release();
                    break;
                case tipoComando.off:
                    sem.WaitOne();
                    scheda.SpegniLinea (cmd.linea);
                    sem.Release();
                    break;
                case tipoComando.offon:
                    sem.WaitOne();
                    scheda.SpegniLinea(cmd.linea);
                    sem.Release();
                    Thread.Sleep(cmd.timeout);
                    sem.WaitOne();
                    scheda.AccendiLinea (cmd.linea);
                    sem.Release();
                    break;
                case tipoComando.onoff:
                    sem.WaitOne();
                    scheda.AccendiLinea(cmd.linea);
                    sem.Release();
                    Thread.Sleep(cmd.timeout);
                    sem.WaitOne();
                    scheda.SpegniLinea(cmd.linea);
                    sem.Release();
                    break;
            }
            


        }
    }
    }
}
