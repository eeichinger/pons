using System;
using System.Web.UI.HtmlControls;

namespace WebGUIApplication1
{
    public class _Default : System.Web.UI.Page
    {
        protected HtmlInputText name;
        protected HtmlButton submit;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            submit.ServerClick += new EventHandler(submit_ServerClick);
        }

        void submit_ServerClick(object sender, EventArgs e)
        {
            this.Session["name"] = name.Value;
            Response.Redirect("DisplayInput.aspx");
        }
    }
}
