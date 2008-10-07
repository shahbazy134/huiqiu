using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;


namespace CSharpRecipes
{
    public static class FileExtensions
    {
        #region FileInfo methods
        public static void Rename(this FileInfo originalFile, string newName)
        {
            originalFile.MoveTo(newName);
        }
        
        public static void DisplayTimestamps(this FileInfo fileInfo)
        {
            Console.WriteLine(fileInfo.CreationTime.ToString());
            Console.WriteLine(fileInfo.LastAccessTime.ToString());
            Console.WriteLine(fileInfo.LastWriteTime.ToString());
        }

        public static void ModifyTimestamps(this FileInfo fileInfo, DateTime dt)
        {
            fileInfo.CreationTime = dt;
            fileInfo.LastAccessTime = dt;
            fileInfo.LastWriteTime = dt;
        }

        public static void MakeFileHidden(this FileInfo fileInfo)
        {
            // Modify this file’s attributes
            fileInfo.Attributes |= FileAttributes.Hidden;
        }
        #endregion // FileInfo methods

        #region DirectoryInfo methods
        public static void Rename(this DirectoryInfo dirInfo, string newName)
        {
            try
            {
                // "rename" it
                dirInfo.MoveTo(newName);
            }
            catch (IOException ioe)
            {
                // most likely given the directory exists or isn't empty
                Trace.WriteLine(ioe.ToString());
            }
        }   

        public static void DisplayTimestamps(this DirectoryInfo dirInfo)
        {
            Console.WriteLine(dirInfo.CreationTime.ToString());
            Console.WriteLine(dirInfo.LastAccessTime.ToString());
            Console.WriteLine(dirInfo.LastWriteTime.ToString());
        }

        public static void ModifyTimestamps(this DirectoryInfo dirInfo, DateTime dt)
        {
            dirInfo.CreationTime = dt;
            dirInfo.LastAccessTime = dt;
            dirInfo.LastWriteTime = dt;
        }

        public static void MakeDirectoryHidden(this DirectoryInfo dirInfo)
        {
            // Modify this directory’s attributes
            dirInfo.Attributes |= FileAttributes.Hidden;
        }
        #endregion // DirectoryInfo methods

        #region FileSystemInfo methods
        public static string ToDisplayString(this FileSystemInfo fileSystemInfo)
        {
            string type = fileSystemInfo.GetType().ToString();
            if (fileSystemInfo is DirectoryInfo)
                type = "DIRECTORY";
            else if (fileSystemInfo is FileInfo)
                type = "FILE";
            return string.Format(Thread.CurrentThread.CurrentCulture,
                "{0}: {1}", type, fileSystemInfo.Name);
        }
        #endregion // FileSystemInfo methods
    }

