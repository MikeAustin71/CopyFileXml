using System;

namespace CopyFilesXml
{
	public static class ExecuteUserInterfaceStartUp
	{
		private static GlobalDisplayElements _gDisplay = new GlobalDisplayElements();

		public static AppConfigurationModes DoStartUp()
		{
			var appModes = new AppConfigurationModes();
			appModes.StartupMode = AppStartUpMode.UserInterfaceMode;

			Console.Clear();
			Console.WriteLine(" ");
			Console.WriteLine(_gDisplay.BannerEqualChar);
			Console.WriteLine(_gDisplay.BannerEqualChar);
			var headr = _gDisplay.AppName;
			var lSpacer = _gDisplay.GetLeftSpacer(headr);
			Console.WriteLine(lSpacer + headr);
			Console.WriteLine(_gDisplay.BannerEqualChar);
			Console.WriteLine(_gDisplay.BannerEqualChar);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(" This Application will copy files and directories to a specific");
			Console.WriteLine(" directory tree, using parameters from an XML file in the");
			Console.WriteLine(" application directory.");
			Console.WriteLine(_gDisplay.BannerDashChar);
			Console.WriteLine(" Do you wish to continue? (Press Y to execute): ");
			Console.ForegroundColor = ConsoleColor.White;

			var key = Console.ReadKey(true);
			if(!(key.KeyChar=='Y') && !(key.KeyChar=='y') )
			{
				appModes.ExecutionMode = AppExecutionMode.AbortAndExit;

				return appModes;
			}

			Console.WriteLine(_gDisplay.BannerDashChar);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(" Do you wish to create a log file in the application directory for");
			Console.WriteLine(" for this copy operation? (Press L to create a Log file): ");
			Console.ForegroundColor = ConsoleColor.White;
			var key2 = Console.ReadKey(true);
			if(!(key2.KeyChar=='L') &&  !(key2.KeyChar=='l'))
			{
				appModes.ExecutionMode = AppExecutionMode.ExecuteWithoutLog;

				return appModes;
			}

			Console.Clear();
			Console.WriteLine("Initiating Copy Operation.....");

			appModes.ExecutionMode = AppExecutionMode.ExecuteWithLog;

			return appModes;
		}
	}
}
