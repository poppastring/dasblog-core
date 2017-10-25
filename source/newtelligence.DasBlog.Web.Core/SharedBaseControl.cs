#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion


using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using System.Web.UI;

namespace newtelligence.DasBlog.Web.Core
{

	/// <summary>
	/// This is a base class for your own Pages which implements
	/// automatic serialization of public and protected fields
	/// into session state or into cookies.
	/// </summary>
	/// <example>
	/// All you need to do is this:
	/// 
	/// using newtelligence.Web.UI
	/// 
	/// public class MyPage : SharedBaseControl
	/// {
	///       [PersistentPageState] public int pageVisitsEver;
	///       [PersistentPageState("Visits")] public int siteVisitsEver;
	///       [SessionPageState] public int pageVisitsThisSession;
	///       [SessionPageState("Visits")] public int siteVisitsThisSession;
	///       [TransientPageState] public int roundtripsThisPage;
	///   
	///       // omissions...
	///       
	///       private void Page_Load(object sender, System.EventArgs e)
	///          {
	///              pageVisitsEver++;
	///              siteVisitsEver++;
	///              pageVisitsThisSession++;
	///              siteVisitsThisSession++;
	///              roundtripsThisPage++;            
	///          }    
	///       
	/// }
	/// </example>
	/// <remarks>
	/// This should really be:
	///
	/// template <class T>
	/// public class SharedBaseControl : T
	/// 
	/// but we don't have generics, yet. Sigh.
	/// </remarks>
	public class SharedBaseControl : System.Web.UI.UserControl
	{
		private EventHandler PreRenderHandler = null;
		private const string keyPrefix = "__$stateManagingControl";

		/// <summary>
		/// Public constructor. Sets up events.
		/// </summary>
		public SharedBaseControl()
		{
			Init += new EventHandler(this.SessionLoad);
			PreRender += PreRenderHandler = new EventHandler(this.SessionStore);
		}

		/// <summary>
		/// Event handler attached to "Page.Init" that recovers
		/// marked fields from session state
		/// </summary>
		/// <param name="o">Object firing the event</param>
		/// <param name="e">Event arguments</param>
		private void SessionLoad(object o, EventArgs e)
		{

			FieldInfo[] fields = GetType().GetFields(BindingFlags.Public|
				BindingFlags.NonPublic|
				BindingFlags.Instance);

			// Persistent, page scope values
			HttpCookie pageCookie = Request.Cookies[GetType().FullName];
			if ( pageCookie != null )
			{
				foreach( FieldInfo field in fields )
				{
					if ( field.IsDefined(typeof(PersistentPageStateAttribute),
						true ) &&
						field.FieldType.IsPrimitive )
					{
						PersistentPageStateAttribute ppsa = 
							(PersistentPageStateAttribute)
							field.GetCustomAttributes(
							typeof(PersistentPageStateAttribute),true)[0];
						if ( ppsa.keyName == null && pageCookie[Page.Request.Path+":"+this.UniqueID+"."+field.Name] != null )
						{
							field.SetValue(this,
								Convert.ChangeType(pageCookie[Page.Request.Path+":"+this.UniqueID+"."+field.Name],
								field.FieldType,
								CultureInfo.InvariantCulture));
						}
					}
				}
			}

			// Persistent, user scope values
			HttpCookie siteCookie = Request.Cookies[keyPrefix];
			if ( siteCookie != null )
			{
				foreach( FieldInfo field in fields )
				{
					if ( field.IsDefined(typeof(PersistentPageStateAttribute),
						true ) &&
						field.FieldType.IsPrimitive )
					{
						PersistentPageStateAttribute ppsa = 
							(PersistentPageStateAttribute)
							field.GetCustomAttributes(
							typeof(PersistentPageStateAttribute),true)[0];

						if ( ppsa.keyName != null && siteCookie[ppsa.keyName] != null )
						{
							field.SetValue(this,
								Convert.ChangeType(siteCookie[ppsa.keyName],
								field.FieldType,
								CultureInfo.InvariantCulture));
						}
					}
				}
			}

			// Session scope values
			foreach( FieldInfo field in fields )
			{
				if ( field.IsDefined(typeof(SessionPageStateAttribute),true ) &&
					field.FieldType.IsSerializable )
				{
					SessionPageStateAttribute spsa = 
						(SessionPageStateAttribute)
						field.GetCustomAttributes(
						typeof(SessionPageStateAttribute),true)[0];

					if ( spsa.keyName == null )
					{
						field.SetValue(this,
							Session[Page.Request.Path+":"+this.UniqueID+"."+field.Name]);
					}
					else
					{
						field.SetValue(this,
							Session[keyPrefix+spsa.keyName]);
					}
				}
			}

			if ( IsPostBack )
			{
				// Conversation scope values
				foreach( FieldInfo field in fields)
				{
					if ( field.IsDefined(typeof(TransientPageStateAttribute),
						true ) &&
						field.FieldType.IsSerializable )
					{
						field.SetValue(this,
							Session[Page.Request.Path+":"+this.UniqueID+"."+field.Name]);
					}
				}
			}
		}

