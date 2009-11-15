using System.Collections.Generic;
using System.Xml.Serialization;

namespace CopyFilesXml
{
	[XmlRoot("CopyFilesOperation")]
	public class CopyFileOperation
	{
		[XmlArray("CopyFileCommands")]
		[XmlArrayItem("CopyFileCommand")]
		public List<CopyFileCommand> CopyFileCommands
		{
			get; set;
		}		
	}
}