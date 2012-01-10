using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Web.UI.WebControls;

namespace NathanaelJones.WebFormsFixes {
    public static class ControlUtils {

        /// <summary>
        /// Doesn't use rendering. Recursively loops through content and concatenates all Literal or LiteralControl contents within
        /// the specified control. Used by GetDescriptionFromSummary()
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetLiteralContents(Control c) {
            if (c is Literal) {
                Literal l = c as Literal;
                if (l != null) return l.Text;
            }
            if (c is LiteralControl) {
                LiteralControl l = c as LiteralControl;
                if (l != null) return l.Text;
            }
            StringBuilder sb = new StringBuilder();
            foreach (Control child in c.Controls) {
                string s = GetLiteralContents(child);
                if (s != null) sb.Append(s);
            }
            if (sb.Length == 0) return null;
            return sb.ToString();
        }

        /// <summary>
        /// Prints an text-based tree of the control structure of the specified Web Control. For debug purposes.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="indentLevel"></param>
        /// <param name="outputStream"></param>
        public static void PrintTree(Control c, int indentLevel, TextWriter outputStream) {
            outputStream.WriteLine(String.Empty.PadLeft(indentLevel, '|') + " " + c.GetType().ToString() + " id=" + c.ID + " uid=" + c.UniqueID);
            outputStream.WriteLine(String.Empty.PadLeft(indentLevel, '|') + " dir=" + c.AppRelativeTemplateSourceDirectory);
            if (c.TemplateControl != null) {
                outputStream.WriteLine(String.Empty.PadLeft(indentLevel, '|') + " tc uid=" + c.TemplateControl.UniqueID);
            }
            if (c is LiteralControl)
                outputStream.WriteLine(String.Empty.PadLeft(indentLevel, '|') + " " + ((LiteralControl)c).Text);
            try {
                if (c is HtmlGenericControl)
                    outputStream.WriteLine(String.Empty.PadLeft(indentLevel, '|') + " " + ((HtmlGenericControl)c).InnerHtml);
            } catch { }
            foreach (Control child in c.Controls) {
                PrintTree(child, indentLevel + 1, outputStream);
            }
        }


        /// <summary>
        /// Iterates over the control structure of the specified object and returns all elements that are
        /// of the specified type
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static List<T> GetControlsOfType<T>(Control parent) where T : Control {
            return GetControlsOfType<T>(parent, false, false);
        }
        /// <summary>
        /// Iterates over the control structure of the specified object and returns all elements that are
        /// of the specified type. If there are two items of the specified type, and one is a child of the other, 
        /// the childrenOnly and parentOnly parameters can be used to control which is selected. If both are false, both controls are returned.
        /// </summary>
        /// <param name="parent">The control to search</param>
        /// <param name="childrenOnly">If true, only the innermost matching children will be returned.</param>
        /// <param name="parentsOnly">If true, only the outermost matching parents will be returned.</param>
        /// <returns></returns>
        public static List<T> GetControlsOfType<T>(Control parent, bool childrenOnly, bool parentsOnly) where T : Control {
            if (parent == null) return null;
            if (childrenOnly && parentsOnly) throw
                new ArgumentException("Only one of childrenOnly and parentsOnly may be true. They are mutually exclusive");

            //We are doing last-minute initialization to minimize the overhead of building one of these.
            //The List<> constructor should only be called n times, where n is the number of ContentPlaceHolder controls.
            List<T> temp = null;

            if (parent.Controls != null) {
                //Loop through all of the child controls
                foreach (Control child in parent.Controls) {
                    //Recursively search them also.
                    List<T> next = GetControlsOfType<T>(child, childrenOnly, parentsOnly);

                    //To save on initialization costs.
                    if (next != null) {
                        if (temp == null) {
                            temp = next; //Use existing collection from recursive call
                        } else {
                            //Merge the collections

                            //If a the same object is the child of two different parents, this will
                            //stop it.
                            foreach (T c in next) {
                                if (!temp.Contains(c)) temp.Add(c);
                            }

                        }
                    }
                }
            }

            //If this item is of the target type, add it 
            if ((parent is T)) {
                //If there are no children or we are trying to discard children
                if (parentsOnly || temp == null) {
                    //Clear the list and add the parent
                    T item = (T)parent;

                    temp = new List<T>();

                    temp.Add(item);
                } else if (!childrenOnly) {
                    //Append the parent with the children
                    T item = (T)parent;

                    if (temp == null) temp = new List<T>();

                    temp.Add(item);
                }
            }

            return temp;
        }
    }
}
