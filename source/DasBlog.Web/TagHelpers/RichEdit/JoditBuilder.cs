using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class JoditBuilder : IRichEditBuilder
	{
		private readonly IUrlResolver urlResolver;
		private const string JODIT_JS_CDN = "https://cdn.jsdelivr.net/npm/jodit@4/es2021/jodit.min.js";
		private const string JODIT_CSS_CDN = "https://cdn.jsdelivr.net/npm/jodit@4/es2021/jodit.min.css";

		public JoditBuilder(IUrlResolver urlResolver)
		{
			this.urlResolver = urlResolver;
		}

		public void ProcessControl(RichEditTagHelper tagHelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "textarea";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("id", tagHelper.Id);
			output.Attributes.SetAttribute("name", tagHelper.Name);
			output.Attributes.SetAttribute("style", "visibility: hidden;");
		}

		public void ProcessScripts(RichEditScriptsTagHelper tagHelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("src", JODIT_JS_CDN);
			output.Attributes.SetAttribute("type", "text/javascript");

			var baseUrl = urlResolver.GetBaseUrl().TrimEnd('/');

			string initScript = $@"
				<link rel=""stylesheet"" href=""{JODIT_CSS_CDN}"" />
				<script>
				document.addEventListener('DOMContentLoaded', function() {{
					var editor = Jodit.make('#{tagHelper.ControlId}', {{
						height: 500,
						toolbarSticky: false,
						showCharsCounter: false,
						showWordsCounter: false,
						showXPathInStatusbar: false,
						sourceEditor: 'area',
						beautifyHTML: true,
						buttons: [
							'bold', 'italic', 'underline', 'strikethrough', '|',
							'ul', 'ol', '|',
							'paragraph', 'font', 'fontsize', '|',
							'image', 'link', 'table', '|',
							'align', 'indent', 'outdent', '|',
							'hr', 'eraser', 'source', '|',
							'undo', 'redo', 'fullsize'
						],
						filebrowser: {{
							ajax: {{
								url: '{baseUrl}/api/image/list',
								method: 'GET',
								withCredentials: true,
								process: function(resp) {{
									return resp;
								}}
							}},
							isSuccess: function(resp) {{
								return resp.success;
							}},
							getMessage: function(resp) {{
								return resp.message || '';
							}},
							uploader: {{
								url: '{baseUrl}/api/image/upload',
								format: 'json',
								filesVariableName: function(i) {{ return 'files'; }},
								withCredentials: true,
								isSuccess: function(resp) {{
									return resp.success;
								}},
								process: function(resp) {{
									return {{
										files: resp.files || [],
										baseurl: '',
										error: resp.success ? 0 : 1,
										message: resp.message || ''
									}};
								}}
							}}
						}},
						uploader: {{
							url: '{baseUrl}/api/image/upload',
							format: 'json',
							imagesExtensions: ['jpg', 'jpeg', 'png', 'gif', 'webp'],
							filesVariableName: function(i) {{ return 'files'; }},
							withCredentials: true,
							headers: {{
								'RequestVerificationToken': document.querySelector('input[name=""__RequestVerificationToken""]')?.value || ''
							}},
							isSuccess: function(resp) {{
								return resp.success;
							}},
							getMsg: function(resp) {{
								return resp.message || 'Upload failed';
							}},
							process: function(resp) {{
								return {{
									files: resp.files || [],
									baseurl: '',
									error: resp.success ? 0 : 1,
									message: resp.message || ''
								}};
							}},
							defaultHandlerSuccess: function(data) {{
								if (data.files && data.files.length) {{
									for (var i = 0; i < data.files.length; i++) {{
										this.s.insertHTML('<figure><img src=""' + data.files[i] + '"" alt="""" /></figure>');
									}}
								}}
							}}
						}}
					}});

					editor.e.on('filebrowser:insertImage', function(url) {{
						editor.s.insertHTML('<figure><img src=""' + url + '"" alt="""" /></figure>');
						return false;
					}});
				}});
				</script>";

			output.PostElement.SetHtmlContent(initScript);
		}
	}
}
