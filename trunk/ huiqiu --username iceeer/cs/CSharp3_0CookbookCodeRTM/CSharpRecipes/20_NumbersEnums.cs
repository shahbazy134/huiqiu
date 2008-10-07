using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;


namespace CSharpRecipes
{
	public static class NumbersEnums
	{
		#region "20.1 Converting Degrees to Radians"
		public static double ConvertDegreesToRadians (double degrees)
		{
			double radians = (Math.PI / 180) * degrees;
			return (radians);
		}

        public static double ConvertRadiansToDegrees(double radians)
		{
			double degrees = (180 / Math.PI) * radians;
			return (degrees);
		}
        #endregion

        #region "20.2 Using the Bitwise Complement Operators with Various Data Types"
		public static void TestBitwiseOperators()
		{
			uint x = 0x00000001;
			Console.WriteLine("~x = " + ~x);

			sbyte B1 = sbyte.MinValue;	
			sbyte B2 = sbyte.MaxValue;
			Console.WriteLine("B1|B2 = " + (((byte)B1|(byte)B2)));

			ushort x2 = 0x00000001;		// Problem
			Console.WriteLine("~x2 = " + ~x2);

			byte y = 1;		// Problem
			//byte B = ~y;
			Console.WriteLine("~y = " + ~y);

			char x3 = (char)1;		// Problem
			Console.WriteLine("~x3 = " + ~x3);

			sbyte x5 = 1;
			Console.WriteLine("~x5 = " + ~x5);
			
			uint IntResult = (uint)~x;
			Console.WriteLine("IntResult = " + IntResult);
			
			byte ByteResult = (byte)~y;
			Console.WriteLine("ByteResult = " + ByteResult);
		}
        #endregion

        #region "20.3 Converting a Number in Another Base to Base10"
		public static void TestBase10()
		{
			string base2 = "11";
			string base8 = "17";
			string base10 = "110";
			string base16 = "11FF";

			Console.WriteLine("Convert.ToInt32(base2, 2) = " + 
				Convert.ToInt32(base2, 2));

			Console.WriteLine("Convert.ToInt32(base8, 8) = " + 
				Convert.ToInt32(base8, 8));

			Console.WriteLine("Convert.ToInt32(base10, 10) = " + 
				Convert.ToInt32(base10, 10));

			Console.WriteLine("Convert.ToInt32(base16, 16) = " + 
				Convert.ToInt32(base16, 16));
		}
        #endregion

        #region "20.4 Determining if a String is a Valid Number"
		public static void TestDetermineIfStringIsNumber()
		{
			string IsNotNumber = "111west";
			string IsNum = "  +111  ";
			string IsFloat = "  23.11  ";
			string IsExp = "  +23 e+11  ";

            int i = 0;
            float f = 0;

			Console.WriteLine(int.TryParse(IsNum, out i));		// 111		// 1.1 will not work here
			Console.WriteLine(float.TryParse(IsNum, out f));		// 111
			Console.WriteLine(float.TryParse(IsFloat, out f));	// 23.11
			//Console.WriteLine(float.Parse(IsExp));	// throws
			
		
			Console.WriteLine(IsInt(IsNum));		// True
            Console.WriteLine(IsInt(IsNotNumber));		// False
            Console.WriteLine(IsInt(IsFloat));		// False
            Console.WriteLine(IsInt(IsExp));		// False
			Console.WriteLine();
			

            Console.WriteLine(IsDoubleFromTryParse(IsNum));		// True
            Console.WriteLine(IsDoubleFromTryParse(IsNotNumber));		// False
            Console.WriteLine(IsDoubleFromTryParse(IsFloat));		// True
            Console.WriteLine(IsDoubleFromTryParse(IsExp));		// False
			Console.WriteLine();
		}
		
		
		public static bool IsInt(string value)
		{
			try
			{
				value = value.Trim();
				int foo = int.Parse(value);
				return (true);
			}
			catch (FormatException e)
			{
				Console.WriteLine("Not an integer value: {0}", e.ToString());
				return (false);
			}
		}
		public static bool IsDoubleFromTryParse(string value)
		{
			double result = 0;
			return (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.CurrentInfo, out result));
		}
		#endregion
		
