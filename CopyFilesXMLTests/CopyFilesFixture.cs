using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CopyFilesXml;
using NUnit.Framework;

namespace CopyFilesXMLTests
{
	[TestFixture]
	public class CopyFilesFixture
	{

		private CopyFileOperation _copyFileOp;


		
		[Ignore]
		public void CreateXMLFile()
		{

			var parms = new CopyCommandParameters
			            	{
			            		CopySubdirectories = true,
			            		DeleteExistingDirectoriesFirst = true,
			            		DeleteExistingFilesFirst = true,
			            		WildCard = "*.*",
			            		FileName = "",
			            		RegExFilter = "",
											PostCopyAction = ""
			            	};



			var cmd1 = new CopyFileCommand
			          	{
			          		CommandParameters = parms,
			          		SourceDirectory = @"C:\T01\TestFolder",
			          		TargetDirectory = @"C:\T02\TestFolder"
			          	};


			var cmd2 = new CopyFileCommand
			           	{
			           		CommandParameters = parms,
			           		SourceDirectory = @"C:\T01",
			           		TargetDirectory = @"C:\T15"
			           	};


			var cmd3 = new CopyFileCommand
			           	{
			           		CommandParameters = parms,
			           		SourceDirectory = @"C:\T01",
			           		TargetDirectory = @"C:\T03"
			           	};


			var list = new List<CopyFileCommand> {cmd1,cmd2,cmd3};

			var root = new CopyFileOperation
			           	{
			           		CopyFileCommands = list
			           	};

			var u = new HelperUtility();

			var serializer = new XmlSerializer(typeof (CopyFileOperation));
			var output = Path.Combine(@"D:\CS_Test", u.XMLCommandFileName);
			var fs = new FileStream(output, FileMode.Create);
			serializer.Serialize(fs,root);
			fs.Close();


		}

		[SetUp]
		public void SetUp()
		{
			
			var parms = new CopyCommandParameters
			            	{
			            		CopySubdirectories = true,
			            		DeleteExistingDirectoriesFirst = true,
			            		DeleteExistingFilesFirst = true,
			            		WildCard = "*.*"
			            	};



			var cmd1 = new CopyFileCommand
			          	{
			          		CommandParameters = parms,
										SourceDirectory = @"D:\CS_Test",
			          		TargetDirectory = @"D:\T002"
			          	};


			var cmd2 = new CopyFileCommand
			           	{
			           		CommandParameters = parms,
			           		SourceDirectory = @"D:\T001\TestFolder",
			           		TargetDirectory = @"D:\T015\TestFolder"
			           	};


			var cmd3 = new CopyFileCommand
			           	{
			           		CommandParameters = parms,
			           		SourceDirectory = @"D:\T001",
			           		TargetDirectory = @"D:\T003"
			           	};


			var list = new List<CopyFileCommand> {cmd1,cmd2,cmd3};

			_copyFileOp = new CopyFileOperation
			           	{
			           		CopyFileCommands = list
			           	};
		}
	
		[Ignore]
		public void TestAssembleCompositeDirectory()
		{
			var origSrcDir = @"D:\T001";
			var currSrcDir = new DirectoryInfo(@"D:\T001\TestFolder");
			var targetDir = @"D:\T002";
			var fU = new FileUtility(new LogAndDisplayController(new AppConfigurationModes{ExecutionMode = AppExecutionMode.ExecuteWithoutLog}));
			var nTarget = fU.ComputeTargetDirectoryFromSourceDirectory(targetDir, origSrcDir, currSrcDir);

			Assert.IsTrue(nTarget == @"D:\T002\TestFolder","T91");


			origSrcDir = @"D:\T001\TestFolder";
			targetDir = @"D:\T002";
			currSrcDir = new DirectoryInfo(@"D:\T001\TestFolder\Bin\");
			nTarget = fU.ComputeTargetDirectoryFromSourceDirectory(targetDir, origSrcDir, currSrcDir);
			Assert.IsTrue(nTarget == @"D:\T002\Bin");

			origSrcDir = @"D:\T001\";
			currSrcDir = new DirectoryInfo(@"D:\T001\TestFolder\Bin");
			targetDir = @"D:\T002";
			nTarget = fU.ComputeTargetDirectoryFromSourceDirectory(targetDir, origSrcDir, currSrcDir);

			Assert.IsTrue(nTarget == @"D:\T002\TestFolder\Bin","T92");

			origSrcDir = @"D:\T001";
			currSrcDir = new DirectoryInfo(@"D:\T001\TestFolder\Bin");
			targetDir = @"D:\T002\";
			nTarget = fU.ComputeTargetDirectoryFromSourceDirectory(targetDir, origSrcDir, currSrcDir);

			Assert.IsTrue(nTarget == @"D:\T002\TestFolder\Bin","T93");

			origSrcDir = @"D:\T001\";
			currSrcDir = new DirectoryInfo(@"D:\T001\TestFolder\Bin\");
			targetDir = @"D:\T002\";
			nTarget = fU.ComputeTargetDirectoryFromSourceDirectory(targetDir, origSrcDir, currSrcDir);

			Assert.IsTrue(nTarget == @"D:\T002\TestFolder\Bin","T94");


		}

		[Test]
		public void TestCreateTargetFileFromSourceFile()
		{
			var file = new FileInfo(@"D:\T001\TestFolder\Bin\DevServcerConflict.psd");
			var uF = new FileUtility(new LogAndDisplayController(new AppConfigurationModes{ExecutionMode = AppExecutionMode.ExecuteWithoutLog}));
			var targetDir = new DirectoryInfo(@"D:\T002");
			var newTargetFile = uF.ComputeTargetFileNameFromSourceFileName(targetDir, file);
			Assert.IsTrue(newTargetFile==@"D:\T002\DevServcerConflict.psd");

		}

		[Test]
		[Ignore]
		public void TestCopyFiles()
		{
			var ex = new DoCopyOperation(new LogAndDisplayController(new AppConfigurationModes{ExecutionMode = AppExecutionMode.ExecuteWithoutLog}));

			ex.Execute(_copyFileOp);

			var cmd1 = _copyFileOp.CopyFileCommands[0];

			var dSrcInfo = new DirectoryInfo(cmd1.SourceDirectory);
			var dTargInfo = new DirectoryInfo(cmd1.TargetDirectory);
			var cnt1 = dSrcInfo.GetFileSystemInfos().Length;
			var cnt2 = dTargInfo.GetFileSystemInfos().Length;
			Assert.IsTrue(cnt1 == cnt2);

		}

	}
}