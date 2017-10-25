~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
BDP Lite v1.2  
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Welcome DasBlog users!

BDP Lite is a professional cross-browser ASP.NET date picker calendar web control. This quick loading DHTML calendar enables rapid, error free, date selection without the use of pop-up windows. Save development time and optimize user experience with this highly fexible, customizable ASP.NET control.

Please visit http://www.basicdatepicker.com/ for more information.

NOTE: BDP Lite can be included and redistributed within your application. Please read the License Agreement for complete details. See http://www.basicdatepicker.com/support/licenseagreement.aspx



~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Samples
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Samples demostrating the use of BDP Lite are available online at http://www.basicdatepicker.com/samples/



~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Questions & Answers 
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Q. How do I get/set the Date in the TextBox?
A. Think of the date picker as a Calendar and not as a TextBox. The control accepts and returns a .NET DateTime object. Example:

[Visual Basic]
Dim myDate As Date = BDPLite1.SelectedDate 'Get SelectedDate
BDPLite1.SelectedDate = Date.Today 'Set SelectedDate


[C#]
DateTime myDate = BDPLite1.SelectedDate; //Get SelectedDate
BDPLite1.SelectedDate = DateTime.Today; //Set SelectedDate


Q. How do I change the size of the TextBox?
A. Set the .TextBoxColumns property. The .TextBoxColumns property maps directly 'size' attribute of the input tag Example:

[Design Mode]
<BDP:BDPLite id="BDPLite1" runat="server" TextBoxColumns="15" />

[Visual Basic]
BDPLite1.TextBoxColumns = 15

[C#]
BDPLite1.TextBoxColumns = 15;


Q. How do I clear the date from the TextBox?
A. Set the .SelectedDate to DateTime.MinValue. There is no 'Null' or 'Nothing' value for a DateTime object in .NET, so DateTime.MinValue is used to represent a Null. Example:


[Visual Basic]
BDPLite1.SelectedDate = Date.MinValue 'Clear the TextBox


[C#]
BDPLite1.SelectedDate = DateTime.MinValue; //Clear the TextBox


Q. I need to dispaly a different date format. For example "M/d/yyyy". How do I do that?
A. The .DateFormat property provides three date format options. Example:

Default = "d-MMM-yyy" // Outputs "1-Feb-2005" for en-US Culture
LongDate = Culture.DateTimeFormat.LongDatePattern // Outputs "February 1, 2005" for en-US Culture 
ShortDate = Culture.DateTimeFormat.ShortDatePattern // Outputs "2/1/2005 for en-US Culture. 

[Visual Basic]
BDPLite1.SelectedDate = Date.Today
BDPLite1.DateFormat = BasicFrame.WebControls.DateFormat.ShortDate
Response.Write(BDPLite1.SelectedDateFormatted)

'This code produces the following output for 1-Dec-2004.
'
'12/1/2004


[C#]
BDPLite1.SelectedDate = DateTime.Today;
BDPLite1.DateFormat = BasicFrame.WebControls..ShortDate;
Response.Write(BDPLite1.SelectedDateFormatted);

/* 
This code produces the following output for 1-Dec-2004.

12/1/2004
*/



~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Support  
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Extensive online support available at http://www.basicdatepicker.com/support/ or send support request to support@basicdatepicker.com.

Documentation
http://www.basicdatepicker.com/documentation/

Support Forums
http://forums.basicdatepicker.com/



~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Other controls 
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
If you like BDP Lite, you might also be interested in the full version. Please visit the BasicDatePicker website for more details. See http://www.basicdatepicker.com/


Copyright 2004-2005 Basic Frame Inc., All rights reserved.

Basic Frame Inc.
    #218, 10113-104 Street
    Edmonton, Alberta, Canada    T5J 1A1
    Phone: 780-428-2600
    Fax: 780-426-3450
    Website: http://www.basicdatepicker.com 
    Email: support@basicdatepicker.com