        #region "20.5 Rounding a Floating Point Value"		
		public static void TestRound()
		{
			int X = (int)Math.Round(2.5555);
			Console.WriteLine(X);
			Console.WriteLine(Math.Round(2.5555, 2));
			Console.WriteLine(Math.Round(2.444444,3));
			Console.WriteLine(Math.Round(2.555555555555555555555555555550001));
			Console.WriteLine(Math.Round(.5));
			Console.WriteLine(Math.Round(1.5));
			Console.WriteLine(Math.Round(2.5));
			Console.WriteLine(Math.Round(3.5));
			Console.WriteLine();
			
			Console.WriteLine(Math.Floor(.5));
			Console.WriteLine(Math.Floor(1.5));
			Console.WriteLine(Math.Floor(2.5));
			Console.WriteLine(Math.Floor(3.5));
			Console.WriteLine();
			
			Console.WriteLine(Math.Ceiling(.5));
			Console.WriteLine(Math.Ceiling(1.5));
			Console.WriteLine(Math.Ceiling(2.5));
			Console.WriteLine(Math.Ceiling(3.5));	
			Console.WriteLine();

			Console.WriteLine(RoundUp(.4));
            Console.WriteLine(RoundUp(.5));
            Console.WriteLine(RoundUp(.6));
            Console.WriteLine(RoundUp(1.4));
            Console.WriteLine(RoundUp(1.5));
            Console.WriteLine(RoundUp(1.6));
            Console.WriteLine(RoundUp(2.4));
            Console.WriteLine(RoundUp(2.5));
            Console.WriteLine(RoundUp(2.6));
            Console.WriteLine(RoundUp(3.4));
            Console.WriteLine(RoundUp(3.5));
            Console.WriteLine(RoundUp(3.6));
			Console.WriteLine();

			Console.WriteLine(RoundDown(.4));
			Console.WriteLine(RoundDown(.5));
			Console.WriteLine(RoundDown(.6));
			Console.WriteLine(RoundDown(1.4));
			Console.WriteLine(RoundDown(1.5));
			Console.WriteLine(RoundDown(1.6));
			Console.WriteLine(RoundDown(2.4));
			Console.WriteLine(RoundDown(2.5));
			Console.WriteLine(RoundDown(2.6));
			Console.WriteLine(RoundDown(3.4));
			Console.WriteLine(RoundDown(3.5));
			Console.WriteLine(RoundDown(3.6));
		}
		#endregion
		
		#region "20.6 Different Rounding Algorithms"
		public static double RoundUp(double valueToRound)
		{
			return Math.Floor(valueToRound + 0.5);
		}
		
		public static double RoundDown(double valueToRound)
		{
			double floorValue = Math.Floor(valueToRound);
			if ((valueToRound - floorValue) > .5)
			{
				return (floorValue + 1);
			}
			else
			{
				return (floorValue);
			}
		}
		#endregion

        #region "20.7 Converting between temperature scales"
        public static double CelsiusToFahrenheit(double celsius)
        {
            return (1.8 * celsius) + 32;
        }

        public static double FahrenheitToCelsius(double fahrenheit)
        {
            return 1.8 * (fahrenheit - 32);
        }

        public static double CelsiusToKelvin(double celsius)
        {
            return celsius + 273;
        }

        public static double KelvinToCelsius(double kelvin)
        {
            return kelvin - 273;
        }

        public static double FahrenheitToKelvin(double fahrenheit)
        {
            return CelsiusToKelvin(FahrenheitToCelsius(fahrenheit));
        }

        public static double KelvinToFahrenheit(double kelvin)
        {
            return CelsiusToFahrenheit(KelvinToCelsius(kelvin));
        }
		#endregion

		#region "20.8 Safely Performing a Narrowing Numeric Cast"
        public static void TestNarrowing()
        {
            // Our two variables are declared and initialized
            int sourceValue = 34000;
            short destinationValue = 0;

            // Determine if sourceValue will lose information in a cast to a short
            if (sourceValue <= short.MaxValue && sourceValue >= short.MinValue)
            {
                destinationValue = (short)sourceValue;
            }
            else
            {
                // Inform the application that a loss of information will occur
            }

            long lhs = 34000;
            long rhs = long.MaxValue;
            lhs.AddChecked(rhs);
        }

        public static void AddChecked(this long lhs, long rhs)
        {
            int result = checked((int)(lhs + rhs));
        }
        #endregion
	
