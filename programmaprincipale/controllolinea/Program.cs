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

        }
    }
}
