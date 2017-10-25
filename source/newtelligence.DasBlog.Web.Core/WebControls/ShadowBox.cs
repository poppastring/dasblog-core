#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
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
//
*/
#endregion

using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using System.IO;

namespace newtelligence.DasBlog.Web.Core.WebControls
{
	
	public enum ShadowImageType
	{
		TopLeft, Top, TopRight,
		Right,BottomRight,Bottom,
		BottomLeft, Left, Blank
	}

    public class ShadowBoxDesigner : System.Web.UI.Design.ContainerControlDesigner
	{
	}

	public class DrawingHelpers
	{
		private static Blend shadowBlendScaleLinear;
		private static Blend shadowBlendScaleRadial;

		static DrawingHelpers()
		{
			shadowBlendScaleLinear = new Blend(3);
			shadowBlendScaleLinear.Positions[0]=0.0f;
			shadowBlendScaleLinear.Factors[0]=0.0f;
			shadowBlendScaleLinear.Positions[1]=0.50f;
			shadowBlendScaleLinear.Factors[1]=0.80f;
			shadowBlendScaleLinear.Positions[2]=1f;
			shadowBlendScaleLinear.Factors[2]=1f;
			shadowBlendScaleRadial = new Blend(3);
			shadowBlendScaleRadial.Positions[0]=0.0f;
			shadowBlendScaleRadial.Factors[0]=0.0f;
			shadowBlendScaleRadial.Positions[1]=0.50f;
			shadowBlendScaleRadial.Factors[1]=0.20f;
			shadowBlendScaleRadial.Positions[2]=1f;
			shadowBlendScaleRadial.Factors[2]=1f;
		}

		public static byte[] CreateShadowImage(
			ShadowImageType imageType,
			int Width,
			int Height,
			Color ShadowColor,
			Color BackColor,
			ImageFormat Format)
		{
			MemoryStream memStream = new MemoryStream();
			Bitmap bmpPaint = new Bitmap(Width,Height);
			Graphics grph = Graphics.FromImage(bmpPaint);
			
			if ( imageType == ShadowImageType.Blank )
			{
				SolidBrush brush;
				brush = new SolidBrush(BackColor);
				grph.FillRectangle(brush,0,0,Width,Height);
			}
			else if ( imageType == ShadowImageType.Top || imageType == ShadowImageType.Bottom ||
				      imageType == ShadowImageType.Left || imageType == ShadowImageType.Right )
			{
				int angle=0;
				switch( imageType )
				{
					case ShadowImageType.Top: 
						angle = 270; break;
					case ShadowImageType.Right: 
						angle = 0; break;
					case ShadowImageType.Bottom: 
						angle = 90; break;
					case ShadowImageType.Left: 
						angle = 180; break;
				}
				LinearGradientBrush brush;
				brush = new LinearGradientBrush(
					new Rectangle(0,0,Width,Height),
					ShadowColor,
					BackColor,
					angle,true);
				brush.Blend = shadowBlendScaleLinear; 
				grph.FillRectangle(brush,0,0,Width,Height);
			}
			else
			{
				GraphicsPath path = new GraphicsPath();
				path.AddEllipse(0, 0, Width*2,Height*2);
				PathGradientBrush brush = new PathGradientBrush(path);
				brush.CenterColor = ShadowColor;
				Color[] colors = {BackColor};
				brush.SurroundColors = colors;
				if ( imageType == ShadowImageType.BottomRight )
				{
					brush.TranslateTransform(-Width,-Height);
				} 
				else if ( imageType == ShadowImageType.TopRight )
				{
					brush.TranslateTransform(-Width,0);
				}
				else if ( imageType == ShadowImageType.BottomLeft )
				{
					brush.TranslateTransform(0,-Height);
				}
				brush.CenterPoint=new PointF(Width,Height);
				brush.Blend = shadowBlendScaleRadial;
				grph.FillRectangle(new SolidBrush(BackColor),0, 0, Width, Height);
				grph.FillRectangle(brush, 0, 0, Width, Height);
			}
			
			bmpPaint.Save(memStream,Format);
            return memStream.GetBuffer();
		}

	}

