using System;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization; 
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;


namespace CSharpRecipes
{
	public class ClassAndStructs
	{
	    #region "3.1 Creating Union Type Structures"
		public static void TestUnions()
		{
			Console.WriteLine("\r\n\r\n");
			SignedNumber sNum = new SignedNumber();
			sNum.Num1 = sbyte.MaxValue;
			Console.WriteLine("Num1 = " + sNum.Num1);
			Console.WriteLine("Num2 = " + sNum.Num2);
			Console.WriteLine("Num3 = " + sNum.Num3);
			Console.WriteLine("Num4 = " + sNum.Num4);
			
			sNum.Num4 = long.MaxValue;
			Console.WriteLine("\r\nNum1 = " + sNum.Num1);
			Console.WriteLine("Num2 = " + sNum.Num2);
			Console.WriteLine("Num3 = " + sNum.Num3);
			Console.WriteLine("Num4 = " + sNum.Num4);
			Console.WriteLine("Num5 = " + sNum.Num5);
			Console.WriteLine("Num6 = " + sNum.Num6);
			Console.WriteLine("Num7 = " + sNum.Num7);
			
			
			Console.WriteLine("\r\n\r\n");
			SignedNumberWithText sNumWithText = new SignedNumberWithText();
			sNumWithText.Text1 = "c";
			//sNumWithText.Text2 = "asdfasdf";
			
			sNumWithText.Num1 = sbyte.MaxValue;
			Console.WriteLine("Num1 = " + sNumWithText.Num1);
			Console.WriteLine("Num2 = " + sNumWithText.Num2);
			Console.WriteLine("Num3 = " + sNumWithText.Num3);
			Console.WriteLine("Num4 = " + sNumWithText.Num4);
			Console.WriteLine("Text1 = " + sNumWithText.Text1);
			//Console.WriteLine("Text2 = " + sNumWithText.Text2);
			
			sNumWithText.Num4 = long.MaxValue;
			Console.WriteLine("\r\nNum1 = " + sNumWithText.Num1);
			Console.WriteLine("Num2 = " + sNumWithText.Num2);
			Console.WriteLine("Num3 = " + sNumWithText.Num3);
			Console.WriteLine("Num4 = " + sNumWithText.Num4);
			Console.WriteLine("Num5 = " + sNumWithText.Num5);
			Console.WriteLine("Num6 = " + sNumWithText.Num6);
			Console.WriteLine("Num7 = " + sNumWithText.Num7);			
			Console.WriteLine("Text1 = " + sNumWithText.Text1);
			//Console.WriteLine("Text2 = " + sNumWithText.Text2);
		}


		[StructLayoutAttribute(LayoutKind.Explicit)]
		struct SignedNumber 
		{
			[FieldOffsetAttribute(0)] 
			public sbyte Num1;
    
			[FieldOffsetAttribute(0)] 
			public short Num2;

			[FieldOffsetAttribute(0)] 
			public int Num3;

			[FieldOffsetAttribute(0)] 
			public long Num4;

			[FieldOffsetAttribute(0)] 
			public float Num5;

			[FieldOffsetAttribute(0)] 
			public double Num6;

			[FieldOffsetAttribute(0)] 
			public decimal Num7;
		}

		[StructLayoutAttribute(LayoutKind.Explicit)]
		struct SignedNumberWithText 
		{
			[FieldOffsetAttribute(0)] 
			public sbyte Num1;
    
			[FieldOffsetAttribute(0)] 
			public short Num2;

			[FieldOffsetAttribute(0)] 
			public int Num3;

			[FieldOffsetAttribute(0)] 
			public long Num4;

			[FieldOffsetAttribute(0)] 
			public float Num5;

			[FieldOffsetAttribute(0)] 
			public double Num6;

			[FieldOffsetAttribute(0)] 
			public decimal Num7;

			[FieldOffsetAttribute(16)] 
			public string Text1;
		}
		#endregion
		
		#region "3.2 Making a Type Sortable"
        public static void TestSort()
        {
            List<Square> listOfSquares = new List<Square>(){
                                        new Square(1,3), 
								        new Square(4,3),
								        new Square(2,1),
								        new Square(6,1)};
        	
	        // Test a List<String>
	        Console.WriteLine("List<String>");
	        Console.WriteLine("Original list");
	        foreach (Square square in listOfSquares)
	        {
		        Console.WriteLine(square.ToString());
	        }


            Console.WriteLine();
            IComparer<Square> heightCompare = new CompareHeight();
            listOfSquares.Sort(heightCompare);
            Console.WriteLine("Sorted list using IComparer<Square>=heightCompare");
            foreach (Square square in listOfSquares)
	        {
		        Console.WriteLine(square.ToString());
	        }
        	
	        Console.WriteLine();
	        Console.WriteLine("Sorted list using IComparable<Square>");
	        listOfSquares.Sort();
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }

        	
	        // Test a SORTEDLIST
            var sortedListOfSquares = new SortedList<int,Square>(){
                                    { 0, new Square(1,3)},
                                    { 2, new Square(3,3)},
                                    { 1, new Square(2,1)},
                                    { 3, new Square(6,1)}};

