using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Web.RegularExpressions;
using System.Linq;
using System.Xml.Linq;


namespace CSharpRecipes
{
	public class RegEx
	{
        #region "10.1 Enumerating Matches"
		
		public static void TestFindSubstrings()
		{
			string matchPattern = "<.*>";

            // New LINQ to XML syntax for constructing XML
            XDocument xDoc = new XDocument(
                                new XDeclaration("1.0", "UTF-8", "yes"),
                                new XComment("my comment"),
                                new XElement("Window", new XAttribute("ID", "Main"),
                                    new XElement("Control", new XAttribute("ID", "TextBox"),
                                       new XElement("Property", new XAttribute("Top", "0"), new XAttribute("Left", "0"), new XAttribute("Text", "BLANK"))),
                                    new XElement("Control", new XAttribute("ID", "Label"),
                                        new XElement("Property", new XAttribute("Top", "0"), new XAttribute("Left", "0"), new XAttribute("Caption", "Enter Name Here"))),
                                    new XElement("Control", new XAttribute("ID", "Label"),
                                        new XElement("Property", new XAttribute("Top", "0"), new XAttribute("Left", "0"), new XAttribute("Caption", "Enter Name Here")))
                                )
                             );

            string source = xDoc.ToString(SaveOptions.None);
            
			Console.WriteLine("UNIQUE MATCHES");
			Match[] x1 = FindSubstrings(source, matchPattern, true);
			foreach(Match m in x1)
			{
				Console.WriteLine(m.Value);
			}

			Console.WriteLine();
			Console.WriteLine("ALL MATCHES");
			Match[] x2 = FindSubstrings(source, matchPattern, false);
			foreach(Match m in x2)
			{
				Console.WriteLine(m.Value);
			}
		}

		public static Match[] FindSubstrings(string source, string matchPattern, 
			bool findAllUnique)
		{
			SortedList uniqueMatches = new SortedList();
			Match[] retArray = null;

			Regex RE = new Regex(matchPattern);//, RegexOptions.Multiline);
			MatchCollection theMatches = RE.Matches(source);

			if (findAllUnique)
			{
				for (int counter = 0; counter < theMatches.Count; counter++)
				{
					if (!uniqueMatches.ContainsKey(theMatches[counter].Value))
					{
						uniqueMatches.Add(theMatches[counter].Value, theMatches[counter]);
					}
				}

				retArray = new Match[uniqueMatches.Count];
				uniqueMatches.Values.CopyTo(retArray, 0);
			}
			else
			{
				retArray = new Match[theMatches.Count];
				theMatches.CopyTo(retArray, 0);
			}

			return (retArray);
		}
		
		#endregion

