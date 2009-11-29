using System;
using System.IO;
using System.Text;

namespace CopyFilesXml
{
	public class LogAndDisplayController : IDisposable 
	{
		private string _logFilePathAndName;

		private TextWriter _tWriter;

		private readonly GlobalDisplayElements _gDisplay = new GlobalDisplayElements();

		private bool _disposed;

		private long _fileCounter;

		private long _directoryCounter;

		public AppConfigurationModes CurrentApplicationMode
		{
			get; set;
		}

		public LogAndDisplayController(AppConfigurationModes appModes)
		{

			CurrentApplicationMode = appModes;

			InitializeLogingOperations();
		}

		public void LogAndDisplay(string msg, Exception e, LoggerMode mode )
		{
			WriteToLog("", false);
			WriteToLog(_gDisplay.BannerEqualChar, false);
			WriteToLog(msg, false);
			WriteToLog("Exception: " + e.Message, false);
			WriteToLog(_gDisplay.BannerEqualChar, true);

			DisplayToConsole(msg, LoggerMode.DisplayConsoleLineOnly);
			DisplayToConsole(e.Message, LoggerMode.DisplayConsoleLineOnly);

		}

		public void LogAndDisplay(Exception e, LoggerMode mode)
		{
			WriteToLog("", false);
			WriteToLog(_gDisplay.BannerEqualChar, false);
			WriteToLog("Exception: " + e.Message, false);
			WriteToLog(_gDisplay.BannerEqualChar, true);
			DisplayToConsole(e.Message,LoggerMode.DisplayConsoleLineOnly);
		}

		public void LogAndDisplay(string msg, LoggerMode mode)
		{
			if(mode==LoggerMode.LogAndDisplayConsoleChars
					|| mode==LoggerMode.LogAndDisplayConsoleLine)
			{
				WriteToLog("", false);
				WriteToLog(msg, true);
			}

			DisplayToConsole(msg, mode);
		}

		public void DisplayToConsole(string msg, LoggerMode mode)
		{
			if(mode==LoggerMode.LogOnlyNoDisplay)
			{
				return;
			}	

			if(mode==LoggerMode.LogAndDisplayConsoleChars
					|| mode==LoggerMode.DisplayConsoleCharsOnly)
			{
				Console.Write(msg);

				return;
			}

			if(mode==LoggerMode.LogAndDisplayConsoleLine
				  || mode==LoggerMode.DisplayConsoleLineOnly)
			{
				Console.WriteLine(msg);

			}

			return;
		}

		public void WriteToLog(string msg, bool flush)
		{
			if(_tWriter==null || CurrentApplicationMode.ExecutionMode == AppExecutionMode.ExecuteWithoutLog)
			{
				return;
			}

			try
			{
				_tWriter.WriteLine(msg);	
			}
			catch
			{
				return;
			}

			if(flush)
			{
				_tWriter.Flush();
			}
		}

		public void IssueCompletionMessage()
		{
			LogAndDisplay(" ", LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(_gDisplay.BannerEqualChar, LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(_gDisplay.BannerEqualChar, LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay("File Copy Operation Completed", LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(DateTime.Now.ToString(), LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(string.Format("Directories Copied:  {0}", _directoryCounter), LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(string.Format("      Files Copied:  {0}", _fileCounter), LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(_gDisplay.BannerEqualChar, LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(_gDisplay.BannerEqualChar, LoggerMode.LogAndDisplayConsoleLine);
			LogAndDisplay(" ", LoggerMode.LogAndDisplayConsoleLine);

		}

		public void DisplayCopyDirectoryStatus(string srcDir, string targetDir)
		{
			_directoryCounter++;

			Console.WriteLine();
			Console.WriteLine(_gDisplay.BannerAsterikChar);
			Console.WriteLine("Copy Dir From: " + srcDir);
			Console.WriteLine("  Copy Dir To: " + targetDir);
			Console.WriteLine(_gDisplay.BannerAsterikChar);
			WriteToLog("",false);
			WriteToLog(_gDisplay.BannerDashChar,false);
			WriteToLog("Copy Dir From: " + srcDir,false);
			WriteToLog("  Copy Dir To: " + targetDir,false);
			WriteToLog(_gDisplay.BannerDashChar,true);
		}


		public void DisplayCopyFileStatus(string srcFile, string targetFile)
		{
			_fileCounter++;

			DisplayToConsole("Copy File: " + targetFile, LoggerMode.DisplayConsoleLineOnly);

			WriteToLog("", false);
			WriteToLog("Copy File From: " + srcFile, false);
			WriteToLog("  Copy File To: " + targetFile, true);


		}

		private void ShutdownLog()
		{
			if (_tWriter == null)
			{
				return;
			}

			_tWriter.Flush();

			_tWriter.Close();

			_tWriter.Dispose();

			_tWriter = null;
		}


		private void InitializeLogingOperations()
		{
			CreateLogFileName();


			if(CurrentApplicationMode.ExecutionMode!=AppExecutionMode.ExecuteWithLog)
			{
				_tWriter = null;

				return;
			}

			try
			{
				_tWriter = new StreamWriter(_logFilePathAndName);
			}
			catch 
			{
				CurrentApplicationMode.ExecutionMode = AppExecutionMode.ExecuteWithoutLog;

				return;
			}


			WriteToLog(_gDisplay.BannerEqualChar,false);
			WriteToLog(_gDisplay.BannerEqualChar,false);
			string tmp = _gDisplay.AppName;
			var lSpacer = _gDisplay.GetLeftSpacer(tmp);
			WriteToLog(lSpacer + tmp, false);
			tmp = "File Copy Operation";
			lSpacer = _gDisplay.GetLeftSpacer(tmp);
			WriteToLog(lSpacer + tmp, false);
			
			tmp = DateTime.Now.ToString();
			lSpacer = _gDisplay.GetLeftSpacer(tmp);
			WriteToLog(lSpacer + tmp, false);

			WriteToLog(_gDisplay.BannerEqualChar,false);
			WriteToLog(_gDisplay.BannerEqualChar,false);
			WriteToLog(" ", true);

		}

		private void CreateLogFileName()
		{
			var uH = new HelperUtility();

			var homePath = uH.AddTrailingDirectorySlash(uH.GetExecutableDirectory());
			var sb = new StringBuilder();
			DateTime now = DateTime.Now;
			sb.Append(homePath);
			sb.Append(now.Year.ToString("0000"));
			sb.Append(now.Month.ToString("00"));
			sb.Append(now.Day.ToString("00_"));
			sb.Append(now.Hour.ToString("00"));
			sb.Append(now.Minute.ToString("00"));
			sb.Append(now.Second.ToString("00"));
			sb.Append("_CopyLog.txt");

			_logFilePathAndName = sb.ToString();

		}


		public void Dispose()
		{
			Dispose(true);

			// Use SupressFinalize in case a subclass
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// If you need thread safety, use a lock around these 
			// operations, as well as in your methods that use the resource.
			if (!_disposed)
			{
				if (disposing)
				{
					ShutdownLog();
				}

				// Indicate that the instance has been disposed.
				_tWriter = null;
				_disposed = true;
			}
		}

	}
}