using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub
{
	public class InboxMessage
	{
		[JsonPropertyName("@context")]
		public object? Context { get; set; }
		public string Actor { get; set; } = default!;
		public List<string>? Cc { get; set; }
		public string? Id { get; set; }
		public string? Object { get; set; }
		public DateTime? Published { get; set; }
		public string? State { get; set; }
		public List<string>? To { get; set; }
		public string Type { get; set; } = default!;

		public bool IsFollow()
		{
			return Type.Equals("Follow");
		}

		public bool IsUndoFollow()
		{
			return Type.Equals("Undo");
		}

		public bool IsDelete()
		{
			return Type.Equals("Delete");
		}

		public bool IsCreateActivity()
		{
			return Type.Equals("Create");
		}

		public bool IsEchoRequest()
		{
			return Type.Equals("EchoRequest");
		}

		public bool IsLikeRequest()
		{
			return Type.Equals("Like");
		}
	}

	public class ObjectContent
	{
		public string Id { get; set; }
		public string InReplyTo { get; set; }
	}
}
