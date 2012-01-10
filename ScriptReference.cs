using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.ComponentModel;

namespace NathanaelJones.WebFormsFixes {

    /// <summary>
    /// A HtmlControl which resolves the 'src' attribute at render time.
    /// </summary>
    public class ScriptReference : HtmlControl {
        public ScriptReference()
            : base("script") { }


        protected override void Render(HtmlTextWriter writer) {
            //Self-closing script tags corrupt the DOM in FF
            writer.WriteBeginTag(this.TagName);
            this.RenderAttributes(writer);
            writer.Write(">");
            writer.WriteEndTag(this.TagName);
        }

        protected override void RenderAttributes(HtmlTextWriter writer) {
            if (!string.IsNullOrEmpty(this.Src)) {
                base.Attributes["src"] = base.ResolveClientUrl(this.Src);
            }
            base.RenderAttributes(writer);
        }

        // Properties
        [UrlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue("")]
        public virtual string Src {
            get {
                string str = base.Attributes["src"];
                if (str == null) {
                    return string.Empty;
                }
                return str;
            }
            set {
                if (String.IsNullOrEmpty(value)) base.Attributes["src"] = null;
                else base.Attributes["src"] = value;
            }
        }
    }
}
