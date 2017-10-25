<%@ Page language="c#" ValidateRequest="false" %>
<!doctype html public "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Insert Code</title>
		<base target="_self">
		<script language="C#" runat="server">

private void Page_Load(object sender, System.EventArgs e) {
	if (!newtelligence.DasBlog.Web.SiteSecurity.IsValidContributor()) 
    {
		Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
    }
}
		</script>
		<style type="text/css">
		.hiddenText {
			display: none;
		}
		</style>
		<META HTTP-EQUIV="Expires" CONTENT="0">
	</HEAD>
	<body style="PADDING-RIGHT:10px; PADDING-LEFT:10px; PADDING-BOTTOM:10px; PADDING-TOP:10px">
		<form id="Form1" runat="server">
			<asp:dropdownlist id="languageDropDown" runat="server">
				<asp:listitem value="C#">C#</asp:listitem>
				<asp:listitem value="VB.NET">VB.NET</asp:listitem>
				<asp:listitem value="J#">J#</asp:listitem>
				<asp:listitem value="T-SQL">T-SQL</asp:listitem>
				<asp:listitem value="JScript">JScript</asp:listitem>
			</asp:dropdownlist>
			<br>
			<asp:textbox id="sourceTextBox" textmode="multiline" columns="60" rows="10" runat="server" Width="100%" />
			<br>
			<asp:button id="parseButton" onclick="parseButton_Click" text="Parse Code" runat="server" />
			<INPUT id="Button1" type="button" onclick="returnCode()" value="Insert Code" name="Button1"
				runat="server">&nbsp;
			<asp:Checkbox runat="server" id="chkLeftJustify" text="Left Justify"></asp:Checkbox>
			<hr>
			<pre style="FONT-SIZE: 11px; FONT-FAMILY: Courier New"><asp:literal id="resultLabel" runat="server"/>
<asp:TextBox id=codeText runat="server" Width="0px" Height="0px" CssClass="hiddenText" TextMode="MultiLine"></asp:TextBox>
		</form>
		<script language="javascript">
function returnCode() {
	var text;
	
	text = document.getElementById('codeText').value;
	if (window.opener) {
		window.opener.addTextToPost(text);
	} else {
		window.returnValue = text;
	}


	window.close();	
}		
		</script>
		<script language="c#" runat="server">
            void parseButton_Click(object sender, EventArgs e)
            {
	            string result;
	            result = sourceTextBox.Text;
            	
                //-- If the user selected to left justify
                //-- then apply it here.	
	            if (chkLeftJustify.Checked)
	            {
		            result = LeftJustifyCode(result);
	            }
            	
	            sourceTextBox.Text=result;
            	
	            AylarSolutions.Highlight.Highlighter h = new AylarSolutions.Highlight.Highlighter();
	            h.ConfigurationFile = Server.MapPath("CodeHighlightDefinitions.xml");
	            h.OutputType = AylarSolutions.Highlight.OutputType.Html;
	            result = h.Highlight(result, languageDropDown.SelectedValue);
	            result = result.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");

	            resultLabel.Text = result;
	            codeText.Text = "<pre>" + result + "</pre>";

            }

		    /// <summary>
		    /// Removes white space from the left of a block of
            /// code w/o removing formatting tabs.
		    /// </summary>
	    	/// <param name="result">string to be formatted.</param>
    		/// <returns></returns>    
            String LeftJustifyCode(String result)
            {
                // replace tabs with 4 spaces
                result = Regex.Replace(result, "\t", "    ");

                //-- Break the string into lines
	            ////String[] delim ={Environment.NewLine};            
	            char[] delim = Environment.NewLine.ToCharArray();
	            
                // System.StringSplitOptions is not supported in the 1.1 Framework
	            ////String[] lines = result.Split(delim,System.StringSplitOptions.RemoveEmptyEntries);
	            String[] tempLines = result.Split(delim);

                // convert the array into an ArrayList	            
	            System.Collections.ArrayList lineList = new System.Collections.ArrayList();
	            
	            foreach (String line in tempLines)
	            {
                    // only add nonblank lines to the list
					if (line.Trim().Length > 0)
					{
						lineList.Add(line);
					}
	            }

                // convert the ArrayList back into an array                	            
	            String[] lines = (String[])lineList.ToArray(typeof(String));
	            
                //-- we need to find the min space at the start of any of the lines.
	            int minSpacing = 100; // start at 100, work our way back.
                //-- loop through the lines
	            for (int i = 0; i < lines.Length ; i++)
	            {
		            int tempSpacing=0;
		            int charVal=0;
                    //-- loop through the characters in this line
		            for (int j = 0; j < lines[i].Length; j++)
		            {
						charVal = (int)lines[i].Substring(j,1)[0];  // use the ASCII code of the character
						if (charVal == 32) // space
			            {
				            tempSpacing++;
			            }
			            else
			            {
				            break;
			            }						
		            }
                    //-- if the spacing is less, then use it.
		            if (tempSpacing < minSpacing)
		            {
			            minSpacing = tempSpacing;
		            }	
	            }
            	//-- strip away the extra spaces 
	            if (minSpacing > 0){
                    //-- create a string we can replace
		            System.Text.StringBuilder sb = new System.Text.StringBuilder();
		            sb.Append(Environment.NewLine);
                    
                    //for (int j = 0;j<minSpacing;j++)
                    //{
                    //    sb.Append(" ");
                    //}
                    sb.Append(new string(' ', minSpacing - 1));  // refactored the loop
                    
                    //-- remove the space from the first line
		            result = result.Substring(minSpacing);
                    //-- remove the space from the rest of the lines
		            result = result.Replace((String)sb.ToString(),(String)Environment.NewLine);
	            }
	        return result;
            }
		</script>
	</body>
</HTML>
