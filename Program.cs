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

			var filePages = new ConcurrentDictionary<string, int>();
			Parallel.ForEach(files, file =>
			{
				using var reader = new PdfReader(file.Path);
				using var document = new PdfDocument(reader);
				var pages = document.GetNumberOfPages();

				filePages.TryAdd(Path.GetRelativePath(file.Root, file.Path), pages);
			});

			var grouped = filePages
				.GroupBy(fp => fp.Key)
				.Select(gr => new
				{
					Pages = gr.Key,
					Files = gr
						.Select(fp => fp.Value),
				})
			;


		}
	}
}