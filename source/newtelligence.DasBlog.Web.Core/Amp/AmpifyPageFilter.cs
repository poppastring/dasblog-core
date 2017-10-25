using System.IO;
using System.Text;

namespace newtelligence.DasBlog.Web.Core.Amp
{
    public class AmpifyPageFilter : MemoryStream
    {
        private Stream _stream = null;

        public AmpifyPageFilter(Stream stream)
        {
            _stream = stream;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string content = UTF8Encoding.UTF8.GetString(buffer);

            content = AmpifyBlogContent(content);

            _stream.Write(UTF8Encoding.UTF8.GetBytes(content), offset, UTF8Encoding.UTF8.GetByteCount(content));

            base.Write(UTF8Encoding.UTF8.GetBytes(content), offset, UTF8Encoding.UTF8.GetByteCount(content));
        }

        private string AmpifyBlogContent(string blogcontent)
        {
            ProcessImage pi = new ProcessImage();
            blogcontent = pi.ReplaceTag(blogcontent);

            ProcessYouTube pf = new ProcessYouTube();
            blogcontent = pf.ReplaceTag(blogcontent);

            ProcessTwitter ti = new ProcessTwitter();
            blogcontent = ti.ReplaceTag(blogcontent);

            return blogcontent;
        }
    }

}
