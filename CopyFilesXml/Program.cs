using System;
using System.IO;
using System.Xml.Serialization;

namespace CopyFilesXml
{
	class Program
	{
		static readonly GlobalDisplayElements _gDisplay = new GlobalDisplayElements();
		
		static void Main(string[] args)
		{

			var modes = ProcessCommandLineArguments(args);

			if(modes.ExecutionMode==AppExecutionMode.AbortAndExit)
			{
				Console.WriteLine("Application Terminating as requested...");
				return;
			}


			if(modes.ExecutionMode==AppExecutionMode.DisplayHelp)
			{
				DisplayHelp();

				return;
			}

			if(modes.StartupMode == AppStartUpMode.UserInterfaceMode)
			{
				modes = ExecuteUserInterfaceStartUp.DoStartUp();
			}

			if(modes.ExecutionMode == AppExecutionMode.AbortAndExit)
			{
				Console.WriteLine("Application Terminating as requested...");
				return;
			}

			var logDisplay = new LogAndDisplayController(modes);

			PerformCopyOperation(logDisplay);


			logDisplay.IssueCompletionMessage();


			logDisplay.Dispose();

		}


		private static void PerformCopyOperation(LogAndDisplayController logDisplay)
		{
			var copyFileOp = AcquireCopyFileOperation(logDisplay);

			if(copyFileOp==null)
			{
				return;
			}

			var doCopyOp = new DoCopyOperation(logDisplay);

			doCopyOp.Execute(copyFileOp);

		}

		private static CopyFileOperation AcquireCopyFileOperation(LogAndDisplayController logDisplay)
		{
			var xmlPrep = new PrepareXMLFile(logDisplay);

			CopyFileOperation copyFileOperation;

			var xmlFile=String.Empty;
		
			try
			{
				if (logDisplay.CurrentApplicationMode.XmlFileMode == AppXmlMode.UserEnteredXmlFile
							&& File.Exists(logDisplay.CurrentApplicationMode.ExternalXmlFile))
				{
					xmlFile = logDisplay.CurrentApplicationMode.ExternalXmlFile;
				}
				else
				{
					xmlFile = xmlPrep.GetDefaultXMLPathAndFileName();
				}

				if(string.IsNullOrEmpty(xmlFile))
				{
					return null;
				}

				var fs = new FileStream(xmlFile, FileMode.Open);
		
				var serializer = new XmlSerializer(typeof(CopyFileOperation));

				copyFileOperation = (CopyFileOperation)serializer.Deserialize(fs);
				
			}
			catch (Exception e)
			{
				logDisplay.LogAndDisplay(String.Format("Error reading XML File: {0}",xmlFile),  e, LoggerMode.LogAndDisplayConsoleLine);

				return null;
			}

			return copyFileOperation;
		}


		private static void DisplayHelp()
		{
			Console.WriteLine(_gDisplay.BannerEqualChar);
			var headr = "Help For " + _gDisplay.AppName;

			var lSpacer = _gDisplay.GetLeftSpacer(headr);

			Console.WriteLine(lSpacer + headr);
			Console.WriteLine(_gDisplay.BannerEqualChar);
			Console.WriteLine(_gDisplay.AppName +  " with no command line arguments = User Interface");
			Console.WriteLine(_gDisplay.AppName + " \\?     = Display Help For " + _gDisplay.AppName + ".");
			Console.WriteLine(_gDisplay.AppName + " \\Y     = Execute Batch Mode WITH log output.");
			Console.WriteLine(_gDisplay.AppName + " \\Y \\L = Execute Batch Mode WITH log output.");
			Console.WriteLine(_gDisplay.AppName + " \\Y \\X = Execute Batch Mode WITHOUT log output.");
			Console.WriteLine(_gDisplay.AppName + " \\Y \\L  \\F=C:\\Custom\\MyCopySpec.xml = Execute Batch Mode WITH logging and use custom xml file.");
			Console.WriteLine(_gDisplay.BannerEqualChar);
			Console.WriteLine(_gDisplay.BannerEqualChar);

		}


		private static AppConfigurationModes ProcessCommandLineArguments(string[] args)
		{

			var appModes = new AppConfigurationModes();
	
			if(args==null ||  args.Length < 1)
			{
				appModes.StartupMode = AppStartUpMode.UserInterfaceMode;

				return appModes;
			}

			appModes.StartupMode = AppStartUpMode.CommandLineBatchMode;

			if(args[0].Contains("?"))
			{
				appModes.ExecutionMode = AppExecutionMode.DisplayHelp;

				return appModes;
			}

			appModes.ExecutionMode = AppExecutionMode.AbortAndExit;
			appModes.ExecutionMode = AppExecutionMode.ExecuteWithoutLog;
			appModes.XmlFileMode = AppXmlMode.DefaultXmlFile;

			foreach (var s in args)
			{
				if(s.ToLower().Contains(@"/y"))
				{
					appModes.ExecutionMode = AppExecutionMode.ExecuteWithoutLog;
				}
				
				if(s.ToLower().Contains(@"/l"))
				{
					appModes.ExecutionMode = AppExecutionMode.ExecuteWithLog;
				}

				if (!s.ToLower().Contains(@"/f="))
				{
					continue;
				}

				var xmlF = s.Remove(0, 3).Trim();

				if (string.IsNullOrEmpty(xmlF) || !File.Exists(xmlF))
				{
					continue;
				}

				appModes.XmlFileMode = AppXmlMode.UserEnteredXmlFile;

				appModes.ExternalXmlFile = xmlF;
			}

			return appModes;
		}

	}
}
