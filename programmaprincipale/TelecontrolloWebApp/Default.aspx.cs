using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace telecontrollo
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MostraDispositivi();            
            if(IsPostBack )
            {
            }
            

        }
        void MostraDispositivi()
        {
            int numDev = Global.scheda.NumeroLineeIO;
            String j="";
            for (int i = 0; i < numDev; i++)
            {
                j += "<div id=\"linea" + i.ToString() + "\" class=\"col-md-1\">\n";
                j += "Linea " + i.ToString() + "\n";
                j += "<button id=\"btn" + i.ToString() + "\" type=\"submit\" class=\"btn btn-success\" runat=\"server\">Success</button>";
                j += "</div>\n";
            }
            jj.InnerHtml=j;
        }

        protected void bt1_Click(object sender, EventArgs e)
        {

        }
        protected void btn1_Click(object sender, EventArgs e)
        {

        }
       

       
    }
}