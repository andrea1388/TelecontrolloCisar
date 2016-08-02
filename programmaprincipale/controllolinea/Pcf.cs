using System;
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


        public Pcf(byte[] indirizzi_pcf)
        {
            i2cdriver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor());
            for (int ip = 0; ip < indirizzi_pcf.Length; ip++)
            {
                conn[ip] = new Pcf8574I2cConnection(i2cdriver.Connect(indirizzi_pcf[ip]));
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
            int tentativi=0;
            UInt16[] ret = new UInt16[conn.Length];
            for (int pcf = 0; pcf < conn.Length; pcf++)
            {
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
                        Console.WriteLine(e.Message);
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
                    deviceConnection.SetPinStatus(bitdacontrollare, true);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    System.Threading.Thread.Sleep(1);
                }
            }
            return false;
        }
    }
}
