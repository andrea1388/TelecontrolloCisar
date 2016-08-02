using System;
using System.Collections.Generic;
using Raspberry.IO.Components.Expanders.Pcf8574;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;
using System.Text;


// controllolinea <numero linea> <on|off>
// controllolinea
namespace telecontrollo
{
    class Program
    {
        static Pcf scheda;
        static byte[] indirizzi_pcf; // indirizzi i2c dei pcf
        static void Main(string[] args)
        {
            IniParser parser;
            Console.WriteLine("controllolinea By Andrea Carrara 2016");
            if(args.Length!=2 && args.Length!=0)
            {
                Console.WriteLine("Uso: controllolinea [-t] | <numero linea> <on|off>");
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
            String[] indirizzi = tmp.Split(',');
            Console.WriteLine("indirizzi pcf=" + tmp);
            indirizzi_pcf=new byte[indirizzi.Length ];
            scheda = new Pcf(indirizzi_pcf);


            // inizio elaborazione
            
            if(args[0].ToLower()=="-t")
            {
                Random rnd = new Random();
                scheda.LeggiLinee();
                while (true )
                {
                    int linea = 1+rnd.Next(16);
                    int cmd = rnd.Next(2);
                    if (cmd == 1) scheda.AccendiLinea(linea); else scheda.SpegniLinea(linea);

                }
            }
            
            if(args.Length==0)
            {
                StampaStatoLinee();
                
                return;
            }
            else
            {
                int linea;
                if (!int.TryParse(args[0], out linea))
                {
                    Console.WriteLine("linea errata");
                    return;
                }
                if (linea < 0 || linea > indirizzi_pcf.Length * 8)
                {
                    Console.WriteLine("linea errata");
                    return;
                }
                switch (args[1].ToUpper())
                {
                    case "ON":
                        scheda.AccendiLinea(linea);
                        break;
                    case "OFF":
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
            for (int i = 0; i < indirizzi_pcf.Length; i++)
            {
                Console.Write(stato[i].ToString("X2"));
            }
            Console.WriteLine();

        }
    }
}
