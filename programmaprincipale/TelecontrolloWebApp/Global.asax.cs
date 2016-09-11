using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace telecontrollo
{
    public class Global : System.Web.HttpApplication
    {
        public static Pcf scheda;
        //public Pcf scheda
        //{ 
        //    get
        //    { return _scheda; }
        //}
        protected void Application_Start(object sender, EventArgs e)
        {
            ImpostazioniIniziali();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
        protected void ImpostazioniIniziali()
        {
            // leggi telecontrollo.conf
            IniParser parser;
            ushort I2cClockDiv = 2500;
            parser = new IniParser("telecontrollo.conf");
            String tmp = parser.GetSetting("ROOT", "indirizziPcf");
            scheda = new Pcf(tmp);
            if (!ushort.TryParse(parser.GetSetting("ROOT", "I2cClockDiv"), out I2cClockDiv)) I2cClockDiv = 2500;
            scheda.i2cClockDiv = I2cClockDiv;
        }

    }
}