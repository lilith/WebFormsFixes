using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using NathanaelJones;

namespace NathanaelJones.WebFormsFixes
{
    /// <summary>
    /// Sets the ID property of the control to null at render time, preventing it from being rendered.
    /// Affects classes that inherit from HtmlControl and WebControl. Applied via App_Browsers to certain controls.
    /// </summary>
    public class HideIDAlwaysAdapter : System.Web.UI.Adapters.ControlAdapter
    {

        protected override void Render(HtmlTextWriter writer)
        {
            bool dontHide = false;
            if (this.Control is HtmlControl)
            {
                HtmlControl hc = this.Control as HtmlControl;
                if (hc.Attributes["HideID"] != null)
                {
                    if (hc.Attributes["HideID"].Equals("false", StringComparison.OrdinalIgnoreCase)) dontHide = true;
                    hc.Attributes.Remove("HideID");
                }
            }
            if (this.Control is WebControl)
            {
                WebControl wc = this.Control as WebControl;
                if (wc.Attributes["HideID"] != null)
                {
                    if (wc.Attributes["HideID"].Equals("false", StringComparison.OrdinalIgnoreCase)) dontHide = true;
                    wc.Attributes.Remove("HideID");
                }
            }

            if (!dontHide) this.Control.ID = null;
            base.Render(writer);
            
        }

    }

}