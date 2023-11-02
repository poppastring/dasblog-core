using System;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class UserPageViewModel
	{
		[JsonPropertyName("@context")]
		public object[] context { get; set; }
		public string id { get; set; }
		public string type { get; set; }
		public string next { get; set; }
		public string prev { get; set; }
		public string partOf { get; set; }
		public OrderedItemViewModel[] orderedItems { get; set; }

		public string ToJson()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UserPageViewModel));
				serializer.WriteObject(stream, this);
				using (StreamReader reader = new StreamReader(stream))
				{
					string json = reader.ReadToEnd();
					return json;
				}
			}
		}

		public static UserPageViewModel FromJson(string json)
		{
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UserPageViewModel));
				return (UserPageViewModel)serializer.ReadObject(stream);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			UserPageViewModel otheruser = (UserPageViewModel)obj;

			bool isEqual = id == otheruser.id && type == otheruser.type;

			return isEqual;
		}
	}

	public class OrderedItemViewModel
	{
		public string id { get; set; }
		public string type { get; set; }
		public string actor { get; set; }
		public DateTime published { get; set; }
		public string[] to { get; set; }
		public string[] cc { get; set; }
		public bool sensitive { get; set; }
		public string content { get; set; }
	}

	public class UserPageContextViewModel
	{
		public string ostatus { get; set; }
		public string atomUri { get; set; }
		public string inReplyToAtomUri { get; set; }
		public string conversation { get; set; }
		public string sensitive { get; set; }
		public string toot { get; set; }
		public string votersCount { get; set; }
		public string blurhash { get; set; }
		public Focalpoint focalPoint { get; set; }
		public string Hashtag { get; set; }
	}
}