	/// <summary>
	/// The ShadowBox is an ASP.NET WebControl that dynamically draws a drop
	/// shadow. 
	/// </summary>
    [ToolboxBitmap(typeof(ShadowBox),"ShadowBoxToolbarItem.bmp")]
	[Designer(typeof(ShadowBoxDesigner))]
    [ToolboxData("<{0}:ShadowBox runat=server>Shadow Box</{0}:ShadowBox>")]
	[ParseChildren(false)]
	[PersistChildren(true)]
	public class ShadowBox : System.Web.UI.WebControls.WebControl,
		IRenderControlImage
	{
		private const string imgTopLeft="tl";
		private const string imgTop="tp";
		private const string imgTopRight="tr";
		private const string imgRight="rg";
		private const string imgBottomRight="br";
		private const string imgBottom="bt";
		private const string imgBottomLeft="bl";
		private const string imgLeft="lf";
		private const string imgBlank="bk";
		private int shadowDepth;
		private Color shadowColor;

		public ShadowBox()
		{
		    shadowDepth = 6;
			shadowColor = Color.Gray;
            
		}

		private void RenderImage( HttpResponse Response, string ImageVariant, int Width, int Height, Color ShadowColor, Color BackgroundColor )
		{
			ShadowImageType imageType;

			switch ( ImageVariant )
			{
				case imgTopLeft:
					imageType=ShadowImageType.TopLeft;break;
				case imgTop:
					imageType=ShadowImageType.Top;break;
				case imgTopRight:
					imageType=ShadowImageType.TopRight;break;
				case imgRight:
					imageType=ShadowImageType.Right;break;
				case imgBottomRight:
					imageType=ShadowImageType.BottomRight;break;
				case imgBottom:
					imageType=ShadowImageType.Bottom;break;
				case imgBottomLeft:
					imageType=ShadowImageType.BottomLeft;break;
				case imgLeft:
					imageType=ShadowImageType.Left;break;
				case imgBlank:
				default:
					imageType=ShadowImageType.Blank;break;
			}

			byte[] image = DrawingHelpers.CreateShadowImage(
				imageType,
				Width,
				Height,
				ShadowColor,
				BackgroundColor,
				ImageFormat.Png );

			Response.ContentType="image/png";
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.OutputStream.Write(image,0,image.Length);
			
		}

		void IRenderControlImage.Render( HttpContext Context, string[] Args )
		{
			if ( Args.Length >= 5 )
			{
				string imageVariant = Args[0];
				int width=Convert.ToInt32(Args[1],10);
				int height=Convert.ToInt32(Args[2],10);
				Color shadowColor = Color.FromArgb(Convert.ToInt32(Args[3],16));
				Color backColor = Color.FromArgb(Convert.ToInt32(Args[4],16));
				RenderImage( Context.Response, imageVariant, width, height, shadowColor, backColor );
			}

		}

		[Browsable(true),Bindable(true)]
		public int ShadowDepth
		{
			get
			{
				return shadowDepth;
			}
			set
			{
				shadowDepth = value;
			}
		}

		[Browsable(true),Bindable(true),
		 Editor("System.Drawing.Design.ColorEditor",
			    "System.Drawing.Design.UITypeEditor")]
		public Color ShadowColor
		{
			get
			{
				return shadowColor;
			}
			set
			{
				shadowColor = value;
			}
		}

		public string GetImageUrl( string imageName )
		{
			Color effectiveBackColor = BackColor;

			if ( effectiveBackColor.A == 0 )
			{
				Control ctrl = this.Parent;
				while ( ctrl != null )
				{
					if ( ctrl is WebControl )
					{
						Color clr = ((WebControl)ctrl).BackColor;
						if ( clr.A != 0 )
						{
							effectiveBackColor = clr;
							break;
						}
					}
					ctrl = ctrl.Parent;
				}
				if ( effectiveBackColor.A == 0)
				{
				   effectiveBackColor = Color.White;
				}
			}
			else
			{
				effectiveBackColor = this.BackColor;
			}

			return ControlImageModule.GetImageHRef(
				Context,GetType(),
				new String[]{
								imageName,
								shadowDepth.ToString(),
								shadowDepth.ToString(),
				                shadowColor.ToArgb().ToString("x"),
				                effectiveBackColor.ToArgb().ToString("x"),
							}
				);
		}

		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			StringWriter baseStream;
			HtmlTextWriter htmlStream;
			Table tbl;
			TableRow tr;
			TableCell td;
			System.Web.UI.WebControls.Image img;

			tbl = new Table();
			tbl.ID=this.ID;
			
			tbl.BackColor=this.BackColor;
			tbl.Width = this.Width;
			tbl.Height=this.Height;
			tbl.CssClass=this.CssClass;
			tbl.BorderWidth=Unit.Pixel(0);
			tbl.CellPadding=0;
			tbl.CellSpacing=0;
            foreach( string key in this.Style.Keys )
            {
                tbl.Style.Add( key, this.Style[key]);
            }
			tbl.Rows.Add(tr = new TableRow());
			tr.Cells.Add(td = new TableCell());
			
			td.BorderWidth=this.BorderWidth;
			td.BorderStyle=this.BorderStyle;
			td.BorderColor=this.BorderColor;
			
			baseStream = new StringWriter(); 
			htmlStream = new HtmlTextWriter(baseStream);
			RenderChildren(htmlStream);
			td.Controls.Add(new LiteralControl(baseStream.ToString()));
			
			tr.Cells.Add(td = new TableCell());
			td.VerticalAlign=VerticalAlign.Top;
			td.Width=Unit.Pixel(shadowDepth);
			td.Style.Add("background","url('"+GetImageUrl(imgRight)+"')");
			td.Controls.Add(img = new System.Web.UI.WebControls.Image() );
			img.Width=Unit.Pixel(shadowDepth);
			img.Height=Unit.Pixel(shadowDepth);
			img.ImageUrl=GetImageUrl(imgBlank);
            img.AlternateText = "[ShadowBox]";
			td.Controls.Add(new LiteralControl("<br />"));
			td.Controls.Add(img = new System.Web.UI.WebControls.Image() );
			img.Width=Unit.Pixel(shadowDepth);
			img.Height=Unit.Pixel(shadowDepth);
            img.ImageUrl = GetImageUrl(imgTopRight);
            img.AlternateText = "[ShadowBox]";

			tbl.Rows.Add(tr = new TableRow());
			tr.Cells.Add(td = new TableCell());
			td.HorizontalAlign=HorizontalAlign.Left;
			td.Height=Unit.Pixel(shadowDepth);
			td.Style.Add("background","url('"+GetImageUrl(imgBottom)+"')");
			td.Controls.Add(img = new System.Web.UI.WebControls.Image() );
			img.Width=Unit.Pixel(shadowDepth);
			img.Height=Unit.Pixel(shadowDepth);
            img.ImageUrl = GetImageUrl(imgBlank);
            img.AlternateText = "[ShadowBox]";
			td.Controls.Add(img = new System.Web.UI.WebControls.Image() );
			img.Width=Unit.Pixel(shadowDepth);
			img.Height=Unit.Pixel(shadowDepth);
            img.ImageUrl = GetImageUrl(imgBottomLeft);
            img.AlternateText = "[ShadowBox]";

			tr.Cells.Add(td = new TableCell());
			td.Width = Unit.Pixel(shadowDepth);
			td.Height = Unit.Pixel(shadowDepth);
			td.Controls.Add(img = new System.Web.UI.WebControls.Image() );
			img.Width=Unit.Pixel(shadowDepth);
			img.Height=Unit.Pixel(shadowDepth);
            img.ImageUrl = GetImageUrl(imgBottomRight);
            img.AlternateText = "[ShadowBox]";

			tbl.RenderControl(output);
			
		}
	}
}
