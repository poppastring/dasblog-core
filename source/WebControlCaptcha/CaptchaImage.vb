Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

''' <summary>
''' 
''' Captcha image generation class
'''
''' Adapted from the excellent code at 
'''    http://www.codeproject.com/aspnet/CaptchaImage.asp
'''
''' Jeff Atwood
''' http://www.codinghorror.com/
''' 
''' </summary>
Public Class CaptchaImage
    Implements IDisposable

    Private _strFontFamilyName As String
    Private _intHeight As Integer
    Private _intWidth As Integer
    Private _rand As Random
    Private _strText As String
    Private _image As Bitmap
    Private _intRandomTextLength As Integer
    Private _strRandomTextChars As String = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"
    Private _fontWarp As FontWarpFactor

    Private Const _strGoodFontList As String = _
        "arial; arial black; comic sans ms; courier new; estrangelo edessa; franklin gothic medium; " & _
        "georgia; lucida console; lucida sans unicode; mangal; microsoft sans serif; palatino linotype; " & _
        "sylfaen; tahoma; times new roman; trebuchet ms; verdana;"

    ''' <summary>
    ''' Amount of random font warping to apply to rendered text
    ''' </summary>
    Public Enum FontWarpFactor
        None
        Low
        Medium
        High
        Extreme
    End Enum

