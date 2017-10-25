Imports System
Imports System.Web
Imports System.Drawing

''' <summary>
''' 
''' Captcha image stream HttpModule
''' allows us to generate images in memory and stream them to the browser
'''
''' You *MUST* enable this HttpHandler in your web.config, like so:
'''
'''	  &lt;httpHandlers&gt;
'''		  &lt;add verb="GET" path="CaptchaImage.aspx" type="WebControlCaptcha.CaptchaImageHandler, WebControlCaptcha" /&gt;
'''	  &lt;/httpHandlers&gt;
'''
''' Jeff Atwood
''' http://www.codinghorror.com/
'''
''' </summary>
Public Class CaptchaImageHandler
    Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
        Dim app As HttpApplication = context.ApplicationInstance

        '-- get the unique GUID of the captcha; this must be passed in via querystring 
        Dim guid As String = app.Request.QueryString("guid")
        Dim ci As CaptchaImage
        Dim o As Object

        If guid = "" Then
            '-- mostly for display purposes when in design mode
            '-- builds a CAPTCHA image with all default settings 
            '-- (this won't reflect any design time changes)
            ci = New CaptchaImage
        Else
            '-- get the CAPTCHA from the ASP.NET cache by GUID
            o = app.Context.Cache.Get(guid)
            ci = CType(o, CaptchaImage)
            app.Context.Cache.Remove(guid)
        End If

        If ci Is Nothing Then
            app.Response.StatusCode = 404
            app.Response.End()
            Return
        End If

        '-- write the image to the HTTP output stream as an array of bytes 
        ci.Image.Save(app.Context.Response.OutputStream, Drawing.Imaging.ImageFormat.Jpeg)
        app.Response.ContentType = "image/jpeg"
        app.Response.StatusCode = 200
        app.Response.End()

    End Sub


    Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
        Get
            Return True
        End Get
    End Property

End Class