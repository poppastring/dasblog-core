namespace newtelligence.DasBlog.Runtime
{
    /// <summary>
    /// Interface representing a post that has a title.
    /// </summary>
    public interface ITitledEntry
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }

        /// <summary>
        /// Gets the compressed title.
        /// </summary>
        /// <value>The compressed title.</value>
        string CompressedTitle { get; }

        /// <summary>
        /// Gets the compressed title unique.
        /// </summary>
        /// <value>The compressed title unique.</value>
        string CompressedTitleUnique { get; }

        /// <summary>
        /// Gets or sets the entry id.
        /// </summary>
        /// <value>The entry id.</value>
        string EntryId { get; set; }
    }
}