		#region "20.9 Displaying an Enumeration Value as a String"
		public static void TestDisplayEnumValue()
		{
			Console.WriteLine(Shapes.Circle.ToString());
			Console.WriteLine(Shapes.Circle.ToString("G"));
			Console.WriteLine(Shapes.Circle.ToString("D"));
			Console.WriteLine(Shapes.Circle.ToString("F"));
			Console.WriteLine(Shapes.Circle.ToString("X"));

			Console.WriteLine();

			Shapes shapeStyle = Shapes.Cylinder;
			Console.WriteLine(shapeStyle.ToString());
			Console.WriteLine(shapeStyle.ToString("G"));
			Console.WriteLine(shapeStyle.ToString("D"));
			Console.WriteLine(shapeStyle.ToString("F"));
			Console.WriteLine(shapeStyle.ToString("X"));

			Console.WriteLine();

			shapeStyle = Shapes.Circle | Shapes.Cylinder;
			Console.WriteLine(shapeStyle.ToString());
			Console.WriteLine(shapeStyle.ToString("G"));
			Console.WriteLine(shapeStyle.ToString("D"));
			Console.WriteLine(shapeStyle.ToString("F"));
			Console.WriteLine(shapeStyle.ToString("X"));


            IceCreamToppings toppings =
                IceCreamToppings.HotFudge | IceCreamToppings.WhippedCream;

            Console.WriteLine(toppings.ToString());
            Console.WriteLine(toppings.ToString("G"));
            Console.WriteLine(toppings.ToString("D"));
            Console.WriteLine(toppings.ToString("F"));
            Console.WriteLine(toppings.ToString("X"));

		}
		#endregion

		#region "20.10 Converting Plain Text to an Equivalent Enumeration Value"
		public static void TestConvertingEnums()
		{
			try
			{
				Language proj1Language = (Language)Enum.Parse(typeof(Language), "VBNET");
				//Language proj2Language = (Language)Enum.Parse(typeof(Language), "UnDefined");

                proj1Language = (Language)Enum.Parse(typeof(Language),"1");
                proj1Language = (Language)Enum.Parse(typeof(Language), "CSharp, VBNET");
			}
			catch (ArgumentException e)
			{
				// Handle an invalid text value here (such as the "UnDefined" string)
				Console.WriteLine(e);
			}
		}
		#endregion

		#region "20.11 Testing For A Valid Enumeration Value"
		public static void HandleEnum(Language language)
		{
            if (CheckLanguageEnumValue(language))
			{
				// Use language here
				Console.WriteLine("OK");
			}
			else
			{
				// Deal with the invalid enum value here
				Console.WriteLine("NOT OK");
			}
		}

        public static bool CheckLanguageEnumValue(Language language)
        {
            switch (language)
            {
                // all valid types for the enum listed here
                // this means only the ones we specify are valid 
                // not any enum value for this enum
                case Language.CSharp:
                case Language.Other:
                case Language.VB6:
                case Language.VBNET:
                    break;
                default:
                    Debug.Assert(false, language + " is not a valid enumeration value to pass.");
                    return false;
            }
            return true;
        }
		#endregion

		#region "20.12 Testing For A Valid Enumeration of Flags"
        public static bool ValidateFlagsEnum(IceCreamToppings topping)
        {
            return ((topping>0) && ((topping & IceCreamToppings.All) == topping));
        }

        public static bool ValidateFlagsEnum(int topping)
        {
	        return ((topping>0) && ((topping & (int)IceCreamToppings.All) == topping));
        }
		#endregion

		#region "20.13 Using Enumerated Members in a Bitmask"
		// See recipe 20.14 in book for explanation.
		#endregion

		#region "20.14 Determining If One or More Enumeration Flags are Set"
		// See recipe 20.15 in book for explanation.

		[Flags]
		public enum LanguageFlags
		{
			CSharp = 0x0001, VBNET = 0x0002, VB6 = 0x0004, Cpp = 0x0008,
			AllLanguagesExceptCSharp = VBNET | VB6 | Cpp
		}