	        Console.WriteLine();
	        Console.WriteLine();
	        Console.WriteLine("SortedList<Square>");
	        foreach (KeyValuePair<int,Square> kvp in sortedListOfSquares)
	        {
		        Console.WriteLine(kvp.Key + " : " + kvp.Value);
	        }
        }


        public class Square : IComparable<Square>
        {
            public Square(){}

            public Square(int height, int width)
            {
                this.Height = height;
                this.Width = width;
            }

            public int Height { get; set; }

            public int Width { get; set; }

            public int CompareTo(object obj)
            {
                Square square = obj as Square;
                if (square != null)
                    return CompareTo(square);
                throw (new ArgumentException("Both objects being compared must be of type Square."));
            }

            public override string ToString()
            {
                return ("Height:" + this.Height + "  Width:" + this.Width);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                Square square = obj as Square;
                if(square != null)
                    return this.Height == square.Height;
                return false;
            }

            public override int GetHashCode()
            {
                 return this.Height.GetHashCode() | this.Width.GetHashCode();
            }

            public static bool operator ==(Square x, Square y)
            {
                return x.Equals(y);
            }
            public static bool operator !=(Square x, Square y)
            {
                return !(x == y);
            }
            public static bool operator <(Square x, Square y)
            {
                return (x.CompareTo(y) < 0);
            }
            public static bool operator >(Square x, Square y)
            {
                return (x.CompareTo(y) > 0);
            }

            #region IComparable<Square> Members

            public int CompareTo(Square other)
            {
                long area1 = this.Height * this.Width;
                long area2 = other.Height * other.Width;

                if (area1 == area2)
	                return 0;
                else if (area1 > area2)
	                return 1;
                else if (area1 < area2)
	                return -1;
                else
	                return -1;
            }

            #endregion
        }

        public class CompareHeight : IComparer<Square>
        {
            public int Compare(object firstSquare, object secondSquare)
            {
                Square square1 = firstSquare as Square;
                Square square2 = secondSquare as Square;
                if (square1 == null || square2 == null)
	                throw (new ArgumentException("Both parameters must be of type Square."));
                else
                    return Compare(firstSquare,secondSquare);
            }

            #region IComparer<Square> Members

            public int Compare(Square x, Square y)
            {
                if (x.Height == y.Height)
                    return 0;
                else if (x.Height > y.Height)
                    return 1;
                else if (x.Height < y.Height)
                    return -1;
                else
                    return -1;
            }

            #endregion
        }
		#endregion
		
		#region "3.3 Making a Type Searchable"
		// See the Square type in the previous code region.
		
        public static void TestSearch()
        {
	        List<Square> listOfSquares = new List<Square> {new Square(1,3), 
												        new Square(4,3),
												        new Square(2,1),
												        new Square(6,1)};
        	
            IComparer<Square> heightCompare = new CompareHeight();
        	
	        // Test a List<Square>
	        Console.WriteLine("List<Square>");
	        Console.WriteLine("Original list");
	        foreach (Square square in listOfSquares)
	        {
		        Console.WriteLine(square.ToString());
	        }
        	
	        Console.WriteLine();
	        Console.WriteLine("Sorted list using IComparer<Square>=heightCompare");
	        listOfSquares.Sort(heightCompare);
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }
        	
	        Console.WriteLine();
	        Console.WriteLine("Search using IComparer<Square>=heightCompare");
	        int found = listOfSquares.BinarySearch(new Square(1,3), heightCompare);
	        Console.WriteLine("Found (1,3): " + found);
        	
