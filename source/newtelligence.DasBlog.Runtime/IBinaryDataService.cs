using System.IO;

namespace newtelligence.DasBlog.Runtime
{
    public interface IBinaryDataService
    {
        /// <summary>
        /// Saves the file to permanent storage and returns the url to the file.
        /// </summary>
        /// <param name="inputFile">The stream containing the file.</param>
        /// <param name="filename">The name of the file being saved, can contain a relative directory. 
        /// The filename is updated to the filename of the actual saved file.</param>
        /// <returns>The full url to the file.</returns>
        string SaveFile(Stream inputFile, ref string filename);

        /// <summary>
        /// Deletes a file from the permanent storage and returns a bool to indicate succes.
        /// </summary>
        /// <param name="path">The full url to the file to remove.</param>
        /// <returns><see langword="true" /> when deleting succeeded; otherwise <see langword="false" />.</returns>
        bool DeleteFile(string path);
    }

    /// <summary>
    /// Represents the different kinds of binary files dasBlog can handle.
    /// </summary>
    public sealed class BinaryFileType
    {
        // fields are not const, to prevent the compiler substituting in 
        // dependent assemblier.

        private BinaryFileType(string type)
        {
            this.Type = type;
        }

        public override string ToString()
        {
            return this.Type;
        }

        public string Type { get; private set; }

        /// <summary>
        /// A general attachement added to a post. e.g. a sourcefile
        /// </summary>
        public static readonly BinaryFileType Attachement = new BinaryFileType("Attachement");
       
        /// <summary>
        /// An images added to the post.
        /// </summary>
        public static readonly BinaryFileType Image = new BinaryFileType("Image");

        /// <summary>
        /// An enclosure added to the post.
        /// </summary>
        public static readonly BinaryFileType Enclosure = new BinaryFileType("Enclosure");
    }
}
