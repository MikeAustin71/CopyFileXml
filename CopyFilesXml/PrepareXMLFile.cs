using System.IO;

namespace CopyFilesXml
{
	public class PrepareXMLFile
	{
		private const string xmlFileName = "CopyDirFiles.xml";

		private readonly LogAndDisplayController _log;

		public PrepareXMLFile(LogAndDisplayController logDisplay)
		{
			_log = logDisplay;
		}

		public string GetDefaultXMLPathAndFileName()
		{

			var u = new HelperUtility();

			var xmlPathAndFileName = u.CombineHomeExePathWithFileName(xmlFileName);

			if(!File.Exists(xmlPathAndFileName))
			{
				_log.LogAndDisplay("Missing XML Config File! Application Terminating!",LoggerMode.LogAndDisplayConsoleLine);

				return null;
			}

			return xmlPathAndFileName;
		}
	}
}