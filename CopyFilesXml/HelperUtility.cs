using System;
using System.IO;
using System.Reflection;

namespace CopyFilesXml
{
	public class HelperUtility
	{

		private const string xmlFileName = "CopyDirFiles.xml";

		public string XMLCommandFileName
		{
			get
			{
				return xmlFileName;
			}
		}

		public string GetExecutableDirectory()
		{
			try
			{
				var homePathAndExeFileName = Assembly.GetEntryAssembly().Location;

				return Path.GetDirectoryName(homePathAndExeFileName);
			}
			catch 
			{
				return string.Empty;
			}
		}

		public string CombineHomeExePathWithXmlFileName()
		{
			var homepath = GetExecutableDirectory();

			return Path.Combine(homepath, XMLCommandFileName);
		}
			

		public string CombineHomeExePathWithFileName(string fileNameAndExtension)
		{
			var homePath = GetExecutableDirectory();

			if(string.IsNullOrEmpty(homePath))
			{
				throw new Exception("Invalid Executable Home Directory!");
			}

			return Path.Combine(homePath, fileNameAndExtension);

		}

		public DirectoryInfo ConvertStringPathToInfo(string directoryPath)
		{
			if(string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
			{
				return null;
			}

			return new DirectoryInfo(directoryPath);

		}

		public string RemoveTrailingDirectorySlash(string dir)
		{
			if(string.IsNullOrEmpty(dir))
			{
				return string.Empty;
			}

			return dir[dir.Length-1]=='\\' ? dir.Substring(0, dir.Length - 1) : dir;
		}

		public string AddTrailingDirectorySlash(string directory)
		{
			if(string.IsNullOrEmpty(directory) || directory.Length < 1)
			{
				return string.Empty;
			}

			return directory[directory.Length - 1] == '\\' ? directory : directory + "\\";
		}
	}
}