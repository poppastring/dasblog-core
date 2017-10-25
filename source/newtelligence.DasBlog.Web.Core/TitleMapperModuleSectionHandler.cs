using System;
using System.Configuration;

namespace newtelligence.DasBlog.Web.Core
{

    /// <summary>
    /// AGB, 10 June 2008
    /// TitleMapperModuleSectionHandler - moving exlusions from <see cref="TitleMapperModule.HttpHandlerStrings"/>
    /// to a configuration section. If the section is not in the Web.config - will fallback to using the 
    /// string array as before.
    /// </summary>
    public class TitleMapperModuleSectionHandler : ConfigurationSection
    {

        public TitleMapperModuleSectionHandler()
        {

        }

        /// <summary>
        /// Private instance for the custom configuration section.
        /// </summary>
        private static TitleMapperModuleSectionHandler _instance = TryGetConfigurationSection();

        /// <summary>
        /// Singleton access to the section. Since this class will never be used remotely it's stafe to use the 
        /// static member pattern for a Singleton. 
        /// </summary>
        public static TitleMapperModuleSectionHandler Settings
        {
            get { return _instance; }
        }
        
        /// <summary>
        /// Helper method to try and load the section during object initialization
        /// </summary>
        /// <returns></returns>
        private static TitleMapperModuleSectionHandler TryGetConfigurationSection()
        {
            TitleMapperModuleSectionHandler settings;

            try
            {
                settings = ConfigurationManager.GetSection("newtelligence.DasBlog.TitleMapper") as TitleMapperModuleSectionHandler;
            }
            catch
            {
                settings = new TitleMapperModuleSectionHandler();
                settings.Exclusions = new ExclusionCollection(); //count will be zero
            }

            return settings;
        }

        // Declare a collection element represented 
        // in the configuration file by the sub-section
        // <exclusions> <add .../> </exclusions> 
        // Note: the "IsDefaultCollection = false" 
        // instructs the .NET Framework to build a nested 
        // section like <exclusions> ...</exclusions>.
        // Note: Use a private member as the store so we can create 
        // a safe Count=0 collection if the section was not loaded.
        private ExclusionCollection _exclusions;
        [ConfigurationProperty("exclusions", IsDefaultCollection = false)]
        public ExclusionCollection Exclusions
        {
            get
            {
                _exclusions = base["exclusions"] as ExclusionCollection;
                
                if (_exclusions == null)
                    _exclusions = new ExclusionCollection();
                
                return _exclusions;
            }
            set
            {
                _exclusions = value;
            }
        }
    }

    // Define the ExclusionCollection that contains ExclusionElements.
    public class ExclusionCollection : ConfigurationElementCollection
    {
        public ExclusionCollection()
        {
            //ExclusionElement exclusion = (ExclusionElement)CreateNewElement();
            //// Add the element to the collection.
            //Add(exclusion);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ExclusionElement();
        }


        protected override ConfigurationElement CreateNewElement(string path)
        {
            return new ExclusionElement(path);
        }


        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ExclusionElement)element).Path;
        }


        public new string AddElementName
        {
            get { return base.AddElementName; }
            set { base.AddElementName = value; }
        }

        public new string ClearElementName
        {
            get { return base.ClearElementName; }
            set { base.AddElementName = value; }
        }

        public new string RemoveElementName
        {
            get { return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }


        public ExclusionElement this[int index]
        {
            get
            {
                return (ExclusionElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        new public ExclusionElement this[string path]
        {
            get
            {
                return (ExclusionElement)BaseGet(path);
            }
        }

        public int IndexOf(ExclusionElement exclusion)
        {
            return BaseIndexOf(exclusion);
        }

        public void Add(ExclusionElement exclusion)
        {
            BaseAdd(exclusion);           
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);            
        }

        public void Remove(ExclusionElement exclusion)
        {
            if (BaseIndexOf(exclusion) >= 0)
                BaseRemove(exclusion.Path);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string path)
        {
            BaseRemove(path);
        }

        public void Clear()
        {
            BaseClear();
            // Add custom code here.
        }
    }


    // Define the ExclusionElement.
    public class ExclusionElement : ConfigurationElement
    {

        public ExclusionElement(String path)
        {
            Path = path;           
        }

        public ExclusionElement()
        {

        }       

        [ConfigurationProperty("path", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override bool Equals(object compareTo)
        {
            if (compareTo is ExclusionElement)
                return this.Path.Equals(((ExclusionElement)compareTo).Path, StringComparison.OrdinalIgnoreCase);
            else
                return false;
        }
    }
}
