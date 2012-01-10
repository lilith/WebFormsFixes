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
using NathanaelJones.WebFormsFixes;

namespace NathanaelJones.WebFormsFixes
{

    /// <summary>
    /// Extends System.Web.UI.Page
    /// Also allows an alternate 'Runtime' master page to be set.
    /// Records the URL than was the original referrer (before all of the postbacks)
    /// Repairs parsing issues where ContentPlaceHolder doesn't parse link and meta information, even if it is located in
    /// a HEAD tag.
    /// 
    /// If you want to use RuntimeMasterPage, OriginalReferrer, modify metdata at runtime, or use stylesheets, you need to inherit
    /// from this class. If this is an article, inherit from ArticleBase.
    /// </summary>
    public partial class PageBase : Page
    {
        /// <summary>
        /// Creates a new PageBase instance. 
        /// </summary>
        public PageBase()
        {
            if (!IsPostBack)
            {
                OriginalReferrer = GetReferrer();
            }

        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this); //For referer tracking
            base.OnInit(e);
        }
        protected string _originalReferrer = null;
        /// <summary>
        /// This the the referrer recorded in viewstate before any post-backs occurred.
        /// </summary>
        public string OriginalReferrer { get { return _originalReferrer; } set { _originalReferrer = value; } }

        protected string GetReferrer(){
            if (HttpContext.Current == null) return null;
            if (HttpContext.Current.Request == null) return null;
            if (HttpContext.Current.Request.UrlReferrer == null) return null;
            if (HttpContext.Current.Request.UrlReferrer.OriginalString == null) return null;
            return HttpContext.Current.Request.UrlReferrer.OriginalString;
        }

        /// <summary>
        /// The original referrer in normalized, yrl form.
        /// </summary>
        public yrl OriginalReferrerYrl { get { return yrl.FromString(OriginalReferrer); } }

        /// <summary>
        /// Loads the stored referrer value (if any) from Control State
        /// </summary>
        /// <param name="savedState"></param>
        protected override void LoadControlState(object savedState)
        {
            object[] rgState = (object[])savedState;
            base.LoadControlState(rgState[0]);
            if (rgState[1] != null)
                _originalReferrer = (string)rgState[1];
        }
        /// <summary>
        /// Saves the referrer value (if any) to Control State
        /// </summary>
        /// <returns></returns>
        protected override object SaveControlState()
        {
            object[] rgState = new object[2];
            rgState[0] = base.SaveControlState();
            if (OriginalReferrer == null) rgState[1] = null;
            else rgState[1] = OriginalReferrer;
            return rgState;
        }

        protected string runtimeMasterPage = null;
        /// <summary>
        /// The master page file to use during runtime. This value is passed through the yrl class for normalization purposes.
        /// </summary>
        public string RuntimeMasterPage
        {
            get
            {
                return runtimeMasterPage;
            }
            set
            {
                yrl validate = new yrl(value);
                runtimeMasterPage = validate.VirtualURL;
            }
        }


        /// <summary>
        /// Sets the runtime master page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreInit(EventArgs e)
        {
            HttpContext.Current.Items["OnPreInitTimestamp"] = DateTime.Now;
            base.OnPreInit(e);
            //Set the runtime master page (if set)
            if (!string.IsNullOrEmpty(RuntimeMasterPage)) 
                this.MasterPageFile = RuntimeMasterPage;
        }





        protected MetadataManager _metadata = null;
        /// <summary>
        /// Manages page metadata. Add, remove, and query metadata 
        /// (Only meta tags with a name attribute are affected, and only those located in the head section)
        /// </summary>
        public MetadataManager Metadata { 
            get {
                if (_metadata == null) _metadata = new MetadataManager(this);
                return _metadata; 
            } 
        }


        protected LinkManager _Stylesheets = null;
        /// <summary>
        /// Manages all of the HtmlLink controls in the head section of the page.
        /// Register, delete, and enumerate all link tags.
        /// </summary>
        public LinkManager Stylesheets
        {
            get
            {
                if (_Stylesheets == null) _Stylesheets = new LinkManager(this);
                return _Stylesheets;
            }
        }

        /// <summary>
        /// Calls the ContentPlaceHolderHeadRepair.Repair method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //Parses link, meta, and script tags that got missed by the Head>CPH bug.
            ContentPlaceHolderFixes.RepairPageHeader(this);

            //Fire the events later
            base.OnLoad(e);
        }


        /// <summary>
        /// Returns the first ContentPlaceHolder with the specified ID. Returns null if no matches are found.
        /// </summary>
        /// <param name="sectionID">Case-insensitive</param>
        /// <returns></returns>
        public ContentPlaceHolder GetSectionControl(string sectionID)
        {
            List<ContentPlaceHolder> cphList = ControlUtils.GetControlsOfType<ContentPlaceHolder>(this);

            foreach (ContentPlaceHolder cph in cphList)
            {
                if (cph.ID.Equals(sectionID, StringComparison.OrdinalIgnoreCase))
                {
                    return cph;
                }
            }
            return null;
        }





        /// <summary>
        /// Returns the printed control tree of the page as a string.  For debug purposes.
        /// </summary>
        /// <returns></returns>
        public string GetTree()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            ControlUtils.PrintTree(this, 0, sw);
            return sb.ToString();
        }

       

    }
}