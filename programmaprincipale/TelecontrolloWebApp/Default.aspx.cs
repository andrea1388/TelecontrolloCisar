using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace telecontrollo
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MostraDispositivi();            
         

        }
        void MostraDispositivi()
        {
            UInt16[] linee=Global.scheda.LeggiLinee();

            int numDev = Global.scheda.NumeroLineeIO;
            String j="";
            for (int i = 0; i < numDev; i++)
            {
                int banco = i / 8;

                bool stato = linee[banco];
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.InnerText = "Linea " + i.ToString();
                div.Attributes.Add("class", "col-md-4");
                Button btn = new Button();
                btn.ID="btn"+i.ToString();
                btn.Attributes.Add("class", "btn btn-success");
                btn.CommandName=btn.ID;
                btn.Text = "Accendi linea " + i.ToString();
                btn.UseSubmitBehavior=true;
                btn.Click += new EventHandler(bt1_Click);
                div.Controls.Add(btn);
                form1.Controls.Add(div);
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