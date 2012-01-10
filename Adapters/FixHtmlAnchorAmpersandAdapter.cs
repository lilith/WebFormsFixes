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
    /// Patches the ampersand bug in HtmlAnchor controls.
    /// Affects HtmlAnchor controls (&lt;a href="" runat="server" /&gt;).
    /// Supports HideID, but not HideIDAlways.
    /// </summary>
    public class FixHtmlAnchorAmpersandAdapter : System.Web.UI.Adapters.ControlAdapter
    {
        public FixHtmlAnchorAmpersandAdapter()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        protected override void Render(HtmlTextWriter writer)
        {
            HtmlAnchor ha = (HtmlAnchor)this.Control;
            ha.HRef = HttpUtility.HtmlAttributeEncode(ha.HRef);
            if (ha.Attributes["HideID"] != null)
            {
                if (ha.Attributes["HideID"].Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    ha.ID = null;
                }
                ha.Attributes.Remove("HideID");
            }
            base.Render(writer);
        }

        

    }

}