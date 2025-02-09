using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
namespace AprilCraft.Web.Static.Data
{
	public class Images
	{
		private readonly IWebHostEnvironment webHostEnvironment;

		public Images(IWebHostEnvironment webHostEnvironment)
		{
			this.webHostEnvironment = webHostEnvironment;
			DesignsLargePath = webHostEnvironment.WebRootPath + "/assets/images/designs";
			DesignsSmallPath = webHostEnvironment.WebRootPath + "/assets/images/thumbnails";
			DesignsLarge = GetFilesFromPath(DesignsLargePath);
			DesignsSmall = GetFilesFromPath(DesignsSmallPath);
		}
		
		public string DesignsLargePath { get; set; }
		public string DesignsSmallPath { get; set; }
		public List<string> DesignsLarge { get; set; }
		public List<string> DesignsSmall { get; set; }

		public class ImgThumb
		{
			public string? ThumbInBase64 { get; set; }
			public string? FileName { get; set; }
		}

		List<string> GetFilesFromPath(string path)
		{
			List<string> files = new List<string>();
			try
			{
				files.AddRange(Directory.GetFiles(path).Select(Path.GetFileName)!);

				/*foreach (string dir in Directory.GetDirectories(path))
				{
					files.AddRange(GetFilesFromPath(dir));
				}*/
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return files;
		}

		public List<string> GetImages(string imagesPath)
		{
			List<string> allFiles = GetFilesFromPath(imagesPath);
			return allFiles;
		}

		public List<ImgThumb> GetImageThumbs(string imagesPath)
		{
			List<string> imagesPaths = GetFilesFromPath(imagesPath);
			// string base64String = "";
			List<ImgThumb> list = new();
			ImgThumb imgThumb = new();
			foreach (var path in imagesPaths)
			{
				//using (Image img = Image.Load(path))
				//{
				//	img.Mutate(x => x.Resize(img.Width / 18, img.Height / 18));
				//	using (var outputStream = new MemoryStream())
				//	{
				//		img.SaveAsBmp(outputStream);
				//		var bytes = outputStream.ToArray();
				//		base64String = Convert.ToBase64String(bytes);
				//	}
				//}
				//imgThumb = new ImgThumb()
				//	{
				//		ThumbInBase64 = "data:image/png;base64," + base64String,
				//		FileName = File.OpenRead(path).Name,
				//	};
				//list.Add(imgThumb);
			}
			return list;
		}
	}
}
