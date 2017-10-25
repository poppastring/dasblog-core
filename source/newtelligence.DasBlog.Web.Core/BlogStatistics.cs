namespace newtelligence.DasBlog.Web.Core
{
    /// <summary>
    /// Represents the statistics for this blog.
    /// </summary>
    internal sealed class BlogStatistics
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlogStatistics" /> class.
        /// </summary>
        public BlogStatistics()
        {
            //...
        }

        // PROPERTIES
        public int AllEntriesCount
        {
            get;
            set;
        }

        public int CommentCount
        {
            get;
            set;
        }

        public int MonthPostCount
        {
            get;
            set;
        }

        public int WeekPostCount
        {
            get;
            set;
        }

        public int YearPostCount
        {
            get;
            set;
        }
    }
}

