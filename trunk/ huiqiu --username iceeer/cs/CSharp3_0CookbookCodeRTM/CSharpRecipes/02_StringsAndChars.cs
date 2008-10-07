using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace CSharpRecipes
{
    #region Extension Methods
    static class CharStrExtMethods
    {
        #region "2.1 Determining The Kind of Character"
		public static CharKind GetCharKind(this char theChar)
		{
			if (Char.IsControl(theChar))
			{
				return CharKind.Control;
			}
			else if (Char.IsDigit(theChar))
			{
				return CharKind.Digit;
			}
			else if (Char.IsLetter(theChar))
			{
				return CharKind.Letter;
			}
			else if (Char.IsNumber(theChar))
			{
				return CharKind.Number;
			}
			else if (Char.IsPunctuation(theChar))
			{
				return CharKind.Punctuation;
			}
			else if (Char.IsSeparator(theChar))
			{
				return CharKind.Separator;
			}
			else if (Char.IsSurrogate(theChar))
			{
				return CharKind.Surrogate;
			}
			else if (Char.IsSymbol(theChar))
			{
				return CharKind.Symbol;
			}
			else if (Char.IsWhiteSpace(theChar))
			{
				return CharKind.Whitespace;
			}
			else
			{
				return CharKind.Unknown;
			}
		}

		public static CharKind GetCharKindInString(this string theString, int charPosition)
		{
			if (Char.IsControl(theString, charPosition))
			{
				return CharKind.Control;
			}
			else if (Char.IsDigit(theString, charPosition))
			{
				return CharKind.Digit;
			}
			else if (Char.IsLetter(theString, charPosition))
			{
				return CharKind.Letter;
			}
			else if (Char.IsNumber(theString, charPosition))
			{
				return CharKind.Number;
			}
			else if (Char.IsPunctuation(theString, charPosition))
			{
				return CharKind.Punctuation;
			}
			else if (Char.IsSeparator(theString, charPosition))
			{
				return CharKind.Separator;
			}
			else if (Char.IsSurrogate(theString, charPosition))
			{
				return CharKind.Surrogate;
			}
			else if (Char.IsSymbol(theString, charPosition))
			{
				return CharKind.Symbol;
			}
			else if (Char.IsWhiteSpace(theString, charPosition))
			{
				return CharKind.Whitespace;
			}
			else
			{
				return CharKind.Unknown;
			}
		}
        #endregion

        #region "2.2 Controlling Case Sensitivity When Comparing Two Characters"
		public static bool IsCharEqual(this char firstChar, char secondChar)
		{
			return (IsCharEqual(firstChar, secondChar, false));
		}

		public static bool IsCharEqual(this char firstChar, char secondChar, bool caseSensitiveCompare)
		{
			if (caseSensitiveCompare)
			{
				return (firstChar.Equals(secondChar));
			}
			else
			{
                return (char.ToUpperInvariant(firstChar).Equals(char.ToUpperInvariant(secondChar)));
			}
		}

		public static bool IsCharEqual(this char firstChar, CultureInfo firstCharCulture, char secondChar, CultureInfo secondCharCulture)
		{
			return (IsCharEqual(firstChar, firstCharCulture, secondChar, secondCharCulture, false));
		}

		public static bool IsCharEqual(this char firstChar, CultureInfo firstCharCulture, char secondChar, CultureInfo secondCharCulture, bool caseSensitiveCompare)
		{
			if (caseSensitiveCompare)
			{
				return (firstChar.Equals(secondChar));
			}
			else
			{
				return (char.ToUpper(firstChar, firstCharCulture).Equals(char.ToUpper(secondChar, secondCharCulture)));
			}
		}
	    #endregion

		#region "2.3 Finding the Location of All Occurrences of a String Within Another String"
		public static int[] FindAll(this string matchStr, string searchedStr, int startPos)
		{
			int foundPos = -1;   // -1 represents not found
			int count = 0;
			List<int> foundItems = new List<int>();

			do
			{
				foundPos = searchedStr.IndexOf(matchStr, startPos, StringComparison.Ordinal);
				if (foundPos > -1)
				{
					startPos = foundPos + 1;
					count++;
					foundItems.Add(foundPos);

					Console.WriteLine("Found item at position: " + foundPos.ToString());
				}
			}while (foundPos > -1 && startPos < searchedStr.Length);

			return ((int[])foundItems.ToArray());
		}

        public static int[] FindAll(this char matchChar, string searchedStr, int startPos)
		{
			int foundPos = -1;   // -1 represents not found
			int count = 0;
			List<int> foundItems = new List<int>();

			do
			{
                foundPos = searchedStr.IndexOf(matchChar, startPos);
				if (foundPos > -1)
				{
					startPos = foundPos + 1;
					count++;
					foundItems.Add(foundPos);

					Console.WriteLine("Found item at position: " + foundPos.ToString());
				}
			}while (foundPos > -1 && startPos < searchedStr.Length);

			return ((int[])foundItems.ToArray());
		}

        public static int[] FindAny(this string matchStr, string searchedStr, int startPos)
		{
			int foundPos = -1;   // -1 represents not found
			int count = 0;
			List<int> foundItems = new List<int>();

			// Factor out case-sensitivity
            searchedStr = searchedStr.ToUpperInvariant();
            matchStr = matchStr.ToUpperInvariant();

			do
			{
                foundPos = searchedStr.IndexOf(matchStr, startPos, StringComparison.Ordinal);
				if (foundPos > -1)
				{
					startPos = foundPos + 1;
					count++;
					foundItems.Add(foundPos);

					Console.WriteLine("Found item at position: " + foundPos.ToString());
				}
			}while (foundPos > -1 && startPos < searchedStr.Length);

			return ((int[])foundItems.ToArray());
		}

        public static int[] FindAny(this char[] matchCharArray, string searchedStr, int startPos)
		{
			int foundPos = -1;   // -1 represents not found
			int count = 0;
			List<int> foundItems = new List<int>();

			do
			{
                foundPos = searchedStr.IndexOfAny(matchCharArray, startPos);
				if (foundPos > -1)
				{
					startPos = foundPos + 1;
					count++;
					foundItems.Add(foundPos);

					Console.WriteLine("Found item at position: " + foundPos.ToString());
				}
			}while (foundPos > -1 && startPos < searchedStr.Length);

			return ((int[])foundItems.ToArray());
		}
	    #endregion

	    #region "2.8 Decoding a Base64-encoded Binary"
		public static byte[] Base64DecodeString(this string inputStr)
		{
			byte[] encodedByteArray = Convert.FromBase64CharArray(inputStr.ToCharArray(), 0, inputStr.Length);
			return (encodedByteArray);
		}
	    #endregion

	    #region "2.9 Encoding a Binary as Base64"
		public static string Base64EncodeBytes(this byte[] inputBytes)
		{
			// Each 3 byte sequence in inputBytes must be converted to a 4 byte sequence
			long arrLength = (long)(4.0d * inputBytes.Length / 3.0d);
			if ((arrLength  % 4) != 0)
            {
				// increment the array length to the next multiple of 4 if it is not already divisible by 4
				arrLength += 4 - (arrLength % 4);
		    }

			char[] encodedCharArray = new char[arrLength];
			Convert.ToBase64CharArray(inputBytes, 0, inputBytes.Length, encodedCharArray, 0);

			return (new string(encodedCharArray));
        }
	    #endregion
    }
    #endregion

    public class StringsAndChars
	{
	    #region "2.4 Controlling Case Sensitivity When Comparing Two Strings"
	    public static void TestCompareCaseControl()
	    {
            string lowerCase = "abc";
            string upperCase = "AbC";

            //int caseSensitiveResult = string.Compare(lowerCase, upperCase, false);
            //int caseInsensitiveResult = string.Compare(lowerCase, upperCase, true);

            int caseInsensitiveResult = string.Compare(lowerCase, upperCase, 
StringComparison.CurrentCultureIgnoreCase);
            int caseSensitiveResult = string.Compare(lowerCase, upperCase, 
StringComparison.CurrentCulture);

            Console.WriteLine(caseSensitiveResult);
            Console.WriteLine(caseInsensitiveResult);
        }
	    #endregion

	    #region "2.5 Comparing a String to the Beginning or End of a Second String"
	    public static void StringBeginEndComparisons()
	    {
			string head = "str";
			string test = "strVarName";
			bool isFound = test.StartsWith(head, StringComparison.Ordinal);
            Console.WriteLine(isFound);
            
			string tail = "Name";
			test = "strVarName";
			isFound = test.EndsWith(tail, StringComparison.Ordinal);
            Console.WriteLine(isFound);

			head = "str";
			test = "strVarName";
			int location = string.Compare(head, 0, test, 0, head.Length, true, System.Threading.Thread.CurrentThread.CurrentCulture);
            Console.WriteLine(location);

			tail = "Name";
			test = "strVarName";
			if (tail.Length <= test.Length)
			{
                location = string.Compare(tail, 0, test, (test.Length - tail.Length), tail.Length, true, System.Threading.Thread.CurrentThread.CurrentCulture);
			}
			else
			{
			    location = -1;
			}
		}
	    #endregion

	    #region "2.6 Inserting Text into a String"
        public static void StringInsert()
        {
			string sourceString = "The Inserted Text is here -><-";

			sourceString  = sourceString.Insert(28, "Insert-This");
			Console.WriteLine(sourceString);

			char insertChar = '1';
			sourceString  = sourceString.Insert(28, Convert.ToString(insertChar));
			Console.WriteLine(sourceString);
		}
        #endregion

	    #region "2.7 Removing or Replacing Characters within a String"
        public static void RemoveReplaceChars()
        {
			string name = "Doe, John";
			name = name.Remove(3, 1);
			Console.WriteLine(name);

			StringBuilder str = new StringBuilder("1234abc5678", 12);
			str.Remove(4, 3);
			Console.WriteLine(str);

			string commaDelimitedString = "100,200,300,400,500";
			commaDelimitedString = commaDelimitedString.Replace(',', ':');
			Console.WriteLine(commaDelimitedString);

			string theName = "Mary";
			string theObject = "car";
			string ID = "This <ObjectPlaceholder> is the property of <NamePlaceholder>.";
			ID = ID.Replace("<ObjectPlaceholder>", theObject);
			ID = ID.Replace("<NamePlaceholder>", theName);
			Console.WriteLine(ID);

			string newName = "John Doe";

			str = new StringBuilder("name = <NAME>");
			str.Replace("<NAME>", newName);
			Console.WriteLine(str.ToString());
			str.Replace('=', ':');
			Console.WriteLine(str.ToString());

			str = new StringBuilder("name1 = <FIRSTNAME>, name2 = <FIRSTNAME>");
			str.Replace("<FIRSTNAME>", newName, 7, 12);
			Console.WriteLine(str.ToString());
			str.Replace('=', ':', 0, 7);
			Console.WriteLine(str.ToString());
		}
        #endregion

	    #region "2.10 Converting a String Returned as a Byte[] Back Into a String"
		public static string FromASCIIByteArray(byte[] characters)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			string constructedString = encoding.GetString(characters);

			return (constructedString);
		}

		public static string FromUnicodeByteArray(byte[] characters)
		{
			UnicodeEncoding encoding = new UnicodeEncoding();
			string constructedString = encoding.GetString(characters);

			return (constructedString);
		}
	    #endregion

	    #region "2.11 Passing a String to a Method that Accepts only a Byte[]"
		public static byte[] ToASCIIByteArray(string characters)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			int numberOfChars = encoding.GetByteCount(characters);
			byte[] retArray = new byte[numberOfChars];

			retArray = encoding.GetBytes(characters);

			return (retArray);
		}

		public static byte[] ToUnicodeByteArray(string characters)
		{
			UnicodeEncoding encoding = new UnicodeEncoding();
			int numberOfChars = encoding.GetByteCount(characters);
			byte[] retArray = new byte[numberOfChars];

			retArray = encoding.GetBytes(characters);

			return (retArray);
		}
        #endregion

        #region "2.12 Converting Strings to Their Equivalent Value Type"
        public static void ConvertStrToVal()
        {
			string longString = "7654321";
			int actualInt = Int32.Parse(longString);    // longString = 7654321
            Console.WriteLine(actualInt);
            
			string dblString = "-7654.321";
			double actualDbl = Double.Parse(dblString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);    // longString = "-7654.321
            Console.WriteLine(actualDbl);
            
			string boolString = "true";
			bool actualBool = Boolean.Parse(boolString);    // actualBool = true
            Console.WriteLine(actualBool);

			string charString = "t";
			char actualChar = char.Parse(charString);    // actualChar = 't'
            Console.WriteLine(actualChar);

			string colorString = "blue";
			// Note that the Parse method below is a method defined by System.Enum, not by Colors
			Colors actualEnum = (Colors)Colors .Parse(typeof(Colors), colorString);      // actualEnum = blue
            Console.WriteLine(actualEnum);
		}
        #endregion

        #region "2.13 Creating a Delimited String"
        public static void DelimitedStr()
        {
			string[] infoArray = {"11", "12", "Checking", "111", "Savings"};
			string delimitedInfo = string.Join(",", infoArray);
            Console.WriteLine(delimitedInfo);
        }
        #endregion

        #region "2.14 Extracting Items from a Delimited String"
		public static void DelimitedStrExtraction()
		{
			string delimitedInfo = "100,200,400,3,67";
			string[] discreteInfo = delimitedInfo.Split(new char[] {','});

			foreach (string Data in discreteInfo)
				Console.WriteLine(Data);
		}
		#endregion

        #region "2.15 Iterating Over Each Character in a String"
		public static void StringIterating()
		{
			string testStr = "abc123";
			foreach (char c in testStr)
			{
				Console.WriteLine(c.ToString());
			}

			for (int counter = 0; counter < testStr.Length; counter++)
			{
				Console.WriteLine(testStr[counter].ToString());
			}
		}
		#endregion

        #region "2.16 Pruning Characters from the Head, Tail, or Both of a String"
		public static void PruningChars()
		{
			string foo = "--TEST--";
			Console.WriteLine(foo.Trim(new char[] {'-'}));            // Displays "TEST"

			foo = ",-TEST-,-";
			Console.WriteLine(foo.Trim(new char[] {'-',','}));        // Displays "TEST"

			foo = "--TEST--";
			Console.WriteLine(foo.TrimStart(new char[] {'-'}));       // Displays "TEST--"

			foo = ",-TEST-,-";
			Console.WriteLine(foo.TrimStart(new char[] {'-',','}));   // Displays "TEST-,-"

			foo = "--TEST--";
			Console.WriteLine(foo.TrimEnd(new char[] {'-'}));         // Displays "--TEST"

			foo = ",-TEST-,-";
			Console.WriteLine(foo.TrimEnd(new char[] {'-',','}));     // Displays ",-TEST"
		}
		#endregion

		#region "2.17 Testing a String for Null or Empty Concurrently"
		public static void TestStringForNullEmpty()
		{
			string testNull = null;
			string testEmpty = string.Empty;
			string testBlank = "";
			string testFilledIn = "abc";

			Console.WriteLine("String.IsNullOrEmpty(testNull) == " + 
String.IsNullOrEmpty(testNull));
			Console.WriteLine("String.IsNullOrEmpty(testEmpty) == " + 
String.IsNullOrEmpty(testEmpty));
			Console.WriteLine("String.IsNullOrEmpty(testBlank) == " + 
String.IsNullOrEmpty(testBlank));
			Console.WriteLine("String.IsNullOrEmpty(testFilledIn) == " + 
String.IsNullOrEmpty(testFilledIn));
		}
		#endregion

		#region "2.18 Appending a Line"
		public static void AppendLine()
		{
			StringBuilder sb = new StringBuilder("First line of string" + 
Environment.NewLine);
			sb.AppendLine("Second line of string");
			sb.AppendLine();
			sb.AppendLine("Fourth line of string");

			Console.WriteLine(sb.ToString());
		}
		#endregion
	}


#region "Enums"
	public enum CharKind
	{
		Control,
		Digit,
		Letter,
		Number,
		Punctuation,
		Separator,
		Surrogate,
		Symbol,
		Whitespace,
		Unknown
	}

	enum Colors
	{
		red, green, blue
	}
#endregion

}

