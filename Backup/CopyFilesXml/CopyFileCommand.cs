using System.Xml.Serialization;

namespace CopyFilesXml
{
	public class CopyFileCommand
	{
		[XmlElement("SourceDirectory")]
		public string SourceDirectory
		{
			get; set;
		}

		[XmlElement("TargetDirectory")]
		public string TargetDirectory
		{
			get; set; 
		}

		
		[XmlElement("CommandParameters")]
		public CopyCommandParameters CommandParameters
		{
			get; set;
		}


		public  CopyFileCommand()
		{
			
		}

		public CopyFileCommand(string srcDir, string targetDir, CopyCommandParameters copyParms)
		{

			SourceDirectory = srcDir;

			TargetDirectory = targetDir;

			CommandParameters = copyParms;

		}


	}
}