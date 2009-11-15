using System;
using System.Collections.Generic;
using System.IO;

namespace CopyFilesXml
{
	
	public class FileUtility
	{
		private readonly LogAndDisplayController _log;

		public FileUtility(LogAndDisplayController logDisplay)
		{
			_log = logDisplay;
		}

		public IList<DirectoryInfo> GetAllDirectoriesInTree(string topDirectory)
		{
			var u = new HelperUtility();
			
			if(string.IsNullOrEmpty(topDirectory))
			{
				_log.DisplayToConsole(string.Format("Top Directory is NULL/Empty: {0}", topDirectory), LoggerMode.LogAndDisplayConsoleLine);

				return null;
			}

			var dInfo = u.ConvertStringPathToInfo(topDirectory);

			if(dInfo==null)
			{
				_log.DisplayToConsole(string.Format("Top Directory Info Object is Invalid:  {0}", topDirectory), LoggerMode.LogAndDisplayConsoleLine);

				return null;
			}

			IList<DirectoryInfo> dirInfoTree = new List<DirectoryInfo>();

			GetAllDirectoriesInTree(dInfo, dirInfoTree);

			return dirInfoTree;

		}

		public void GetAllDirectoriesInTree(DirectoryInfo topDirectory, IList<DirectoryInfo> directoryInfos)
		{

			if(topDirectory==null)
			{
				_log.DisplayToConsole("GetAllDirectoriesInTree(): Top Directory is NULL", LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			if(directoryInfos==null)
				directoryInfos = new List<DirectoryInfo>();

			directoryInfos.Add(topDirectory);

			var dirs = topDirectory.GetDirectories();

			foreach (var dir in dirs)
			{
				GetAllDirectoriesInTree(dir, directoryInfos);
			}
		}

		public void DeleteAllFilesInDirectoryTree(string dirTree)
		{
			var u = new HelperUtility();

			var dInfoTopdir = u.ConvertStringPathToInfo(dirTree);

			if(dInfoTopdir==null)
			{
				return;
			}

			DeleteAllFilesInDirectoryTree(dInfoTopdir);

		}

		public void DeleteAllFilesInDirectoryTree(DirectoryInfo dirTree)
		{
			if(dirTree==null)
			{
				return;
			}

			var files = dirTree.GetFiles();

			foreach(var file in files)
			{
				var attrs = file.Attributes;

				if((attrs & FileAttributes.Directory)==FileAttributes.Directory)
				{
					continue;	
				}

				if((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly
					   || (attrs & FileAttributes.Hidden)==FileAttributes.Hidden)
				{
					file.Attributes = FileAttributes.Normal;
				}

				try
				{
					file.Delete();
				}
				catch(Exception e) 
				{
					_log.LogAndDisplay(string.Format("File Deletion Failed: {0}",file.FullName),LoggerMode.LogOnlyNoDisplay);
					_log.LogAndDisplay(e,LoggerMode.LogOnlyNoDisplay);
					continue;	
				}				

			}

			var dirs = dirTree.GetDirectories();

			foreach(var dir in dirs)
			{
				DeleteAllFilesInDirectoryTree(dir);
			}

		}

		public void DeleteAllDirectoriesInTreeExceptTopDirectory(string startingTopDirectory)
		{
			if(!Directory.Exists(startingTopDirectory))
			{
				_log.LogAndDisplay(string.Format("Top Directory Does Not Exist: {0}", startingTopDirectory),LoggerMode.LogAndDisplayConsoleLine);

				return;
			}

			var dirList = GetAllDirectoriesInTree(startingTopDirectory);

			if(dirList==null)
			{
				return;
			}

			//Skip Top Directory
			dirList.RemoveAt(0);

			for(int i = dirList.Count; i > 0; i--)
			{
				dirList[i-1].Delete();				
			}

		}

		public string ComputeTargetDirectoryFromSourceDirectory(string targetDir, string srcBaseDirectory, DirectoryInfo currDirectory)
		{
			var u = new HelperUtility();
			var baseTarget = u.ConvertStringPathToInfo(u.RemoveTrailingDirectorySlash(targetDir));

			if(baseTarget==null)
			{
				_log.LogAndDisplay(String.Format("Invalid Directory Info For: {0}",targetDir),LoggerMode.LogAndDisplayConsoleLine);

				return null;
			}

			var baseSource = u.ConvertStringPathToInfo(u.RemoveTrailingDirectorySlash(srcBaseDirectory));

			if(baseSource==null)
			{
				throw new Exception(String.Format("Invalid Source Directory Info: {0}",srcBaseDirectory));
			}

			var relDir = u.RemoveTrailingDirectorySlash(currDirectory.FullName.Remove(0, baseSource.FullName.Length));

			return baseTarget.FullName + relDir;
		}

		public void CopyFilesFromSrcToTarget(FileInfo file, DirectoryInfo targetDirInfo)
		{
			if(file==null )
			{
				_log.WriteToLog("CopyFilesFromSrcToTarget(): FileInfo is NULL!", true);
				return;
			}

			if(targetDirInfo==null)
			{
				_log.WriteToLog("CopyFilesFromSrcToTarget(): Target Directory Info is Null!", true);
				return;
			}

			var targetFile = ComputeTargetFileNameFromSourceFileName(targetDirInfo, file);

			try
			{
				File.Copy(file.FullName, targetFile);
				_log.DisplayCopyFileStatus(file.FullName, targetFile);
			}
			catch(Exception e)
			{

				_log.LogAndDisplay(string.Format("File Copy Failed: {0}", file.FullName),e,LoggerMode.LogAndDisplayConsoleLine);

			}

		}

		internal string ComputeTargetFileNameFromSourceFileName(DirectoryInfo targetDirInfo, FileInfo srcFileInfo)
		{
			var uH = new HelperUtility();

			var targetDir = uH.AddTrailingDirectorySlash(targetDirInfo.FullName);

			return targetDir + srcFileInfo.Name;
	
		}
	}
}