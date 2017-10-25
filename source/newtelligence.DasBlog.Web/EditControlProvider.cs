using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.TextEditors;

namespace newtelligence.DasBlog.Web
{
    /// <summary>
    /// Responsible for creating edit controls
    /// </summary>
    public static class EditControlProvider
    {
        /// <summary>
        /// Creates and returns instance of a <see cref="Core.EditControlAdapter"/> based on the site configuration.
        /// </summary>
        /// <returns>An instance of <see cref="Core.EditControlAdapter"/></returns>
        public static EditControlAdapter CreateEditControl<TDefault>()
            where TDefault : EditControlAdapter, new()
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            string configuredEditControl = siteConfig.EntryEditControl;

            Core.EditControlAdapter editControl;

            Type editControlType;
            // try the configured control
            if (TryGetType(configuredEditControl, out editControlType) && TryCreateControl(editControlType, out editControl))
            {
                return editControl;
            }

            // try the requested default
            if (TryCreateControl(typeof(TDefault), out editControl))
            {
                return editControl;
            }

            // if all else failed, default to the plain textbox, since that doesn't depend on external assemblies.
            return new TextBoxAdapter();
        }

        private static bool TryCreateControl(Type editControlType, out EditControlAdapter editControl)
        {
            editControl = null;

            try
            {
                editControl = (EditControlAdapter)Activator.CreateInstance(editControlType);
                return true;
            }
            catch (Exception exc)
            {
                // prevents the editentry page from failing when the configured editor is not (or no longer) supported
                ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                string message = String.Format("Failed to load configured editor type. \"{0}\" Exception: \"{1}\"", editControlType, exc);
                loggingService.AddEvent(new EventDataItem(EventCodes.Error, message, HttpContext.Current.Request.Url.ToString()));
            }

            return false;
        }

        private static bool TryGetType(string typeName, out Type type)
        {
            type = null;

            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            try
            {
                type = Type.GetType(typeName, /* throw on error */ true, /* ignore case */ true);
                return true;
            }
            catch (Exception exc)
            {
                // prevents the editentry page from failing when the configured editor is not (or no longer) supported
                ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                string message = String.Format("Failed to load configured editor type. \"{0}\" Exception: \"{1}\"", typeName, exc);
                loggingService.AddEvent(new EventDataItem(EventCodes.Error, message, HttpContext.Current.Request.Url.ToString()));
            }

            return false;
        }
    }
}