		/// <summary>
		/// Event handler attached to "Page.Unload" that sticks marked fields
		/// into session state
		/// </summary>
		/// <param name="o">Object firing the event</param>
		/// <param name="e">Event arguments</param>
		private void SessionStore(object o, EventArgs e)
		{

			FieldInfo[] fields = GetType().GetFields(BindingFlags.Public|
				BindingFlags.NonPublic|
				BindingFlags.Instance);

			// Persistent state
			HttpCookie pageCookie = new HttpCookie(GetType().FullName);
			HttpCookie siteCookie = Request.Cookies[keyPrefix];

			if ( siteCookie == null )
			{
				siteCookie = new HttpCookie(keyPrefix);
			}
			siteCookie.Expires = pageCookie.Expires = DateTime.Now.ToUniversalTime().AddYears(2);

			foreach( FieldInfo field in fields )
			{
				if ( field.IsDefined(typeof(PersistentPageStateAttribute),true ))
				{
					if ( field.FieldType.IsPrimitive )
					{
						PersistentPageStateAttribute ppsa = 
							(PersistentPageStateAttribute)
							field.GetCustomAttributes(
							typeof(PersistentPageStateAttribute),true)[0];
						if ( ppsa.keyName == null )
						{
							pageCookie[Page.Request.Path+":"+this.UniqueID+"."+field.Name] = 
								(string)Convert.ChangeType(field.GetValue(this),
								typeof(string),
								CultureInfo.InvariantCulture);
						}
						else
						{
							siteCookie[ppsa.keyName] = 
								(string)Convert.ChangeType(field.GetValue(this),
								typeof(string),
								CultureInfo.InvariantCulture);
						}
					}
					else
					{
						throw new SerializationException(
							String.Format("Field '{0}' is not a primitive type",field.Name));
					}
				}
			}

			if ( pageCookie.Values.Count > 0 )
			{
				Response.AppendCookie(pageCookie);
			}
			if ( siteCookie.Values.Count > 0 )
			{
				Response.AppendCookie(siteCookie);
			}

			// Transient & Session scope state
			foreach( FieldInfo field in fields)
			{
				if ( field.IsDefined(typeof(TransientPageStateAttribute),true ) ||
					field.IsDefined(typeof(SessionPageStateAttribute),true ) )
				{
					if (  field.FieldType.IsSerializable )
					{
						string fieldName;

						fieldName = Page.Request.Path+":"+this.UniqueID+"."+field.Name;
						if ( field.IsDefined(typeof(SessionPageStateAttribute),true) )
						{
							SessionPageStateAttribute spsa = 
								(SessionPageStateAttribute)
								field.GetCustomAttributes(
								typeof(SessionPageStateAttribute),true)[0];
							if ( spsa.keyName != null )
							{
								fieldName = keyPrefix+spsa.keyName;
							}
						}

						Session[fieldName] = field.GetValue(this);
					}
					else
					{
						throw new SerializationException(
							String.Format("Type {0} of field '{0}' is not serializable",
							field.FieldType.FullName,field.Name));
					}
				}
			}
		}

		/// <summary>
		/// Private utility function invoked by Transfer and Redirect. 
		/// Clears out all transient state of the current page.
		/// </summary>
		private void SessionDiscard()
		{
			// Discard transient page state only
			foreach( FieldInfo field in GetType().GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance))
			{
				if ( field.IsDefined(typeof(TransientPageStateAttribute),true ) &&
					field.FieldType.IsSerializable )
				{
					Session.Remove(Page.Request.Path+":"+this.UniqueID+"."+field.Name);
				}
			}
		}

		/// <summary>
		/// Use this as a replacement for Server.Transfer. This
		/// method will discard the transient page state and 
		/// call Server.Transfer.
		/// </summary>
		/// <param name="target"></param>  
		public virtual void Transfer( string target )
		{
			// ;store persistent session aspects, since PreRender isn't fired now
			SessionStore(null, null); 
			// ;and then discard the transient state
			SessionDiscard();
			Server.Transfer(target);
		}

		/// <summary>
		/// Use this as a replacement for Response.Redirect. This
		/// method will discard the transient page state and 
		/// call Response.Redirect.
		/// </summary>
		/// <param name="target"></param>
		public virtual void Redirect( string target )
		{
			// ;store persistent session aspects, since PreRender isn't fired now
			SessionStore(null, null); 
			// ;and then discard the transient state
			SessionDiscard();
			Response.Redirect(target,true);
		}
	}
}

