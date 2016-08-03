﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raspberry.IO.Components.Expanders.Pcf8574;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;

namespace telecontrollo
{
    class Pcf
    {
        I2cDriver i2cdriver;
        Pcf8574I2cConnection[] conn;
        const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
        const ConnectorPin sclPin = ConnectorPin.P1Pin05;


        public Pcf(String indirizzi_pcf)
        {
            String[] indirizzi = indirizzi_pcf.Split(',');
            i2cdriver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor());
            conn = new Pcf8574I2cConnection[indirizzi.Length]; 
            for (int ip = 0; ip < indirizzi.Length; ip++)
            {
                conn[ip] = new Pcf8574I2cConnection(i2cdriver.Connect(Convert.ToByte(indirizzi[ip], 16)));
            }
            LeggiLinee();

        }
        public int NumeroLineeIO 
        { 
            get
            {
                return conn.Length * 8;
            }
        }
        public int NumeroPcf
        {
            get
            {
                return conn.Length;
            }
        }
        public ushort i2cClockDiv
        {
            get
            {
                return (ushort)i2cdriver.ClockDivider;
            }
            set
            {
                i2cdriver.ClockDivider=value ;
            }
        }
        public bool AccendiLinea(int numero)
        {
            return _accendispegnilinea(numero, true);
        }
        public bool SpegniLinea(int numero)
        {
            return _accendispegnilinea(numero, false);
        }
        public UInt16[] LeggiLinee()
        {
            UInt16[] ret = new UInt16[conn.Length];
            for (int pcf = 0; pcf < conn.Length; pcf++)
            {
                int tentativi = 0;
                var deviceConnection = conn[pcf];
                ret[pcf] = 0x100;
                while(tentativi++ < 5)
                {
                    try
                    {
                        ret[pcf] = deviceConnection.GetPinsStatus();
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore GetPinsStatus: tentativo {0} - {1}", tentativi, e.Message);
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
            return ret;
        }
        // internal
        private bool _accendispegnilinea(int numero, bool on)
        {
            int tentativi = 0;
            Pcf8574Pin bitdacontrollare = (Pcf8574Pin)(Math.Pow(2, ((numero - 1) % 8)));
            var deviceConnection = conn[(numero - 1) / 8];
            while (tentativi++ < 5)
            {
                try
                {
                    deviceConnection.SetPinStatus(bitdacontrollare, on);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Errore SetPinStatus: tentativo {0} - {1}", tentativi, e.Message);
                    System.Threading.Thread.Sleep(1);
                }
            }
            return false;
        }
    }
}