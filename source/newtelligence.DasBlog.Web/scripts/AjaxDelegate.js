<!--
/*
AjaxDelegate(url, callback)
	url = the url of the page that will do the server-side processing
	callback = the name of the function to call once the call has completed
	
	Any number of additional arguments can also be specified. The extra 
	arguments will be available to the callback function when it is called.
	
	The callback function will receive the following arguments:
		callback(url, response, [argument[0]], etc.)
	where:
		url = the url of the page that did the server-side processing
		response = the actual HTTP responseText returned from the call
		[argument[0]], etc. = the remaining arguments that were originally passed to the AjaxDelegate constructor
*/
function AjaxDelegate(url, callback, entryId, title, author, postText)
{
	// basic properties
	this.url = url;
	this.callback = callback;
	
	// Justice: This is a bit of weirdness that I'm not sure how to get around.
	// If I simply assign arguments to this.callbackArguments, the very next time I refer to this.callbackArguments
	// I receive a javascript exception.  Hence why I am doing this manually.
	// TODO:  Add the title and author as parameters.
	this.callbackArguments = new Array();
	this.callbackArguments[0] = url;  
	this.callbackArguments[1] = callback;  
	this.callbackArguments[2] = entryId;  
	// Note: if someone enters the "]]>" sequence into the title, then the autosave breaks.  To get around
	// this, we simply escape any right angle brackets (for now) and put them back in the handler.
	this.callbackArguments[3] = title.replace(/]]>/g, "]]&gt;");  
	this.callbackArguments[4] = author;  
	
	// See above regarding the "]]>" sequence.  The FreeTextBox already escapes this for us if typing it in the
	// normal view.  However, if someone punched it into the HTML view (or the simpler textbox), it would kill
	// the autosave. So any occurence of "]]>" gets escaped out to look like "]]&gt;".  Frankly,
	// the only situation I can see this happening is either a typo or a deliberate attempt to break, but...	
	this.callbackArguments[5] = postText.replace(/]]>/, "]]&gt;");  
	
	// methods
	this.Fetch = ajaxFetch;

	// XmlHttpRequest object
	this.request = null;
}

/*
ajaxFetch()
	Asynchronously calls the url specified in the AjaxDelegate constructor.
	When the call completes, the callback specified in the AjaxDelegate constructor
	is called, passing the responseText data in.
*/
function ajaxFetch()
{
	// this gets the variables into a local scope
    var request = this.request;
    var callback = this.callback;
    var callbackArguments = this.callbackArguments;

    // branch for native XMLHttpRequest object
    if(window.XMLHttpRequest) {
    	try {
			request = new XMLHttpRequest();
        } catch(e) {
			request = null;
        }
    // branch for IE/Windows ActiveX version
    } else if(window.ActiveXObject) {
       	try {
        	request = new ActiveXObject("Msxml2.XMLHTTP");
      	} catch(e) {
        	try {
          		request = new ActiveXObject("Microsoft.XMLHTTP");
        	} catch(e) {
          		request = null;
        	}
		}
    }

	// if we were able to create the XmlHTTPRequest object, we can make the request
	if(request) {
		request.onreadystatechange = function () {
										if(request.readyState == 4) {
											if(request.status == 200) {
												// we replace the second argument (callback function) with the response data)
												// kind of cheesy, but it is nice and easy and works well
												callbackArguments[1] = request.responseText;
												callback.apply(this, callbackArguments);
											} else {
												
												// If an error pops up here, we don't want the user being disturbed every minute.
												// alert("There was a problem retrieving the XML data:\n" + request.statusText);
											}

											// clean up
											request = null;
										}
									}
		// Small modification to ajax delegate in order to throw the text content and entry id into the form values.
		
		// Throw the data into an XML fragment for the AutoSaveEntryHandler to read.
		// alert("A debug delay.");
		var xmlSubmit = "<savedpostdata><entryid>"+callbackArguments[2]+"</entryid>";
		xmlSubmit += "<title><![CDATA["+callbackArguments[3]+"]]></title>";
		xmlSubmit += "<author>"+callbackArguments[4]+"</author>";
		xmlSubmit += "<posttext><![CDATA["+callbackArguments[5]+"]]></posttext>";
		xmlSubmit += "</savedpostdata>";				
		request.open("POST", this.url, true);
		request.setRequestHeader('Content-Type', 'text/xml');
		request.send(xmlSubmit);
	}
}
//-->
