using Microsoft.JSInterop;
namespace craft_ui.Data
{
	public static class JSInteropStatics
	{
		private static readonly IWebHostEnvironment hostEnvironment;

		[JSInvokable]
		public static string SaveVisitor(string visitData, long trackId, string origin)
		{
			string filePath = "vistors_data/" + Convert.ToString(trackId) + ".json";
			if(!Directory.Exists("vistors_data"))
			{
				Directory.CreateDirectory("vistors_data");
			}
			if (!File.Exists(filePath))
			{
				File.WriteAllText(filePath, visitData);
			}
			return File.ReadAllText(filePath);
		}
	}
}
