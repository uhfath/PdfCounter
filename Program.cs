using iText.Kernel.Pdf;
using System.Collections.Concurrent;

namespace PdfCounter
{
	internal class Program
	{
		private const string GroupedRootFolderName = "группировка";

		static void Main(string[] args)
		{
			Console.WriteLine("Папки для обработки");
			foreach (string arg in args)
			{
				Console.WriteLine(arg);
			}

			var files = args
				.Where(a => Directory.Exists(a))
				.SelectMany(a => Directory.EnumerateFiles(a, "*.pdf", SearchOption.AllDirectories)
					.Select(f => new
					{
						Root = a,
						Path = f,
					}))
			;

			Console.WriteLine();

			Parallel.ForEach(files, file =>
			//foreach (var file in files)
			{
				using var reader = new PdfReader(file.Path);
				using var document = new PdfDocument(reader);

				var pages = document.GetNumberOfPages();
				var relative = Path.GetRelativePath(file.Root, file.Path);
				var destination = Path.Combine(file.Root, GroupedRootFolderName, pages.ToString(), relative);

				Directory.CreateDirectory(Path.GetDirectoryName(destination));
				File.Copy(file.Path, destination);
			}
			);
		}
	}
}