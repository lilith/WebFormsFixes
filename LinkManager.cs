using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace NathanaelJones.WebFormsFixes {
    /// <summary>
    /// Manages &lt;link> tags (controls) on the current page.
    /// </summary>
    public class LinkManager {
        protected Page _page = null;
        /// <summary>
        /// Creates a new Link Manager attached to the specified Page instance
        /// </summary>
        /// <param name="parent"></param>
        public LinkManager(Page parent) {
            _page = parent;
        }
        /// <summary>
        /// Adds a CSS reference. Paths must be 1) relative to the page, 2) application-relative, or 3) absolute
        /// </summary>
        /// <param name="href"></param>
        public void AddLink(string href) {
            AddLink(href, "stylesheet", "text/css");
        }
        /// <summary>
        /// Adds a CSS stylsheet reference, but only if there isn't one yet for that path.
        /// Paths must be 1) relative to the page, 2) application-relative, or 3) absolute
        /// </summary>
        /// <param name="href"></param>
        /// <param name="resolveFirst">If true, compares resolved paths instead of raw paths</param>
        public void AddLinkIfMissing(string href) {
            if (this.FindLinkControl(href) == null) {
                AddLink(href);
            }
        }
        /// <summary>
        /// Adds a link tag with the specified rel and type attributes
        /// Paths must be 1) relative to the page, 2) application-relative, or 3) absolute
        /// </summary>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        /// <param name="type"></param>
        public void AddLink(string href, string rel, string type) {
            HtmlLink l = new HtmlLink();

            l.EnableViewState = false;
            l.Href = href;
            l.Attributes["type"] = type;
            l.Attributes["rel"] = rel;
            l.AppRelativeTemplateSourceDirectory = _page.AppRelativeTemplateSourceDirectory;
            _page.Header.Controls.Add(l);
        }
        /// <summary>
        /// Removes all meta tags with a matching href.
        /// Paths must be 1) relative to the page, 2) application-relative, or 3) absolute
        /// </summary>
        /// <param name="href"></param>
        /// <param name="resolveFirst">If true, compares resolved paths instead of raw paths</param>
        public void RemoveLinks(string href) {
            bool resolveFirst = false;
            List<HtmlLink> controls = GetControls();
            string searchfor = href;
            if (resolveFirst) searchfor = _page.ResolveUrl(searchfor);
            foreach (HtmlLink hl in controls) {
                string thishref = hl.Href;

                if (resolveFirst) thishref = _page.ResolveUrl(thishref);

                if (thishref.Equals(searchfor, StringComparison.OrdinalIgnoreCase)) {
                    hl.Parent.Controls.Remove(hl);
                }
            }

        }
        /// <summary>
        /// Returns a collection of all HtmlLink controls in the page header.
        /// </summary>
        /// <returns></returns>
        public List<HtmlLink> GetControls() {
            return ControlUtils.GetControlsOfType<HtmlLink>(_page.Header);
        }
        /// <summary>
        /// Returns a collection of the hrefs in each link tag in the head section.
        /// Paths are 1) relative to the page, 2) application-relative, or 3) absolute
        /// </summary>
        /// <returns></returns>
        public List<string> GetHrefs() {
            List<HtmlLink> list = GetControls();
            List<string> hrefs = new List<string>();
            foreach (HtmlLink l in list) {
                hrefs.Add(l.Href);
            }
            return hrefs;
        }
        /// <summary>
        /// Case-insensitive. Returns the first HtmlLink control in the heirarchy that matches the href. Only scans inside the head tag.
        /// returns null if no match is found.
        /// 
        /// </summary>
        /// <param name="href">Paths must be 1) relative to the page, 2) application-relative, or 3) absolute</param>
        /// <param name="resolveFirst">If true, compares resolved paths instead of raw paths</param>
        /// <returns></returns>
        public HtmlLink FindLinkControl(string href) {
            bool resolveFirst = false;
            List<HtmlLink> controls = GetControls();
            string searchfor = href;
            if (resolveFirst) searchfor = _page.ResolveUrl(searchfor);
            foreach (HtmlLink hl in controls) {
                string thishref = hl.Href;

                if (resolveFirst) thishref = _page.ResolveUrl(thishref);

                if (thishref.Equals(searchfor, StringComparison.OrdinalIgnoreCase)) return hl;
            }
            return null;
        }


    }

}