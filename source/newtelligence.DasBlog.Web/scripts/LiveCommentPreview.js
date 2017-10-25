/*
Adds Live Comment Preview for comment forms.

USAGE: Simply add the css class "livepreview" to the textarea that
is the source of the comment and to the <div> element that
will display the live preview.

Note that livepreview does not have to be the only css class.

ex... Make this edit in PostComment.ascx

<asp:TextBox id="tbComment" runat="server" Rows="10" Columns="40" width="100%" Height="193px"
					TextMode="MultiLine" class="livepreview"></asp:TextBox>

<div class="comment livepreview"></div>

Original Source for this JS taken from the Subtext Project:
///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
//
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at SourceForge at http://sourceforge.net/projects/subtext
// The development mailing list is at subtext-devs@lists.sourceforge.net
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////
*/

var commentLivePreview =
{
	livePreviewClass: "livepreview",
	allowedTags: allowedHtmlTags,
	allowedTagsRegExp: null,
	previewElement: null,
	paraRegExp: new RegExp("(.*)\n\n([^#*\n\n].*)", "g"),
	lineBreakRegExp: new RegExp("(.*)\n([^#*\n].*)", "g"),
	updatingPreview: false,

	init: function()
	{
		if (!document.getElementsByTagName) { return; }

		var divs = document.getElementsByTagName("div");
		commentLivePreview.previewElement = commentLivePreview.getPreviewDisplayElement(divs);
		if(!commentLivePreview.previewElement) { return; }

		var textareas = document.getElementsByTagName("textarea");
		var tagNamesRegex = "";

		for(var i = 0; i < commentLivePreview.allowedTags.length; i++)
		{
			tagNamesRegex += commentLivePreview.allowedTags[i] + "|";
		}

		if(tagNamesRegex.length > 0)
		{
			tagNamesRegex = tagNamesRegex.substring(0, tagNamesRegex.length - 1);
		}

		commentLivePreview.allowedTagsRegExp = new RegExp("&lt;(/?(" + tagNamesRegex + ")(\\s+.*?)?)&gt;", "g");

		// Loop through all input tags.
		var textarea;
		for (i = 0; i < textareas.length; i++)
		{
			textarea = textareas[i];
			if (commentLivePreview.getClassName(textarea).indexOf(commentLivePreview.livePreviewClass) >= 0)
			{
				textarea.onkeyup = function()
				{
					// Subject to race condition. But it's not a big deal. The next keypress
					// will solve it. Worst case is the preview is off by the last char in rare
					// situations.
					if(!commentLivePreview.updatingPreview)
					{
						commentLivePreview.updatingPreview = true;
						window.setTimeout("commentLivePreview.reloadPreview('" + this.id + "')", 20);
					}
					return false;
				};

				commentLivePreview.reloadPreview(textarea.id);
			}
		}
	},

	// Returns the html element responsible for previewing
	// comments.
	getPreviewDisplayElement: function(elements)
	{
		var element;

		for (var i = 0; i < elements.length; i++)
		{
			element = elements[i];

			if (commentLivePreview.getClassName(element).indexOf(commentLivePreview.livePreviewClass) >= 0)
			{
				return element;
			}
		}
	},

	getClassName: function(element)
	{
		if(element.getAttribute && element.getAttribute("class"))
		{
			return element.getAttribute("class");
		}
		else if(element.className)
		{
			return element.className;
		}
		return "";
	},

	reloadPreview: function(textareaId)
	{
		var textarea = document.getElementById(textareaId);
		var previewString = textarea.value;

		if (previewString.length > 0)
		{
			previewString = commentLivePreview.htmlUnencode(previewString);
			previewString = previewString.replace(commentLivePreview.paraRegExp, "<p>$1</p><p>$2</p>");
			previewString = previewString.replace(commentLivePreview.lineBreakRegExp, "$1<br />$2");
			previewString = previewString.replace(commentLivePreview.allowedTagsRegExp, "<$1>");
		}
		try
		{
			commentLivePreview.previewElement.innerHTML = previewString;
		}
		catch(e)
		{
			alert("Sorry, but inserting a block element within is not allowed here.");
		}
		commentLivePreview.updatingPreview = false;
	},

	htmlUnencode: function(s)
	{
		return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
	},

	addEvent: function(obj, eventTypes, callback)
	{
		var eventType;
		for(var i = 0; i < eventTypes.length; i++)
		{
			eventType = eventTypes[i];

			if (obj.attachEvent)
			{
				obj["e" + eventType + callback] = callback;
				obj[eventType + callback] = function()
				{
					obj["e" + eventType + callback](window.event);
				};
				obj.attachEvent("on" + eventType, obj[eventType + callback]);
			}
			else
			{
				obj.addEventListener(eventType, callback, false);
			}
		}
	}
};


commentLivePreview.addEvent(window, ["load"], commentLivePreview.init);