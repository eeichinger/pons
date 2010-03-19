using System;

namespace WebGUIApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            storedName.Value = (string) this.Session["name"];
        }
    }
}
