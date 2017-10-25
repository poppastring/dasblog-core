<%@ Page language="c#" %>
<html>
<head>
<title>Color Picker</title>
<style>
.cc {
	width:10;height:8;
}
</style>
<script launguage="JavaScript">
function colorOver(theTD) {
	previewColor(theTD.style.backgroundColor);
	setTextField(theTD.style.backgroundColor);
}
function colorClick(theTD) {
	setTextField(theTD.style.backgroundColor);
	returnColor(theTD.style.backgroundColor);
}
function setTextField(ColorString) {
	document.getElementById("ColorText").value = ColorString.toUpperCase();
}
function returnColor(ColorString) {
	window.returnValue = ColorString;
	window.close();	
}		
function userInput(theinput) {
	previewColor(theinput.value);
}
function previewColor(theColor) {
	try {
		PreviewDiv.style.backgroundColor = theColor;
	} catch (e) {
	}
}
</script>		
</head>

<body style="background-color:d4d0c8; margin: 2 2 2 2;">
<form runat="server">

<table cellpadding=0 cellspacing=0 border=0>
<tr><td colspan=3>
	<asp:Literal id="Colors" 
		runat="server" EnableViewState="false" />
</td></tr>
<tr>	
	<td><input type="text" name="ColorText" id="ColorText" style="width:60;height:22;" onkeyup="userInput(this);"></td>
	<td align=center><div id="PreviewDiv" style="width:50;height:20;border: 1 solid black; background-color: #ffffff;">&nbsp;</div></td>
	<td align=right><input type="button" value="Use Color" onclick="returnColor(ColorText.value);" id="ColorButton"  style="width:80;"></td>
</tr>
</table>

</form>
</body>
</html>
<script language="c#" runat="server">
private void Page_Load(object sender, System.EventArgs e) {
	// Change to support contributors as well as admins.
	if (!newtelligence.DasBlog.Web.SiteSecurity.IsValidContributor()) 
    {
		Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
    }
    else
    {
	int onrow=1;
	int oncol=1;
	int count=1;
	string s ="";
	string set1 = "";
	string set2 = "";
	for (int r=5; r>=0; r--) {
		for (int g=5; g>=0; g--) {
			for (int b=5; b>=0; b--) {
				if (oncol==1) s += "<tr>\n";
				s += "\t<td onmouseover=\"colorOver(this);\" onclick=\"colorClick(this);\" style=\"background-color:"+ReturnHex(r)+ReturnHex(g)+ReturnHex(b)+";\" class=cc></td>\n";
				oncol++;
				if (oncol >= 19) {
					s+="</tr>\n";
					oncol = 1;
					
					if (onrow % 2 == 0) {
						set2 += s;
					} else {
						set1 += s;
					}
					s = "";
					onrow++;
				}				
				count++;
			}
		}
	}
	Colors.Text = "<table cellpadding=0 cellspacing=1 style=\"background-color:ffffff;\" border=0 >" + set1 + set2 + "</table>";
	
}
}

string ReturnHex(int i) {
	switch (i) {
		default:
		case 0:
			return "00";
		case 1:
			return "33";
		case 2:
			return "66";
		case 3:
			return "99";			
		case 4:
			return "CC";
		case 5:
			return "FF";			
	}
}
</script>
