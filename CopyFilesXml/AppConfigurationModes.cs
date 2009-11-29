namespace CopyFilesXml
{
	public class AppConfigurationModes
	{
		public AppExecutionMode ExecutionMode
		{
			get; set;
		}

		public AppStartUpMode StartupMode
		{
			get; set;
		}

		public AppXmlMode XmlFileMode
		{
			get; set;
		}

		public string ExternalXmlFile
		{
			get; set;
		}
	}
}