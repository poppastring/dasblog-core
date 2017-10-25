Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections
Imports System.Collections.Specialized

''' <summary>
'''
''' Captcha ASP.NET user control
'''
''' add a reference to this DLL and add the CaptchaControl to your toolbox;
''' then just drag and drop the control on a web form and set properties on it.
'''
''' Jeff Atwood
''' http://www.codinghorror.com/
'''
''' </summary>
<DefaultProperty("Text")> _
Public Class CaptchaControl
    Inherits System.Web.UI.WebControls.WebControl
    Implements INamingContainer
    Implements IPostBackDataHandler

    Public Enum Layout
        Horizontal
        Vertical
    End Enum

    Private _intTimeoutSeconds As Integer = 0
    Private _blnUserValidated As Boolean = False
    Private _blnShowSubmitButton As Boolean = True
    Private _strText As String = "Enter the code shown above:"
    Private _strFont As String = ""
    Private _captcha As New CaptchaImage
    Private _LayoutStyle As Layout = Layout.Horizontal

    Public Event UserValidationEvent(ByVal Validated As Boolean)

    <DefaultValue("Enter the code shown above:"), Description("Instructional text displayed next to CAPTCHA image."), Category("Captcha")> _
    Public Property [Text]() As String
        Get
            Return _strText
        End Get
        Set(ByVal Value As String)
            _strText = Value
        End Set
    End Property

    <DefaultValue(GetType(CaptchaControl.Layout), "Horizontal"), Description("Determines if image and input area are displayed horizontally, or vertically."), Category("Captcha")> _
    Public Property LayoutStyle() As Layout
        Get
            Return _LayoutStyle
        End Get
        Set(ByVal Value As Layout)
            _LayoutStyle = Value
        End Set
    End Property

    <Description("Returns True if the user was CAPTCHA validated after a postback."), Category("Captcha")> _
    Public ReadOnly Property UserValidated() As Boolean
        Get
            Return _blnUserValidated
        End Get
    End Property

    <DefaultValue(True), Description("Show a Submit button in the control to enable postback."), Category("Captcha")> _
    Public Property ShowSubmitButton() As Boolean
        Get
            Return _blnShowSubmitButton
        End Get
        Set(ByVal Value As Boolean)
            _blnShowSubmitButton = Value
        End Set
    End Property

    <DefaultValue(""), Description("Font used to render CAPTCHA text. If font name is blankd, a random font will be chosen."), Category("Captcha")> _
    Public Property CaptchaFont() As String
        Get
            Return _strFont
        End Get
        Set(ByVal Value As String)
            _strFont = Value
            _captcha.Font = _strFont
        End Set
    End Property

    <DefaultValue(""), Description("Characters used to render CAPTCHA text. A character will be picked randomly from the string."), Category("Captcha")> _
    Public Property CaptchaChars() As String
        Get
            Return _captcha.TextChars
        End Get
        Set(ByVal Value As String)
            _captcha.TextChars = Value
        End Set
    End Property

    <DefaultValue(5), Description("Number of CaptchaChars used in the CAPTCHA text"), Category("Captcha")> _
    Public Property CaptchaLength() As Integer
        Get
            Return _captcha.TextLength
        End Get
        Set(ByVal Value As Integer)
            _captcha.TextLength = Value
        End Set
    End Property

    <DefaultValue(0), Description("Number of seconds this CAPTCHA is valid after it is generated. Zero means valid forever."), Category("Captcha")> _
    Public Property CaptchaTimeout() As Integer
        Get
            Return _intTimeoutSeconds
        End Get
        Set(ByVal Value As Integer)
            _intTimeoutSeconds = Value
        End Set
    End Property

    <DefaultValue(GetType(CaptchaImage.FontWarpFactor), "Low"), Description("Amount of random font warping used on the CAPTCHA text"), Category("Captcha")> _
    Public Property CaptchaFontWarping() As CaptchaImage.FontWarpFactor
        Get
            Return _captcha.FontWarp
        End Get
        Set(ByVal Value As CaptchaImage.FontWarpFactor)
            _captcha.FontWarp = Value
        End Set
    End Property

    '-- viewstate is required, so let's hide this property
    <Browsable(False)> _
    Public Overrides Property EnableViewState() As Boolean
        Get
            Return MyBase.EnableViewState
        End Get
        Set(ByVal Value As Boolean)
            MyBase.EnableViewState = Value
        End Set
    End Property

    ''' <summary>
    ''' randomly generated captcha text is stored in the session 
    ''' using the control ID as a unique identifier
    ''' </summary>
    Private Property CaptchaText() As String
        Get
            If HttpContext.Current.Session(Me.ID & ".String") Is Nothing Then
                Return ""
            Else
                Return Convert.ToString(HttpContext.Current.Session(Me.ID & ".String"))
            End If
        End Get
        Set(ByVal Value As String)
            If Value = "" Then
                HttpContext.Current.Session.Remove(Me.ID & ".String")
            Else
                HttpContext.Current.Session(Me.ID & ".String") = Value
            End If
        End Set
    End Property

    ''' <summary>
    ''' date and time of Captcha generation is stored in the Session 
    ''' using the control ID as a unique tag
    ''' </summary>
    Private Property GeneratedAt() As Nullable(Of DateTime)
        Get
            If HttpContext.Current.Session(Me.ID & ".date") Is Nothing Then
                Return Nothing
            Else
                Return Convert.ToDateTime(HttpContext.Current.Session(Me.ID & ".date"))
            End If
        End Get
        Set(ByVal Value As Nullable(Of DateTime))
            If Value.HasValue = False Then
                HttpContext.Current.Session.Remove(Me.ID & ".date")
            Else
                HttpContext.Current.Session(Me.ID & ".date") = Value
            End If
        End Set
    End Property

    ''' <summary>
    ''' guid used to identify the unique captcha image is stored in the page ViewState
    ''' </summary>
    Private Property LocalGuid() As String
        Get
            Return Convert.ToString(ViewState("guid"))
        End Get
        Set(ByVal Value As String)
            ViewState("guid") = Value
        End Set
    End Property

    ''' <summary>
    ''' are we in design mode?
    ''' </summary>
    Private ReadOnly Property IsDesignMode() As Boolean
        Get
            Return HttpContext.Current Is Nothing
        End Get
    End Property

    ''' <summary>
    ''' Returns true if user input valid CAPTCHA text, and raises UserValidationEvent
    ''' </summary>
    Private Function ValidateCaptcha(ByVal strUserEntry As String) As Boolean
        If String.Compare(strUserEntry, Me.CaptchaText, True) = 0 Then
            If Me.CaptchaTimeout = 0 Then
                _blnUserValidated = True
            Else
                '-- ok, it's valid, but was it entered quickly enough?
                _blnUserValidated = (Me.GeneratedAt.HasValue) AndAlso Me.GeneratedAt.Value.AddSeconds(Me.CaptchaTimeout) > Now
            End If
        Else
            _blnUserValidated = False
        End If
        RaiseEvent UserValidationEvent(_blnUserValidated)
    End Function

    Public Function LoadPostData(ByVal PostDataKey As String, ByVal Values As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
        ValidateCaptcha(Convert.ToString(Values(Me.UniqueID)))
        GenerateNewCaptcha()
        Return False
    End Function

    Public Sub RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
        '-- Part of the IPostBackDataHandler contract.  Invoked if we ever returned true from the
        '-- LoadPostData method (indicates that we want a change notification raised).  Since we
        '-- always return false from LoadPostData, this method is just a no-op.
    End Sub

    ''' <summary>
    ''' returns HTML-ized color strings
    ''' </summary>
    Private Function HtmlColor(ByVal color As Drawing.Color) As String
        If color.IsEmpty Then Return ""
        If color.IsNamedColor Then
            Return color.ToKnownColor.ToString
        End If
        If color.IsSystemColor Then
            Return color.ToString
        End If
        Return "#" & color.ToArgb.ToString("x").Substring(2)
    End Function

    ''' <summary>
    ''' returns css "style=" tag for this control
    ''' based on standard control visual properties
    ''' </summary>
    Private Function CssStyle() As String
        Dim sb As New System.Text.StringBuilder
        Dim strColor As String

        With sb
            .Append(" style='")

            If BorderWidth.ToString.Length > 0 Then
                .Append("border-width:")
                .Append(BorderWidth.ToString)
                .Append(";")
            End If
            If BorderStyle <> WebControls.BorderStyle.NotSet Then
                .Append("border-style:")
                .Append(BorderStyle.ToString)
                .Append(";")
            End If
            strColor = HtmlColor(BorderColor)
            If strColor.Length > 0 Then
                .Append("border-color:")
                .Append(strColor)
                .Append(";")
            End If

            strColor = HtmlColor(BackColor)
            If strColor.Length > 0 Then
                .Append("background-color:" & strColor & ";")
            End If

            strColor = HtmlColor(ForeColor)
            If strColor.Length > 0 Then
                .Append("color:" & strColor & ";")
            End If

            If Font.Bold Then
                .Append("font-weight:bold;")
            End If

            If Font.Italic Then
                .Append("font-style:italic;")
            End If

            If Font.Underline Then
                .Append("text-decoration:underline;")
            End If

            If Font.Strikeout Then
                .Append("text-decoration:line-through;")
            End If

            If Font.Overline Then
                .Append("text-decoration:overline;")
            End If

            If Font.Size.ToString.Length > 0 Then
                .Append("font-size:" & Font.Size.ToString & ";")
            End If

            If Font.Names.Length > 0 Then
                Dim strFontFamily As String
                .Append("font-family:")
                For Each strFontFamily In Font.Names
                    .Append(strFontFamily)
                    .Append(",")
                Next
                .Length = .Length - 1
                .Append(";")
            End If

            If Height.ToString <> "" Then
                .Append("height:" & Height.ToString & ";")
            End If
            If Width.ToString <> "" Then
                .Append("width:" & Width.ToString & ";")
            End If

            .Append("'")
        End With
        If sb.ToString = " style=''" Then
            Return ""
        Else
            Return sb.ToString
        End If
    End Function

    ''' <summary>
    ''' render raw control HTML to the page
    ''' </summary>
    Protected Overrides Sub Render(ByVal Output As HtmlTextWriter)
        With Output
            '-- master DIV
            .Write("<div")
            If CssClass <> "" Then
                .Write(" class='" & CssClass & "'")
            End If
            .Write(CssStyle)
            .Write(">")

            '-- image DIV/SPAN
            If Me.LayoutStyle = Layout.Vertical Then
                .Write("<div style='text-align:center;margin:5px;'>")
            Else
                .Write("<span style='margin:5px;float:left;'>")
            End If
            '-- this is the URL that triggers the CaptchaImageHandler
            .Write("<img src=""CaptchaImage.aspx")
            If Not IsDesignMode Then
                .Write("?guid=" & Convert.ToString(ViewState("guid")))
            End If
            .Write(""" border='0' alt=""[Captcha]""")
            If ToolTip.Length > 0 Then
                .Write(" alt='" & ToolTip & "'")
            End If
            .Write(" />")
            If Me.LayoutStyle = Layout.Vertical Then
                .Write("</div>")
            Else
                .Write("</span>")
            End If

            '-- text input and submit button DIV/SPAN
            If Me.LayoutStyle = Layout.Vertical Then
                .Write("<div style='text-align:center;margin:5px;'>")
            Else
                .Write("<span style='margin:5px;float:left;'>")
            End If
            If _strText.Length > 0 Then
                .Write(_strText)
                .Write("<br />")
            End If
            .Write("<input name=""" & UniqueID & """ type=""text"" size=""")
            .Write(_captcha.TextLength.ToString)
            .Write(""" maxlength=""")
            .Write(_captcha.TextLength.ToString)
            If AccessKey.Length > 0 Then
                .Write(""" accesskey=""" & AccessKey)
            End If
            If Not Enabled Then
                .Write(""" disabled=""disabled""")
            End If
            If TabIndex > 0 Then
                .Write(""" tabindex=""" & TabIndex.ToString)
            End If
            .Write(""" value='' />")
            If _blnShowSubmitButton Then
                .Write("&nbsp;")
                .Write("<input type=""Submit"" value=""Submit""")
                If Not Enabled Then
                    .Write(" disabled=""disabled""")
                End If
                If TabIndex > 0 Then
                    .Write(" tabindex=""" & TabIndex.ToString & """")
                End If
                .Write(" />")
            End If
            If Me.LayoutStyle = Layout.Vertical Then
                .Write("</div>")
            Else
                .Write("</span>")
                .Write("<br clear='all' />")
            End If

            '-- closing tag for master DIV
            .Write("</div>")
        End With
    End Sub

    ''' <summary>
    ''' generate a new captcha and store it in the ASP.NET Cache by unique GUID
    ''' </summary>
    Private Sub GenerateNewCaptcha()
        LocalGuid = Guid.NewGuid.ToString
        If Not IsDesignMode Then
            'HttpContext.Current.Cache.Add(LocalGuid, _captcha, Nothing, DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout), TimeSpan.Zero, Caching.CacheItemPriority.Normal, Nothing)
            'No reason to keep the captcha in the cache very long.
            HttpContext.Current.Cache.Add(LocalGuid, _captcha, Nothing, DateTime.Now.AddMinutes(1), TimeSpan.Zero, Caching.CacheItemPriority.High, Nothing)
        End If
        Me.CaptchaText = _captcha.Text
        Me.GeneratedAt = Now
    End Sub

    Protected Overrides Sub OnPreRender(ByVal E As EventArgs)
        If LocalGuid = "" Then GenerateNewCaptcha()
    End Sub

End Class