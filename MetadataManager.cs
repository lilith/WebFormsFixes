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
    /// Manages &lt;meta> tags (controls) on the current page. Not designed for HTTP-EQUIV tags - they are ignored and skipped unless they have a name attribute.
    /// Only deals with meta tags within the Head of the page. The page must have a server-side head tag.
    /// </summary>
    public class MetadataManager {
        protected Page _page = null;

        /// <summary>
        /// Creates a new MetadataManager and attaches it to the current page.
        /// </summary>
        /// <param name="parent"></param>

        public MetadataManager(Page parent) {
            _page = parent;
        }
        /// <summary>
        /// Gets or sets the Content attribute for the specified metadata tag.
        /// Returns null if pair does not exist.
        /// Creates a new metadata tag if it does not exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name] {

            get {
                HtmlMeta m = FindMetaControl(name, _page.Header);
                if (m == null) return null;
                return m.Content;
            }
            set {
                HtmlMeta m = FindMetaControl(name, _page.Header);
                if (m != null) {
                    m.Content = value;
                } else {
                    HtmlMeta newm = new HtmlMeta();
                    newm.EnableViewState = false;
                    newm.Name = name;
                    newm.Content = value;
                    _page.Header.Controls.Add(newm);
                }

            }

        }
        /// <summary>
        /// Returns a collection of all HtmlMeta controls in the header
        /// </summary>
        /// <returns></returns>
        public List<HtmlMeta> GetControls() {
            return ControlUtils.GetControlsOfType<HtmlMeta>(_page.Header);
        }
        /// <summary>
        /// Returns a name:value collection of meta name:content pairs from the page.
        /// If there are multiple meta tags with the same name, the contents are comma-delimited (NameValueCollection.Add behavior)
        /// </summary>
        /// <returns></returns>
        public NameValueCollection GetNameContentPairs() {
            List<HtmlMeta> list = ControlUtils.GetControlsOfType<HtmlMeta>(_page.Header);
            NameValueCollection pairs = new NameValueCollection();
            foreach (HtmlMeta m in list) {
                pairs.Add(m.Name, m.Content);
            }
            return pairs;
        }
        /// <summary>
        /// Returns the first HtmlMeta control with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HtmlMeta GetControl(string name) {
            return FindMetaControl(name, this._page.Header);
        }
        /// <summary>
        /// Whether to include or exclude matches
        /// </summary>
        public enum FilterType {
            ReturnMatches = 1,
            ReturnNonMatches = 2
        }
        /// <summary>
        /// Returns all meta tags that don't match 'pattern' 
        /// To exclude all, specify "*". Otherwise, specify a list of exclusions: "date,expires,description,flags".
        /// Not regex, but case-insensitive.
        /// </summary>
        /// <param name="pattern">To exclude all, specify "*". Otherwise, specify a list of exclusions: "date,expires,description,flags".</param>
        /// <returns></returns>
        public List<HtmlMeta> GetNonMatches(string pattern) {
            return GetMatches(pattern, FilterType.ReturnNonMatches);
        }
        /// <summary>
        /// Returns all meta tags with a name that matches 'pattern' 
        /// To match all, specify "*". Otherwise, specify a list of possibilities: "date,expires,description,flags".
        /// Not regex, but case-insensitive.
        /// </summary>
        /// <param name="pattern">To match all, specify "*". Otherwise, specify a list of possibilities: "date,expires,description,flags".</param>
        /// <returns></returns>
        public List<HtmlMeta> GetMatches(string pattern) {
            return GetMatches(pattern, FilterType.ReturnMatches);
        }

        /// <summary>
        /// Removes the specified HtmlMeta controls from their parents.
        /// </summary>
        /// <param name="list"></param>
        public void RemoveControls(List<HtmlMeta> list) {
            foreach (HtmlMeta m in list) {
                if (m.Parent != null) {
                    m.Parent.Controls.Remove(m);
                }
            }
        }

        /// <summary>
        /// Hides the specified HtmlMeta controls from rendering
        /// </summary>
        /// <param name="list"></param>
        public void HideControls(List<HtmlMeta> list) {
            foreach (HtmlMeta m in list) {
                m.Visible = false;
                m.EnableViewState = false;
            }
        }
        /// <summary>
        /// Returns a collection of HtmlMeta tags that match 'pattern' (or don't match, depending on 'filter').
        /// Pattern is not a regex, but supports alternations and is case-insensitive. if Pattern="*", then everything matches.
        /// Pattern can be a single meta name, or a list of meta names (comma or | delimited).
        /// </summary>
        /// <param name="pattern">To match all, specify "*". Otherwise, specify a list of possibilities: "date,expires,description,flags".</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<HtmlMeta> GetMatches(string pattern, FilterType filter) {

            //List of all meta controls in the head
            List<HtmlMeta> list = ControlUtils.GetControlsOfType<HtmlMeta>(_page.Header);

            //Parse pattern string
            bool wildcard = (pattern.Equals("*", StringComparison.OrdinalIgnoreCase));

            string[] parts = pattern.Replace(',', '|').Split('|');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Trim().ToLowerInvariant();

            //Index valid names in a binary tree
            List<string> names = new List<string>(parts);
            names.Sort();

            //Create collections to hold matches and non-matches.
            List<HtmlMeta> matches = new List<HtmlMeta>();
            List<HtmlMeta> nonmatches = new List<HtmlMeta>();

            //Loop throught controls and distribute to the appropriate collection.
            foreach (HtmlMeta m in list) {
                //Skip meta tags with an no name (probably HTTP-EQIV)
                if (m.Name == null) continue;

                if (wildcard) {
                    matches.Add(m);
                } else if ((names.BinarySearch(m.Name.ToLowerInvariant()) > 0)) {
                    matches.Add(m);
                } else {
                    nonmatches.Add(m);
                }
            }

            //Return the correct collection based upon the filter type
            if (filter == FilterType.ReturnMatches) return matches;
            else if (filter == FilterType.ReturnNonMatches) return nonmatches;
            else throw new ArgumentException("filter must be a valid enumeration value", "filter");
        }
        /// <summary>
        /// Recursively searches the hierarchy of 'parent' for the first HtmlMeta instance with the specified Name attribute.
        /// Case-insensitive. 
        /// </summary>
        /// <param name="name">Case-insensitive. </param>
        /// <param name="parent">Control tree to search</param>
        /// <returns></returns>
        protected static HtmlMeta FindMetaControl(string name, Control parent) {
            if (parent is HtmlMeta) {
                HtmlMeta m = parent as HtmlMeta;
                if (m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return m;
            }
            foreach (Control c in parent.Controls) {
                HtmlMeta m = FindMetaControl(name, c);
                if (m != null) return m;
            }
            return null;
        }

    }

}