using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class JoditBuilder : IRichEditBuilder
	{
		private readonly IUrlResolver urlResolver;
		private const string JODIT_JS_CDN = "https://cdn.jsdelivr.net/npm/jodit@4.19.10/es2021/jodit.min.js";
		private const string JODIT_CSS_CDN = "https://cdn.jsdelivr.net/npm/jodit@4.19.10/es2021/jodit.min.css";

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

					var imageApiPath = new URL('{baseUrl}', window.location.href).pathname.replace(/\/$/, '') + '/api/image';
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
							view: 'tiles',
							saveStateInStorage: {{ storeLastOpenedFolder: false, storeView: true, storeSortBy: true }},
							preview: true,
							showPreviewNavigation: true,
							showSelectButtonInPreview: true,
							ajax: {{
								url: imageApiPath + '/list',
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
							fileRemove: {{
								url: imageApiPath + '/delete',
								method: 'POST',
								contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
								prepareData: function(data) {{
									var form = document.getElementById('{tagHelper.ControlId}')?.closest('form');
									var token = form?.querySelector('input[name=""__RequestVerificationToken""]')?.value;
									if (!token) throw new Error('Security token missing. Refresh the page and try again.');
									return {{ name: data.name, path: data.path, source: data.source, __RequestVerificationToken: token }};
								}}
							}},
							uploader: {{
								url: imageApiPath + '/upload',
								customUploadFunction: uploadImage,
								format: 'json',
								filesVariableName: function(i) {{ return 'files'; }},
								withCredentials: true,
								isSuccess: function(resp) {{
									return resp.success;
								}},
								getMessage: function(resp) {{
									return resp.message || 'Upload failed. Refresh the page and try again.';
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
							url: imageApiPath + '/upload',
							customUploadFunction: uploadImage,
							format: 'json',
							imagesExtensions: ['jpg', 'jpeg', 'png', 'gif', 'webp'],
							filesVariableName: function(i) {{ return 'files'; }},
							withCredentials: true,
							prepareData: function(formData) {{
								formData.set('path', editor.filebrowser.state.currentPath || '/');
							}},
							headers: {{
								'RequestVerificationToken': document.querySelector('input[name=""__RequestVerificationToken""]')?.value || ''
							}},
							isSuccess: function(resp) {{
								return resp.success;
							}},
							getMessage: function(resp) {{
								return resp.message || 'Upload failed. Refresh the page and try again.';
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
										this.j.s.insertHTML('<figure><img src=""' + data.files[i] + '"" alt="""" /></figure>');
									}}
								}}
							}}
						}}
					}});

					async function imageRequest(action, data, operation) {{
						var form = document.getElementById('{tagHelper.ControlId}')?.closest('form');
						var token = form?.querySelector('input[name=""__RequestVerificationToken""]')?.value;
						if (!token) throw new Error('Security token missing. Refresh the page and try again.');
						data.set('__RequestVerificationToken', token);

						var response;
						try {{
							response = await fetch(imageApiPath + '/' + action, {{ method: 'POST', credentials: 'same-origin', body: data }});
						}} catch (error) {{
							throw new Error(operation + ' failed: unable to reach the server. Check your connection and try again.');
						}}
						if (response.redirected || response.status === 401 || response.status === 403) {{
							throw new Error('Your session may have expired. Refresh the page and sign in again.');
						}}
						if (response.status === 413) throw new Error('The upload exceeds the server size limit. Try a smaller image.');
						if (!(response.headers.get('content-type') || '').toLowerCase().includes('application/json')) {{
							throw new Error(operation + ' failed (HTTP ' + response.status + '). Refresh the page and try again.');
						}}
						var result = await response.json();
						if (!response.ok || !result.success) throw new Error(result.message || operation + ' failed.');
						return result;
					}}

					async function uploadImage(data, showProgress) {{
						showProgress(10);
						try {{
							return await imageRequest('upload', data, 'Image upload');
						}} catch (error) {{
							return {{ success: false, message: error.message }};
						}} finally {{
							showProgress(100);
						}}
					}}

					var browser = editor.filebrowser;
					browser.e.on('beforeOpen', function() {{
						browser.state.currentPath = '';
						browser.state.currentSource = 'default';
						browser.state.activeElements = [];
					}});
					browser.e.off('addFolder.filebrowser').on('addFolder.filebrowser', function(data) {{
						browser.prompt('Enter Directory name', 'Create directory', function(name) {{
							imageRequest('create-folder', new URLSearchParams({{ name: name, path: data.path, source: data.source }}), 'Folder creation').then(function() {{
								browser.status('Folder created: ' + name, true);
								browser.e.fire('update.filebrowser');
							}}).catch(function(error) {{ browser.status(error); }});
						}}, 'type dir name');
					}});
					browser.e.off('removeFolder.filebrowser').on('removeFolder.filebrowser', function(data) {{
						browser.confirm('Delete this empty folder?', 'Delete folder', function(yes) {{
							if (!yes) return;
							imageRequest('delete-folder', new URLSearchParams({{ name: data.name, path: data.path, source: data.source }}), 'Folder deletion').then(function(result) {{
								browser.status(result.message, true);
								browser.e.fire('update.filebrowser');
							}}).catch(function(error) {{ browser.status(error); }});
						}});
					}});

					browser.e.off('fileRemove.filebrowser').on('fileRemove.filebrowser', function() {{
						var selected = browser.state.activeElements.slice();
						if (!selected.length) return;
						browser.confirm('Delete the selected images? Existing posts may still reference them.', 'Delete images', function(yes) {{
							if (!yes) return;
							(async function() {{
								try {{
									for (var item of selected) {{
										var result = await imageRequest('delete', new URLSearchParams({{ name: item.file || item.name, path: item.path, source: item.sourceName }}), 'Image deletion');
										browser.status(result.message, true);
									}}
								}} catch (error) {{
									browser.status(error);
								}} finally {{
									browser.state.activeElements = [];
									browser.e.fire('update.filebrowser');
								}}
							}})();
						}});
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
