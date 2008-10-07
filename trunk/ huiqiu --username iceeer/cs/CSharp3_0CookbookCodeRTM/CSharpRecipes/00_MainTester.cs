using System;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRecipes
{
    class MainTester
    {
        #region "(1) LINQ CHAPTER TEST CODE"
        static void TestLINQ()
        {
            LINQ.TestWeightedMovingAverage();
            LINQ.TestSetSemantics();
            LINQ.TestCompiledQuery();
            LINQ.TestLinqMessageQueue();
            LINQ.TestLinqForCulture();
            LINQ.TestQueryConfig();
            LINQ.TestLinqToDataSet();
            LINQ.TestXmlFromDatabase();
            LINQ.TestTakeSkipWhile();
            LINQ.TestUsingNonIEnumT();
        }
        #endregion // (1) LINQ CHAPTER TEST CODE

        #region "(2) STRING & CHARS CHAPTER TEST CODE"
        enum Colors
        {
            red, green, blue
        }

        static void TestStringsAndChars()
        {
            PrintHeader("String and Character Chapter Tests");
            char c = 'f';
            Console.WriteLine(c.GetCharKind());
            c = 'f';
			Console.WriteLine(c.GetCharKind());
            c = '0';
			Console.WriteLine(c.GetCharKind());
            c = '.';
			Console.WriteLine(c.GetCharKind());
            c = '}';
			Console.WriteLine(c.GetCharKind());
            string s = "abcdef";
			Console.WriteLine(s.GetCharKindInString(4));

			c = 'a';
			Console.WriteLine(c.IsCharEqual('a'));
			Console.WriteLine(c.IsCharEqual('b'));
			Console.WriteLine(c.IsCharEqual('A'));
			Console.WriteLine(c.IsCharEqual('b', false));
			Console.WriteLine(c.IsCharEqual('B', false));
			Console.WriteLine(c.IsCharEqual('A', false));
            Console.WriteLine(c.IsCharEqual('A', true));

            Console.WriteLine(c.IsCharEqual(CultureInfo.CurrentCulture, 'a', CultureInfo.InvariantCulture));
            Console.WriteLine(c.IsCharEqual(CultureInfo.CurrentCulture, 'b', CultureInfo.InvariantCulture));
            Console.WriteLine(c.IsCharEqual(CultureInfo.CurrentCulture, 'A', CultureInfo.CurrentCulture));
            Console.WriteLine(c.IsCharEqual(CultureInfo.CurrentCulture, 'b', CultureInfo.CurrentCulture, false));
            Console.WriteLine(c.IsCharEqual(CultureInfo.CurrentCulture, 'B', CultureInfo.CurrentCulture, false));
            Console.WriteLine(c.IsCharEqual(CultureInfo.CurrentCulture, 'A', CultureInfo.CurrentCulture, false));
            Console.WriteLine(c.IsCharEqual(CultureInfo.CurrentCulture, 'A', CultureInfo.CurrentCulture, true));


            c = 'a';
            foreach (int i in c.FindAll("asdf", 0))
				Console.WriteLine(i);
			Console.WriteLine();
            c = 'd';
            foreach (int i in c.FindAll("asdf", 0))
				Console.WriteLine(i);
			Console.WriteLine();
            c = 'g';
            foreach (int i in c.FindAll("asdf", 0))
				Console.WriteLine(i);
			Console.WriteLine();
            c = 'a';
            foreach (int i in c.FindAll("asdf", 1))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Red";
			foreach (int i in s.FindAll("BlueTealRedredGreenRedYellow", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Blue";
			foreach (int i in s.FindAll("BlueTealRedredGreenRedYellow", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Redd";
			foreach (int i in s.FindAll("BlueTealRedredGreenRedYellow", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Red";
			foreach (int i in s.FindAll("BlueTealRedredGreenRedYellow", 10))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "a";
			foreach (int i in s.FindAny("asdf", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			char[] ca = new char[1] { 'a' };
			foreach (int i in ca.FindAny("asdf", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "d";
			foreach (int i in s.FindAny("asdf", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "g";
			foreach (int i in s.FindAny("asdf", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			ca = new char[1] { 'a' };
			foreach (int i in ca.FindAny("asdf", 1))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Red";
			foreach (int i in s.FindAny("BlueTealRedredGreenRedYellow", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			ca = new char[3] { 'R', 'e', 'd' };
			foreach (int i in ca.FindAny("BlueTealRedredGreenRedYellow", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Blue";
			foreach (int i in s.FindAny("BlueTealRedredGreenRedYellow", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Redd";
			foreach (int i in s.FindAny("BlueTealRedredGreenRedYellow", 0))
				Console.WriteLine(i);
			Console.WriteLine();
			s = "Red";
			foreach (int i in s.FindAny("BlueTealRedredGreenRedYellow", 10))
				Console.WriteLine(i);
			Console.WriteLine();
            char[] data = new char[] {'R', 'r'};
            int[] allOccurances = data.FindAny("BlueTealRedredGreenRedYellow", 0);
            Console.WriteLine();

			StringsAndChars.TestCompareCaseControl();

			StringsAndChars.StringBeginEndComparisons();

			StringsAndChars.StringInsert();

			StringsAndChars.RemoveReplaceChars();

			string SourceString = "The Inserted Text is here -><-";
			SourceString = SourceString.Insert(28, "Insert-This");
			Console.WriteLine(SourceString);
			SourceString = "The Inserted Text is here -><-";
			char InsertChar = '1';
			SourceString = SourceString.Insert(28, Convert.ToString(InsertChar));
			Console.WriteLine(SourceString);
			StringBuilder SourceStringSB = new StringBuilder("The Inserted Text is here -><-");
			SourceStringSB.Insert(28, "Insert-This");
			Console.WriteLine(SourceStringSB);
			char CharToInsert = '1';
			SourceStringSB = new StringBuilder("The Inserted Text is here -><-");
			SourceStringSB.Insert(28, CharToInsert);
			Console.WriteLine(SourceStringSB);


            byte[] ba = new byte[5] { 32, 33, 34, 35, 36 };
			Console.WriteLine(ba.Base64EncodeBytes());
            foreach (byte b in ba.Base64EncodeBytes().Base64DecodeString())
				Console.WriteLine(b);

                        try
                        {
                FileStream fstrm = new FileStream(@"C:\WINDOWS\winnt.bmp", FileMode.Open, FileAccess.Read);
			    BinaryReader reader = new BinaryReader(fstrm);
			    byte[] image = new byte[reader.BaseStream.Length];
			    for (int i = 0; i < reader.BaseStream.Length; i++)
			    {
			    	image[i] = reader.ReadByte();
			    }
		 	    reader.Close();
			    fstrm.Close();
                string output = image.Base64EncodeBytes();
			    Console.WriteLine(output);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                // This is the code to embed CRLF after every 76 chars in the Base64 encoded
                //   string so that the entire encoded string may be sent as an embedded MIME
                //   attachment in an email (MIME attachements have a limit of 76 chars per line)
                StringBuilder originalStr = new StringBuilder(output);
                StringBuilder newStr = new StringBuilder();
                const int mimeBoundary = 76;
                int cntr = 1;
                while ((cntr * mimeBoundary) < (originalStr.Length - 1))
                {
                    newStr.AppendLine(originalStr.ToString(((cntr - 1) * mimeBoundary), mimeBoundary));
                    cntr++;
                }
                if (((cntr - 1) * mimeBoundary) < (originalStr.Length - 1))
                {
                    newStr.AppendLine(originalStr.ToString(((cntr - 1) * mimeBoundary), ((originalStr.Length) - ((cntr - 1) * mimeBoundary))));
                }
                Console.WriteLine(newStr.ToString());

                byte[] imageBytes = output.Base64DecodeString();
                fstrm = new FileStream(@"C:\winnt.bmp", FileMode.CreateNew, FileAccess.Write);
			    BinaryWriter writer = new BinaryWriter(fstrm);
			    writer.Write(imageBytes);
			    writer.Close();
			    fstrm.Close();

                File.Delete(@"C:\winnt.bmp");  // Remove this line to actually see the file
                       }
                       catch (System.IO.DirectoryNotFoundException dnfe)
                       {
                            Console.WriteLine(dnfe.ToString());
                       }

			byte[] SourceArray = {128, 83, 111, 117, 114, 99, 101, 
			                         32, 83, 116, 114, 105, 110, 103, 128};
			Console.WriteLine(StringsAndChars.FromASCIIByteArray(SourceArray));
			byte[] SourceArray2 = {128, 0, 83, 0, 111, 0, 117, 0, 114, 0, 99, 0, 
                                      101, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 128, 0};
			Console.WriteLine(StringsAndChars.FromUnicodeByteArray(SourceArray2));

			string SourceStr = "Source String";
			foreach (byte b in StringsAndChars.ToASCIIByteArray(SourceStr))
				Console.WriteLine(b);
			foreach (byte b in StringsAndChars.ToUnicodeByteArray(SourceStr))
				Console.WriteLine(b);

			float InitialValue = 0;
			int FinalValue = 0;
			InitialValue = (float)13.499;
			FinalValue = (int)InitialValue;
			Console.WriteLine(FinalValue.ToString());
			InitialValue = (float)13.5;
			FinalValue = (int)InitialValue;
			Console.WriteLine(FinalValue.ToString());
			InitialValue = (float)13.501;
			FinalValue = (int)InitialValue;
			Console.WriteLine(FinalValue.ToString());
			FinalValue = Convert.ToInt32((float)13.449);
			Console.WriteLine(FinalValue.ToString());
			FinalValue = Convert.ToInt32((float)13.5);
			Console.WriteLine(FinalValue.ToString());
			FinalValue = Convert.ToInt32((float)13.501);
			Console.WriteLine(FinalValue.ToString());

			string LongString = "7654321";
			Console.WriteLine(Int32.Parse(LongString));    // LongString = 7654321
			string DblString = "-7654.321";
            Console.WriteLine(Double.Parse(DblString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign));    // LongString = "-7654.321
			string BoolString = "true";
			Console.WriteLine(Boolean.Parse(BoolString));    // ActualBool = true
			string CharString = "t";
			Console.WriteLine(char.Parse(CharString));    // ActualChar = 't'
			string ColorString = "blue";
			Console.WriteLine((Colors)Colors.Parse(typeof(Colors), ColorString));	// ActualEnum = blue

			int ID = 12345;
			double Weight = 12.3558;
			char Row = 'Z';
			string Section = "1A2C";
            string Output = string.Format(@"The item ID = {0:G} having weight = {1:G} is found in row {2:G} and section {3:G}", ID, Weight, Row, Section);
			Console.WriteLine(Output);
            Output = string.Format(@"The item ID = {0:N} having weight = {1:E} is found in row {2:E} and section {3:E}", ID, Weight, Row, Section);
			Console.WriteLine(Output);
            Output = string.Format(@"The item ID = {0:N} having weight = {1:N} is found in row {2:E} and section {3:D}", ID, Weight, Row, Section);
			Console.WriteLine(Output);
            Output = string.Format(@"The item ID = {0:(#####)} having weight = {1:0000.00 lbs} is found in row {2} and section {3}", ID, Weight, Row, Section);
			Console.WriteLine(Output);
            Console.WriteLine(@"The item ID = {0,5:G} having weight = {1,10:G} is found in row {2,-5:G} and section {3,-10:G}", ID, Weight, Row, Section);

            string[] InfoArray = new string[5] { "11", "12", "Checking", "111", "Savings" };
			Console.WriteLine(string.Join(",", InfoArray));
			InfoArray = new string[4] { "11", "12", "Checking", "Savings" };
			string DelimitedInfoBegin = string.Join(",", InfoArray, 0, 2);
			string DelimitedInfoEnd = string.Join(",", InfoArray, 2, 2);
			string[] DelimitedInfoTotal = new string[2] {DelimitedInfoBegin, 
			                                                DelimitedInfoEnd};
			string DelimitedInfoFinal = string.Join(":", DelimitedInfoTotal);
			Console.WriteLine(DelimitedInfoFinal);

			string DelimitedInfo = "100,200,400,3,67";
			string[] DiscreteInfo = DelimitedInfo.Split(new char[1] { ',' });

			foreach (string Data in DiscreteInfo)
				Console.WriteLine(Data);

			System.Text.StringBuilder sbMax = new System.Text.StringBuilder(10, 10);
			sbMax.Append("123456789");
			sbMax.Append("0");
			try
			{
				sbMax.Append("#");
			}
			catch (ArgumentOutOfRangeException aoore)
			{
				Console.WriteLine(aoore.ToString());
			}

			string Equation = "1 + 2 - 4 * 5";
			string[] EquationTokens = Equation.Split(new char[1] { ' ' });
			foreach (string Tok in EquationTokens)
				Console.WriteLine(Tok);

			string FullName1 = "John Doe";
			string FullName2 = "Doe,John";
			string FullName3 = "John Q. Doe";
			string[] NameTokens1 = FullName1.Split(new char[3] { ' ', ',', '.' });
			string[] NameTokens2 = FullName2.Split(new char[3] { ' ', ',', '.' });
			string[] NameTokens3 = FullName3.Split(new char[3] { ' ', ',', '.' });
			foreach (string Tok in NameTokens1)
				Console.WriteLine(Tok);
			Console.WriteLine("");
			foreach (string Tok in NameTokens2)
				Console.WriteLine(Tok);
			Console.WriteLine("");
			foreach (string Tok in NameTokens3)
				Console.WriteLine(Tok);

			StringsAndChars.PruningChars();
			string Foo = "--TEST--";
			Console.WriteLine(Foo.Trim(new char[1] { '-' }));
			Foo = ",-TE-,ST-,-";
			Console.WriteLine(Foo.Trim(new char[2] { '-', ',' }));
			Foo = " --TEST-- ";
			Console.WriteLine(Foo.Trim(new char[2] { '-', ' ' }));
			Foo = "TEST";
			Console.WriteLine(Foo.Trim(new char[1] { '-' }));
			Foo = "--TEST--";
			Console.WriteLine(Foo.TrimStart(new char[1] { '-' }));
			Foo = ",-TEST-,-";
			Console.WriteLine(Foo.TrimStart(new char[2] { '-', ',' }));
			Foo = " --TEST-- ";
			Console.WriteLine(Foo.TrimStart(new char[2] { '-', ' ' }));
			Foo = "--TEST--";
			Console.WriteLine(Foo.TrimEnd(new char[1] { '-' }));
			Foo = ",-TEST-,-";
			Console.WriteLine(Foo.TrimEnd(new char[2] { '-', ',' }));
			Foo = " --TEST-- ";
			Console.WriteLine(Foo.TrimEnd(new char[2] { '-', ' ' }));

			StringsAndChars.TestStringForNullEmpty();

			StringsAndChars.AppendLine();
        }
        #endregion
        
        #region "(3) CLASS & STRUCT CHAPTER TEST CODE"
        static void TestClassesAndStructs()
        {
            PrintHeader("Classes and Structures Chapter Tests");
			ClassAndStructs.TestUnions();
			ClassAndStructs.TestSort();
			ClassAndStructs.TestSearch();
			ClassAndStructs.TestObjState();
			ClassAndStructs.ConvertObj(new ClassAndStructs.Base());
			ClassAndStructs.ConvertObj(new ClassAndStructs.Specific());
			ClassAndStructs.TestCloning();
        }
        #endregion

        #region "(4) GENERICS CHAPTER TEST CODE"
        static void TestGenerics()
        {
            Generics.TestGenericClassInstanceCounter();
            Generics.UseGenericStack();
            Generics.UseNonGenericStack();
            Generics.UseGenericQueue();
            Generics.UseNonGenericQueue();
            Generics.UseNonGenericArrayList();
            Generics.UseGenericList();
            Generics.TestAdapterVSConstructor();
            Generics.TestCloneVSGetRange();
            Generics.TestGenericRepeat();
            Generics.CloneGenericList();
            Generics.CloneQueue();
            Generics.CloneStack();
            Generics.UseNonGenericHashtable();
            Generics.MakeCollectionReadOnly();
            Generics.UseGenericDictionary();
            Generics.CloneGenericDictionary();
            Generics.CopyToGenericDictionary();
            Generics.ShowForEachWithDictionary();
            Generics.ShowSettingFieldsToDefaults();
            Generics.UseLinkedList();
            Generics.TestNullableStruct();
            Generics.TestDisposableListCls();
            Generics.TestComparableListCls();
            Generics.TestConversionCls();
            Generics.TestReversibleSortedList();
		}
        #endregion

		#region "(5) COLLECTIONS CHAPTER TEST CODE"
		static void TestCollections()
		{
			PrintHeader("Collections Chapter Tests");
			Collections.TestSwapArrayElements();
			Collections.TestArrayReversal();
            Collections.TestFlexibleStackTrace();
			Collections.TestArrayListEx();
			Collections.TestArrayListEx2();
			Collections.TestArrayInsertRemove();
			Collections.TestSortedList();
			Collections.TestSortKeyValues();
            Collections.TestMaxMinValueHash();
			Collections.TestDisplayDataAsDelStr();
			Collections.TestListSnapshot();
			Collections.TestSerialization();
			Collections.TestArrayForNulls();
			Collections.TestArrayForEach();
			Collections.TestReadOnlyArray();
		}
		#endregion

		#region "(6) ITERATORS AND PARTIAL CLASSES CHAPTER TEST CODE"
		static void TestIteratorsPartialClassesPartialMethods()
		{
            IteratorsAndPartialTypesAndPartialMethods.CreateNestedObjects();
            IteratorsAndPartialTypesAndPartialMethods.TestIterators();
            IteratorsAndPartialTypesAndPartialMethods.TestShoppingCart();
            IteratorsAndPartialTypesAndPartialMethods.TestStampCollection();
            IteratorsAndPartialTypesAndPartialMethods.TestIteratorsAndLinq();
            IteratorsAndPartialTypesAndPartialMethods.TestYieldBreak();
            IteratorsAndPartialTypesAndPartialMethods.TestFinallyAndIterators();
            IteratorsAndPartialTypesAndPartialMethods.TestPartialMethods();
		}
		#endregion

		#region "(7) EXCEPTION HANDLING CHAPTER TEST CODE"
        static void TestExceptionHandling()
        {
            PrintHeader("Exception Handling Chapter Tests");
            ExceptionHandling.NotPreventLossOfException();
            ExceptionHandling.PreventLossOfException();
            ExceptionHandling.ReflectionException();

            // Uncomment this to see the effects of an uncaught
            // exception and how you can at least be notified
            // before the application terminates...
            //ExceptionHandling.SetupLastChanceSEH();
            try
            {
                ExceptionHandling.TestToFullDisplayString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            ExceptionHandling.TestSpecializedException();
            Console.WriteLine(ExceptionHandling.GetStackTraceInfo(System.Environment.StackTrace));
            Console.WriteLine(ExceptionHandling.GetStackTraceDepth(System.Environment.StackTrace));
            try
            {
                ExceptionHandling.TestPollingAsyncDelegate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            ExceptionHandling.TestPollingAsyncDelegate();
            ExceptionHandling.TestExceptionData();
            ExceptionHandling.TestGetNestedSetOfExceptions();
        }
        #endregion

        #region "(8) DIAGNOSTICS CHAPTER TEST CODE"
        static void TestDiagnostics()
        {
            PrintHeader("Diagnostics Chapter Tests");
            Diagnostics.EnableTracing();
            Diagnostics.ProcessRespondingState state;
            foreach (Process P in Process.GetProcesses())
            {
                state = Diagnostics.IsProcessResponding(P);
                if (state == Diagnostics.ProcessRespondingState.NotResponding)
                {
                    Console.WriteLine("{0} is not responding.", P.ProcessName);
                }
            }
            Diagnostics.TestEventLogClass();
            Diagnostics.FindAnEntryInEventLog();
            Diagnostics.FindAnEntryInEventLog2();
            Diagnostics.WatchForAppEvent((EventLog.GetEventLogs())[0]);
            Diagnostics.TestCreateSimpleCounter();
            Diagnostics.TestTraceFactoryClass();
            Diagnostics.TestCaptureOutput();
            Diagnostics.TestCustomDebuggerDisplay();
        }
        #endregion
        		
        #region "(9) DELEGATES, EVENTS, FUNCTIONAL PROGRAMMING CHAPTER TEST CODE"
        static void TestDelegatesEventsFunctionalProgramming()
        {
            PrintHeader("Delegates, Events, Lambda Expressions Chapter Tests");
            DelegatesEventsLambdaExpressions.InvokeInReverse();
            DelegatesEventsLambdaExpressions.InvokeEveryOtherOperation();
            DelegatesEventsLambdaExpressions.InvokeWithTest();
            DelegatesEventsLambdaExpressions.TestIndividualInvokesReturnValue();
            DelegatesEventsLambdaExpressions.TestIndividualInvokesExceptions();
            DelegatesEventsLambdaExpressions.TestSimpleSyncDelegate();
            DelegatesEventsLambdaExpressions.TestSimpleAsyncDelegate();
            DelegatesEventsLambdaExpressions.TestComplexSyncDelegate();
            DelegatesEventsLambdaExpressions.TestCallbackAsyncDelegate();
            DelegatesEventsLambdaExpressions.FindSpecificInterfaces();
            DelegatesEventsLambdaExpressions.TestObserverPattern();
            DelegatesEventsLambdaExpressions.TestUsingLambdaExpressions();
            DelegatesEventsLambdaExpressions.TestParameterModifiers();
            DelegatesEventsLambdaExpressions.TestClosure();
            DelegatesEventsLambdaExpressions.TestFunctors();
			
        }
        #endregion
        
        #region "(10) REGULAR EXPRESSIONS CHAPTER TEST CODE"
        static void TestRegularExpressions()
        {
			PrintHeader("Regular Expressions Chapter Tests");
			RegEx.TestFindSubstrings();
			RegEx.TestExtractGroupings();
			RegEx.TestUserInputRegEx("");
			RegEx.TestUserInputRegEx(@"");
			RegEx.TestUserInputRegEx("foo");
			RegEx.TestUserInputRegEx(@"\\\");
			RegEx.TestUserInputRegEx(@"\\\\");
			RegEx.TestFindLast();
			RegEx.TestComplexReplace();
			RegEx.TestTokenize();
			RegEx.TestLineCount();
			RegEx.TestGetLine();
			RegEx.TestOccurrencesOf();
        }
        #endregion
        
        #region "(11) DATA STRUCTS AND ALGORITHMS CHAPTER TEST CODE"
        static void TestDataStructsAndAlgorithms()
        {
            PrintHeader("Data Structures and Algorithm Chapter Tests");
			DataStructsAndAlgorithms.CreateHashCodeDataType();
			DataStructsAndAlgorithms.CreatePriorityQueue();
			DataStructsAndAlgorithms.TestMultiMap();
			DataStructsAndAlgorithms.TestBinaryTree();
			DataStructsAndAlgorithms.TestManagedTreeWithNoBinaryTreeClass();
			DataStructsAndAlgorithms.TestNTree();
			DataStructsAndAlgorithms.TestSet();
        }
        #endregion
        
        #region "(12) FILE SYSTEM IO CHAPTER TEST CODE"
        static void TestFileSystemIO()
        {
            PrintHeader("File System IO Chapter Tests");

            FileSystemIO.ManipulateFileAttributes();
            FileSystemIO.RenameFile();
            FileSystemIO.PlatformIndependentEol();
            FileSystemIO.ManipulateDirectoryAttributes();
            FileSystemIO.RenameDirectory();
            FileSystemIO.SearchDirFileWildcards();
            FileSystemIO.ObtainDirTree();
            FileSystemIO.ParsePath();
            FileSystemIO.ParsePathsEnvironmentVariables();
            FileSystemIO.LaunchInteractConsoleUtilities();
            FileSystemIO.LockSubsectionsOfAFile();
            FileSystemIO.WaitFileSystemAction();
            FileSystemIO.CompareVersionInfo();
            FileSystemIO.TestAllDriveInfo();
            FileSystemIO.TestCompressNewFile();
        }
        #endregion
        
        #region "(13) REFLECTION CHAPTER TEST CODE"
        static void TestReflection()
        {
            PrintHeader("Reflection Chapter Tests");

            //Reflection.ListImportedAssemblies();
            //Reflection.ListExportedTypes();
            //Reflection.FindOverriddenMethods();
            //Reflection.FindMembersInAssembly();
            //Reflection.ObtainNestedTypes();
            //Reflection.DisplayInheritanceHierarchy();
            //Reflection.FindSubclasses();
            //Reflection.FindSerializableTypes();
            //Reflection.DynamicInvocation();
            //Reflection.TestIsGeneric();
            //Reflection.TestGetLocalVars();
            //Reflection.CreateMultiMap();
            //Reflection.TestGetIL();
        }
        #endregion

		#region "(14) WEB CHAPTER TEST CODE"
		static void TestWeb()
		{
            Web.ConvertIPToHostName();
            Web.ConvertingHostNameToIP();
            Web.ParsingUri();
            Web.HandlingWebServerErrors();
            Web.CommunicatingWithWebServer();
            Web.GoingThroughProxy();
            Web.ObtainingHtmlFromUrl();
            Web.TestEscapeUnescape();
            Web.TestUriBuilder();
            Web.TestCache();
            Web.TestBuildAspNetPages();
            Web.GetCustomErrorPageLocations();
            Web.GetCustomErrorPageLocationsLinq();
        }
		#endregion

		#region "(15) XML CHAPTER TEST CODE"
		static void TestXML()
		{
			PrintHeader("XML Chapter Tests");

            XML.AccessXml();
            XML.ReadXmlWeb();
            XML.QueryXml();
            XML.ValidateXml();
            XML.CreateXml();
            XML.DetectXmlChanges();
            XML.HandleInvalidChars();
            XML.TransformXml();
            XML.ProcessInvoice();
            XML.ReceiveInvoice();
            XML.TestContinualValidation();
            XML.TestExtendingTransformations();
            XML.TestBulkSchema();
            XML.TestXsltParameters();
		}
		#endregion

		#region "(16) NETWORKING CHAPTER TEST CODE"
		static void TestNetworking()
		{
			PrintHeader("Networking Chapter Tests");

            Networking.SimulatingFormExecution();
            Networking.UploadingDataToServer();
            Networking.DownloadingDataFromServer();
            Networking.TestPing();
            Networking.TestSendMail();
            Networking.TestSockets();
            Networking.GetInternetSettings();
            Networking.TestFtpDownload();
            Networking.TestFtpUpload();
		}
		#endregion

		#region "(17) SECURITY CHAPTER TEST CODE"
        static void TestSecurity()
        {
            PrintHeader("Security Chapter Tests");
            Security.ControlAccess();
            Security.EncDecString();
            Security.EncDecFile();
            Security.CleanUpCrypto();
            Security.VerifyNonStringCorruption();
            Security.SecurelyStoringData();
            Security.SafeAssert();
            Security.VerifyAssemblyPerms();
            Security.MinimizeAttackSurface();
            Security.TestSecureString();
            Security.TestUnicodeEncodingWithSecurity();
        }
        #endregion
        
        #region "(18) THREADING AND SYNCHRONIZATION CHAPTER TEST CODE"
        static void TestThreadingSync()
        {
            PrintHeader("Threading Chapter Tests");

            ThreadingSync.PerThreadStatic();
            ThreadingSync.ThreadSafeAccess();
            ThreadingSync.CompletionAsyncDelegate();
            ThreadingSync.PreventSilentTermination();
            ThreadingSync.StoreThreadDataPrivately();
            ThreadingSync.Halo3Session.Play();
            ThreadingSync.TestResetEvent();
            ThreadingSync.TestManualNamedEvent();
            ThreadingSync.TestAutoNamedEvent();
            ThreadingSync.TestInterlocked();
            ThreadingSync.TestReaderWriterLockSlim();
        }
        #endregion
        
		#region "(19) TOOLBOX CHAPTER TEST CODE"
		static void TestToolbox()
		{
            Toolbox.PreventBadShutdown();
            Toolbox.TestServiceManipulation();
            string searchAssm = "System.Data.dll";
            IEnumerable<Process> processes = Toolbox.GetProcessesAssemblyIsLoadedIn(searchAssm);
            foreach (Process p in processes)
            {
                Console.WriteLine("Found {0} in {1}", searchAssm, p.MainModule.ModuleName);
            }
            Toolbox.TestMessageQueue();
            Console.WriteLine(Toolbox.GetCurrentFrameworkPath());
            Toolbox.PrintGacRegisteredVersions("mscorlib");
            Toolbox.PrintGacRegisteredVersions("System.Web.dll");
            Toolbox.RedirectOutput();
            Toolbox.RunCodeInNewAppDomain();
            Console.WriteLine(Toolbox.GetOSAndServicePack()); 
        }
		#endregion

        #region "(20) NUMBERS AND ENUMERATIONS CHAPTER TEST CODE"
        static void TestNumbersAndEnums()
        {
            PrintHeader("Number Chapter Tests");
            Console.WriteLine(((float)1 / 3));						// .3333333
            Console.WriteLine(((float)1 / 3 == (float)0.33333));		// False
            Console.WriteLine(((float)1 / 3 == (float).3333333));		// True
            Console.WriteLine("double.Epsilon: " + double.Epsilon);
            Console.WriteLine("float.Epsilon: " + float.Epsilon);
            Console.WriteLine();

            Console.WriteLine(NumbersEnums.ConvertDegreesToRadians(0));
            Console.WriteLine(NumbersEnums.ConvertDegreesToRadians(90));
            Console.WriteLine(NumbersEnums.ConvertDegreesToRadians(180));
            Console.WriteLine(NumbersEnums.ConvertDegreesToRadians(270));
            Console.WriteLine(NumbersEnums.ConvertDegreesToRadians(360));
            Console.WriteLine(NumbersEnums.ConvertDegreesToRadians(12));

            Console.WriteLine(NumbersEnums.ConvertRadiansToDegrees(1.5707963267949));
            Console.WriteLine(NumbersEnums.ConvertRadiansToDegrees(3.14159265358979));
            Console.WriteLine(NumbersEnums.ConvertRadiansToDegrees(4.71238898038469));
            Console.WriteLine(NumbersEnums.ConvertRadiansToDegrees(6.28318530717959));
            Console.WriteLine(NumbersEnums.ConvertRadiansToDegrees(5));
            Console.WriteLine(NumbersEnums.ConvertRadiansToDegrees(6));
            Console.WriteLine();


            NumbersEnums.TestBitwiseOperators();
            Console.WriteLine();

            NumbersEnums.TestBase10();
            Console.WriteLine();

            NumbersEnums.TestDetermineIfStringIsNumber();
            Console.WriteLine();


            NumbersEnums.TestRound();
            Console.WriteLine();


            Console.WriteLine(NumbersEnums.CelsiusToFahrenheit(0));
            Console.WriteLine(NumbersEnums.CelsiusToFahrenheit(40));
            Console.WriteLine(NumbersEnums.CelsiusToFahrenheit(50));
            Console.WriteLine(NumbersEnums.CelsiusToFahrenheit(-50));
            Console.WriteLine();
            Console.WriteLine(NumbersEnums.CelsiusToKelvin(0));
            Console.WriteLine(NumbersEnums.CelsiusToKelvin(40));
            Console.WriteLine(NumbersEnums.CelsiusToKelvin(50));
            Console.WriteLine(NumbersEnums.CelsiusToKelvin(-50));
            Console.WriteLine();
            Console.WriteLine(NumbersEnums.FahrenheitToCelsius(0));
            Console.WriteLine(NumbersEnums.FahrenheitToCelsius(40));
            Console.WriteLine(NumbersEnums.FahrenheitToCelsius(50));
            Console.WriteLine(NumbersEnums.FahrenheitToCelsius(-50));
            Console.WriteLine();
            Console.WriteLine(NumbersEnums.FahrenheitToKelvin(0));
            Console.WriteLine(NumbersEnums.FahrenheitToKelvin(40));
            Console.WriteLine(NumbersEnums.FahrenheitToKelvin(50));
            Console.WriteLine(NumbersEnums.FahrenheitToKelvin(-50));
            Console.WriteLine();

            NumbersEnums.TestNarrowing();
            Console.WriteLine();


            PrintHeader("Enumerations Chapter Tests");
            NumbersEnums.TestDisplayEnumValue();
            NumbersEnums.TestConvertingEnums();
            NumbersEnums.HandleEnum(Language.All);
            NumbersEnums.HandleEnum(Language.CSharp);
            NumbersEnums.HandleEnum(Language.Other);
            NumbersEnums.HandleEnum(Language.VB6);
            NumbersEnums.HandleEnum(Language.VBNET);
            NumbersEnums.HandleEnum((Language)100);
            NumbersEnums.HandleEnum((Language)1);
            NumbersEnums.HandleEnum((Language)100);
            NumbersEnums.HandleEnum((Language)8);
            NumbersEnums.HandleEnum((Language)3);

            NumbersEnums.TestEnumFlags();

            NumbersEnums.TestTruncate();
        }
        #endregion
		
		static void Main()
        {
            try
            {
                // Call test code here...
                TestLINQ();
                TestStringsAndChars();
                TestClassesAndStructs();
                TestGenerics();
                TestCollections();
                TestIteratorsPartialClassesPartialMethods();
                TestExceptionHandling();
                try
                {
                    TestDiagnostics();
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine(e);
                }
                TestDelegatesEventsFunctionalProgramming();
                TestRegularExpressions();
                TestDataStructsAndAlgorithms();
                TestFileSystemIO();
                TestReflection();
                TestWeb();
                TestXML();
                TestNetworking();
                TestSecurity();
                TestThreadingSync();
                TestToolbox();
                TestNumbersAndEnums();
            }
            catch(Exception e)
            {
               Console.WriteLine(e.ToString());
            }

            // wait for enter to be pressed
            Console.WriteLine("Press ENTER to finish...");
            Console.ReadLine();
        }


        #region UtilityCode
        static void PrintHeader(string testtype)
        {
            Console.WriteLine("************************************************************");
            Console.WriteLine("**    Running " + testtype + "...");
            Console.WriteLine("************************************************************");
        }
        #endregion
    }
}