		public static void TestEnumFlags()
		{
			LanguageFlags lang = LanguageFlags.CSharp | LanguageFlags.VBNET;

			if ((lang & LanguageFlags.CSharp) == LanguageFlags.CSharp)
			{
				Console.WriteLine("lang contains at least Language.CSharp");
			}

			if (lang == LanguageFlags.CSharp)
			{
				Console.WriteLine("lang contains only the Language.CSharp");
			}

			if ((lang > 0) && ((lang & (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
			   (LanguageFlags.CSharp | LanguageFlags.VBNET)))
			{
				Console.WriteLine("lang contains at least Language.CSharp and Language.VBNET");
			}

			if ((lang > 0) && ((lang | (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
			   (LanguageFlags.CSharp | LanguageFlags.VBNET)))
			{
				Console.WriteLine("lang contains only the Language.CSharp and Language.VBNET");
			}

			lang = LanguageFlags.CSharp;
			if ((lang & LanguageFlags.CSharp) == LanguageFlags.CSharp)
			{
				//Language_1_21.CSharp      0001
				//lang                 0001
				//ANDed bit values     0001

				Console.WriteLine("CSharp found in AND comparison (CSharp value)");
			}

			lang = LanguageFlags.CSharp;
			if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
			{
				// CSharp is found using OR logic
			}

			lang = LanguageFlags.CSharp | LanguageFlags.VB6 | LanguageFlags.Cpp;
			if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
			{
				// CSharp is found using OR logic
			}

			lang = LanguageFlags.VBNET | LanguageFlags.VB6 | LanguageFlags.Cpp;
			if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
			{
				// CSharp is found using OR logic
			}

			lang = LanguageFlags.VBNET;
			if ((lang & LanguageFlags.CSharp) == LanguageFlags.CSharp)
			{
				//Language_1_21.CSharp      0001
				//lang                 0010
				//ANDed bit values     0000
				Console.WriteLine("CSharp found in AND comparison (VBNET value)");
			}

			lang = LanguageFlags.CSharp;

			if (lang == LanguageFlags.CSharp)
			{
				//Language_1_21.CSharp      0001
				//lang                 0001
				//ORed bit values      0001

			}

			if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
			{
			}

			lang = LanguageFlags.CSharp | LanguageFlags.Cpp | LanguageFlags.VB6;

			if (lang == LanguageFlags.CSharp)
			{
				//Language_1_21.CSharp      0001
				//lang                 1101
				//ORed bit values      1101
			}

			lang = LanguageFlags.CSharp | LanguageFlags.VBNET;
			if ((lang > 0) && ((lang & (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
			   (LanguageFlags.CSharp | LanguageFlags.VBNET)))
			{
				//we can test multiple bits to determine whether they are both on and all other bits are off.
				Console.WriteLine("Found just CSharp and VBNET");
			}

			// now check with Cpp added
			lang = LanguageFlags.CSharp | LanguageFlags.VBNET | LanguageFlags.Cpp;
			if ((lang > 0) && ((lang & (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
			   (LanguageFlags.CSharp | LanguageFlags.VBNET)))
			{
				//we can test multiple bits to determine whether they are at least both on regardless of what else is in there.
				Console.WriteLine("Found at least CSharp and VBNET ");
			}

			lang = LanguageFlags.CSharp | LanguageFlags.VBNET;
			if ((lang > 0) && ((lang | (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
			   (LanguageFlags.CSharp | LanguageFlags.VBNET)))
			{
				//we can determine whether at least these bits are turned on.
				Console.WriteLine("Found CSharp or VBNET");
			}

			lang = LanguageFlags.CSharp | LanguageFlags.VBNET | LanguageFlags.Cpp;
			if ((lang > 0) && ((lang | (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
			   (LanguageFlags.CSharp | LanguageFlags.VBNET)))
			{
				//we can determine whether at least these bits are turned on.
				Console.WriteLine("Found CSharp or VBNET");
			}

			if ((lang > 0)&&(lang | LanguageFlags.AllLanguagesExceptCSharp) ==
				LanguageFlags.AllLanguagesExceptCSharp)
			{
				Console.WriteLine("Only CSharp is not specified");
			}

		}
		#endregion

		#region "20.15 Determining the integral part of a decimal or double"
		public static void TestTruncate()
		{
			decimal pi = (decimal)System.Math.PI;
			decimal decRet = System.Math.Truncate(pi);
			double trouble = 5.555;
			double dblRet = System.Math.Truncate(trouble);
		}
		#endregion
	}

    #region "Defined enumerations"
    [Flags]
    public enum Shapes
    {
        Square = 0, Circle = 1, Cylinder = 2, Octagon = 4
    }

    [Flags]
    public enum IceCreamToppings
    {
        Sprinkles = 0,
        HotFudge = 1, 
        Cherry = 2, 
        WhippedCream = 4,
        All = Sprinkles | HotFudge | Cherry | WhippedCream
    }

    public enum Language
    {
        Other = 0, CSharp = 1, VBNET = 2, VB6 = 3,
        All = (Other | CSharp | VBNET | VB6)
    }

    [Flags]
    public enum RecycleItems
    {
        Glass           = 0x01,
        AluminumCans    = 0x02,
        MixedPaper      = 0x04,
        Newspaper       = 0x08,
        TinCans         = 0x10,
        Cardboard       = 0x20,
        ClearPlastic    = 0x40,
        All = (Glass | AluminumCans | MixedPaper | Newspaper | TinCans | Cardboard | ClearPlastic)
    }
    #endregion
}