    public class FileSystemIO
	{
		#region "12.1 Manipulating File Attributes"
		public static void ManipulateFileAttributes()
		{
			string path = Path.GetTempFileName();
			if (!File.Exists(path))
				File.Create(path);
				
			try
			{
                DateTime dt = new DateTime(2001, 2, 8);
                // Use the File class static methods to play with the timestamps and attributes
				DisplayFileTimestamps(path);
				ModifyFileTimestamps(path,dt);


				// Use the FileInfo class instance methods to play with the timestamps and attributes
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.DisplayTimestamps();
				fileInfo.ModifyTimestamps(dt);
                DisplayFileHiddenAttribute(path);
                fileInfo.MakeFileHidden();
                DisplayFileHiddenAttribute(path);
            }
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void DisplayFileTimestamps(string path)
		{
			Console.WriteLine(File.GetCreationTime(path).ToString());
			Console.WriteLine(File.GetLastAccessTime(path).ToString());
			Console.WriteLine(File.GetLastWriteTime(path).ToString());
		}

		public static void ModifyFileTimestamps(string path, DateTime dt)
		{
			File.SetCreationTime(path, dt);
			File.SetLastAccessTime(path, dt);
			File.SetLastWriteTime(path, dt);
		}

        public static void DisplayFileHiddenAttribute(string path)
        {
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);

                // Display whether this file is hidden
                Console.WriteLine("Is file hidden? = " +
                    ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden));
            }
        }

		#endregion

		#region "12.2 Rename a File"
		public static void RenameFile()
		{
            //File.Delete(@"C:\temp\foo.txt");
			if(!Directory.Exists(@"c:\temp"))
				Directory.CreateDirectory(@"c:\temp");
            File.Delete(@"C:\temp\bar.dat");

			if (!File.Exists(@"c:\temp\foo.txt"))
				File.Create(@"c:\temp\foo.txt");

            Rename(@"c:\temp\foo.txt",@"c:\temp\bar.dat");

			Rename(@"c:\temp\bar.dat",@"c:\temp\foo.txt");

            FileInfo originalFile = new FileInfo(@"c:\temp\foo.txt");
            originalFile.Rename(@"c:\temp\bar.dat");
		}

        public static void Rename(string originalName, string newName)
        {
            File.Move(originalName, newName);
        }

		#endregion

		#region "12.3 Outputting A Platform Independent EOL Character"
		public static void PlatformIndependentEol()
		{
			// 1) Remember to use Environment.NewLine on every block of text 
			// we format that we want platform correct newlines inside of
			string line;
			line = String.Format("FirstLine {0} SecondLine {0} ThirdLine {0}",Environment.NewLine);

			// get a temp file to work with
			string file = Path.GetTempFileName();
			FileStream stream = File.Create(file);
			byte[] bytes = Encoding.Unicode.GetBytes(line);
			stream.Write(bytes,0,bytes.Length);
			// close the file
			stream.Close();

			// remove the file (good line to set a breakpoint to examine the file
			// we created)
			File.Delete(file);


			// 2) Set up a text writer and tell it to use the certain newline
			// string
			// get a new temp file
			file = Path.GetTempFileName();
			line = "Double spaced line";
			StreamWriter streamWriter = new StreamWriter(file);
			// make this always write out double lines
			streamWriter.NewLine = Environment.NewLine + Environment.NewLine;
			// WriteLine on this stream will automatically use the newly specified
			// newline sequence (double newline in our case)
			streamWriter.WriteLine(line);
			streamWriter.WriteLine(line);
			streamWriter.WriteLine(line);
			// close the file
			streamWriter.Close();
			// remove the file (good line to set a breakpoint to check out the file
			// we created)
			File.Delete(file);

			// 3) Just use any of the normal WriteLine methods as they use the 
			// Environment.NewLine by default
			line = "Default line";
			Console.WriteLine(line);

		}
		#endregion

		#region "12.4 Manipulating Directory Attributes"
		public static void ManipulateDirectoryAttributes()
		{
			string path = Path.GetTempPath() + @"\MyTemp";
			Directory.CreateDirectory(path);
			try
			{
                DateTime dt = new DateTime(2003, 5, 10);
                // Display, manipulate the directory timestamps using the static methods of the 
				// Directory class.
				DisplayDirectoryTimestamps(path);
				ModifyDirectoryTimestamps(path,dt);


				// Use the DirectoryInfo class instance methods to play with the timestamps and attributes
                DirectoryInfo dirInfo = new DirectoryInfo(path);
				dirInfo.DisplayTimestamps();
				dirInfo.ModifyTimestamps(dt);
                DisplayDirectoryHiddenAttribute(path);
                dirInfo.MakeDirectoryHidden();
                DisplayDirectoryHiddenAttribute(path);
            }
			catch(Exception e)
			{
				Console.WriteLine("{0}",e.ToString());
			}
			Directory.Delete(path,true);
		}

	    public static void DisplayDirectoryTimestamps(string path)
	    {
		    Console.WriteLine(Directory.GetCreationTime(path).ToString());
		    Console.WriteLine(Directory.GetLastAccessTime(path).ToString());
		    Console.WriteLine(Directory.GetLastWriteTime(path).ToString());
	    }

	    public static void ModifyDirectoryTimestamps(string path, DateTime dt)
	    {
		    Directory.SetCreationTime(path, dt);
		    Directory.SetLastAccessTime(path, dt);
		    Directory.SetLastWriteTime(path, dt);
	    }

        public static void DisplayDirectoryHiddenAttribute(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            // Display whether this directory is hidden
            Console.WriteLine("Is directory hidden? = " +
                ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden));
        }

		#endregion

		#region "12.5 Renaming a Directory"
		public static void RenameDirectory()
		{
			string path = Path.GetTempPath() + "MyTemp";
			string newpath = path + "1";
			Directory.CreateDirectory(path);

			RenameDirectory(path,newpath);
            DirectoryInfo dirInfo = new DirectoryInfo(newpath);
			dirInfo.Rename(path);
		}

	    public static void RenameDirectory(string originalName, string newName)
	    {
		    try
		    {
			    // "rename" it
			    Directory.Move(originalName, newName);
		    }
		    catch(IOException ioe) 
		    {
			    // most likely given the directory exists or isn't empty
			    Console.WriteLine(ioe.ToString());
		    }
	    }

		#endregion

		#region "12.6 Searching for Directories or Files Using Wildcards"
		public static void SearchDirFileWildcards()
		{
			try
			{
				// set up initial paths
				string tempPath = Path.GetTempPath();		
				string tempDir = tempPath + @"\MyTempDir";
				string tempDir2 = tempDir + @"\MyNestedTempDir";
				string tempDir3 = tempDir + @"\MyNestedTempDirPattern";
				string tempFile = tempDir + @"\MyTempFile.PDB";
				string tempFile2 = tempDir2 + @"\MyNestedTempFile.PDB";
				string tempFile3 = tempDir + @"\MyTempFile.TXT";

				// create temp dirs and files
				Directory.CreateDirectory(tempDir);
				Directory.CreateDirectory(tempDir2);
				Directory.CreateDirectory(tempDir3);
				FileStream stream = File.Create(tempFile);
				FileStream stream2 = File.Create(tempFile2);
				FileStream stream3 = File.Create(tempFile3);
				// close files before using
				stream.Close();
				stream2.Close();
				stream3.Close();

				// print out all of the items in the temp dir
				DisplayFilesForADirectory(tempDir);

				// print out the items matching a pattern
				string pattern = "*.PDB";
				DisplayFilesWithPattern(tempDir,pattern);

				// print out the directories in the path
				DisplayDirectories(tempDir);

				// print out directories matching a pattern
				pattern = "*Pattern*";
				DisplayDirectoriesWithPattern(tempDir,pattern);
				
				// print out all files
				DisplayFiles1(tempDir);

				// print out all files matching a pattern
				pattern = "*.PDB";
				DisplayFiles3(tempDir,pattern);

				// Now use methods that return actual objects instead of just the string path
				
				// get the items
                DisplayDirectoryContents(tempDir);

				// get the items that match the pattern
				pattern = "*.PDB";
                DisplayDirectoryContentsWithPattern(tempDir, pattern);

				// get the directory infos
				DisplayDirectoriesFromInfo(tempDir);

				// get the directory infos that match a pattern
				pattern = "*Pattern*";
				DisplayDirectoriesWithPattern(tempDir,pattern);

				// get the file infos
				DisplayFiles2(tempDir);

				// get the file infos that match a pattern
				pattern = "*.PDB";
				DisplayFiles4(tempDir,pattern);

				// clean up temp stuff we looked at
				File.Delete(tempFile3);
				File.Delete(tempFile2);
				File.Delete(tempFile);
				Directory.Delete(tempDir3);
				Directory.Delete(tempDir2);
				Directory.Delete(tempDir);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void DisplayFilesForADirectory(string path)
		{
			string[] items = Directory.GetFileSystemEntries(path);
			foreach (string item in items)
			{
				Console.WriteLine(item);
			}
		}
		
		public static void DisplayDirectories(string path)
		{
            string[] items = Directory.GetDirectories(path);
			foreach (string item in items)
			{
				Console.WriteLine(item);
			}
		}

		public static void DisplayFiles1(string path)
		{
			string[] items = Directory.GetFiles(path);
			foreach (string item in items)
			{
				Console.WriteLine(item);
			}
		}

	    public static void DisplayDirectoryContents(string path)
	    {
            DirectoryInfo mainDir = new DirectoryInfo(path);
            IEnumerable<string> fileSystemDisplayInfos =
                from fsi in mainDir.GetFileSystemInfos()
                where fsi is FileSystemInfo || fsi is DirectoryInfo
                select fsi.ToDisplayString();

            foreach (string s in fileSystemDisplayInfos)
            {
                Console.WriteLine(s);
            }
        }

		public static void DisplayDirectoriesFromInfo(string path)
		{
			DirectoryInfo mainDir = new DirectoryInfo(path);
			DirectoryInfo[] items = mainDir.GetDirectories();
			foreach (DirectoryInfo item in items)
			{
				Console.WriteLine("DIRECTORY: " + item.Name);
			}
		}

		public static void DisplayFiles2(string path)
		{
			DirectoryInfo mainDir = new DirectoryInfo(path);
			FileInfo[] items = mainDir.GetFiles();
			foreach (FileInfo item in items)
			{
				Console.WriteLine("FILE: " + item.Name);
			}
		}

		// The pattern passed in is a string equal to "*.PDB"
		public static void DisplayFilesWithPattern(string path, string pattern)
		{
			string[] items = Directory.GetFileSystemEntries(path, pattern);
			foreach (string item in items)
			{
				Console.WriteLine(item);
			}
		}

		// The pattern passed in is a string equal to "*.PDB"
		public static void DisplayDirectoriesWithPattern(string path, string pattern)
		{
			string [] items = Directory.GetDirectories(path, pattern);
			foreach (string item in items)
			{
				Console.WriteLine(item);
			}
		}

		// The pattern passed in is a string equal to "*.PDB"
		public static void DisplayFiles3(string path, string pattern)
		{
			string [] items = Directory.GetFiles(path, pattern);
			foreach (string item in items)
			{
				Console.WriteLine(item);
			}
		}

		// The pattern passed in is a string equal to "*.PDB"
	    public static void DisplayDirectoryContentsWithPattern(string path, string pattern)
	    {
            DirectoryInfo mainDir = new DirectoryInfo(path);
            IEnumerable<string> fileSystemDisplayInfos =
                from fsi in mainDir.GetFileSystemInfos(pattern)
                where fsi is FileSystemInfo || fsi is DirectoryInfo
                select fsi.ToDisplayString();

            foreach (string s in fileSystemDisplayInfos)
            {
                Console.WriteLine(s);
            }
	    }

		// The pattern passed in is a string equal to "*.PDB"
		public static void DisplayDirectoriesWithPatternFromInfo(string path, string pattern)
		{
			DirectoryInfo mainDir = new DirectoryInfo(path);
			DirectoryInfo[] items = mainDir.GetDirectories(pattern);
			foreach (DirectoryInfo item in items)
			{
				Console.WriteLine("DIRECTORY: " + item.Name);
			}
		}

		// The pattern passed in is a string equal to "*.PDB"
		public static void DisplayFiles4(string path, string pattern)
		{
			DirectoryInfo mainDir = new DirectoryInfo(path);
			FileInfo[] items = mainDir.GetFiles(pattern);
			foreach (FileInfo item in items)
			{
				Console.WriteLine("FILE: " + item.Name);
			}
		}

		#endregion

		#region "12.7 Obtaining the Directory Tree"
		public static void ObtainDirTree()
		{
			try
			{
				// set up items to find...
				string tempPath = Path.GetTempPath();		
				string tempDir = tempPath + @"\MyTempDir";
				string tempDir2 = tempDir + @"\Chapter 1 - The Beginning";
				string tempDir3 = tempDir2 + @"\Chapter 1a - The Rest Of The Beginning";
				string tempDir4 = tempDir2 + @"\IHaveAPDBFile";
				string tempFile = tempDir + @"\MyTempFile.PDB";
				string tempFile2 = tempDir2 + @"\MyNestedTempFile.PDB";
				string tempFile3 = tempDir3 + @"\Chapter 1 - MyDeepNestedTempFile.PDB";
				string tempFile4 = tempDir4 + @"\PDBFile.PDB";
				// Tree looks like this
				// TEMP\
				//		MyTempDir\
				//				Chapter 1 - The Beginning\
				//								Chapter 1a - The Rest Of The Beginning\
				//								IHaveAPDBFile
				//

				// create temp dirs and files
				Directory.CreateDirectory(tempDir);
				Directory.CreateDirectory(tempDir2);
				Directory.CreateDirectory(tempDir3);
				Directory.CreateDirectory(tempDir4);
				FileStream stream = File.Create(tempFile);
				FileStream stream2 = File.Create(tempFile2);
				FileStream stream3 = File.Create(tempFile3);
				FileStream stream4 = File.Create(tempFile4);
				// close files before using
				stream.Close();
				stream2.Close();
				stream3.Close();
				stream4.Close();

                //tempDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                // list all of the files using recursion
                Stopwatch watch1 = Stopwatch.StartNew();
                DisplayAllFilesAndDirectories(tempDir);
                watch1.Stop();
                Console.WriteLine("*************************");

                // list all of the files without recursion
                Stopwatch watch2 = Stopwatch.StartNew();
                DisplayAllFilesAndDirectoriesWithoutRecursion(tempDir);
                watch2.Stop();
                Console.WriteLine("*************************");
                Console.WriteLine("Recursive method time elapsed {0}", watch1.Elapsed.ToString());
                Console.WriteLine("Non-Recursive method time elapsed {0}", watch2.Elapsed.ToString());

                //// obtain a listing of all files with the extension of PDB  
                DisplayAllFilesWithExtensionWithoutRecursion(tempDir, ".PDB");

				// clean up temp stuff we looked at
				File.Delete(tempFile4);
				File.Delete(tempFile3);
				File.Delete(tempFile2);
				File.Delete(tempFile);
				Directory.Delete(tempDir4);
				Directory.Delete(tempDir3);
				Directory.Delete(tempDir2);
				Directory.Delete(tempDir);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}



        public static void DisplayAllFilesAndDirectories(string dir)
        {
            var strings = from fileSystemInfo in GetAllFilesAndDirectories(dir)
                          select fileSystemInfo.ToDisplayString();

            foreach (string s in strings)
                Console.WriteLine(s);
        }

        public static void DisplayAllFilesWithExtension(string dir, string extension)
        {
            var strings = from fileSystemInfo in GetAllFilesAndDirectories(dir)
                          where fileSystemInfo is FileInfo &&
                                fileSystemInfo.FullName.Contains("Chapter 1") &&
                                (string.Compare(fileSystemInfo.Extension, extension,
                                                StringComparison.OrdinalIgnoreCase) == 0) 
                          select fileSystemInfo.ToDisplayString();

            foreach (string s in strings)
                Console.WriteLine(s);
        }


        public static IEnumerable<FileSystemInfo> GetAllFilesAndDirectories(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            FileSystemInfo[] fileSystemInfos = dirInfo.GetFileSystemInfos();
            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
            {
                yield return fileSystemInfo;
                if (fileSystemInfo is DirectoryInfo)
                {
                    foreach (FileSystemInfo fsi in GetAllFilesAndDirectories(fileSystemInfo.FullName))
                        yield return fsi;
                }
            }
        }

        public static void DisplayAllFilesAndDirectoriesWithoutRecursion(string dir)
        {
            var strings = from fileSystemInfo in GetAllFilesAndDirectoriesWithoutRecursion(dir)
                          select fileSystemInfo.ToDisplayString();

            foreach (string s in strings)
                Console.WriteLine(s);
        }

        public static void DisplayAllFilesWithExtensionWithoutRecursion(string dir, string extension)
        {
            var strings = from fileSystemInfo in GetAllFilesAndDirectoriesWithoutRecursion(dir)
                          where fileSystemInfo is FileInfo &&
                                fileSystemInfo.FullName.Contains("Chapter 1") &&
                                (string.Compare(fileSystemInfo.Extension, extension,
                                                StringComparison.OrdinalIgnoreCase) == 0)
                          select fileSystemInfo.ToDisplayString();

            foreach (string s in strings)
                Console.WriteLine(s);
        }

        public static IEnumerable<FileSystemInfo> GetAllFilesAndDirectoriesWithoutRecursion(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            Stack<FileSystemInfo> stack = new Stack<FileSystemInfo>();

            stack.Push(dirInfo);
            while (dirInfo != null || stack.Count > 0)
            {
                FileSystemInfo fileSystemInfo = stack.Pop();
                DirectoryInfo subDirectoryInfo = fileSystemInfo as DirectoryInfo;
                if (subDirectoryInfo != null)
                {
                    yield return subDirectoryInfo;
                    foreach (FileSystemInfo fsi in subDirectoryInfo.GetFileSystemInfos())
                        stack.Push(fsi);
                    dirInfo = subDirectoryInfo;
                }
                else
                {
                    yield return fileSystemInfo;
                    dirInfo = null;
                }
            }
        }





        //public static IEnumerable<T> PreorderTraversal<T>(Tree<T> node)
        //{
        //    Stack<Tree<T>> stack = new Stack<Tree<T>>();
        //    stack.Push(node);
        //    while (stack.Count > 0)
        //    {
        //        node = stack.Pop();
        //        if (node.Right != null) stack.Push(node.Right);
        //        if (node.Left != null) stack.Push(node.Left);
        //        yield return node.Value;
        //    }
        //}

		#endregion

		#region "12.8 Parsing a Path"
		public static void ParsePath()
		{
			ParsePath(@"c:\test\tempfile.txt");
		}

		public static void ParsePath(string path)
		{
			string root = Path.GetPathRoot(path);
			string dirName = Path.GetDirectoryName(path);
			string fullFileName = Path.GetFileName(path);
			string fileExt = Path.GetExtension(path);
			string fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);
			StringBuilder format = new StringBuilder();
			format.Append("ParsePath of {0} breaks up into the following pieces:\r\n\tRoot: {1}\r\n\t");
			format.Append("Directory Name: {2}\r\n\tFull File Name: {3}\r\n\t");
			format.Append("File Extension: {4}\r\n\tFile Name Without Extension: {5}\r\n");
			Console.WriteLine(format.ToString(),path,root,dirName,fullFileName,fileExt,fileNameWithoutExt);
		}

		#endregion

		#region "12.9 Parsing Paths in Environment Variables"
		public static void ParsePathsEnvironmentVariables()
		{
			ParsePathEnvironmentVariable();
		}

		public static void ParsePathEnvironmentVariable()
		{
	        string originalPathEnv = Environment.GetEnvironmentVariable("PATH");
	        string[] paths = originalPathEnv.Split(Path.PathSeparator);
	        foreach (string s in paths)
	        {
		        string pathEnv = Environment.ExpandEnvironmentVariables(s);            
		        if(!string.IsNullOrEmpty(pathEnv))
			        Console.WriteLine("Individual Path = " + pathEnv);
		        Console.WriteLine();
	        }
		}

		#endregion

		#region "12.10 Launching and Interacting with Console Utilities "
		public static void LaunchInteractConsoleUtilities()
		{
			RunProcessToReadStdIn();
		}

		public static void RunProcessToReadStdIn()
		{
			Process application = new Process();
			// run the command shell
			application.StartInfo.FileName = @"cmd.exe";

			// turn on standard extensions
			application.StartInfo.Arguments = "/E:ON";

			application.StartInfo.RedirectStandardInput = true;

			application.StartInfo.UseShellExecute = false;

			// start it up
			application.Start();

			// get stdin
			StreamWriter input = application.StandardInput;

			// run the command to display the time
			input.WriteLine("TIME /T");

			// stop the application we launched
			input.WriteLine("exit");
		}

		#endregion

        #region "12.11 Locking Subsections of a File"
        public static void LockSubsectionsOfAFile()
        {
            string path = Path.GetTempFileName();
            CreateLockedFile(path);
            File.Delete(path);
        }

        public static void CreateLockedFile(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName,
                      FileMode.Create,
                      FileAccess.ReadWrite,
                      FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine("The First Line");
                    streamWriter.WriteLine("The Second Line");
                    streamWriter.Flush();

                    try
                    {
                        // Lock all of the file.
                        fileStream.Lock(0, fileStream.Length);

                        // Do some lengthy processing here…
                        Thread.Sleep(1000);
                    }
                    finally
                    {
                        // Make sure we unlock the file.
                        // If a process terminates with part of a file locked or closes a file
                        // that has outstanding locks, the behavior is undefined which is MS
                        // speak for bad things….
                        fileStream.Unlock(0, fileStream.Length);
                    }

                    streamWriter.WriteLine("The Third Line");
                }
            }
        }

        #endregion

		#region "12.12 Waiting for an Action to Occur in the File System"
		public static void WaitFileSystemAction()
		{
			string tempPath = Path.GetTempPath();
			string fileName = "temp.zip";
			File.Delete(tempPath + fileName);
			
			WaitForZipCreation(tempPath,fileName);
		}

		public static void WaitForZipCreation(string path, string fileName)
		{
			FileSystemWatcher fsw = null;
			try
			{
				fsw = new FileSystemWatcher();
				string [] data = new string[] {path,fileName};
				fsw.Path = path;
				fsw.Filter = fileName;
				fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
					| NotifyFilters.FileName | NotifyFilters.DirectoryName;

				// Run the code to generate the file we are looking for
				// Normally you wouldn’t do this as another source is creating
				// this file
				if(ThreadPool.QueueUserWorkItem(new WaitCallback(PauseAndCreateFile),
					data))
				{
					Thread.Sleep(2000);
					// block waiting for change
					WaitForChangedResult result =
						fsw.WaitForChanged(WatcherChangeTypes.Created);
					Console.WriteLine("{0} created at {1}.", result.Name, path);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			
			// clean it up
			File.Delete(fileName);
    
			if(fsw != null)
				fsw.Dispose();
		}

		static void PauseAndCreateFile(Object stateInfo) 
		{
			try
			{
				string[] data = (string[])stateInfo;
				// wait a sec...
				Thread.Sleep(1000);
				// create a file in the temp directory
				string path = data[0];
				string file = path + data[1];
				Console.WriteLine("Creating {0} in PauseAndCreateFile...",file);
				FileStream fileStream = File.Create(file);
				fileStream.Close();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
		#endregion

		#region "12.13 Comparing Version Information of Two Executable Modules"
		public static void CompareVersionInfo()
		{
			// Version comparison
			string file1 = Path.GetTempFileName();
			string file2 = Path.GetTempFileName();
			FileSystemIO.FileComparison result = 
				FileSystemIO.CompareFileVersions(file1,file2);
			// cleanup
			File.Delete(file1);
			File.Delete(file2);
		}

		public enum FileComparison
		{
			Error = 0,
			Newer = 1,
			Older = 2,
			Same = 3
		}


        private static FileComparison ComparePart(int p1, int p2) 
        { 
            return p1 > p2 ? FileComparison.Newer : (p1 < p2 ? FileComparison.Older : FileComparison.Same );
        }

	    public static FileComparison CompareFileVersions(string file1, string file2)
	    {
		    FileComparison retValue = FileComparison.Error;
		    // get the version information
		    FileVersionInfo file1Version = FileVersionInfo.GetVersionInfo(file1);
		    FileVersionInfo file2Version = FileVersionInfo.GetVersionInfo(file2);

            retValue = ComparePart(file1Version.FileMajorPart, file2Version.FileMajorPart);
            if (retValue != FileComparison.Same)
            {
                retValue = ComparePart(file1Version.FileMinorPart, file2Version.FileMinorPart);
                if (retValue != FileComparison.Same)
                {
                    retValue = ComparePart(file1Version.FileBuildPart, file2Version.FileBuildPart);
                    if (retValue != FileComparison.Same)
                        retValue = ComparePart(file1Version.FilePrivatePart, file2Version.FilePrivatePart);
                }
            }
		    return retValue;
	    }

		#endregion

		#region "12.14 Querying Information for All Drives on a System"
		public static void TestAllDriveInfo()
		{
			DisplayOneDrivesInfo();
			DisplayAllDriveInfo();
		}
		
		public static void DisplayOneDrivesInfo()
		{
			DriveInfo drive = new DriveInfo("D");
			if (drive.IsReady)
				Console.WriteLine("The space available on the D:\\ drive: " + drive.AvailableFreeSpace);
			else
				Console.WriteLine("Drive D:\\ is not ready.");
	
			// If the drive is not ready you will get a:
			//   System.IO.IOException: The device is not ready.	
			Console.WriteLine();
		}
				
		public static void DisplayAllDriveInfo()
		{
			foreach (DriveInfo drive in DriveInfo.GetDrives())
			{
				if (drive.IsReady)
				{
					Console.WriteLine("Drive " + drive.Name + " is ready.");
					Console.WriteLine("AvailableFreeSpace: " + drive.AvailableFreeSpace);
					Console.WriteLine("DriveFormat: " + drive.DriveFormat);
					Console.WriteLine("DriveType: " + drive.DriveType);
					Console.WriteLine("Name: " + drive.Name);
					Console.WriteLine("RootDirectory.FullName: " + drive.RootDirectory.FullName);
					Console.WriteLine("TotalFreeSpace: " + drive.TotalFreeSpace);
					Console.WriteLine("TotalSize: " + drive.TotalSize);
					Console.WriteLine("VolumeLabel: " + drive.VolumeLabel);
				}
				else
				{
					Console.WriteLine("Drive " + drive.Name + " is not ready.");
				}
				
				Console.WriteLine();
			}
		}
		#endregion

		#region "12.15 Compressing and Decompressing Your Files"
        public static void TestCompressNewFile()
        {
	        byte[] data = new byte[10000000];
	        for (int i = 0; i < 10000000; i++)
		        data[i] = (byte)i;
        		
	        FileStream fs = 
                new FileStream(@"C:\NewNormalFile.txt", 
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
	        using(fs)
            {
                fs.Write(data,0,data.Length);
            }

            CompressFile(@"C:\NewNormalFile.txt", @"C:\NewCompressedFile.txt", 
                CompressionType.Deflate);

            DecompressFile(@"C:\NewCompressedFile.txt", @"C:\NewDecompressedFile.txt",
                CompressionType.Deflate);
        	
	        CompressFile(@"C:\NewNormalFile.txt", @"C:\NewGZCompressedFile.txt", 
                CompressionType.GZip);

            DecompressFile(@"C:\NewGZCompressedFile.txt", @"C:\NewGZDecompressedFile.txt",
                CompressionType.GZip);

	        //Normal file size == 10,000,000 bytes
            //GZipped file size == 155,204
            //Deflated file size == 155,168
	        // 36 bytes are related to the GZip CRC
        }

        /// <summary>
        /// Compress the source file to the destination file.
        /// This is done in 1MB chunks to not overwhelm the memory usage.
        /// </summary>
        /// <param name="sourceFile">the uncompressed file</param>
        /// <param name="destinationFile">the compressed file</param>
        /// <param name="compressionType">the type of compression to use</param>
        public static void CompressFile(string sourceFile, 
                                        string destinationFile, 
                                        CompressionType compressionType)
        {
            if (sourceFile != null)
            {
                FileStream streamSource = null;
                FileStream streamDestination = null;
                Stream streamCompressed = null;

                using (streamSource = File.OpenRead(sourceFile))
                {
                    using (streamDestination = File.OpenWrite(destinationFile))
                    {
                        // read 1MB chunks and compress them
                        long fileLength = streamSource.Length;

                        // write out the fileLength size
                        byte[] size = BitConverter.GetBytes(fileLength);
                        streamDestination.Write(size, 0, size.Length);

                        long chunkSize = 1048576; // 1MB
                        while (fileLength > 0)
                        {
                            // read the chunk
                            byte[] data = new byte[chunkSize];
                            streamSource.Read(data, 0, data.Length);

                            // compress the chunk
                            MemoryStream compressedDataStream =
                                new MemoryStream();

                            if (compressionType == CompressionType.Deflate)
                                streamCompressed =
                                    new DeflateStream(compressedDataStream,
                                        CompressionMode.Compress);
                            else
                                streamCompressed =
                                    new GZipStream(compressedDataStream,
                                        CompressionMode.Compress);

                            using (streamCompressed)
                            {
                                // write the chunk in the compressed stream
                                streamCompressed.Write(data, 0, data.Length);
                            }
                            // get the bytes for the compressed chunk
                            byte[] compressedData =
                                compressedDataStream.GetBuffer();

                            // write out the chunk size
                            size = BitConverter.GetBytes(chunkSize);
                            streamDestination.Write(size, 0, size.Length);

                            // write out the compressed size
                            size = BitConverter.GetBytes(compressedData.Length);
                            streamDestination.Write(size, 0, size.Length);

                            // write out the compressed chunk
                            streamDestination.Write(compressedData, 0,
                                compressedData.Length);

                            // subtract the chunk size from the file size
                            fileLength -= chunkSize;

                            // if chunk is less than remaining file use
                            // remaining file
                            if (fileLength < chunkSize)
                                chunkSize = fileLength;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function will decompress the chunked compressed file
        /// created by the CompressFile function.
        /// </summary>
        /// <param name="sourceFile">the compressed file</param>
        /// <param name="destinationFile">the destination file</param>
        /// <param name="compressionType">the type of compression to use</param>
        public static void DecompressFile(string sourceFile, 
                                        string destinationFile,
                                        CompressionType compressionType)
        {
            FileStream streamSource = null;
            FileStream streamDestination = null;
            Stream streamUncompressed = null;

            using (streamSource = File.OpenRead(sourceFile))
            {
                using (streamDestination = File.OpenWrite(destinationFile))
                {
                    // read the fileLength size
                    // read the chunk size
                    byte[] size = new byte[sizeof(long)];
                    streamSource.Read(size, 0, size.Length);
                    // convert the size back to a number
                    long fileLength = BitConverter.ToInt64(size, 0);
                    long chunkSize = 0;
                    int storedSize = 0;
                    long workingSet = Process.GetCurrentProcess().WorkingSet64;
                    while (fileLength > 0)
                    {
                        // read the chunk size
                        size = new byte[sizeof(long)];
                        streamSource.Read(size, 0, size.Length);
                        // convert the size back to a number
                        chunkSize = BitConverter.ToInt64(size, 0);
                        if (chunkSize > fileLength ||
                            chunkSize > workingSet)
                            throw new InvalidDataException();

                        // read the compressed size
                        size = new byte[sizeof(int)];
                        streamSource.Read(size, 0, size.Length);
                        // convert the size back to a number
                        storedSize = BitConverter.ToInt32(size, 0);
                        if (storedSize > fileLength ||
                            storedSize > workingSet)
                            throw new InvalidDataException();

                        if (storedSize > chunkSize)
                            throw new InvalidDataException();

                        byte[] uncompressedData = new byte[chunkSize];
                        byte[] compressedData = new byte[storedSize];
                        streamSource.Read(compressedData, 0,
                            compressedData.Length);

                        // uncompress the chunk
                        MemoryStream uncompressedDataStream =
                            new MemoryStream(compressedData);

                        if (compressionType == CompressionType.Deflate)
                            streamUncompressed =
                                new DeflateStream(uncompressedDataStream,
                                    CompressionMode.Decompress);
                        else
                            streamUncompressed =
                                new GZipStream(uncompressedDataStream,
                                    CompressionMode.Decompress);

                        using (streamUncompressed)
                        {
                            // read the chunk in the compressed stream
                            streamUncompressed.Read(uncompressedData, 0,
                                uncompressedData.Length);
                        }

                        // write out the uncompressed chunk
                        streamDestination.Write(uncompressedData, 0,
                            uncompressedData.Length);

                        // subtract the chunk size from the file size
                        fileLength -= chunkSize;

                        // if chunk is less than remaining file use remaining file
                        if (fileLength < chunkSize)
                            chunkSize = fileLength;
                    }
                }
            }
        }

		public enum CompressionType
		{
			Deflate,
			GZip
		}
		#endregion

	}
}
