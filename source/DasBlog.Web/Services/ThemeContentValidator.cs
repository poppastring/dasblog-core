using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.FileProviders;

namespace DasBlog.Web.Services
{
	public interface IThemeContentValidator
	{
		ThemeValidationResult Validate(string relativePath, string content);
	}

	public sealed class ThemeValidationResult
	{
		public bool IsValid => Errors.Count == 0;
		public IReadOnlyList<ThemeValidationError> Errors { get; init; } = Array.Empty<ThemeValidationError>();

		public static ThemeValidationResult Success { get; } =
			new() { Errors = Array.Empty<ThemeValidationError>() };

		public static ThemeValidationResult Failure(IEnumerable<ThemeValidationError> errors) =>
			new() { Errors = errors?.ToList() ?? new List<ThemeValidationError>() };

		public string Summarize(int max = 3)
		{
			if (IsValid) return string.Empty;
			var sb = new StringBuilder();
			foreach (var e in Errors.Take(max))
			{
				sb.Append("Line ").Append(e.Line);
				if (e.Column > 0) sb.Append(", col ").Append(e.Column);
				sb.Append(": ").AppendLine(e.Message);
			}
			if (Errors.Count > max)
			{
				sb.Append("(+").Append(Errors.Count - max).Append(" more)");
			}
			return sb.ToString().TrimEnd();
		}
	}

	public sealed class ThemeValidationError
	{
		public int Line { get; init; }
		public int Column { get; init; }
		public string Message { get; init; }
	}

	/// <summary>
	/// Server-side pre-flight checks for theme files before they're written to disk.
	/// Catches the common "blogger broke their site" mistakes (unbalanced braces,
	/// unterminated Razor blocks, malformed CSS) without attempting full Roslyn
	/// view compilation.
	/// </summary>
	public sealed class ThemeContentValidator : IThemeContentValidator
	{
		public ThemeValidationResult Validate(string relativePath, string content)
		{
			if (string.IsNullOrEmpty(relativePath))
			{
				return ThemeValidationResult.Success;
			}

			var ext = Path.GetExtension(relativePath).ToLowerInvariant();

			return ext switch
			{
				".cshtml" => ValidateRazor(relativePath, content ?? string.Empty),
				".css" => ValidateCss(content ?? string.Empty),
				_ => ThemeValidationResult.Success
			};
		}

		private static ThemeValidationResult ValidateRazor(string relativePath, string content)
		{
			try
			{
				var fileSystem = new InMemoryRazorProjectFileSystem(relativePath, content);
				var engine = RazorProjectEngine.Create(
					RazorConfiguration.Default,
					fileSystem,
					builder => { });

				var item = fileSystem.GetItem(relativePath, fileKind: null);
				var codeDocument = engine.Process(item);

				var diagnostics = codeDocument.GetCSharpDocument().Diagnostics
					.Where(d => d.Severity == RazorDiagnosticSeverity.Error)
					.Select(ToError)
					.ToList();

				return diagnostics.Count == 0
					? ThemeValidationResult.Success
					: ThemeValidationResult.Failure(diagnostics);
			}
			catch (Exception ex)
			{
				return ThemeValidationResult.Failure(new[]
				{
					new ThemeValidationError { Line = 1, Column = 1, Message = "Razor parse failed: " + ex.Message }
				});
			}
		}

		private static ThemeValidationError ToError(RazorDiagnostic d)
		{
			var span = d.Span;
			return new ThemeValidationError
			{
				Line = span.LineIndex + 1,
				Column = span.CharacterIndex + 1,
				Message = d.GetMessage()
			};
		}

		private static ThemeValidationResult ValidateCss(string content)
		{
			var errors = new List<ThemeValidationError>();
			var line = 1;
			var col = 1;
			var braceDepth = 0;
			var inBlockComment = false;
			var commentStartLine = 0;
			var commentStartCol = 0;

			for (var i = 0; i < content.Length; i++)
			{
				var c = content[i];
				var next = i + 1 < content.Length ? content[i + 1] : '\0';

				if (inBlockComment)
				{
					if (c == '*' && next == '/')
					{
						inBlockComment = false;
						i++;
						col += 2;
						continue;
					}
				}
				else
				{
					if (c == '/' && next == '*')
					{
						inBlockComment = true;
						commentStartLine = line;
						commentStartCol = col;
						i++;
						col += 2;
						continue;
					}

					if (c == '{')
					{
						braceDepth++;
					}
					else if (c == '}')
					{
						braceDepth--;
						if (braceDepth < 0)
						{
							errors.Add(new ThemeValidationError
							{
								Line = line,
								Column = col,
								Message = "Unmatched '}' (no matching opening brace)."
							});
							braceDepth = 0;
						}
					}
				}

				if (c == '\n')
				{
					line++;
					col = 1;
				}
				else
				{
					col++;
				}
			}

			if (inBlockComment)
			{
				errors.Add(new ThemeValidationError
				{
					Line = commentStartLine,
					Column = commentStartCol,
					Message = "Unterminated /* block comment."
				});
			}

			if (braceDepth > 0)
			{
				errors.Add(new ThemeValidationError
				{
					Line = line,
					Column = col,
					Message = $"Unbalanced braces: {braceDepth} '{{' without matching '}}'."
				});
			}

			return errors.Count == 0
				? ThemeValidationResult.Success
				: ThemeValidationResult.Failure(errors);
		}

		// Minimal RazorProjectFileSystem that serves a single in-memory file.
		// Avoids touching disk during validation.
		private sealed class InMemoryRazorProjectFileSystem : RazorProjectFileSystem
		{
			private readonly string filePath;
			private readonly InMemoryProjectItem item;

			public InMemoryRazorProjectFileSystem(string filePath, string content)
			{
				this.filePath = NormalizePath(filePath);
				this.item = new InMemoryProjectItem(this.filePath, content);
			}

			public override IEnumerable<RazorProjectItem> EnumerateItems(string basePath) =>
				new[] { item };

			public override RazorProjectItem GetItem(string path, string fileKind) => item;

			[Obsolete]
			public override RazorProjectItem GetItem(string path) => item;

			private static string NormalizePath(string path)
			{
				path = (path ?? string.Empty).Replace('\\', '/');
				return path.StartsWith("/") ? path : "/" + path;
			}
		}

		private sealed class InMemoryProjectItem : RazorProjectItem
		{
			private readonly string content;

			public InMemoryProjectItem(string filePath, string content)
			{
				FilePath = filePath;
				this.content = content ?? string.Empty;
			}

			public override string BasePath => "/";
			public override string FilePath { get; }
			public override string PhysicalPath => null;
			public override bool Exists => true;

			public override Stream Read() => new MemoryStream(Encoding.UTF8.GetBytes(content));
		}
	}
}
