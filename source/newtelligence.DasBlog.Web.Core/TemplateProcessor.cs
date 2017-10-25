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
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// Summary description for BaseTemplateProcessor.
	/// </summary>
	public class TemplateProcessor
	{
		public TemplateProcessor()
		{                                                                  
			
		}

		public void ProcessTemplate(SharedBasePage page, string templateString, Control contentPlaceHolder, Macros macros)
		{
			ProcessTemplate(page, null, templateString, contentPlaceHolder, macros);
		}

		private static readonly Regex templateFinder = new Regex("<(?<esc>\\$|%)\\s*(?<macro>(?!\\k<esc>).+?)\\s*\\k<esc>>",RegexOptions.Compiled);

		public void ProcessTemplate(SharedBasePage page, Entry entry, string templateString, Control contentPlaceHolder, Macros macros)
		{
			int lastIndex = 0;

			MatchCollection matches = templateFinder.Matches(templateString);
			foreach( Match match in matches )
			{
				if ( match.Index > lastIndex )
				{
					contentPlaceHolder.Controls.Add(new LiteralControl(templateString.Substring(lastIndex,match.Index-lastIndex)));
				}
				Group g = match.Groups["macro"];
				Capture c = g.Captures[0];
				
				Control ctrl = null;
				object targetMacroObj = macros;
				string captureValue = c.Value;
				
				//Check for a string like: <%foo("bar", "bar")|assemblyConfigName%>
				int assemblyNameIndex = captureValue.IndexOf(")|");
				if (assemblyNameIndex != -1) //use the default Macros
				{
					//The QN minus the )| 
					string macroAssemblyName = captureValue.Substring(assemblyNameIndex+2);
					//The method, including the )
					captureValue = captureValue.Substring(0,assemblyNameIndex+1);

					try
					{
						targetMacroObj = MacrosFactory.CreateCustomMacrosInstance(page, entry, macroAssemblyName);
					}
					catch (Exception ex)
					{
						string ExToString = ex.ToString();
						if (ex.InnerException != null)
						{
							ExToString += ex.InnerException.ToString();
						}
						page.LoggingService.AddEvent(new EventDataItem(EventCodes.Error,String.Format("Error executing Macro: {0}",ExToString),string.Empty));
					}
				}

				try
				{
					ctrl = InvokeMacro(targetMacroObj,captureValue) as Control;
					if (ctrl != null)
					{
						contentPlaceHolder.Controls.Add(ctrl);
					}
					else 
					{
						page.LoggingService.AddEvent(new EventDataItem(EventCodes.Error,String.Format("Error executing Macro: {0} returned null.",captureValue),string.Empty));
					}
				}
				catch (Exception ex)
				{
					string error = String.Format("Error executing macro: {0}. Make sure it you're calling it in your BlogTemplate with parentheses like 'myMacro()'. Macros with parameter lists and overloads must be called in this way. Exception: {1}",c.Value, ex.ToString());
					page.LoggingService.AddEvent(new EventDataItem(EventCodes.Error,error,string.Empty));
				}
				lastIndex = match.Index+match.Length;
			}
			if ( lastIndex < templateString.Length)
			{
				contentPlaceHolder.Controls.Add(new LiteralControl(templateString.Substring(lastIndex,templateString.Length-lastIndex)));
			}
		}

		private bool IsMemberEligibleForMacroCall(MemberInfo m, object filterCriteria )
		{
			//This has to be case-insensitive and culture-invariant or 
			// "Item" and "item" won't match if the current culture is Turk
			return String.Compare(m.Name,(string)filterCriteria,true,System.Globalization.CultureInfo.InvariantCulture)==0;
		}

        /// <summary>
        /// Gotta replace that with regex one day....
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fromOffset"></param>
        /// <param name="endOffset"></param>
        /// <returns></returns>
		private string[] SplitArgs( string args, int fromOffset, ref int endOffset )
		{
			List<string> arrList = new List<string>();
			int argmark=fromOffset+1;
			bool indquote=false,insquote=false,inesc=false;
			char termChar = args[fromOffset]=='['?']':')';
			int argwalk;

			for( argwalk=fromOffset+1;argwalk<args.Length;argwalk++)
			{
				char chargwalk = args[argwalk];
				if ( char.IsWhiteSpace(chargwalk) && !indquote && !insquote )
				{
					continue;
				}
				if ( chargwalk == '\"' && !inesc )
				{
					indquote = !indquote;
					continue;
				}
				if ( chargwalk == '\'' && !inesc )
				{
					insquote = !insquote;
					continue;
				}
				if ( chargwalk == '\\' && (indquote || insquote) )
				{
					inesc=!inesc;
					continue;
				}
				if ( !indquote && !insquote && (chargwalk == ',' || chargwalk == termChar))
				{
					if ( argwalk > argmark )
					{
						string foundArg = args.Substring(argmark,argwalk-argmark).Trim();
                        if ( foundArg[0] == foundArg[foundArg.Length-1] )
                        {
                            foundArg = foundArg.Trim('\"','\'');
                        }
                        arrList.Add(foundArg);
						argmark = argwalk+1;
					}
					if ( chargwalk == termChar) 
					{
						break;
					}
				}
				inesc=false;
			}
			endOffset = argwalk+1;

			return arrList.ToArray();
		}

        private static Dictionary<string, CachedMacro> macros = new Dictionary<string, CachedMacro>();
		private static object macrosLock = new object();

		private class CachedMacro
		{
			public CachedMacro(MemberInfo macro) : this(macro, null){}

			public CachedMacro(MemberInfo macro, object[] argumentList)
			{
				this.Macro = macro;
				this.ArgumentList = argumentList;
			}

			public MemberInfo Macro = null;
			public object[] ArgumentList = null;

			public object Invoke(object obj)
			{
				PropertyInfo p = Macro as PropertyInfo;
				if (p != null)
				{
					return p.GetValue(obj,ArgumentList);
				}
				MethodInfo m = Macro as MethodInfo;
				if(m != null)
				{
					return m.Invoke(obj,ArgumentList);
				}
				FieldInfo f = Macro as FieldInfo;
				if (f != null)
				{
					return f.GetValue(obj);
				}
				throw new NotSupportedException("Invalid cached macro!");
			}
		}

		/// <summary>
        /// This method invokes the macros. It's very tolerant about calls
        /// it can't make, meaning that it absorbs the exceptions
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
		private object InvokeMacro( object obj, string expression )
		{
			int subexStartIndex = 0;
			int subexEndIndex = 0;
			object subexObject = obj;
    			
			try
			{
				do
				{
					subexEndIndex = expression.IndexOfAny(new char[]{'.','(','['}, subexStartIndex);
			
					if ( subexEndIndex == -1 ) subexEndIndex=expression.Length;
					string subex = expression.Substring(subexStartIndex,subexEndIndex-subexStartIndex);


					int subexWithArgsEndIndex = expression.IndexOfAny(new char[]{'(','['}, subexStartIndex);
			
					string subexWithArgs;
					if ( subexWithArgsEndIndex != -1 ) 
						subexWithArgs = expression.Substring(subexStartIndex);
					else
						subexWithArgs = subex;

					// build the cacheKey used for caching the macros: typename.macroname
					// obj can be null, so we should check for that
					// we use obj instead of subexObject to short-cut the loop
					string cacheKey = ( obj != null ? obj.GetType().FullName + "." : "" ) + subexWithArgs;

					CachedMacro subCached;
					if (!macros.TryGetValue(cacheKey, out subCached) || subCached == null)
					{
						lock(macrosLock)
						{
							subexStartIndex = subexEndIndex+1;
							MemberInfo memberToInvoke;
							MemberInfo[] members = subexObject.GetType().FindMembers(
								MemberTypes.Field|MemberTypes.Method|MemberTypes.Property,
								BindingFlags.IgnoreCase|BindingFlags.Instance|BindingFlags.Public,
								new MemberFilter(this.IsMemberEligibleForMacroCall), subex.Trim() );
							string[] arglist=null;

							if ( members.Length == 0 )
							{
								throw new MissingMemberException(subexObject.GetType().FullName,subex.Trim());
							}
							if ( subexEndIndex<expression.Length && (expression[subexEndIndex] == '[' || expression[subexEndIndex] == '(') )
							{
								arglist = SplitArgs(expression,subexEndIndex, ref subexStartIndex);
							}

							//SDH: We REALLY need to refactor this whole Clemens thing - it's getting hairy.
							memberToInvoke = null;
							if ( members.Length > 1 )
							{
								foreach(MemberInfo potentialMember in members)
								{
									MethodInfo potentialMethod = potentialMember as MethodInfo;
									if(potentialMethod != null)
									{
										ParameterInfo[] parameters = potentialMethod.GetParameters();
										if(parameters != null && parameters.Length > 0)
										{
											if(parameters.Length == arglist.Length)
											{
												memberToInvoke = potentialMember;
												break;
											}
										}
									}
								}
							}

							if(memberToInvoke == null)//Previous behavior, use the first one.
							{
								memberToInvoke = members[0];
							}


							if ( memberToInvoke.MemberType == MemberTypes.Property &&
								(subexEndIndex==expression.Length ||
								expression[subexEndIndex] == '.' ||
								expression[subexEndIndex] == '[' ))
							{
								PropertyInfo propInfo = memberToInvoke as PropertyInfo;
								if ( subexEndIndex<expression.Length && expression[subexEndIndex] == '[' )
								{
									System.Reflection.ParameterInfo[] paramInfo = propInfo.GetIndexParameters();
									if ( arglist.Length > paramInfo.Length )
									{
										throw new InvalidOperationException(String.Format("Parameter list length mismatch {0}",memberToInvoke.Name));
									}
									object[] oarglist = new object[paramInfo.Length];
									for( int n=0;n<arglist.Length;n++)
									{
										oarglist[n] = Convert.ChangeType(arglist[n],paramInfo[n].ParameterType);
									}
									CachedMacro macro = new CachedMacro(propInfo,oarglist);
									macros[cacheKey] = macro;
									subexObject = macro.Invoke(subexObject);
									//subexObject = propInfo.GetValue(subexObject,oarglist);
								}
								else
								{
									CachedMacro macro = new CachedMacro(propInfo,null);
									macros[cacheKey] = macro;
									subexObject = macro.Invoke(subexObject);
									//subexObject = propInfo.GetValue(subexObject,null);
								}
							}
							else if ( memberToInvoke.MemberType == MemberTypes.Field &&
								(subexEndIndex==expression.Length ||
								expression[subexEndIndex] == '.'))
							{
								FieldInfo fieldInfo = memberToInvoke as FieldInfo;
			
								CachedMacro macro = new CachedMacro(fieldInfo);
								macros[cacheKey] = macro;
								subexObject = macro.Invoke(subexObject);
								//subexObject = fieldInfo.GetValue(subexObject);
							}
							else if ( memberToInvoke.MemberType == MemberTypes.Method &&
								subexEndIndex<expression.Length && expression[subexEndIndex] == '(' )
							{
								MethodInfo methInfo = memberToInvoke as MethodInfo;
								System.Reflection.ParameterInfo[] paramInfo = methInfo.GetParameters();
								if ( arglist.Length > paramInfo.Length && 
									!(paramInfo.Length>0 && paramInfo[paramInfo.Length-1].ParameterType == typeof(string[])))
								{
									throw new InvalidOperationException(String.Format("Parameter list length mismatch {0}",memberToInvoke.Name));
								}
								object[] oarglist = new object[paramInfo.Length];
								for( int n=0;n<arglist.Length;n++)
								{
									if ( n == paramInfo.Length-1 && 
										arglist.Length>paramInfo.Length)
									{
										string[] paramsArg = new string[arglist.Length-paramInfo.Length+1];
										for( int m=n;m<arglist.Length;m++)
										{
											paramsArg[m-n] = Convert.ChangeType(arglist[n],typeof(string)) as string;
										}
										oarglist[n] = paramsArg;
										break;
									}
									else
									{
										oarglist[n] = Convert.ChangeType(arglist[n],paramInfo[n].ParameterType);
									}
								}

								CachedMacro macro = new CachedMacro(methInfo,oarglist);
								macros[cacheKey] = macro;
								subexObject = macro.Invoke(subexObject);
								//subexObject = methInfo.Invoke(subexObject,oarglist);
							}
						}
					}
					else
					{
						subexStartIndex = subexEndIndex+1;
						subexObject = subCached.Invoke(subexObject);
						if (subexObject is Control) 
						{
							return subexObject;
						}
					}
				}
				while(subexEndIndex<expression.Length && subexStartIndex < expression.Length);
				
				return subexObject==null?new LiteralControl(""):subexObject;
			}
			catch( Exception exc )
			{
				ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
				throw;
			}
		}

	}
}
