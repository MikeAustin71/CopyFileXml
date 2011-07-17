using System;
using System.Xml.Serialization;

namespace CopyFilesXml
{
	public class CopyCommandParameters
	{
		[XmlElement("DeleteExistingFilesFirst")]
		public bool DeleteExistingFilesFirst
		{
			get; set;
		}

		[XmlElement("DeleteExistingDirectoriesFirst")]
		public bool DeleteExistingDirectoriesFirst
		{
			get; set;
		}

		[XmlElement("WildCard")]
		public string WildCard
		{
			get; set;
		}

		[XmlElement("FileName")]
		public string FileName
		{
			get; set;
		}


		[XmlElement("RegExFilter")]
		public string RegExFilter
		{
			get; set;
		}

		[XmlElement("CopySubdirectories")]
		public bool CopySubdirectories
		{
			get; set;
		}

		[XmlElement("PostCopyAction")]
		public string PostCopyAction
		{
			get; set;
		}

		[XmlIgnore]
		public CopyMode Mode
		{
			get
			{
				return DetermineCopyType();
			}			
		}

		public CopyCommandParameters()
		{
			return;
		}

		public CopyCommandParameters(string deleteFilesFirst, string deleteDirectoriesFirst, string wildCard, string fileName, string regExFilter, string copySubDirs, string postCopyAction)
		{
			WildCard = wildCard;

			FileName = fileName;

			RegExFilter = regExFilter;

			DetermineDeleteFirstAction(deleteFilesFirst, deleteDirectoriesFirst);

			CopySubdirectories = IsCopySubdirectories(copySubDirs);
      
			PostCopyAction = postCopyAction;

		}

		private void DetermineDeleteFirstAction(string deleteFilesFirst, string deleteDirectoriesFirst)
		{
			if(!string.IsNullOrEmpty(deleteFilesFirst)&& deleteFilesFirst.ToLower().Contains("true"))
			{
				DeleteExistingFilesFirst = true;
			}
			else
			{
				return;
			}

			if(!string.IsNullOrEmpty(deleteDirectoriesFirst) && deleteDirectoriesFirst.ToLower().Contains("true"))
			{
				DeleteExistingDirectoriesFirst = true;
			}


		}

		private CopyMode DetermineCopyType()
		{
			if(!String.IsNullOrEmpty(WildCard))
			{
				return CopyMode.CopyByWildCard;
			}	

			if(!String.IsNullOrEmpty(RegExFilter))
			{
				return CopyMode.CopyByRegEx;
			}

			if(!String.IsNullOrEmpty(FileName))
			{
				return CopyMode.CopyByFileName;
			}

			// Default
			WildCard = "*.*";
			return CopyMode.CopyByWildCard;

		}

		private bool IsCopySubdirectories(string copySubdirectories)
		{
			return !string.IsNullOrEmpty(copySubdirectories) && copySubdirectories.ToLower().Contains("true");
		}
	}
}