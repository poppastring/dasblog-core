using System;
using System.Net;

namespace Subtext.Akismet
{
	/// <summary>
	/// Exception thrown when a response other than 200 is returned.
	/// </summary>
	/// <remarks>
	/// This exception does not have any custom properties, 
	/// thus it does not implement ISerializable.
	/// </remarks>
	[Serializable]
	public sealed class InvalidResponseException : Exception
	{
		HttpStatusCode status = (HttpStatusCode)0;

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
		/// </summary>
		public InvalidResponseException() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public InvalidResponseException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public InvalidResponseException(string message, HttpStatusCode status) : base(message)
		{
			this.status = status;
		}

		/// <summary>
		/// Gets the HTTP status returned by the service.
		/// </summary>
		/// <value>The HTTP status.</value>
		public HttpStatusCode HttpStatus
		{
			get
			{
				return this.status;
			}
		}
	}
}
