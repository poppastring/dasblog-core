// BookControl.cs by Scott Watermasysk http://scottwater.com/articles/BookControl

using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace newtelligence.DasBlog.Web.Core
{
	[Description("The book control will render a one randmom book image and amazon link from an xml file"),
	DefaultProperty("BookXML"),
	Designer(typeof(BookControlDesigner)),
	ToolboxData("<{0}:BookControl runat= \"server\"></{0}:BookControl>")]
	public class BookControl : HyperLink
	{
		public BookControl(){}

		private string _bookXML = null;
		//Location of books xml. Elements should be Image, ISBN, and Title
		public string BookXML
		{
			get{return _bookXML;}
			set{_bookXML = value;}
		}

		private string _imageDirectory = "~/images/books";
		//Where to find the book images
		public string ImageDirectory
		{
			get
			{return _imageDirectory;}
			set{_imageDirectory = value;}
		}
	
		// TODO: pull the amazon ID from the siteConfig or the BookControl.xml file
		private string _amazonID = "";
		
		public string AmazonID
		{
			get{return _amazonID;}
			set{_amazonID = value;}
		}

		protected override void OnPreRender(EventArgs e)
		{
			DataRow dr = this.GetRandomBook();
			string imgToken = ((String)dr["ISBN"]).PadRight(10,'X'); 
			this.ImageUrl = string.Format("http://images.amazon.com/images/P/{0}.01._PI_SCMZZZZZZZ_.jpg",imgToken);
			this.NavigateUrl = string.Format("http://www.amazon.com/exec/obidos/ASIN/{0}/{1}",dr["ISBN"],this.AmazonID);
				this.ToolTip = (string)dr["Title"];
			this.Height = 150;
			this.Width = 120;
			base.OnPreRender (e);
		}


		protected virtual DataRow GetRandomBook()
		{
            DataCache cache = CacheFactory.GetCache();

			DataTable dt = (DataTable)cache["BooksTable"];
			if(dt == null)
			{
				string path = SiteUtilities.MapPath(BookXML);
				DataSet ds = new DataSet();
				ds.ReadXml(path);
				dt = ds.Tables[0];
				cache.Insert("BooksTable",dt,new CacheDependency(path));
			}
			int seed = DateTime.Now.Millisecond;

			//if we add more than one control to the page, we want to make sure our seeds are unique
			if(this.ID != null)
			{
				seed += this.ID.GetHashCode();
			}
			//Not really random...but good enough
			Random rnd = new Random(seed);
			return dt.Rows[rnd.Next(dt.Rows.Count)];
		}

	}

	//Quick designer
	internal class BookControlDesigner : ControlDesigner
	{
		public BookControlDesigner(){}

		private BookControl bc = null;
		public override void Initialize(IComponent component)
		{
			bc = (BookControl)component;
			base.Initialize(component);
		}

		public override bool AllowResize
		{
			get    {return false;}
		}
	   
		public override string GetDesignTimeHtml()
		{
			StringWriter sw = new StringWriter();
			HtmlTextWriter htw = new HtmlTextWriter(sw);
			Table t = new Table();
			t.BorderColor = Color.Navy;
			t.BackColor =  ColorTranslator.FromHtml("#B6C9E7");
			t.BorderStyle = BorderStyle.Solid;
			TableRow tr = new TableRow();
			tr.Width = 120;
			tr.Height = 150;
			TableCell td = new TableCell();
			td.Text = string.Format("<strong>ScottWater</strong><br />Book Control<br />{0}",bc.ID);
			td.VerticalAlign = VerticalAlign.Middle;
			td.HorizontalAlign = HorizontalAlign.Center;
			tr.Cells.Add(td);
			t.Rows.Add(tr);
			t.RenderControl(htw);
			return sw.ToString();
		}
	}
}