using System;

namespace newtelligence.DasBlog.Runtime
{
	/// <summary>
	/// A description of the quality of a piece of feedback
	/// </summary>
	public enum SpamState { 
		/// <summary>
		/// The feedback has not been examined for quality
		/// </summary>
		NotChecked,
		/// <summary>
		/// The feedback is not considered a nuisance
		/// </summary>
		NotSpam,
		/// <summary>
		/// The feedback is considered a nuisance
		/// </summary>
		Spam
	}

}
