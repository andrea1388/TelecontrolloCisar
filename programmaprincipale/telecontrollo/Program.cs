﻿//#define Debug
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
        int tempomassimoduratacomandodtmf = 150000; // tempo max entro il quale completare il comandoi dtmf
        byte stato = 0;
        char comando;
        String linea;
        CodiceDtmf codicedtmf;
        ProcessorPins pins, lettura;
        IGpioConnectionDriver  driver;
        byte[] indirizzi_pcf; // indirizzi i2c dei pcf
        I2cDriver i2cdriver;
        const ProcessorPin pinAggangioLinea=ProcessorPin.Pin10;

        public void mainloop(String path2conffile)
        {

            IniParser parser = new IniParser(path2conffile);
            bool tonodisponibile, squillotelefono;
            TonoDtmf tono;
            int intervallopolling; // tempo tra un polling degli ingressi e il successivo in ms
            int duratasquillo = 0; // durata squillo in ms
            int tempominimoduratasquillo = 250; // tempo minimo durata dello squillo prima di considerarlo valido, in ms
            bool lineaconnessa = false; // indica se la linea è agganciata
            int tempomassimochiamata = 30000; // tempo max durata telefonata
            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;
           
            if(!int.TryParse(parser.GetSetting("ROOT", "intervallopolling"),out intervallopolling)) intervallopolling=10;
            String tmp= parser.GetSetting("ROOT", "codicedtmf");
            if(tmp==null) tmp = "1968*";
            codicedtmf=new CodiceDtmf(tmp);
            buffer = new BufferCircolare((byte)(codicedtmf.Length+10));
            //log("codicedtmf=" + codicedtmf.ToString() );
            tmp = parser.GetSetting("ROOT", "indirizziPcf");
            String[] indirizzi = tmp.Split(',');
            indirizzi_pcf=new byte[indirizzi.Length ];
            for (int ip = 0; ip < indirizzi.Length; ip++) indirizzi_pcf[ip] = Convert.ToByte (indirizzi[ip],16);

            log("Server telecontrollo partito");
            log("By iw3gcb - luglio 2016");
            log("intervallopolling=" + intervallopolling);
            Stopwatch tempochiamata = new Stopwatch();
            
            // definizione pin i/o
            // pin da leggere con una sola lettura contemporanea
            // 17= tonodisponibile, 21=bit0 tono, 22=bit1, 23= bit3, 24=bit3, 25=squillo
            #if !Debug
            i2cdriver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor());
            pins = ProcessorPins.Pin17 | ProcessorPins.Pin21 | ProcessorPins.Pin22 | ProcessorPins.Pin23 | ProcessorPins.Pin24 | ProcessorPins.Pin25;
            driver = GpioConnectionSettings.DefaultDriver;
            driver.Allocate(pinAggangioLinea, PinDirection.Output);
            driver.Allocate(ProcessorPin.Pin17, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin21, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin22, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin23, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin24, PinDirection.Input);
            driver.Allocate(ProcessorPin.Pin25, PinDirection.Input);
#endif
            while (true)
            {
                leggiingressi(out tonodisponibile, out squillotelefono, out tono);
                if (tonodisponibile)
                {
                    log("ricevuto tono: " + tono.Valore);
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
            #if !Debug
            lettura = driver.Read(pins);
            tonodisponibile = (lettura & ProcessorPins.Pin17)>0;
            squillotelefono = (lettura & ProcessorPins.Pin17)>0;
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
            #if !Debug
            driver.Write(pinAggangioLinea, true);
            #endif
        }
        void SganciaLineaTelefonica()
        {
            #if !Debug
            driver.Write(pinAggangioLinea, false);
            #endif

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
        void ComandoRicevuto(char comando,String linea)
        {
            int l;
            if(!int.TryParse(linea,out l))
            {
                log("ComandoRicevuto:Numero linea errata:" + linea);
                return;
            }
            byte indirizzo_pcf=indirizzi_pcf[l / 8];
            Pcf8574Pin bitdacontrollare = (Pcf8574Pin)(Math.Pow (2, (l % 8)));
            #if !Debug
            var deviceConnection = new Pcf8574I2cConnection(i2cdriver.Connect(indirizzo_pcf));
            switch (comando)
            {
                case '0': //spegni
                    deviceConnection.SetPinStatus(bitdacontrollare , false);
                    break;
                case '1': //accendi
                    deviceConnection.SetPinStatus(bitdacontrollare, true);
                    break;
                
            }
            #endif
        }
      
        static void log(String msg)
        {
            Console.WriteLine(msg);
        }
    }
    }
}
