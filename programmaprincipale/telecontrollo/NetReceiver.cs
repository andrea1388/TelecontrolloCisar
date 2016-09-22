using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace telecontrollo
{
    class NetReceiver
    {
        String Elabora(String cmd)
        {
            String[] param = cmd.Split(' ');
            int linea,timeout;
            int linee = Program.m.scheda.NumeroLineeIO;
            if (param.Length < 1 || param.Length > 3) return "comando errato";
            Thread newThread = new Thread(Program.m.Azione);
            CComando thcmd = new CComando();
            thcmd.timeout = 1;
            switch (param[0].ToLower())
            {
                case "on":
                    if (param.Length != 2) return "comando errato";
                    if (!int.TryParse(param[1], out linea)) return "linea errata";
                    if (linea < 1 || linea > linee) return "linea errata";
                    thcmd.linea = linea;
                    thcmd.comando = tipoComando.on;
                    newThread.Start(thcmd);
                    return "ok";
                case "off":
                    if (param.Length != 2) return "comando errato";
                    if (!int.TryParse(param[1], out linea)) return "linea errata";
                    if (linea < 1 || linea > linee) return "linea errata";
                    thcmd.linea = linea;
                    thcmd.comando = tipoComando.off;
                    newThread.Start(thcmd);
                    return "ok";
                case "offon":
                    if (!int.TryParse(param[1], out linea)) return "linea errata";
                    if (linea < 1 || linea > linee) return "linea errata";
                    if (param.Length == 3)
                    {
                        if (!int.TryParse(param[2], out timeout)) return "timeout errato";
                        if (timeout < 1 || timeout > 999) return "timeout errato";
                        thcmd.timeout = timeout;
                    }
                    thcmd.linea = linea;
                    thcmd.comando = tipoComando.offon;
                    newThread.Start(thcmd);
                    return "ok";
                case "onoff":
                    if (!int.TryParse(param[1], out linea)) return "linea errata";
                    if (linea < 1 || linea > linee) return "linea errata";
                    if (param.Length == 3)
                    {
                        if (!int.TryParse(param[2], out timeout)) return "timeout errato";
                        if (timeout < 1 || timeout > 999) return "timeout errato";
                        thcmd.timeout = timeout;
                    }
                    thcmd.linea = linea;
                    thcmd.comando = tipoComando.onoff;
                    newThread.Start(thcmd);
                    return "ok";
                case "stato":
                    if (param.Length != 1) return "comando errato";
                    return Program.m.scheda.LeggiLineeHEX();
                default:
                    return "Comando non valido";
            }
        }
        public void StartListener()
        {
            bool done = false;
            const int listenPort=11000;
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Loopback, listenPort);

            try
            {
                while (!done)
                {
                    Program.log ("Waiting for broadcast 2");
                    byte[] bytes = listener.Receive(ref groupEP);
                    String s = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    String lg = string.Format("Received broadcast from {0}: {1}", groupEP.ToString(), s);
                    Program.log(lg);
                    String r = Elabora(s);
                    byte[] sendbuf = Encoding.ASCII.GetBytes(r);
                    listener.Send(sendbuf, sendbuf.Length, groupEP);
                }

            }
            catch (Exception e)
            {
                Program.log(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
