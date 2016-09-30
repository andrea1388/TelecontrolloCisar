using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if !DEBUG
using Raspberry.IO.Components.Expanders.Pcf8574;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;
#endif
namespace telecontrollo
{
    public class Pcf
    {
        Semaphore sem = new Semaphore(1, 1);
        #if !DEBUG
        I2cDriver i2cdriver;
        Pcf8574I2cConnection[] conn;
        const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
        const ConnectorPin sclPin = ConnectorPin.P1Pin05;
        #else
        UInt16[] statolineadubug = new UInt16[2];
        #endif

        public Pcf(String indirizzi_pcf)
        {
            String[] indirizzi = indirizzi_pcf.Split(',');
             #if !DEBUG
            i2cdriver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor());
            conn = new Pcf8574I2cConnection[indirizzi.Length]; 
            for (int ip = 0; ip < indirizzi.Length; ip++)
            {
                conn[ip] = new Pcf8574I2cConnection(i2cdriver.Connect(Convert.ToByte(indirizzi[ip], 16)));
            }
            LeggiLinee();
#endif
        }
        public int NumeroLineeIO 
        { 
            get
            {
#if !DEBUG
                return conn.Length * 8;
#else
                return 16;
#endif
            }
        }
        public int NumeroPcf
        {
            get
            {
     #if !DEBUG
                return conn.Length;
#else
                return 2;
#endif
            }
        }
        public ushort i2cClockDiv
        {
            get
            {
#if !DEBUG
                return (ushort)i2cdriver.ClockDivider;
#else
                return 1;
#endif
            }
            set
            {
                #if !DEBUG
                i2cdriver.ClockDivider=value ;
#endif
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
#if !DEBUG
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
#else

          
            return statolineadubug;
#endif
            
            
        }
        public String LeggiLineeHEX()
        {
            UInt16[] stato = LeggiLinee();
            String ret="";
            for (int i = NumeroPcf - 1; i > -1; i--)
            {
                if (stato[i] >= 0x100) ret=ret+"--"; else ret=ret+stato[i].ToString("X2");
            }
            return ret;
        }
        // internal
        private bool _accendispegnilinea(int numero, bool on)
        {
            bool ok=false ;
            sem.WaitOne();
            #if !DEBUG
            int tentativi = 0;
            Pcf8574Pin bitdacontrollare = (Pcf8574Pin)(Math.Pow(2, ((numero - 1) % 8)));
            var deviceConnection = conn[(numero - 1) / 8];
            while (tentativi++ < 5)
            {
                try
                {
                    deviceConnection.SetPinStatus(bitdacontrollare, on);
                    ok = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Errore SetPinStatus: tentativo {0} - {1}", tentativi, e.Message);
                    System.Threading.Thread.Sleep(1);
                    ok= false;
                }
            }
            #else
            int banco = (numero - 1) / 8;
            UInt16 bit = (UInt16)Math.Pow(2, ((numero - 1) % 8));
            //Program.log("banco: " + banco.ToString() + " bit: ");
            if (on) 
                   statolineadubug[banco] = (ushort)(statolineadubug[banco] | bit);
            else
                statolineadubug[banco] = (ushort)(statolineadubug[banco] ^ bit);
            ok = true;
            #endif
            Program.log("linea " + numero.ToString() +  (on ? " accesa" : " spenta"));
            sem.Release();
            return ok;

        }
        public bool statolinea(int linea , ushort[] stati)
        {
            int banco = (linea-1) / 8;
            int bitdacontrollare = (int)Math.Pow(2, ((linea - 1) % 8));
            return (stati[banco] & bitdacontrollare) > 0;
        }
    }
}
