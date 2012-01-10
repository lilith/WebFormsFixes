using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using fbs;

namespace fbs.Adapters
{
    /// <summary>
    /// Allows controls to prevent rendering of the "id" attribute. 
    /// Adds support for the HideID attribute to classes that inherit from HtmlControl and WebControl.
    /// If HideID="true", the ID property will be set to null at render time, causing no id to be rendered.
    /// </summary>
    public class HideIDAdapter : System.Web.UI.Adapters.ControlAdapter
    {

        protected override void Render(HtmlTextWriter writer)
        {

            if (this.Control is HtmlControl)
            {
                HtmlControl hc = this.Control as HtmlControl;
                if (hc.Attributes["HideID"] != null)
                {
                    if (hc.Attributes["HideID"].Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Control.ID = null;
                    }
                    hc.Attributes.Remove("HideID");
                }
            }
            if (this.Control is WebControl)
            {
                WebControl wc = this.Control as WebControl;
                if (wc.Attributes["HideID"] != null)
                {
                    if (wc.Attributes["HideID"].Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Control.ID = null;
                    }
                    wc.Attributes.Remove("HideID");
                }
            }

            base.Render(writer);
        }

    }

}