	        Console.WriteLine();
	        Console.WriteLine("Sorted list using IComparable<Square>");
	        listOfSquares.Sort();
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }

	        Console.WriteLine("Search using IComparable<Square>");
	        found = listOfSquares.BinarySearch(new Square(6,1));  // Use IComparable
	        Console.WriteLine("Found (6,1): " + found);
        	
        	
	        // Test a SortedList<Square>
	        var sortedListOfSquares = new SortedList<int,Square>(){
                                            {0, new Square(1,3)},
                                            {2, new Square(4,3)},
                                            {1, new Square(2,1)},
                                            {4, new Square(6,1)}};

	        Console.WriteLine();
	        Console.WriteLine();
	        Console.WriteLine("SortedList<Square>");
	        foreach (KeyValuePair<int,Square> kvp in sortedListOfSquares)
	        {
		        Console.WriteLine(kvp.Key + " : " + kvp.Value);
	        }

	        Console.WriteLine();
	        bool foundItem = sortedListOfSquares.ContainsKey(2);
            Console.WriteLine("sortedListOfSquares.ContainsKey(2): " + foundItem);

	        // Does not use IComparer or IComparable
	        // -- uses a linear search along with the Equals method
	        //    which has not been overloaded
	        Square value = new Square(6,1);
            foundItem = sortedListOfSquares.ContainsValue(value);
            Console.WriteLine("sortedListOfSquares.ContainsValue(new Square(6,1)): " + foundItem);
        }
        #endregion

        #region "3.4 Indirectly Overloading the +=, -=, /=, and *= Operators"
		public class Foo
		{
			// Other class members...

			// Overloaded binary operators
			public static Foo operator +(Foo foo1, Foo foo2)
			{
				if (foo1 == null || foo2 == null)
				{
					throw (new ArgumentException("Neither object may be null."));
				}

				Foo result = new Foo();

				// Add f1 and f2 here...
				// Place result of the addition into the result variable

				return result;
			}

			public static Foo operator +(int constant, Foo foo1)
			{
				Foo result = new Foo();

				// Add the constant integer and f1 here...
				// Place result of the addition into the result variable

				return result;
			}

			public static Foo operator +(Foo foo1, int constant)
			{
				Foo result = new Foo();

				// Add the constant integer and f1 here...
				// Place result of the addition into the result variable

				return result;
			}

			public static Foo operator -(Foo foo1, Foo foo2)
			{
				Foo result = new Foo();

				// Subtract f1 and f2 here...
				// Place result of the subtraction into the result variable

				return result;
			}

			public static Foo operator -(int constant, Foo foo1)
			{
				Foo result = new Foo();

				// Subtract the constant integer and f1 here...
				// Place result of the subtraction into the result variable

				return result;
			}

			public static Foo operator -(Foo foo1, int constant)
			{
				Foo result = new Foo();

				// Subtract the constant integer and f1 here...
				// Place result of the subtraction into the result variable

				return result;
			}

			public static Foo operator *(Foo foo1, Foo foo2)
			{
				Foo result = new Foo();

				// Multiply f1 and f2 here...
				// Place result of the multiplication into the result variable

				return result;
			}

			public static Foo operator *(int multiplier, Foo foo1)
			{
				Foo result = new Foo();

				// Multiply multiplier and f1 here...
				// Place result of the multiplication into the result variable

				return result;
			}

			public static Foo operator *(Foo foo1, int multiplier)
			{
				return (multiplier * foo1);
			}

			public static Foo operator /(Foo foo1, Foo foo2)
			{
				Foo result = new Foo();

				// Divide f1 and f2 here...
				// Place result of the division into the result variable

				return result;
			}

			public static Foo operator /(int numerator, Foo foo1)
			{
				Foo result = new Foo();

				// Divide numerator and f1 here...
				// Place result of the division into the result variable

				return result;
			}

			public static Foo operator /(Foo foo1, int denominator)
			{
				return (1 / (denominator / foo1));
			}
		}
		#endregion
        
        #region "3.5 Indirectly Overloading the &&, || and ?: Operators"
		public static void TestObjState()
		{
			ObjState osOn = new ObjState(1);
			ObjState osOff = new ObjState(-1);
			
			Console.WriteLine((osOn && osOn));		// 1		true
			Console.WriteLine((osOn && osOff));		// 1		false
			Console.WriteLine((osOff && osOn));		// -1		false
			Console.WriteLine((osOn || osOff));		// 1		true
			Console.WriteLine((osOff || osOff));	// -1		false
//			Console.WriteLine((osOn & osOff));		// -1
//			Console.WriteLine((osOn & osOn));		// 1
//			Console.WriteLine((osOn | osOff));		// 1
//			Console.WriteLine((osOff | osOff));		// -1
			Console.WriteLine((osOn ? "A" : "B" ));	// A
			Console.WriteLine((osOff ? "C" : "D"));	// D
			Console.WriteLine((osOn ? "A" : "B" ));	// A
			Console.WriteLine((osOff ? "C" : "D"));	// D
			Console.WriteLine(((osOff || osOn) ? "C" : "D"));	// C
			
			Console.WriteLine((osOn.RetObj(1) && osOff.RetObj(1)));	
			Console.WriteLine((osOn.RetObj(-1) && osOff.RetObj(1)));
			Console.WriteLine((osOn.RetObj(1) && osOff.RetObj(-1)));
			Console.WriteLine((osOn.RetObj(-1) && osOff.RetObj(-1)));		
		}


        public class ObjState
        {
            public ObjState(int state)
            {
                this.State = state;
            }

            public int State {get; set;}

            public static implicit operator bool(ObjState obj) 
            {
                return (obj.State > 0);
            }

            public ObjState RetObj(int state)
            {
                return (new ObjState(state));
            }

            public static ObjState operator &(ObjState obj1, ObjState obj2)
            {
                if (obj1 == null || obj2 == null)
                    throw (new ArgumentNullException("Neither object may be null."));

                if (obj1.State >= 0 && obj2.State >= 0)
                    return (new ObjState(1));
                else
                    return (new ObjState(-1));
            }

            public static ObjState operator |(ObjState obj1, ObjState obj2)
            {
                if (obj1.State < 0 && obj2.State < 0)
                    return (new ObjState(-1));
                else
                    return (new ObjState(1));
            }

            public static bool operator true(ObjState obj)
            {
                if (obj.State >= 0)
                    return true;
                else
                    return false;
            }

            public static bool operator false(ObjState obj)
            {
                if (obj.State >= 0)
                    return true;
                else
                    return false;
            }

            public override string ToString()
            {
                return State.ToString();
            }
        }
		#endregion

		#region "3.6 Making Your Expressions Error-Free"
		// See recipe 3.6 in book for explaination.
		#endregion

		#region "3.7 Minimizing (Reducing) Your Boolean Logic"
		// See recipe 3.7 in book for explaination.
		#endregion

		#region "3.8 Converting Between Simple Types in a Language Agnostic Manner"
		public static void ConvertCode()
		{
			float initialValue = 0;
			int finalValue = 0;

			initialValue = (float)13.499;
			finalValue = (int)initialValue;
			Console.WriteLine(finalValue.ToString());

			initialValue = (float)13.5;
			finalValue = (int)initialValue;
			Console.WriteLine(finalValue.ToString());

			initialValue = (float)13.501;
			finalValue = (int)initialValue;
			Console.WriteLine(finalValue.ToString());
			
			
			finalValue = Convert.ToInt32((float)13.449);
			Console.WriteLine(finalValue.ToString());

			finalValue = Convert.ToInt32((float)13.5);
			Console.WriteLine(finalValue.ToString());

			finalValue = Convert.ToInt32((float)13.501);
			Console.WriteLine(finalValue.ToString());
		}
		#endregion

        #region "3.9 Determining Whether to Use the Cast Operator, the as Operator, or the is Operator"
		// See recipe 3.14 in book for explaination.
		#endregion
		        
        #region "3.10 Casting With The as Operator"
		public class Base {}
		public class Specific : Base {}
		
		public static void ConvertObj(Base baseObj)
		{
			Specific specificObj = baseObj as Specific; 
			if (specificObj == null)
			{
				// Cast failed
				Console.WriteLine("Cast failed");
			}
			else
			{
				// Cast was successful
				Console.WriteLine("Cast was successful");
			}
		}
		
		
		public class TestAsOp<T>
			where T: class
		{
			public T ConvertSomething(object obj)
			{
				return (obj as T);
			}
		}
		#endregion
        
        #region "3.11 Determining a Variable’s Type with the is Operator"
		public class Point2D {}
		public class Point3D {}
		public class ExPoint2D : Point2D {}
		public class ExPoint3D : Point3D {}
		
		public object CreatePoint(int pointType)
		{
			switch (pointType) 
			{
				case 0:
					return (new Point2D());
				case 1:
					return (new Point3D());
				case 2:
					return (new ExPoint2D());
				case 3:
					return (new ExPoint3D());
				default:
					return (null);
			}
		}
		
		public void CreateAndHandlePoint()
		{
			// Create a new point object and return it
			object retObj = CreatePoint(3);

			// Handle the point object based on its actual type
			if (retObj is ExPoint2D)
			{
				Console.WriteLine("Use the ExPoint2D type");
			}
			else if (retObj is ExPoint3D)
			{
				Console.WriteLine("Use the ExPoint3D type");
			}
			else if (retObj is Point2D)
			{
				Console.WriteLine("Use the Point2D type");
			}
			else if (retObj is Point3D)
			{
				Console.WriteLine("Use the Point3D type");
			}
			else 
			{
				Console.WriteLine("Invalid point type");
			}
		}
		#endregion
        
        #region "3.12 Returning Multiple Items From A Method"
		public void ReturnDimensions(int inputShape, 
			out int height, 
			out int width, 
			out int depth)
		{
			height = 0;
			width = 0;
			depth = 0;

			// Calculate height, width, depth from the inputShape value
		}

		public Dimensions ReturnDimensions(int inputShape)
		{
			// The default ctor automatically defaults this structure’s members to 0
			Dimensions objDim = new Dimensions();

			// Calculate objDim.Height, objDim.Width, objDim.Depth from the inputShape value

			return (objDim);
		}

		public struct Dimensions
		{
			public int Height;
			public int Width;
			public int Depth;
		}
		#endregion
        
        #region "3.13 Parsing Command Line Parameters"
        public static void TestParser(string[] argumentStrings)
        {
            //Important point: why am I immediately converting the parsed arguments to an array?  
            //Because query results are CALCULATED LAZILY and RECALCULATED ON DEMAND.  
            //If we just did the transformation without forcing it to an array, then EVERY SINGLE TIME 
            //we iterated the collection it would reparse.  Remember, the query logic does not know that 
            //the argumentStrings collection isn’t changing!  It is not an immutable object, so every time 
            //we iterate the collection, we run the query AGAIN, and that reparses everything.  
            //Since we only want to parse everything once, we iterate it once and store the results in an array.
            //Now that we’ve got our parsed arguments, we’ll do an error checking pass:
            var arguments = (from argument in argumentStrings
                select new Argument(argument)).ToArray();

            Console.Write("Command line: ");
            foreach (Argument a in arguments)
            {
                Console.Write(a.Original + " ");
            }
            Console.WriteLine("");

            ArgumentSemanticAnalyzer analyzer = new ArgumentSemanticAnalyzer();
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("output", 
                    "/output:[path to output]", 
                    "Specifies the location of the output file.",
                    x => x.IsCompoundSwitch));
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("trialMode", 
                    "/trialmode", 
                    "If this is specified it places the product into trial mode",
                    x => x.IsSimpleSwitch));
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("DeBuGoUtPuT", 
                    "/debugoutput:[value1];[value2];[value3]", 
                    "A listing of the files the debug output information will be written to",
                    x => x.IsComplexSwitch));
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("", 
                    "[literal value]",
                    "A literal value",
                    x => x.IsSimple));

            if (!analyzer.VerifyArguments(arguments))
            {
                string invalidArguments = analyzer.InvalidArgumentsDisplay();
                Console.WriteLine(invalidArguments);
                ShowUsage(analyzer);
                return;
            }

            //We’ll come back to that.  Assuming that our error checking pass gave the thumbs up, 
            //we’ll extract the information out of the parsed arguments that we need to run our program. 
            //Here’s the information we need:
            string output = string.Empty;
            bool trialmode = false;
            IEnumerable<string> debugOutput = null;
            List<string> literals = new List<string>();

            //For each parsed argument we want to apply an action so add them to the analyzer.  
            analyzer.AddArgumentAction("OUTPUT", x => { output = x.SubArguments[0]; });
            analyzer.AddArgumentAction("TRIALMODE", x => { trialmode = true; });
            analyzer.AddArgumentAction("DEBUGOUTPUT", x => { debugOutput = x.SubArguments; });
            analyzer.AddArgumentAction("", x=>{literals.Add(x.Original);});

            // check the arguments and run the actions
            analyzer.EvaluateArguments(arguments);

            // display the results
            Console.WriteLine("");
            Console.WriteLine("OUTPUT: {0}", output);
            Console.WriteLine("TRIALMODE: {0}", trialmode);
            if (debugOutput != null)
            {
                foreach (string item in debugOutput)
                {
                    Console.WriteLine("DEBUGOUTPUT: {0}", item);
                }
            }
            foreach (string literal in literals)
            {
                Console.WriteLine("LITERAL: {0}",literal);
            }

            //and we are ready to run our program:
            //Program program = new Program(output, trialmode, debugOutput, literals);
            //program.Run();
        }

        public static void ShowUsage(ArgumentSemanticAnalyzer analyzer)
        {
            Console.WriteLine("Program.exe allows the following arguments:");
            foreach (ArgumentDefinition definition in analyzer.ArgumentDefinitions)
            {
                Console.WriteLine("\t{0}: ({1}){2}\tSyntax: {3}",
                    definition.ArgumentSwitch, definition.Description, 
                    Environment.NewLine,definition.Syntax);
            }
        }

        public sealed class Argument
        {
            public string Original { get; private set; }
            public string Switch { get; private set; }
            public ReadOnlyCollection<string> SubArguments { get; private set; }
            private List<string> subArguments;
            public Argument(string original)
            {
                Original = original;
                Switch = string.Empty;
                subArguments = new List<string>();
                SubArguments = new ReadOnlyCollection<string>(subArguments);
                Parse();
            }

            private void Parse()
            {
                if (string.IsNullOrEmpty(Original))
                {
                    return;
                }
                char[] switchChars = { '/', '-' };
                if (!switchChars.Contains(Original[0]))
                {
                    return;
                }
                string switchString = Original.Substring(1);
                string subArgsString = string.Empty;
                int colon = switchString.IndexOf(':');
                if (colon >= 0)
                {
                    subArgsString = switchString.Substring(colon + 1);
                    switchString = switchString.Substring(0, colon);
                }
                Switch = switchString;
                if (!string.IsNullOrEmpty(subArgsString))
                    subArguments.AddRange(subArgsString.Split(';'));
            }

            public bool IsSimple
                { get { return SubArguments.Count == 0; } }
            public bool IsSimpleSwitch
                { get { return !string.IsNullOrEmpty(Switch) && SubArguments.Count == 0; } }
            public bool IsCompoundSwitch
                { get { return !string.IsNullOrEmpty(Switch) && SubArguments.Count == 1; } }
            public bool IsComplexSwitch
                { get { return !string.IsNullOrEmpty(Switch) && SubArguments.Count > 0; } }
        }

        public sealed class ArgumentDefinition
        {
            public string ArgumentSwitch { get; private set; }
            public string Syntax { get; private set; }
            public string Description { get; private set; }
            public Func<Argument, bool> Verifier { get; private set; }

            public ArgumentDefinition(string argumentSwitch,
                                      string syntax,
                                      string description,
                                      Func<Argument, bool> verifier)
            {
                ArgumentSwitch = argumentSwitch.ToUpper();
                Syntax = syntax;
                Description = description;
                Verifier = verifier;
            }

            public bool Verify(Argument arg)
            {
                return Verifier(arg);
            }
        }

        public sealed class ArgumentSemanticAnalyzer
        {
            private List<ArgumentDefinition> argumentDefinitions = 
                new List<ArgumentDefinition>();
            private Dictionary<string, Action<Argument>> argumentActions = 
                new Dictionary<string, Action<Argument>>();

            public ReadOnlyCollection<Argument> UnrecognizedArguments { get; private set; }
            public ReadOnlyCollection<Argument> MalformedArguments { get; private set; }
            public ReadOnlyCollection<Argument> RepeatedArguments { get; private set; }

            public ReadOnlyCollection<ArgumentDefinition> ArgumentDefinitions
            {
                get { return new ReadOnlyCollection<ArgumentDefinition>(argumentDefinitions); }
            }

            public IEnumerable<string> DefinedSwitches
            {
                get
                {
                    return from argumentDefinition in argumentDefinitions
                           select argumentDefinition.ArgumentSwitch;
                }
            }

            public void AddArgumentVerifier(ArgumentDefinition verifier)
            {
                argumentDefinitions.Add(verifier);
            }

            public void RemoveArgumentVerifier(ArgumentDefinition verifier)
            {
                var verifiersToRemove = from v in argumentDefinitions
                                        where v.ArgumentSwitch == verifier.ArgumentSwitch
                                        select v;
                foreach (var v in verifiersToRemove)
                    argumentDefinitions.Remove(v);
            }

            public void AddArgumentAction(string argumentSwitch, Action<Argument> action)
            {
                argumentActions.Add(argumentSwitch, action);
            }

            public void RemoveArgumentAction(string argumentSwitch)
            {
                if (argumentActions.Keys.Contains(argumentSwitch))
                    argumentActions.Remove(argumentSwitch);
            }

            public bool VerifyArguments(IEnumerable<Argument> arguments)
            {
                // no parameter to verify with, fail.
                if (!argumentDefinitions.Any())
                    return false;

                // Identify if any of the arguments are not defined
                this.UnrecognizedArguments = (  from argument in arguments
                                                where !DefinedSwitches.Contains(argument.Switch.ToUpper())
                                                select argument).ToList().AsReadOnly();


                //Check for all the arguments where the switch matches a known switch, 
                //but our well-formedness predicate is false. 
                this.MalformedArguments = ( from argument in arguments
                                            join argumentDefinition in argumentDefinitions 
                                            on argument.Switch.ToUpper() equals 
                                                argumentDefinition.ArgumentSwitch
                                            where !argumentDefinition.Verify(argument)
                                            select argument).ToList().AsReadOnly();

                //Sort the arguments into “groups” by their switch, count every group, 
                //and select any groups that contain more than one element, 
                //We then get a read only list of the items.
                this.RepeatedArguments =
                        (from argumentGroup in
                            from argument in arguments
                            where !argument.IsSimple
                            group argument by argument.Switch.ToUpper()
                        where argumentGroup.Count() > 1
                        select argumentGroup).SelectMany(ag => ag).ToList().AsReadOnly();

                if (this.UnrecognizedArguments.Any() || 
                    this.MalformedArguments.Any() ||
                    this.RepeatedArguments.Any())
                    return false;

                return true;
            }

            public void EvaluateArguments(IEnumerable<Argument> arguments)
            {
                //Now we just apply each action:
                foreach (Argument argument in arguments)
                    argumentActions[argument.Switch.ToUpper()](argument);
            }

            public string InvalidArgumentsDisplay()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Invalid arguments: {0}",Environment.NewLine);
                // Add the unrecognized arguments
                FormatInvalidArguments(builder, this.UnrecognizedArguments,
                    "Unrecognized argument: {0}{1}");

                // Add the malformed arguments
                FormatInvalidArguments(builder, this.MalformedArguments,
                    "Malformed argument: {0}{1}");

                // For the repeated arguments, we want to group them for the display
                // so group by switch and then add it to the string being built.
                var argumentGroups = from argument in this.RepeatedArguments
                                     group argument by argument.Switch.ToUpper() into ag
                                     select new { Switch = ag.Key, Instances = ag};

                foreach (var argumentGroup in argumentGroups)
                {
                    builder.AppendFormat("Repeated argument: {0}{1}", 
                        argumentGroup.Switch, Environment.NewLine);
                    FormatInvalidArguments(builder, argumentGroup.Instances.ToList(),
                        "\t{0}{1}");
                }
                return builder.ToString();
            }

            private void FormatInvalidArguments(StringBuilder builder, 
                IEnumerable<Argument> invalidArguments, string errorFormat)
            {
                if (invalidArguments != null)
                {
                    foreach (Argument argument in invalidArguments)
                    {
                        builder.AppendFormat(errorFormat,
                            argument.Original, Environment.NewLine);
                    }
                }
            }
        }
		#endregion

        #region "3.14 Initializing A Constant Field at Runtime"
		public class Foo2
		{
			public readonly int Bar;

			public Foo2() {}

			public Foo2(int constInitValue)
			{
				Bar = constInitValue;
			}

			// Rest of class...
		}

		public class Foo22
		{
			public const int Bar = 100;

			public Foo22() {}

			public Foo22(int constInitValue)
			{
				//Bar = constInitValue;    // This line causes a compile-time error
			}

			// Rest of class...
		}
		#endregion

        #region "3.15 Building Cloneable Classes"
		public static void TestCloning()
		{
			ShallowClone sc = new ShallowClone();
			sc.ListData.Add("asdf");
			ShallowClone scCloned = sc.ShallowCopy();
			Console.WriteLine("scCloned.ListData.Remove(\"asdf\") == " + scCloned.ListData.Remove("asdf"));

			DeepClone dc = new DeepClone();
			dc.ListData.Add("asdf");
			DeepClone dcCloned = dc.DeepCopy();
			dcCloned.ListData.Remove("asdf");
			Console.WriteLine("dc.ListData.Contains(\"asdf\") == " + dc.ListData.Contains("asdf"));
			Console.WriteLine("dcCloned.ListData.Contains(\"asdf\") == " + dcCloned.ListData.Contains("asdf"));

			MultiClone mc = new MultiClone();
			mc.ListData.Add("asdf");
			MultiClone mcCloned = mc.DeepCopy();
			Console.WriteLine("mcCloned.ListData.Contains(\"asdf\") == " + mcCloned.ListData.Contains("asdf"));
			Console.WriteLine("mc.ListData.Contains(\"asdf\") == " + mc.ListData.Contains("asdf"));
		}

        public interface IShallowCopy<T>
        {
            T ShallowCopy();
        }
        public interface IDeepCopy<T>
        {
            T DeepCopy();
        }

        public class ShallowClone : IShallowCopy<ShallowClone>
        {
	        public int Data = 1;
	        public List<string> ListData = new List<string>();
	        public object ObjData = new object();

            public ShallowClone ShallowCopy()
	        {
		        return (ShallowClone)this.MemberwiseClone();
	        }
        }

        [Serializable]
        public class DeepClone : IDeepCopy<DeepClone>
        {
	        public int data = 1;
	        public List<string> ListData = new List<string>();
	        public object objData = new object();

	        public DeepClone DeepCopy()
	        {
		        BinaryFormatter BF = new BinaryFormatter();
		        MemoryStream memStream = new MemoryStream();

		        BF.Serialize(memStream, this);
		        memStream.Flush();
		        memStream.Position = 0;

		        return (DeepClone)BF.Deserialize(memStream);
	        }
        }

        [Serializable]
        public class MultiClone : IShallowCopy<MultiClone>,
                                  IDeepCopy<MultiClone>
        {
	        public int data = 1;
	        public List<string> ListData = new List<string>();
	        public object objData = new object();

            public MultiClone ShallowCopy()
            {
                return (MultiClone)this.MemberwiseClone();
            }

            public MultiClone DeepCopy()
            {
                BinaryFormatter BF = new BinaryFormatter();
                MemoryStream memStream = new MemoryStream();

                BF.Serialize(memStream, this);
                memStream.Flush();
                memStream.Position = 0;

                return (MultiClone)BF.Deserialize(memStream);
            }
        }
		#endregion

        #region "3.16 Assuring an Object's Disposal"
		public static void DisposeObj1()
		{
			using(FileStream FS = new FileStream("Test.txt", FileMode.Create))
			{
				FS.WriteByte((byte)1);
				FS.WriteByte((byte)2);
				FS.WriteByte((byte)3);

				using(StreamWriter SW = new StreamWriter(FS))
				{
					SW.WriteLine("some text.");
				}
			}
		}
		
		public static void DisposeObj2()
		{
			FileStream FS = new FileStream("Test.txt", FileMode.Create);
			try
			{
				FS.WriteByte((byte)1);
				FS.WriteByte((byte)2);
				FS.WriteByte((byte)3);

				StreamWriter SW = new StreamWriter(FS);
				try
				{
					SW.WriteLine("some text.");
				}
				finally
				{
					if (SW != null)
					{
						((IDisposable)SW).Dispose();
					}
				}
			}
			finally
			{
				if (FS != null)
				{
					((IDisposable)FS).Dispose();
				}
			}
		}
        #endregion

        #region "3.17 Disposing of Unmanaged Resources"
        public static class NativeMethods
        {
            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr CreateSemaphore(IntPtr lpSemaphoreAttributes, int lInitialCount, int lMaximumCount, string lpName);

            [DllImport("Kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReleaseSemaphore(IntPtr hSemaphore, int lReleaseCount, out IntPtr lpPreviousCount);
        }
        
		public class FooD : IDisposable
		{
			public FooD() {}

			// Replace SomeCOMObj with your COM object type
			private SomeComObj comObj = new SomeComObj();
			private FileStream fileStream = new FileStream(@"c:\test.txt", FileMode.OpenOrCreate);
			private ArrayList aList = new ArrayList();
			private bool hasBeenDisposed = false;
			private IntPtr hSemaphore = IntPtr.Zero;
			// Container for objects
			private System.ComponentModel.Container containedObjs = new System.ComponentModel.Container();
			// Somewhere in your class you will populate this container with objects

			public class SomeComObj {}

			// Protect these members from being used on a disposed object
			public void WriteToFile(string text)
			{
				if(hasBeenDisposed)
				{
					throw (new ObjectDisposedException(this.ToString(), 
						"Object has been disposed"));
				}

				System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
				fileStream.Write(enc.GetBytes(text), 0, text.Length);
			}

			public void UseComObj()
			{
				if(hasBeenDisposed)
				{
					throw (new ObjectDisposedException(this.ToString(), 
						"Object has been disposed"));
				}

				Console.WriteLine("GUID: " + comObj.GetType().GUID);
			}

			public void AddToList(object obj)
			{
				if(hasBeenDisposed)
				{
					throw (new ObjectDisposedException(this.ToString(), 
						"Object has been disposed"));
				}

				aList.Add(obj);
			}

			public void CreateSemaphore()
			{
				// Create unmanaged handle here
				hSemaphore = NativeMethods.CreateSemaphore(IntPtr.Zero, 5, 5, null);
			}
			

			// The Dispose methods
			public void Dispose()
			{
				Dispose(true);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!hasBeenDisposed)
				{
					if (disposing)
					{
						// Dispose managed objects that do not implement IDisposable

						// Dispose all items in an array or ArrayList
						foreach (object obj in aList)
						{
							IDisposable disposableObj = obj as IDisposable;
							if (disposableObj != null)
							{
								disposableObj.Dispose();
							}
						}

						// Dispose managed objects implementing IDisposable
						fileStream.Close();
		
						// Dispose objects in the container that implement IDisposable
						containedObjs.Dispose();

						// Reduce reference count on RCW
						while (Marshal.ReleaseComObject(comObj) > 0);

						GC.SuppressFinalize(this);
					}

					// Release unmanaged handle here
					IntPtr prevCnt = new IntPtr();
                    NativeMethods.ReleaseSemaphore(hSemaphore, 1, out prevCnt);

					hasBeenDisposed = true;
				}
			}

			// The destructor
			~FooD()
			{
				Dispose(false);
			}

			// Optional Close method
			public void Close()
			{
				Dispose();
			}
		}

		// Class inherits from an IDisposable class
		public class Bar : FooD
		{
			//...

			private bool hasBeenDisposed;


			protected override void Dispose(bool disposing)
			{
				if (!hasBeenDisposed)
				{
					try
					{
						if(disposing)
						{
							// Call Dispose/Close/Clear on any managed objects here...        
						}

						// Release any unmanaged objects here...
					}
					finally
					{
						// Call base class' Dispose method
						base.Dispose(disposing);
						hasBeenDisposed = true;
					}
				}
			}
		}
		#endregion

		#region "3.18 Determining Where Boxing And Unboxing Occur"
		// See recipe 3.18 in book for explaination.
		#endregion
	}
}
