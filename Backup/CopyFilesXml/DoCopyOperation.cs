using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CopyFilesXml
{
	public class DoCopyOperation
	{

		private readonly LogAndDisplayController _log;

		public DoCopyOperation(LogAndDisplayController logDisplay)
		{
			_log = logDisplay;
		}

		public void Execute(CopyFileOperation copyOp)
		{
			if(copyOp==null || copyOp.CopyFileCommands == null || copyOp.CopyFileCommands.Count < 1)
			{
				throw new Exception("Invalid Copy File Commands Object!");
			}

			foreach (var cmd in copyOp.CopyFileCommands)
			{
				if(ValidateCopyCommand(cmd))
				{
					ProcessCommand(cmd);	
				}
				
			}

		}

		private bool ValidateCopyCommand(CopyFileCommand command)
		{
			if(!Directory.Exists(command.SourceDirectory))
			{
				_log.LogAndDisplay(string.Format("Source Directory Invalid:  {0}", command.SourceDirectory),LoggerMode.LogAndDisplayConsoleLine);
				return false;
			}

			if (!Directory.Exists(command.TargetDirectory))
			{
				try
				{
					Directory.CreateDirectory(command.TargetDirectory);
				}
				catch(Exception e)
				{
					_log.LogAndDisplay("Failed to create Target Directory!",LoggerMode.LogAndDisplayConsoleLine);
					_log.LogAndDisplay(" Target: " + command.TargetDirectory,LoggerMode.LogAndDisplayConsoleLine);
					_log.LogAndDisplay(e,LoggerMode.LogOnlyNoDisplay);
					return false;
				}

				_log.LogAndDisplay(string.Format("Successfully Created Target Directory: {0}", command.TargetDirectory),
				     LoggerMode.LogOnlyNoDisplay);
			}

			return true;
		}

		private void ProcessCommand(CopyFileCommand command)
		{
			var fUtil = new FileUtility(_log);

			// By this point, command parameters have been validated
			if(command.CommandParameters.DeleteExistingFilesFirst)
			{
				fUtil.DeleteAllFilesInDirectoryTree(command.TargetDirectory);

				if(command.CommandParameters.DeleteExistingDirectoriesFirst)
				{
					fUtil.DeleteAllDirectoriesInTreeExceptTopDirectory(command.TargetDirectory);
				}
			}

			DoCopyCommand(command, command.SourceDirectory);			
		}

		private void DoCopyCommand(CopyFileCommand cmd, string currSourceDir)
		{

			var uF = new FileUtility(_log);

      var uH = new HelperUtility();

			var currSrcInfo = uH.ConvertStringPathToInfo(currSourceDir);

			if(currSrcInfo==null)
			{
				throw new Exception(String.Format("Invalid Srouce Directory Info: {0}", currSourceDir));
			}

			var newTargetDir = uF.ComputeTargetDirectoryFromSourceDirectory(cmd.TargetDirectory,cmd.SourceDirectory, currSrcInfo);

			if(newTargetDir == null)
			{
				return;
			}

			if(!Directory.Exists(newTargetDir))
			{
				try
				{
					Directory.CreateDirectory(newTargetDir);
				}
				catch(Exception e)
				{
					_log.LogAndDisplay(String.Format("Error Creating Target Directory: {0}",newTargetDir),e,LoggerMode.LogAndDisplayConsoleLine);

					return;
				}
			}

			var newTargInfo = uH.ConvertStringPathToInfo(newTargetDir);
 
			if(newTargInfo==null)
			{
				_log.LogAndDisplay(String.Format("Invalid Target Directory Info: {0}", newTargetDir),LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			_log.DisplayCopyDirectoryStatus(currSrcInfo.FullName, newTargInfo.FullName);

			AssignCopyOperationByCopyMode(cmd, currSrcInfo, newTargInfo);

			if(!cmd.CommandParameters.CopySubdirectories)
			{
				return;	
			}
			
			var dirs = currSrcInfo.GetDirectories();

			if(dirs.Length < 1)
			{
				return;
			}

			foreach (var dir in dirs)
			{
				DoCopyCommand(cmd, uH.RemoveTrailingDirectorySlash(dir.FullName));
			}

		}

		private void AssignCopyOperationByCopyMode(CopyFileCommand cmd, DirectoryInfo currSrcInfo, DirectoryInfo newTargInfo)
		{
			switch (cmd.CommandParameters.Mode)
			{
				case CopyMode.CopyByWildCard:
					DoCopyFilesByWildCard(cmd.CommandParameters, newTargInfo, currSrcInfo);
					break;
				case CopyMode.CopyByFileName:
					DoCopyFilesByFileName(cmd.CommandParameters, newTargInfo, currSrcInfo);
					break;
				case CopyMode.CopyByRegEx:
					DoCopyFilesByRegEx(cmd.CommandParameters, newTargInfo, currSrcInfo);
					break;
				default:
					throw new Exception(String.Format("Invlaid Copy Mode Presented: {0}",cmd.CommandParameters.Mode));
			}
		}

		private void DoCopyFilesByRegEx(CopyCommandParameters parms, DirectoryInfo targDirInfo, DirectoryInfo srcDirInfo)
		{
			if(srcDirInfo==null)
			{
				_log.LogAndDisplay("DoCopyFilesByRegEx(): Invalid Source Directory Info!", LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			if(targDirInfo==null)
			{
					
				_log.LogAndDisplay("DoCopyFilesByRegEx(): Invalid Target Directory Info!", LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			var files = srcDirInfo.GetFiles();

			if(files.Length < 1)
			{
				return;
			}

			var uF = new FileUtility(_log);

			var rExp = new Regex(parms.RegExFilter);

			foreach (var file in files)
			{
				if(IsFileADirectory(file))
				{
					continue;
				}

				if(rExp.IsMatch(file.Name) )
				{
					uF.CopyFilesFromSrcToTarget(file, targDirInfo);				
				}
			}

		}

		private void DoCopyFilesByFileName(CopyCommandParameters parms, DirectoryInfo targDirInfo, DirectoryInfo srcDirInfo)
		{
			
			if(srcDirInfo==null)
			{
				_log.LogAndDisplay("DoCopyFilesByFileName(): Invalid Source Directory Info!", LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			if(targDirInfo==null)
			{
				_log.LogAndDisplay("DoCopyFilesByFileName(): Invalid Target Directory Info!", LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			var files = srcDirInfo.GetFiles();

			if(files.Length < 1)
			{
				return;
			}

			var uF = new FileUtility(_log);

			foreach (var file in files)
			{
				if(IsFileADirectory(file))
				{
					continue;
				}

				if(file.Name.ToLower() == parms.FileName.ToLower())
				{
					uF.CopyFilesFromSrcToTarget(file, targDirInfo);				
				}
			}
		}

		private void DoCopyFilesByWildCard(CopyCommandParameters parms, DirectoryInfo targDirInfo, DirectoryInfo srcDirInfo)
		{

			if(srcDirInfo==null)
			{
				_log.LogAndDisplay("DoCopyFilesByWildCard(): Invalid Source Directory Info!", LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			if(targDirInfo==null)
			{
				_log.LogAndDisplay("DoCopyFilesByWildCard(): Invalid Target Directory Info!", LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			var files = srcDirInfo.GetFiles(parms.WildCard);

			if(files.Length < 1)
			{
				return;
			}

			var uF = new FileUtility(_log);

			foreach (var file in files)
			{
				if(IsFileADirectory(file))
				{
					continue;
				}

				uF.CopyFilesFromSrcToTarget(file, targDirInfo);				
			}

		}

		private bool IsFileADirectory(FileSystemInfo file)
		{
			return (file.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
		}

	}
}