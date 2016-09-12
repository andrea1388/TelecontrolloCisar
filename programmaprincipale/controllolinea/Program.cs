using System;
using System.Collections.Generic;
using System.Threading;
using Raspberry.IO.Components.Expanders.Pcf8574;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;
using System.Text;


// controllolinea <numero linea> <on|off|offonoff [tempo]>
// controllolinea
namespace telecontrollo
{
    class Program
    {
        static Pcf scheda;
        static void Main(string[] args)
        {
            IniParser parser;
            ushort I2cClockDiv = 2500;
            Console.WriteLine("controllolinea By Andrea Carrara 2016");
            if (args.Length==1 && args[0].ToLower() == "-h")
            {
                Console.WriteLine("Uso: controllolinea [-t] | <numero linea> <on|off|offonoff [tempo]>");
                return;
            }
            try
            {
                 parser = new IniParser("telecontrollo.conf");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problemi con il file di configurazione\n" +ex.Message );
                return;
            }
            String tmp = parser.GetSetting("ROOT", "indirizziPcf");
            scheda = new Pcf(tmp);
            
            Console.WriteLine("indirizzi pcf=" + tmp);
            if (!ushort.TryParse(parser.GetSetting("ROOT", "I2cClockDiv"), out I2cClockDiv)) I2cClockDiv = 2500;
            scheda.i2cClockDiv = I2cClockDiv;
            Console.WriteLine("I2cClockDiv: " + scheda.i2cClockDiv.ToString() );


            // inizio elaborazione

            if (args.Length == 1 && args[0].ToLower() == "-t")
            {
                Random rnd = new Random();
                scheda.LeggiLinee();
                while (!Console.KeyAvailable )
                {
                    int linea = 1+rnd.Next(16);
                    int cmd = rnd.Next(2);
                    Console.WriteLine("Linea {0} - comando {1}", linea, cmd);
                    if (cmd == 1) scheda.AccendiLinea(linea); else scheda.SpegniLinea(linea);
                    System.Threading.Thread.Sleep(10);

                }
                return;
            }
            
            if(args.Length==0)
            {
                StampaStatoLinee();
                
                return;
            }
            if (args.Length == 2 || args.Length == 3)
            {
                int linea;
                if (!int.TryParse(args[0], out linea))
                {
                    Console.WriteLine("linea errata");
                    return;
                }
                if (linea < 0 || linea > scheda.NumeroLineeIO )
                {
                    Console.WriteLine("linea errata");
                    return;
                }
                Console.WriteLine("Linea {0} - comando {1}", linea, args[1].ToUpper());
                //scheda.LeggiLinee(); // carica lo stato iniziale
                switch (args[1].ToUpper())
                {
                    case "ON":
                        scheda.AccendiLinea(linea);
                        break;
                    case "OFF":
                        scheda.SpegniLinea(linea);
                        break;
                    case "OFFONOFF":
                        int tempo = 0;
                        if (args.Length!=3)
                        {
                            Console.WriteLine("tempo mancante");
                            return;
                        }
                        if (!int.TryParse(args[2], out tempo))
                        {
                            Console.WriteLine("tempo errato");
                            return;
                        }
                        scheda.SpegniLinea(linea);
                        Thread.Sleep(100);
                        scheda.AccendiLinea (linea);
                        Thread.Sleep(1000 * tempo);
                        scheda.SpegniLinea(linea);
                        break;
                    default:
                        Console.WriteLine("Comando errato");
                        return;
                }
                StampaStatoLinee();

            }


        }
        static void StampaStatoLinee()
        {
            UInt16[] stato = scheda.LeggiLinee();
            for (int i = scheda.NumeroPcf-1;i>-1  ; i--)
            {
                if (stato[i] >= 0x100) Console.Write("--"); else Console.Write(stato[i].ToString("X2"));
            }
            Console.WriteLine();

        }
    }
}
