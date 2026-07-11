#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DasBlog.Core.Security
{
	[Serializable]
	[XmlType("TwoFactor")]
	public class TwoFactorSettings
	{
		[XmlAttribute("enabled")]
		public bool Enabled { get; set; }

		[XmlElement("AuthenticatorSecret")]
		public string AuthenticatorSecret { get; set; }

		[XmlArray("RecoveryCodes")]
		[XmlArrayItem("Code")]
		public List<RecoveryCode> RecoveryCodes { get; set; } = new List<RecoveryCode>();

		public bool ShouldSerializeAuthenticatorSecret()
		{
			return !string.IsNullOrEmpty(AuthenticatorSecret);
		}

		public bool ShouldSerializeRecoveryCodes()
		{
			return RecoveryCodes?.Any() == true;
		}
	}
}
