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
					function toRelativeImageUrl(url) {{
						if (!url) return '';
						try {{
							var parsed = new URL(url, window.location.origin);
							if (parsed.origin === window.location.origin) {{
								return parsed.pathname + parsed.search + parsed.hash;
							}}
						}} catch (e) {{
						}}

						return url;
					}}

					function fallbackCopyText(text) {{
						var input = document.createElement('textarea');
						input.value = text;
						input.setAttribute('readonly', 'readonly');
						input.style.position = 'fixed';
						input.style.left = '-9999px';
						document.body.appendChild(input);
						input.select();
						document.execCommand('copy');
						document.body.removeChild(input);
					}}

					function showImageUrlCopyMessage(message, isError) {{
						if (typeof bootstrap === 'undefined' || !bootstrap.Toast) {{
							console[isError ? 'error' : 'log'](message);
							return;
						}}

						var container = document.getElementById('imageUrlCopyToastContainer');
						if (!container) {{
							container = document.createElement('div');
							container.id = 'imageUrlCopyToastContainer';
							container.className = 'toast-container position-fixed top-0 end-0 p-3';
							container.style.zIndex = '1080';
							document.body.appendChild(container);
						}}

						var toastElement = document.createElement('div');
						toastElement.className = 'toast align-items-center text-bg-' + (isError ? 'danger' : 'success') + ' border-0';
						toastElement.setAttribute('role', 'status');
						toastElement.setAttribute('aria-live', 'polite');
						toastElement.setAttribute('aria-atomic', 'true');
						toastElement.innerHTML = '<div class=""d-flex""><div class=""toast-body""></div><button type=""button"" class=""btn-close btn-close-white me-2 m-auto"" data-bs-dismiss=""toast"" aria-label=""Close""></button></div>';
						toastElement.querySelector('.toast-body').textContent = message;
						container.appendChild(toastElement);

						var toast = bootstrap.Toast.getOrCreateInstance(toastElement, {{ delay: 4000 }});
						toastElement.addEventListener('hidden.bs.toast', function() {{ toastElement.remove(); }});
						toast.show();
					}}

					function copySelectedImageUrl(url) {{
						var relativeUrl = toRelativeImageUrl(url);
						if (!relativeUrl) {{
							showImageUrlCopyMessage('No image URL was selected.', true);
							return;
						}}

						if (navigator.clipboard && navigator.clipboard.writeText) {{
							navigator.clipboard.writeText(relativeUrl)
								.then(function() {{ showImageUrlCopyMessage('Copied image URL: ' + relativeUrl, false); }})
								.catch(function() {{
									fallbackCopyText(relativeUrl);
									showImageUrlCopyMessage('Copied image URL: ' + relativeUrl, false);
								}});
							return;
						}}

						fallbackCopyText(relativeUrl);
						showImageUrlCopyMessage('Copied image URL: ' + relativeUrl, false);
					}}

					function getImageUrlFromFileBrowserData(data) {{
						if (!data) return '';

						var files = data.files || data;
						if (!files || !files.length) return '';

						var first = files[0];
						if (typeof first === 'string') return first;
						if (first && first.url) return first.url;

						var name = first && (first.name || first.file);
						if (!name) return '';

						return (data.baseurl || data.baseUrl || '') + name;
					}}

					function openImageUrlBrowser(editor) {{
						if (!editor || !editor.filebrowser || typeof editor.filebrowser.open !== 'function') return;

						editor.filebrowser.open(function(data) {{
							copySelectedImageUrl(getImageUrlFromFileBrowserData(data));
						}}, false);
					}}

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
							'image', {{ name: 'imageUrl', tooltip: 'Copy image URL', icon: '<svg viewBox=""0 0 16 16"" xmlns=""http://www.w3.org/2000/svg""><path d=""M5 1.5A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5V2h1.5A1.5 1.5 0 0 1 14 3.5v11a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 14.5v-11A1.5 1.5 0 0 1 3.5 2H5v-.5Zm1.5-.25a.25.25 0 0 0-.25.25v1c0 .138.112.25.25.25h3a.25.25 0 0 0 .25-.25v-1a.25.25 0 0 0-.25-.25h-3ZM3.5 3.25a.25.25 0 0 0-.25.25v11c0 .138.112.25.25.25h9a.25.25 0 0 0 .25-.25v-11a.25.25 0 0 0-.25-.25H11A1.5 1.5 0 0 1 9.5 4h-3A1.5 1.5 0 0 1 5 3.25H3.5Z""/></svg>', exec: function(editor) {{ openImageUrlBrowser(editor); }} }}, 'link', 'table', '|',
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

					// Expose the editor so other UI (e.g. the hero image picker)
					// can reuse Jodit's filebrowser and read the live editor value.
					window.dasBlogEditor = editor;
					var ta = document.getElementById('{tagHelper.ControlId}');
					if (ta) {{ ta.jodit = editor; }}
				}});
				</script>";

			output.PostElement.SetHtmlContent(initScript);
		}
	}
}