        #region "10.2 Extracting Groups from a MatchCollection"
		public static void TestExtractGroupings()
		{
			string source = @"Path = ""\\MyServer\MyService\MyPath;
                              \\MyServer2\MyService2\MyPath2\""";
			string matchPattern = @"\\\\					# \\
									(?<TheServer>\w*)		# Server name
									\\						# \
									(?<TheService>\w*)\\	# Service name";

//			foreach(Match m in theMatches)
//			{
//				for (int counter = 0; counter < m.Groups.Count; counter++)
//				{
//					Console.WriteLine(m.Groups[0].GroupNameFromNumber(counter), m.Groups[counter]);
//				}
//			}


			foreach (Dictionary<string, Group> grouping in ExtractGroupings(source, matchPattern, true))
			{
				foreach (KeyValuePair<string, Group> kvp in grouping)
					Console.WriteLine("Key/Value = " + kvp.Key + " / " + kvp.Value);
				Console.WriteLine("");
			}
		}

		public static List<Dictionary<string, Group>> ExtractGroupings(string source, string matchPattern, 
													   bool wantInitialMatch)
		{
			List<Dictionary<string, Group>> keyedMatches = new List<Dictionary<string, Group>>();
			int startingElement = 1;
			if (wantInitialMatch)
			{
				startingElement = 0;
			}

			Regex RE = new Regex(matchPattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			MatchCollection theMatches = RE.Matches(source);

//			return (theMatches);
			
			
			foreach(Match m in theMatches)
			{
				Dictionary<string, Group> groupings = new Dictionary<string, Group>();

				for (int counter = startingElement; counter < m.Groups.Count; counter++)
				{
					groupings.Add(RE.GroupNameFromNumber(counter), m.Groups[counter]);
				}

				keyedMatches.Add(groupings);
			}

			return (keyedMatches);
		}
        #endregion

        #region "10.3 Verifying the Syntax of a Regular Expression"

		public static void TestUserInputRegEx(string regEx)
		{
			if (VerifyRegEx(regEx))
				Console.WriteLine("This is a valid regular expression.");
			else
				Console.WriteLine("This is not a valid regular expression.");
		}

		public static bool VerifyRegEx(string testPattern)
		{
			bool isValid = true;

			if ((testPattern != null) && (testPattern.Length > 0))
			{
				try
				{
					Regex.Match("", testPattern);
				}
				catch (ArgumentException ae)
				{
					// BAD PATTERN: Syntax error
					isValid = false;
					Console.WriteLine(ae);
				}
			}
			else
			{
				//BAD PATTERN: Pattern is null or empty
				isValid = false;
			}

			return (isValid);
		}
 
        #endregion

        #region "10.4 Quickly Finding Only The Last Match In A String"
		public static void TestFindLast()
		{
			Match theMatch = Regex.Match("one two three two one", "two", RegexOptions.RightToLeft);
			
			Regex RE = new Regex("two", RegexOptions.RightToLeft);
			theMatch = RE.Match("one two three two one");
			
			Console.WriteLine(theMatch.Value);
		}
		#endregion
		
        #region "10.5 Augmenting the Basic String Replacement Function"
		public static string MatchHandler(Match theMatch)
		{
			// Handle Top property of the Property tag
			if (theMatch.Value.StartsWith("<Property", StringComparison.Ordinal))
			{
				long topPropertyValue = 0;

				// Obtain the numeric value of the Top property
				Match topPropertyMatch = Regex.Match(theMatch.Value, "Top=\"([-]*\\d*)");
				if (topPropertyMatch.Success)
				{
					if (string.IsNullOrEmpty(topPropertyMatch.Groups[1].Value.Trim()))
					{
						// If blank, set to zero
						return (theMatch.Value.Replace("Top=\"\"", "Top=\"0\""));
					}
					else if (topPropertyMatch.Groups[1].Value.Trim().Equals("-"))
					{
						// If only a negative sign (syntax error), set to zero
						return (theMatch.Value.Replace("Top=\"-\"", "Top=\"0\""));
					}
					else
					{
						// We have a valid number
						// Convert the matched string to a numeric value
						topPropertyValue = long.Parse(topPropertyMatch.Groups[1].Value, 
							System.Globalization.NumberStyles.Any);

						// If the Top property is out of the specified range, set it to zero
						if (topPropertyValue < 0 || topPropertyValue > 5000)
						{
							return (theMatch.Value.Replace("Top=\"" + topPropertyValue + "\"", 
								"Top=\"0\""));
						}
					}
				}
			}

			return (theMatch.Value);
		}

		public static void ComplexReplace(string matchPattern, string source)
		{
			MatchEvaluator replaceCallback = new MatchEvaluator(MatchHandler);
			string newString = Regex.Replace(source, matchPattern, replaceCallback);

			Console.WriteLine("Replaced String = " + newString); 
		}

		public static void TestComplexReplace()
		{
			string matchPattern = "<.*>";
			string source = @"<?xml version=""1.0\"" encoding=\""UTF-8\""?>
        <Window ID=""Main"">
            <Control ID=""TextBox"">
                <Property Top=""100"" Left=""0"" Text=""BLANK""/>
            </Control>
            <Control ID=""Label"">
                <Property Top=""99990"" Left=""0"" Caption=""Enter Name Here""/>
            </Control>
        </Window>";

			ComplexReplace(matchPattern, source);
		}
        #endregion
        
        #region "10.6 A Better Tokenizer"
		public static string[] Tokenize(string equation)
		{
			Regex RE = new Regex(@"([\+\-\*\(\)\^\\])");
			return (RE.Split(equation));
		}

		public static void TestTokenize()
		{
			foreach(string token in Tokenize("(y - 3)(3111*x^21 + x + 320)"))
				Console.WriteLine("String token = " + token.Trim());
		}
        #endregion

        #region "10.7 Counting the Number of Lines of Text"
		public static long LineCount(string source, bool isFileName)
		{
			// Start Timer Code...
			Counter c = new Counter();
			c.Clear();
			c.Start();
			// Start Timer Code...
			
			
			if (source != null)
			{
				string text = source;

				if (isFileName)
				{
					FileStream FS = new FileStream(source, FileMode.Open, 
						FileAccess.Read, FileShare.Read);
					StreamReader SR = new StreamReader(FS);
					text = SR.ReadToEnd();
					SR.Close();
					FS.Close();
				}

				Regex RE = new Regex("\n", RegexOptions.Multiline);
				MatchCollection theMatches = RE.Matches(text);

				if (isFileName)
					Console.WriteLine("LineCount: " + (theMatches.Count).ToString());
				else
					Console.WriteLine("LineCount: " + (theMatches.Count + 1).ToString());
						
			
				// End Timer Code...
				c.Stop();
				Console.WriteLine("Seconds: " + c.Seconds.ToString());
				// End Timer Code...


				// Needed for files with zero length
				//   Note that a string will always have a line terminator and thus will
				//        always have a length of 1 or more
				if (isFileName)
				{
					return (theMatches.Count);
				}
				else
				{
					return (theMatches.Count) + 1;
				}
			}
			else
			{
				// End Timer Code...
				c.Stop();
				Console.WriteLine("Seconds: " + c.Seconds.ToString());
				// End Timer Code...


				// Handle a null source here
				return (0);
			}
		}

		public static long LineCount2(string source, bool isFileName)
		{
			// Start Timer Code...
			Counter c = new Counter();
			c.Clear();
			c.Start();
			// Start Timer Code...


			if (source != null)
			{
				string text = source;
				long numOfLines = 0;
				
				if (isFileName)
				{
					FileStream FS = new FileStream(source, FileMode.Open, 
						FileAccess.Read, FileShare.Read);
					StreamReader SR = new StreamReader(FS);
					
					while (text != null)
					{
						text = SR.ReadLine();
						
						if (text != null)
						{
							++numOfLines;
						}
					}
					
					SR.Close();
					FS.Close();

					Console.WriteLine("LineCount: " + numOfLines.ToString());
			
			
					// End Timer Code...
					c.Stop();
					Console.WriteLine("Seconds: " + c.Seconds.ToString());
					// End Timer Code...

					
					return (numOfLines);
				}
				else
				{
					Regex RE = new Regex("\n", RegexOptions.Multiline);
					MatchCollection theMatches = RE.Matches(text);

					Console.WriteLine("LineCount: " + (theMatches.Count + 1).ToString());
			
			
					// End Timer Code...
					c.Stop();
					Console.WriteLine("Seconds: " + c.Seconds.ToString());
					// End Timer Code...


					return (theMatches.Count + 1);
				}
			}
			else
			{
				// End Timer Code...
				c.Stop();
				Console.WriteLine("Seconds: " + c.Seconds.ToString());
				// End Timer Code...
			
			
				// Handle a null source here
				return (0);
			}
		}

		public static void TestLineCount()
		{
			// Count the lines within the file TestFile.txt
			LineCount(@"..\..\TestFile.txt", true);
			Console.WriteLine();
			
			// Count the lines within the string TestString
			LineCount("Line1\r\nLine2\r\nLine3\nLine4", false);
			Console.WriteLine();
			
			// Count the lines within the string TestString
			LineCount("", false);
			Console.WriteLine();

			// Count the lines within the file TestFile.txt
			LineCount2(@"..\..\TestFile.txt", true);
			Console.WriteLine();

			// Count the lines within the string TestString
			LineCount2("Line1\r\nLine2\r\nLine3\nLine4", false);
			Console.WriteLine();
			
			// Count the lines within the string TestString
			LineCount2("", false);
		}
        #endregion

        #region "10.8 Returning the Entire Line in Which a Match is Found"
		public static List<string> GetLines2(string source, string pattern, bool isFileName)
		{
			// Start Timer Code...
			Counter c = new Counter();
			c.Clear();
			c.Start();
			// Start Timer Code...
			
			
			string text = source;
			List<string> matchedLines = new List<string>();
			
			// If this is a file, get the entire file's text
			if (isFileName)
			{
				FileStream FS = new FileStream(source, FileMode.Open, 
					FileAccess.Read, FileShare.Read);
				StreamReader SR = new StreamReader(FS);
					
				while (text != null)
				{
					text = SR.ReadLine();

					if (text != null)
					{
						// Run the regex on each line in the string
						Regex RE = new Regex(pattern, RegexOptions.Multiline);
						MatchCollection theMatches = RE.Matches(text);

						if (theMatches.Count > 0)
						{
							// Get the line if a match was found
							matchedLines.Add(text);
						}
					}
				}
					
				SR.Close();
				FS.Close();
			}
			else
			{
				// Run the regex once on the entire string
				Regex RE = new Regex(pattern, RegexOptions.Multiline);
				MatchCollection theMatches = RE.Matches(text);

				// Get the line for each match
				foreach (Match m in theMatches)
				{
					int lineStartPos = GetBeginningOfLine(text, m.Index);
					int lineEndPos = GetEndOfLine(text, (m.Index + m.Length - 1));
					string line = text.Substring(lineStartPos, lineEndPos - lineStartPos);
					matchedLines.Add(line);
				}
			}
			
	
			// End Timer Code...
			c.Stop();
			Console.WriteLine("Seconds: " + c.Seconds.ToString());
			// End Timer Code...


			return (matchedLines);
		}


		public static List<string> GetLines(string source, string pattern, bool isFileName)
		{
			// Start Timer Code...
			Counter c = new Counter();
			c.Clear();
			c.Start();
			// Start Timer Code...
			
			
			string text = source;
			List<string> matchedLines = new List<string>();
			
			// If this is a file, get the entire file's text
			if (isFileName)
			{
				FileStream FS = new FileStream(source, FileMode.Open, 
					FileAccess.Read, FileShare.Read);
				StreamReader SR = new StreamReader(FS);
				text = SR.ReadToEnd();
				SR.Close();
				FS.Close();
			}

			// Run the regex once on the entire string
			Regex RE = new Regex(pattern, RegexOptions.Multiline);
			MatchCollection theMatches = RE.Matches(text);

			// Get the line for each match
			foreach (Match m in theMatches)
			{
				int lineStartPos = GetBeginningOfLine(text, m.Index);
				int lineEndPos = GetEndOfLine(text, (m.Index + m.Length - 1));
				string line = text.Substring(lineStartPos, lineEndPos - lineStartPos);
				matchedLines.Add(line);
			}

	
			// End Timer Code...
			c.Stop();
			Console.WriteLine("Seconds: " + c.Seconds.ToString());
			// End Timer Code...


			return (matchedLines);
		}

		public static int GetBeginningOfLine(string text, int startPointOfMatch)
		{
			if (startPointOfMatch > 0)
			{
				--startPointOfMatch;
			}
			
			if (startPointOfMatch >= 0 && startPointOfMatch < text.Length)
			{
				// Move to the left until the first '\n char is found
				for (int index = startPointOfMatch; index >= 0; index--)
				{
					if (text[index] == '\n')
					{
						return (index + 1);
					}
				}
				
				return (0);
			}
			
			return (startPointOfMatch);
		}
		
		public static int GetEndOfLine(string text, int endPointOfMatch)
		{
			if (endPointOfMatch >= 0 && endPointOfMatch < text.Length)
			{
				// Move to the right until the first '\n char is found
				for (int index = endPointOfMatch; index < text.Length; index++)
				{
					if (text[index] == '\n')
					{
						return (index);
					}
				}
				
				return (text.Length);
			}
			
			return (endPointOfMatch);
		}
		
		public static void TestGetLine()
		{
			// Get the matching lines within the file TestFile.txt
			Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
			List<string> lines = GetLines(@"..\..\TestFile.txt", "\n", true);
			foreach (string s in lines)
				Console.WriteLine("MatchedLine: " + s);

			// Get the matching lines within the string TestString
			Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
			lines = GetLines("Line1\r\nLine2\r\nLine3\nLine4", "Line", false);
			foreach (string s in lines)
				Console.WriteLine("MatchedLine: " + s);

			// Get the matching lines within the string TestString
			Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
			lines = GetLines("\rLLLLLL", "L", false);
			foreach (string s in lines)
				Console.WriteLine("MatchedLine: " + s);
				
			// Get the matching lines within the file TestFile.txt
			Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
			lines = GetLines2(@"..\..\TestFile.txt", "\n", true);
			foreach (string s in lines)
				Console.WriteLine("MatchedLine: " + s);

			// Get the matching lines within the string TestString
			Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
			lines = GetLines2("Line1\r\nLine2\r\nLine3\nLine4", "Line", false);
			foreach (string s in lines)
				Console.WriteLine("MatchedLine: " + s);

			// Get the matching lines within the string TestString
			Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
			lines = GetLines2("\rLLLLLL", "L", false);
			foreach (string s in lines)
				Console.WriteLine("MatchedLine: " + s);
		}
		#endregion
		
		#region "10.9 Finding a Particular Occurrence of a Match"
		public static Match FindOccurrenceOf(string source, string pattern, int occurrence)
		{
			if (occurrence < 1)
			{
				throw (new ArgumentException("Cannot be less than 1", "occurrence"));
			}

			// Make occurrence zero-based
			--occurrence;

			// Run the regex once on the source string
			Regex RE = new Regex(pattern, RegexOptions.Multiline);
			MatchCollection theMatches = RE.Matches(source);

			if (occurrence >= theMatches.Count)
			{
				return (null);
			}
			else
			{
				return (theMatches[occurrence]);
			}
		}

		public static List<Match> FindEachOccurrenceOf(string source, string pattern, int occurrence)
		{
			List<Match> occurrences = new List<Match>();

			// Run the regex once on the source string
			Regex RE = new Regex(pattern, RegexOptions.Multiline);
			MatchCollection theMatches = RE.Matches(source);

			for (int index = (occurrence - 1); index < theMatches.Count; index += occurrence)
			{
				occurrences.Add(theMatches[index]);
			}

			return (occurrences);
		}
		
		public static void TestOccurrencesOf()
		{
			Match matchResult = FindOccurrenceOf("one two three one two three one two three one"
				+ " two three one two three one two three", "two", 2);
			if (matchResult != null)
				Console.WriteLine(matchResult.ToString() + "\t" + matchResult.Index);

			Console.WriteLine();
			List<Match> results = FindEachOccurrenceOf("one one two three one two three one two" + 
				" three one two three", "one", 2);
			foreach (Match m in results)
				Console.WriteLine(m.ToString() + "\t" + m.Index);
		}
		#endregion

		#region "10.10 Common Patterns"
		/*
Match only alphanumeric characters along with the characters ?, +, ., and any whitespace:
	
	    ^([\w\.+?]|\s)*$

Be careful using the - character within a character class—a regular expression enclosed within [ and ]. That character is also used to specify a range of characters, as in a-z for a through z inclusive. If you want to use a literal - character, either escape it with \ or put it at the end of the expression, as shown in the previous and next examples.

Match only alphanumeric characters along with the characters ?, +, ., and any whitespace, with the stipulation that there is at least one of these characters and no more than 10 of these characters:
	
	    ^([\w\.+?]|\s){1,10}$
Match a person’s name, up to 55 characters:
	
	    ^[a-zA-Z\'\-\s]{1,55}$
Match a positive or negative integer:
	
	    ^((\+|\?)\d)?\d+*$
Match a positive or negative floating point number only, this pattern does not match integers:
	
	    ^(\+|\?)?(\d*\.\d+)$
Match a floating point or integer number that can have a positive or negative value:

	    ^(\+|\?)?(\d*\.)?\d+|\d+)$
Match a date in the form ##/##/####, where the day and month can be a one- or two-digit value and year can either only be a two- or four-digit value:
	
	    ^\d{1,2}\/\d{1,2}\/\d{2,4}$
Match a time to be entered in the form ##:## with an optional am or pm extension (note that this regular expression also handles military time):
	
	    ^\d{1,2}:\d{2}\s?([ap]m)?$
Verify if the input is a Ssocial sSecurity number of the form ###-##-####:
	
	    ^\d{3}-\d{2}-\d{4}$
Match an IPv4 address:
	
	    ^([0-2]?[0-95]?[0-95]\.){3}[0-2]?[0-95]?[0-95]$
Verify that an email address is in the form name@address where address is not an IP address:
	
	    ^[A-Za-z0-9_\-\.]+@(([A-Za-z0-9\-])+\.)+([A-Za-z\-])+$
Verify that an email address is in the form name@address where address is an IP address:
	
	    ^[A-Za-z0-9_\-\.]+@([0-2]?[0-95]?[0-95]\.){3}[0-2]?[0-59]?[0-95]$
Match or verify a URL that uses either the HTTP, HTTPS, or FTP protocol. Note that this regular expression will not match relative URLs:.
	
	    ^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-
	    9\-\._\?\,\'/\\\+&%\$#\=~])*$
Match only a dollar amount with the optional $ and + or -preceding characters (note that any number of decimal places may be added):
	
	    ^\$?[+-]?[\d,]*(\.\d*)?$
This is similar to the previous regular expression, except that no more than two decimal places are allowed:

	    ^\$?[+-]?[\d,]*\.?\d{0,2}$
Match a credit card number to be entered as four sets of four digits separated with a space, -, or no character at all:
	
	    ^((\d{4}[- ]?){3}\d{4})$
Match a zip code to be entered as five digits with an optional four-digit extension:
	
	    ^\d{5}(-\d{4})?$
Match a North American phone number with an optional area code and an optional - character to be used in the phone number and no extension:
	
	    ^(\(?[0-9]{3}\)?)?\-?[0-9]{3}\-?[0-9]{4}$
Match a phone number similar to the previous regular expression but allow an optional five-digit extension prefixed with either ext or extension:
	
	    ^(\(?[0-9]{3}\)?)?\-?[0-9]{3}\-?[0-9]{4}(\s*ext(ension)?[0-9]{5})?$
Match a full path beginning with the drive letter and optionally match a filename with a three-character extension (note that no .. characters signifying to move up the directory hierarchy are allowed, nor is a directory name with a . followed by an extension):
	
	    ^[a-zA-Z]:[\\/]([_a-zA-Z0-9]+[\\/]?)*([_a-zA-Z0-9]+\.[_a-zA-Z0-9]{0,3})?$
Verify if the input password string matches some specific rules for entering a password (i.e., the password is between 6 and 25 characters in length and contains alphanumeric characters):
	
	    ^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,25}$
Determine if any malicious characters were input by the user. Note that this regular expression will not prevent all malicious input, and it also prevents some valid input, such as last names that contain a single quote:.
	
	    ^([^\)\(\<\>\"\'\%\&\+\;][(-{2})])*$
Extract a tag from an XHTML, HTML, or XML string. This regular expression will return the beginning tag and ending tag, including any attributes of the tag. Note that you will need to replace TAGNAME with the real tag name you want to search for:. 
	
	    <TAGNAME.*?>(.*?)</TAGNAME>
Extract a comment line from code. The following regular expression extracts HTML comments from a web page. This can be useful in determining if any HTML comments that are leaking sensitive information need to be removed from your code base before it goes into production:. 
	
	    <!--.*?-->
Match a C# single line comment:

	    //.*$
Match a C# multi-line comment:

	    /\*.*?\*/
		#endregion
	}
	
	
	
	#region Timing Code Helper Class
	// Downloaded code...
	public class Counter 
	{
		long elapsedCount = 0;
		long startCount = 0;

		public void Start()
		{
			startCount = 0;
			QueryPerformanceCounter(ref startCount);
		}
		
		public void Stop()
		{
			long stopCount = 0;
			QueryPerformanceCounter(ref stopCount);

			elapsedCount += (stopCount - startCount);
		}

		public void Clear()
		{
			elapsedCount = 0;
		}

		public float Seconds
		{
			get
			{
				long freq = 0;
				QueryPerformanceFrequency(ref freq);
				return((float) elapsedCount / (float) freq);
			}
		}

		public override string ToString()
		{
			return String.Format("{0} seconds", Seconds);
		}

		static long Frequency 
		{
			get 
			{
				long freq = 0;
				QueryPerformanceFrequency(ref freq);
				return freq;
			}
		}
		static long Value 
		{
			get 
			{
				long count = 0;
				QueryPerformanceCounter(ref count);
				return count;
			}
		}

		[System.Runtime.InteropServices.DllImport("KERNEL32")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		private static extern bool QueryPerformanceCounter(  ref long lpPerformanceCount);

		[System.Runtime.InteropServices.DllImport("KERNEL32")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		private static extern bool QueryPerformanceFrequency( ref long lpFrequency);                     
	}
	#endregion
}