#Region "  Public Properties"

    ''' <summary>
    ''' returns the rendered Captcha image based on the current property values
    ''' </summary>
    Public ReadOnly Property Image() As Bitmap
        Get
            If _image Is Nothing Then
                _image = GenerateImagePrivate()
            End If
            Return _image
        End Get
    End Property

    ''' <summary>
    ''' Font family to use when drawing the Captcha text 
    ''' </summary>
    Public Property Font() As String
        Get
            Return _strFontFamilyName
        End Get
        Set(ByVal Value As String)
            Try
                Dim font1 As Font = New Font(Value, 12.0!)
                _strFontFamilyName = Value
                font1.Dispose()
            Catch ex As Exception
                _strFontFamilyName = Drawing.FontFamily.GenericSerif.Name
            End Try
        End Set
    End Property

    ''' <summary>
    ''' Amount of random warping to apply to the Captcha text.
    ''' </summary>
    Public Property FontWarp() As FontWarpFactor
        Get
            Return _fontWarp
        End Get
        Set(ByVal Value As FontWarpFactor)
            _fontWarp = Value
        End Set
    End Property

    ''' <summary>
    ''' A string of valid characters to use in the Captcha text. 
    ''' A random character will be selected from this string for each character.
    ''' </summary>
    Public Property TextChars() As String
        Get
            Return _strRandomTextChars
        End Get
        Set(ByVal Value As String)
            _strRandomTextChars = Value
        End Set
    End Property

    ''' <summary>
    ''' Number of characters to use in the Captcha text. 
    ''' </summary>
    Public Property TextLength() As Integer
        Get
            Return _intRandomTextLength
        End Get
        Set(ByVal Value As Integer)
            _intRandomTextLength = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns the randomly generated Captcha text.
    ''' </summary>
    Public Property [Text]() As String
        Get
            If _strText.Length = 0 Then
                _strText = GenerateRandomText()
            End If
            Return _strText
        End Get
        Set(ByVal Value As String)
            _strText = Value
        End Set
    End Property

    ''' <summary>
    ''' Width of Captcha image to generate, in pixels 
    ''' </summary>
    Public Property Width() As Integer
        Get
            Return _intWidth
        End Get
        Set(ByVal Value As Integer)
            If (Value <= 60) Then
                Throw New ArgumentOutOfRangeException("width", Value, "width must be greater than 60.")
            End If
            _intWidth = Value
        End Set
    End Property

    ''' <summary>
    ''' Height of Captcha image to generate, in pixels 
    ''' </summary>
    Public Property Height() As Integer
        Get
            Return _intHeight
        End Get
        Set(ByVal Value As Integer)
            If Value <= 30 Then
                Throw New ArgumentOutOfRangeException("height", Value, "height must be greater than 30.")
            End If
            _intHeight = Value
        End Set
    End Property

#End Region

    Public Sub New()
        _rand = New Random
        Me.FontWarp = FontWarpFactor.Low
        Me.Width = 180
        Me.Height = 50
        Me.TextLength = 5
        Me.Font = RandomFontFamily()
        Me.Text = ""
    End Sub

    ''' <summary>
    ''' Forces a new Captcha image to be generated using current property values
    ''' </summary>
    Public Sub GenerateImage()
        _image = GenerateImagePrivate()
    End Sub

    ''' <summary>
    ''' returns random font family name
    ''' .. that's not on our font blacklist (illegible for CAPTCHA)
    ''' </summary>
    Private Function RandomFontFamily() As String
        Dim fc As New System.Drawing.Text.InstalledFontCollection
        Dim ff() As System.Drawing.FontFamily = fc.Families
        Dim strFontFamilyName As String = "bogus"

        Do While _strGoodFontList.IndexOf(strFontFamilyName) = -1
            strFontFamilyName = ff(_rand.Next(0, fc.Families.Length)).Name.ToLower
        Loop

        'Debug.WriteLine(strFontFamilyName)
        Return strFontFamilyName
    End Function

    ''' <summary>
    ''' generate human friendly random text
    ''' eg, try to avoid numbers and characters that look alike
    ''' </summary>
    Private Function GenerateRandomText() As String
        Dim sb As New System.Text.StringBuilder
        Dim n As Integer
        Dim intMaxLength As Integer = _strRandomTextChars.Length
        For n = 0 To _intRandomTextLength - 1
            sb.Append(_strRandomTextChars.Substring(_rand.Next(intMaxLength), 1))
        Next
        Return sb.ToString
    End Function

    Private Function RandomPoint(ByVal xmin As Integer, ByVal xmax As Integer, ByRef ymin As Integer, ByRef ymax As Integer) As PointF
        Return New PointF(_rand.Next(xmin, xmax), _rand.Next(ymin, ymax))
    End Function

    Private Function GenerateImagePrivate() As Bitmap
        Dim rectF As RectangleF
        Dim rect As Rectangle

        Dim ef1 As SizeF
        Dim font1 As Font
        Dim bitmap1 As Bitmap = New Bitmap(_intWidth, _intHeight, PixelFormat.Format32bppArgb)
        Dim graphics1 As Graphics = Graphics.FromImage(bitmap1)

        graphics1.SmoothingMode = SmoothingMode.AntiAlias

        rectF = New RectangleF(0, 0, _intWidth, _intHeight)
        rect = New Rectangle(0, 0, _intWidth, _intHeight)

        Dim brush1 As HatchBrush = New HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.White)
        graphics1.FillRectangle(brush1, rect)

        Dim prevWidth As Single = 0
        Dim fsize As Single = Convert.ToInt32(_intHeight * 0.8)
        Do
            font1 = New Font(_strFontFamilyName, fsize, FontStyle.Bold)
            ef1 = graphics1.MeasureString(Me.Text, font1)

            '-- does our text fit in the rect?
            If ef1.Width <= _intWidth Then Exit Do

            '-- doesn't fit in the rect, scale font down and try again.
            If prevWidth > 0 Then
                Dim intEstSize As Integer = Convert.ToInt32((ef1.Width - _intWidth) / (prevWidth - ef1.Width))
                If intEstSize > 0 Then
                    fsize = fsize - intEstSize
                Else
                    fsize -= 1
                End If
            Else
                fsize -= 1
            End If
            prevWidth = ef1.Width
        Loop While True

        '-- the resulting font size is usally conservative, so bump it up a few sizes.
        fsize += 4
        font1 = New Font(_strFontFamilyName, fsize, FontStyle.Bold)

        '-- get our textpath for the given font/size combo
        Dim sf As StringFormat = New StringFormat
        sf.Alignment = StringAlignment.Center
        sf.LineAlignment = StringAlignment.Center
        Dim textPath As GraphicsPath = New GraphicsPath
        textPath.AddString(Me.Text, font1.FontFamily, CType(font1.Style, Integer), font1.Size, rect, sf)

        '-- are we warping the text?
        If Me.FontWarp <> FontWarpFactor.None Then
            Dim intWarpDivisor As Integer

            Select Case _fontWarp
                Case FontWarpFactor.Low
                    intWarpDivisor = 6
                Case FontWarpFactor.Medium
                    intWarpDivisor = 5
                Case FontWarpFactor.High
                    intWarpDivisor = 4
                Case FontWarpFactor.Extreme
                    intWarpDivisor = 3
            End Select

            Dim intHrange As Integer = Convert.ToInt32(rect.Height / intWarpDivisor)
            Dim intWrange As Integer = Convert.ToInt32(rect.Width / intWarpDivisor)

            Dim p1 As PointF = RandomPoint(0, intWrange, 0, intHrange)
            Dim p2 As PointF = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p1.X)), rect.Width, 0, intHrange)
            Dim p3 As PointF = RandomPoint(0, intWrange, rect.Height - (intHrange - Convert.ToInt32(p1.Y)), rect.Height)
            Dim p4 As PointF = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p3.X)), rect.Width, rect.Height - (intHrange - Convert.ToInt32(p2.Y)), rect.Height)

            Dim points As PointF() = New PointF() {p1, p2, p3, p4}
            Dim m As New Matrix
            m.Translate(0, 0)
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0)
        End If

        '-- write our (possibly warped) text
        brush1 = New HatchBrush(HatchStyle.LargeConfetti, Color.LightGray, Color.DarkGray)
        graphics1.FillPath(brush1, textPath)

        '-- add noise to image
        Dim intMaxDim As Integer = Math.Max(rect.Width, rect.Height)
        Dim i As Integer
        For i = 0 To Convert.ToInt32(((rect.Width * rect.Height) / 30))
            graphics1.FillEllipse(brush1, _rand.Next(rect.Width), _rand.Next(rect.Height), _
                _rand.Next(Convert.ToInt32(intMaxDim / 50)), _rand.Next(Convert.ToInt32(intMaxDim / 50)))
        Next

        '-- it's important to clean up unmanaged resources
        font1.Dispose()
        brush1.Dispose()
        graphics1.Dispose()

        Return bitmap1
    End Function

#Region "  Destructors"

    Overridable Sub Dispose() Implements IDisposable.Dispose
        GC.SuppressFinalize(Me)
        Me.Dispose(True)
    End Sub

    Overridable Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            Me.Image.Dispose()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Try
            Me.Dispose(False)
        Finally
            MyBase.Finalize()
        End Try
    End Sub

#End Region

End Class