# Metadata


## Example usage of the metadata classes.


Here's a single example of Metadata class usage. This does 1 things. 

1. Applies configured allow/deny rules to filter out unwanted meta tags that are CMS-specific.
2. Sets the 'description' meta tag using the text content of the "Summary" section in the content page.

 
        /// <summary>
        /// Doesn't render the summary. Returns the concatenated contents of all literals and literalcontrols in the Summary secion (ContentPlaceHolder). Will contain html tags unless they are runat="server"
        /// Returns an empty string if missing or blank.
        /// </summary>
        /// <returns></returns>
        public string GetDescriptionFromSummaryControl()
        {
            Control c = this.GetSectionControl("Summary");
            if (c == null)
            {
                return String.Empty;
            }
            string s = PageBase.GetLiteralContents(c);
            if (s == null) return String.Empty;
            return s.Trim();

        }
        /// <summary>
        /// Filters out meta tags based on AppSettings["AllowedMetaTags"] or AppSettings["FilteredMetaTags"].
        /// Inserts the description meta tag based on the summary contents.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Strip out unneeded meta tags

            string allow = ConfigurationManager.AppSettings["AllowedMetaTags"];
            string deny = ConfigurationManager.AppSettings["FilteredMetaTags"];

            if (!string.IsNullOrEmpty(allow)) this.Metadata.HideControls(this.Metadata.GetNonMatches(allow));
            if (!string.IsNullOrEmpty(deny)) this.Metadata.HideControls(this.Metadata.GetMatches(deny));

            //Populate the description meta tag
            if (this.Metadata["description"] == null)
            {
                //TODO: Won't this insert <p> tags if they are in Summary?
                this.Metadata["description"] = GetDescriptionFromSummaryControl();
            }
        }