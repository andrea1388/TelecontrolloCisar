﻿using System;
using System.Collections.Generic;
using Raspberry.IO.Components.Expanders.Pcf8574;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;
using System.Text;


// controllolinea <numero linea> <on|off>
namespace telecontrollo
{
    class Program
    {
        
        static void Main(string[] args)
        {
            byte[] indirizzi_pcf; // indirizzi i2c dei pcf
            IniParser parser;
            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;
            Console.WriteLine("controllolinea By Andrea Carrara 2016");
            if(args.Length!=2)
            {
                Console.WriteLine("Uso: controllolinea <numero linea> <on|off>");
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
            for (int ip = 0; ip < indirizzi.Length; ip++) indirizzi_pcf[ip] = Convert.ToByte (indirizzi[ip],16);
            int linea;
            if(!int.TryParse(args[0],out linea))
            {
                Console.WriteLine("linea errata");
                return;
            }
            if(linea<0 || linea>indirizzi_pcf.Length *8)
            {
                Console.WriteLine("linea errata");
                return;
            }
            byte indirizzo_pcf = indirizzi_pcf[linea / 8];
            Console.WriteLine("Uso indirizzo: 0x" + indirizzo_pcf.ToString("X"));
            Pcf8574Pin bitdacontrollare = (Pcf8574Pin)(Math.Pow(2, ((linea-1) % 8)));
            var i2cdriver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor());
            var deviceConnection = new Pcf8574I2cConnection(i2cdriver.Connect(indirizzo_pcf));
            deviceConnection.GetPinsStatus();
            Console.WriteLine("bit: " + bitdacontrollare.ToString("X"));
            switch (args[1].ToUpper())
            {
                case "ON":
                    deviceConnection.SetPinStatus(bitdacontrollare, true);
                    break;
                case "OFF":
                    deviceConnection.SetPinStatus(bitdacontrollare, false);
                    break;
                default:
                    Console.WriteLine("Comando errato");
                    return;
            }


        }
    }
}
