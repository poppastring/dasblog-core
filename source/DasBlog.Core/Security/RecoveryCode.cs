#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
*/
#endregion

using System;
using System.Xml.Serialization;

namespace DasBlog.Core.Security
{
	[Serializable]
	public class RecoveryCode
	{
		[XmlAttribute("hash")]
		public string Hash { get; set; }

		[XmlAttribute("used")]
		public bool Used { get; set; }
	}
}
