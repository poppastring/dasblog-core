using System;

namespace newtelligence.DasBlog.Runtime
{
	[Serializable]
	public class PingbackInfo
	{
		private string pingbackService;
		private string sourceUrl;
		private string sourceTitle;
		private string sourceExcerpt;
		private string sourceBlogName;

		public PingbackInfo(string sourceUrl, string sourceTitle, string sourceExcerpt, string sourceBlogName)
		{
			this.sourceBlogName = sourceBlogName;
			this.sourceExcerpt = sourceExcerpt;
			this.sourceTitle = sourceTitle;
			this.sourceUrl = sourceUrl;
		}

		public string PingbackService
		{
			get { return pingbackService; }
			set { pingbackService = value; }
		}

		public string SourceUrl
		{
			get { return sourceUrl; }
			set { sourceUrl = value; }
		}

		public string SourceTitle
		{
			get { return sourceTitle; }
			set { sourceTitle = value; }
		}

		public string SourceExcerpt
		{
			get { return sourceExcerpt; }
			set { sourceExcerpt = value; }
		}

		public string SourceBlogName
		{
			get { return sourceBlogName; }
			set { sourceBlogName = value; }
		}
	}
}