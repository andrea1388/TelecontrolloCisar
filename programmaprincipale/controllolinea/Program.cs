using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// controllolinea <numero linea> <on|off>
namespace telecontrollo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("controllolinea By Andrea Carrara 2016");
            if(args.Length!=2)
            {
                Console.WriteLine("Uso: controllolinea <numero linea> <on|off>");
            }
            try
            {
                IniParser parser = new IniParser("telecontrollo.conf");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problemi con il file di configurazione\n" +ex.Message );
                return;
            }
            String tmp = parser.GetSetting("ROOT", "indirizziPcf");
            String[] indirizzi = tmp.Split(',');
            indirizzi_pcf=new byte[indirizzi.Length ];
            for (int ip = 0; ip < indirizzi.Length; ip++) indirizzi_pcf[ip] = Convert.ToByte (indirizzi[ip],16);
            int linea;
            if(!int.TryParse(args[1],out linea))
            {
                log("linea errata");
                return;
            }
            if(linea<0 || linea>indirizzi_pcf.Length *8)
            {
                log("linea errata");
                return;
            }
            if (args[2]!=)
}

        }
    }
}
