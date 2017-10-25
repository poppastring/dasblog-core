using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace newtelligence.DasBlog.Web.Core
{
    /// <summary>
    /// Converts a 3rd party text editor control's interface to the common interface needed by dasBlog
    /// </summary>
    public abstract class EditControlAdapter
    {

        /// <summary>
        /// An instance of the control being adapted
        /// </summary>
        public abstract Control Control { get; }

        /// <summary>
        /// Gets or sets the text content of the control being adapted
        /// </summary>
        public abstract string Text
        {
            get;
            set;
        }

        /// <summary>
        /// true if the Text property holds a non-default value; otherwise false.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasText() { return String.IsNullOrEmpty(this.Text); }

        /// <summary>
        /// Performs additional initialization required after the control has been 
        /// added to a parent control tree.
        /// </summary>
        /// <remarks>While most initialization should be performed in the adapter's 
        /// constructor, there may be some steps that cannot be performed until the 
        /// adapted control has been added to a control tree (ex: Page).</remarks>
        public virtual void Initialize() { }

        /// <summary>
        /// Gets or sets the width of the adapted control
        /// </summary>
        public abstract Unit Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the adapted control
        /// </summary>
        public abstract Unit Height { get; set; }

        /// <summary>
        /// Sets the direction of the text in the Text property.
        /// </summary>
        /// <param name="textDirection">The direction of the text in the Text property</param>
        public virtual void SetTextDirection(SharedBasePage.TextDirection textDirection) { }

        /// <summary>
        /// Sets the language of the text in the Text property.
        /// </summary>
        /// <param name="language">The language of the Text property.</param>
        public virtual void SetLanguage(string language) { }

    }
}
