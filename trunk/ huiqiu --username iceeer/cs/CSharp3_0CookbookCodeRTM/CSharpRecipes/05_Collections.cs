using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Linq;
//using System.Runtime.Serialization;


namespace CSharpRecipes
{
    #region Extension Methods
    static class CollectionExtMethods
    {
        #region "5.1 Swapping Two Elements in an Array"
        public unsafe static void SwapElementsInArray(this int[] theArray, int index1, int index2)
		{
			if (theArray.Length > 0)
			{
				if (index1 >= theArray.Length || index2 >= theArray.Length || index1 < 0 || index2 < 0)
				{
					Console.WriteLine("index passed in to this method is out of bounds.");
				}
				else
				{
					fixed (int* PtrToSomeArray = theArray)
					{
						int tempHolder = theArray[index1];
						theArray[index1] = theArray[index2];
						theArray[index2] = tempHolder;
					}
				}
			}
			else
			{
				Console.WriteLine("Nothing to reverse");
			}
		}
		
		public static void SwapElementsInArray<T>(this T[] theArray, int index1, int index2)
		{
			if (theArray.Length > 0)
			{
				if (index1 >= theArray.Length || index2 >= theArray.Length || index1 < 0 || index2 < 0)
				{
					Console.WriteLine("index passed in to this method is out of bounds.");
				}
				else
				{
					T tempHolder = theArray[index1];
					theArray[index1] = theArray[index2];
					theArray[index2] = tempHolder;
				}
			}
			else
			{
				Console.WriteLine("Nothing to reverse");
			}
		}
		#endregion
		
		#region "5.2 Quickly Reversing an Array"
		public static void DoReversal<T>(this T[] theArray)
		{
			T tempHolder = default(T);

            if (theArray == null)
                throw new ArgumentNullException("theArray");
                
			if (theArray.Length > 0)
			{
				for (int counter = 0; counter < (theArray.Length / 2); counter++)
				{
					tempHolder = theArray[counter];                        
					theArray[counter] = theArray[theArray.Length - counter - 1];   
					theArray[theArray.Length - counter - 1] = tempHolder;      
				}
			}
			else
			{
				Console.WriteLine("Nothing to reverse");
			}
		}
        #endregion
   
        #region 5.3 Writing a More Flexible StackTrace Class
        public static ReadOnlyCollection<StackFrame> ToList(this StackTrace stackTrace)
        {
            if (stackTrace == null) 
            {
                throw new ArgumentNullException("stackTrace");
            }
            
            var frames = new StackFrame[stackTrace.FrameCount];
            for (int counter = 0; counter < stackTrace.FrameCount; counter++)
            {
                frames[counter] = stackTrace.GetFrame(counter);
            }
            
            return new 	ReadOnlyCollection<StackFrame>(frames);
        }
        #endregion

        #region 5.4 Determining the Number of Times an Item Appears in an List<T>
        // Count the number of times an item appears in this 
        //   unsorted or sorted List<T>
        public static int CountAll<T>(this List<T> myList, T searchValue)
        {
            return ((from t in myList where t.Equals(searchValue) select t).Count());
        }

        // Count the number of times an item appears in this sorted List<T>
        public static int BinarySearchCountAll<T>(this List<T> myList, T searchValue)
        {
            // Search for first item.
            int center = myList.BinarySearch(searchValue);
            int left = center;
            while (left < 0 && myList[left - 1].Equals(searchValue))
            {
                left -= 1;
            }

            int right = center;
            while (right < (myList.Count - 1) && myList[right + 1].Equals(searchValue))
            {
                right += 1;
            }

            return (right - left) + 1;
        }
        #endregion

        #region "5.5 Retrieving All Instances of a Specific Item in a List<T>"
        // The method to retrieve all matching objects in a 
        //  sorted or unsorted ListEx<T>
        public static IEnumerable<T> GetAll<T>(this List<T> myList, T searchValue)
        {
            return (from t in myList where t.Equals(searchValue) select t);
        }

        // The method to retrieve all matching objects in a sorted ListEx<T>
        public static T[] BinarySearchGetAll<T>(this List<T> myList, T searchValue)
        {
            List<T> RetObjs = new List<T>();

            // Search for first item.
            int center = myList.BinarySearch(searchValue);
            if (center > 0)
            {
                RetObjs.Add(myList[center]);

                int left = center;
                while (left > 0 && myList[left - 1].Equals(searchValue))
                {
                    left -= 1;
                    RetObjs.Add(myList[left]);
                }

                int right = center;
                while (right < (myList.Count - 1) &&
                    myList[right + 1].Equals(searchValue))
                {
                    right += 1;
                    RetObjs.Add(myList[right]);
                }
            }

            return (RetObjs.ToArray());
        }
        #endregion
		
		#region "5.6 Inserting and Removing Items from a Array"
		public static void InsertIntoArray(this Array target, object value, int index)
		{
			if (index < target.GetLowerBound(0) || index > target.GetUpperBound(0))
			{
				throw (new ArgumentOutOfRangeException("index", index, 
					"Array index out of bounds."));
			}
			else if (index == target.GetLowerBound(0))
			{
				Array.Copy(target, index, target, index + 1, 
					target.Length - index - 1);
			}
			else
			{
				Array.Copy(target, index, target, index + 1, 
					target.Length - index - 1);
			}

			target.SetValue(value, index);
		}

		public static void RemoveFromArray<T>(this T[] target, int index)
		{
			if (index < target.GetLowerBound(0) || index > target.GetUpperBound(0))
			{
				throw (new ArgumentOutOfRangeException("index", index, 
					"Array index out of bounds."));
			}
			else if (index < target.GetUpperBound(0))
			{
				Array.Copy(target, index + 1, target, index, 
					target.Length - index - 1);
			}

			target.SetValue(null, target.GetUpperBound(0));
		}
        #endregion
    }
    #endregion
    
	public class Collections
	{
	    #region "5.1 Swapping Two Elements in an Array (TESTER)"
		public static void TestSwapArrayElements()
		{
			int[] someArray = {1,2,3,4,5};
			
			for (int counter = 0; counter < someArray.Length; counter++)
			{
				Console.WriteLine("Element " + counter + " = " + 
					someArray[counter]);
			}

            someArray.SwapElementsInArray(0, someArray.Length - 1);

			for (int counter = 0; counter < someArray.Length; counter++)
			{
				Console.WriteLine("Element " + counter + " = " + 
					someArray[counter]);
			}

			Console.WriteLine();
			object[] SomeObjArray = {1,2,3,4,5};
			
			for (int counter = 0; counter < SomeObjArray.Length; counter++)
			{
				Console.WriteLine("Element " + counter + " = " + 
					SomeObjArray[counter]);
			}

            SomeObjArray.SwapElementsInArray(0, someArray.Length - 1);

			for (int counter = 0; counter < SomeObjArray.Length; counter++)
			{
				Console.WriteLine("Element " + counter + " = " + 
					SomeObjArray[counter]);
			}
		}

	    #endregion

        #region "5.2 Quickly Reversing an Array (TESTER)"
        public unsafe static void TestArrayReversal()
        {
            int[] someArray = { 1, 2, 3, 4, 5 };

            for (int counter = 0; counter < someArray.Length; counter++)
            {
                Console.WriteLine("Element " + counter + " = " + someArray[counter]);
            }

            someArray.DoReversal();

            for (int counter = 0; counter < someArray.Length; counter++)
            {
                Console.WriteLine("Element " + counter + " = " + someArray[counter]);
            }

            Array.Reverse(someArray);

            for (int counter = 0; counter < someArray.Length; counter++)
            {
                Console.WriteLine("Element " + counter + " = " + someArray[counter]);
            }
        }
        #endregion

        #region 5.3 Writing a More Flexible StackTrace Class (TESTER)
        public static void TestFlexibleStackTrace()
        {
            StackTrace sTrace = new StackTrace();
            IList<StackFrame> frames = sTrace.ToList();
            
            // Display the first stack frame.
            Console.WriteLine(frames[0].ToString());

            // Display all stack frames.
            foreach (StackFrame SF in frames)
            {
                Console.WriteLine("stackframe: " + SF.ToString());
            }

            // Test the copy to array functionality
            Console.WriteLine("---------------------");
            StackFrame[] myNewArray = new StackFrame[frames.Count];
            frames.CopyTo(myNewArray, 0);

            // Display all stack frames.
            foreach (StackFrame SF in myNewArray)
            {
                Console.WriteLine("stackframe: " + SF.ToString());
            }

            // Test ctor that accepts an exception obj
            Console.WriteLine("---------------------");
            sTrace = new StackTrace(new Exception(), true);
            frames = sTrace.ToList();

            Console.WriteLine("TOSTRING: " + Environment.NewLine + frames.ToString());
            foreach (StackFrame SF in frames)
            {
                Console.WriteLine(SF.ToString());
            }
        }
        
        //public class StackTraceList : StackTrace, IList
        //{
        //    public StackTraceList()
        //        : base()
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(bool needFileInfo)
        //        : base(needFileInfo)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(Exception e)
        //        : base(e)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(int skipFrames)
        //        : base(skipFrames)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(StackFrame frame)
        //        : base(frame)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(Exception e, bool needFileInfo)
        //        : base(e, needFileInfo)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(Exception e, int skipFrames)
        //        : base(e, skipFrames)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(int skipFrames, bool needFileInfo) :
        //        base(skipFrames, needFileInfo)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(Thread targetThread, bool needFileInfo) :
        //        base(targetThread, needFileInfo)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    public StackTraceList(Exception e, int skipFrames, bool needFileInfo) :
        //        base(e, skipFrames, needFileInfo)
        //    {
        //        InitInternalFrameArray();
        //    }

        //    private StackFrame[] internalFrameArray = null;

        //    private void InitInternalFrameArray()
        //    {
        //        internalFrameArray = new StackFrame[base.FrameCount];

        //        for (int counter = 0; counter < base.FrameCount; counter++)
        //        {
        //            internalFrameArray[counter] = base.GetFrame(counter);
        //        }
        //    }

        //    public string GetFrameAsString(int index)
        //    {
        //        StringBuilder str = new StringBuilder("\tat ");
        //        str.Append(GetFrame(index).GetMethod().DeclaringType.FullName);
        //        str.Append(".");
        //        str.Append(GetFrame(index).GetMethod().Name);
        //        str.Append("(");
        //        foreach (ParameterInfo PI in GetFrame(index).GetMethod().GetParameters())
        //        {
        //            str.Append(PI.ParameterType.Name);
        //            if (PI.Position <
        //                (GetFrame(index).GetMethod().GetParameters().Length - 1))
        //            {
        //                str.Append(", ");
        //            }
        //        }
        //        str.Append(")");

        //        return (str.ToString());
        //    }

        //    // IList properties/methods
        //    public bool IsFixedSize
        //    {
        //        get { return (internalFrameArray.IsFixedSize); }
        //    }

        //    public bool IsReadOnly
        //    {
        //        get { return (true); }

        //    }

        //    // Note that this indexer must return an object to comply
        //    // with the IList interface for this indexer.
        //    public object this[int index]
        //    {
        //        get { return (internalFrameArray[index]); }
        //        set
        //        {
        //            throw (new NotSupportedException(
        //                "The set indexer method is not supported on this object."));
        //        }
        //    }

        //    public int Add(object value)
        //    {
        //        return (((IList)internalFrameArray).Add(value));
        //    }

        //    public void Insert(int index, object value)
        //    {
        //        ((IList)internalFrameArray).Insert(index, value);
        //    }

        //    public void Remove(object value)
        //    {
        //        ((IList)internalFrameArray).Remove(value);
        //    }

        //    public void RemoveAt(int index)
        //    {
        //        ((IList)internalFrameArray).RemoveAt(index);
        //    }

        //    public void Clear()
        //    {
        //        // Throw an exception here to prevent the loss of data.
        //        throw (new NotSupportedException(
        //            "The Clear method is not supported on this object."));
        //    }

        //    public bool Contains(object value)
        //    {
        //        return (((IList)internalFrameArray).Contains(value));
        //    }

        //    public int IndexOf(object value)
        //    {
        //        return (((IList)internalFrameArray).IndexOf(value));
        //    }

        //    // IEnumerable method
        //    public IEnumerator GetEnumerator()
        //    {
        //        return (internalFrameArray.GetEnumerator());

        //    }

        //    // ICollection properties/methods
        //    public int Count
        //    {
        //        get { return (internalFrameArray.Length); }
        //    }

        //    public bool IsSynchronized
        //    {
        //        get { return (internalFrameArray.IsSynchronized); }
        //    }

        //    public object SyncRoot
        //    {
        //        get { return (internalFrameArray.SyncRoot); }
        //    }

        //    public void CopyTo(Array array, int index)
        //    {
        //        internalFrameArray.CopyTo(array, index);
        //    }
        //}
        #endregion
        
        #region "5.4 Determining the Number of Times an Item Appears in an List<T> (TESTER)"
        public static void TestArrayListEx()
		{
			List<int> arrayExt = new List<int>() {-2,-2,-1,-1,1,2,2,2,2,3,100,4,5};

			Console.WriteLine("--CONTAINS TOTAL--");
            int count = arrayExt.CountAll(2);
            Console.WriteLine("Count2: " + count);

            count = arrayExt.CountAll(3);
            Console.WriteLine("Count3: " + count);

            count = arrayExt.CountAll(1);
            Console.WriteLine("Count1: " + count);

			Console.WriteLine("\r\n--BINARY SEARCH COUNT ALL--");
            arrayExt.Sort();
            count = arrayExt.BinarySearchCountAll(2);
            Console.WriteLine("Count2: " + count);

            count = arrayExt.BinarySearchCountAll(3);
            Console.WriteLine("Count3: " + count);

            count = arrayExt.BinarySearchCountAll(1);
            Console.WriteLine("Count1: " + count);
        }
		#endregion
        
        #region "5.5 Retrieving All Instances of a Specific Item in a List<T> (TESTER)"
		public static void TestArrayListEx2()
		{
			List<int> arrayExt = new List<int>() {-1,-1,1,2,2,2,2,3,100,4,5};

			Console.WriteLine("--GET All--");
			IEnumerable<int> objects = arrayExt.GetAll(2);
			foreach (object o in objects)
			{
				Console.WriteLine("obj2: " + o);
			}

			Console.WriteLine();
			objects = arrayExt.GetAll(-2);
			foreach (object o in objects)
			{
				Console.WriteLine("obj-2: " + o);
			}

			Console.WriteLine();
			objects = arrayExt.GetAll(5);
			foreach (object o in objects)
			{
				Console.WriteLine("obj5: " + o);
			}

			Console.WriteLine("\r\n--BINARY SEARCH GET ALL--");
			arrayExt.Sort();
            int[] objs = arrayExt.BinarySearchGetAll(-2);
            foreach (object o in objs)
			{
				Console.WriteLine("obj-2: " + o);
			}

			Console.WriteLine();
            objs = arrayExt.BinarySearchGetAll(2);
            foreach (object o in objs)
			{
				Console.WriteLine("obj2: " + o);
			}

			Console.WriteLine();
            objs = arrayExt.BinarySearchGetAll(5);
            foreach (object o in objs)
			{
				Console.WriteLine("obj5: " + o);
			}
		}
        #endregion		
   
        #region "5.6 Inserting and Removing Items from a Array (TESTER)"
        public static void TestArrayInsertRemove()
        {
            string[] numbers = { "one", "two", "four", "five", "six" };

            numbers.InsertIntoArray("three", 2);
            foreach (string number in numbers)
            {
                Console.WriteLine(number);
            }

            Console.WriteLine();

            numbers.RemoveFromArray(2);
            foreach (string number in numbers)
            {
                Console.WriteLine(number);
            }
        }
        #endregion

        #region "5.7 Keeping Your List<T> Sorted"
		public static void TestSortedList()
		{
			// Create a SortedArrayList and populate it with 
			//    randomly choosen numbers
			SortedList<int> SortedAL = new SortedList<int>();
			SortedAL.Add(200);
			SortedAL.Add(20);
			SortedAL.Add(2);
			SortedAL.Add(7);
			SortedAL.Add(10);
			SortedAL.Add(0);
			SortedAL.Add(100);
			SortedAL.Add(-20);
			SortedAL.Add(56);
			SortedAL.Add(55);
			SortedAL.Add(57);
			SortedAL.Add(200);
			SortedAL.Add(-2);
			SortedAL.Add(-20);
			SortedAL.Add(55);
			SortedAL.Add(55);

			// Display it
			foreach (int i in SortedAL)
			{
				Console.WriteLine(i);
			}

			// Now modify a value at a particular index
			SortedAL.ModifySorted(0, 5);
			SortedAL.ModifySorted(1, 10);
			SortedAL.ModifySorted(2, 11);
			SortedAL.ModifySorted(3, 7);
			SortedAL.ModifySorted(4, 2);
			SortedAL.ModifySorted(2, 4);
			SortedAL.ModifySorted(15, 0);
			SortedAL.ModifySorted(0, 15);
			SortedAL.ModifySorted(223, 15);

			// Display it
			Console.WriteLine();
			foreach (int i in SortedAL)
			{
				Console.WriteLine(i);
			}
			
			// Doing it the hard way
			List<int> Test = new List<int>();
			Test.Add(200);
			Test.Sort();
			Test.Add(20);
			Test.Sort();
			Test.Add(2);
			Test.Sort();
			Test.Add(7);
			Test.Sort();
			Test.Add(10);
			Test.Sort();
			Test.Add(0);
			Test.Sort();
			Test.Add(100);
			Test.Sort();
			Test.Add(-20);
			Test.Sort();
			Test.Add(56);
			Test.Sort();
			Test.Add(55);
			Test.Sort();
			Test.Add(57);
			Test.Sort();
			Test.Add(200);
			Test.Sort();
		}
/*  ORIGINAL DATA
-20
-20
-2
0
2
7
10
20
55
55
55
56
57
100
200
200
 
-20
0
0
0
2
2
3
4
10
15
20
55
55
57
100
223
*/

		public class SortedList<T> : List<T>
		{
			public new void Add(T item) 
			{
				int position = this.BinarySearch(item);
				if (position < 0)
				{
					position = ~position;
				}

				this.Insert(position, item);
			}

			public void ModifySorted(T item, int index)
			{
				this.RemoveAt(index);

				int position = this.BinarySearch(item);
				if (position < 0)
				{
					position = ~position;
				}

				this.Insert(position, item);
			}
		}
		#endregion
                 
        #region "5.8 Sorting a Dictionary’s Keys and/or Values"
        public static void TestSortKeyValues()
        {
			// Define a Dictionary<T,U> object
			Dictionary<string, string> hash = new Dictionary<string, string>();
//			hash.Add(2, "two");
//			hash.Add(1, "one");
//			hash.Add(5, "five");
//			hash.Add(4, "four");
//			hash.Add(3, "three");
            hash.Add("2", "two");
            hash.Add("1", "one");
            hash.Add("5", "five");
            hash.Add("4", "four");
            hash.Add("3", "three");
			
			
			
			
			var x = from k in hash.Keys orderby k ascending select k;
			foreach (string s in x)
                Console.WriteLine("Key: " + s + "    Value: " + hash[s]);

            Console.WriteLine();

            x = from k in hash.Keys orderby k descending select k;
            foreach (string s in x)
                Console.WriteLine("Key: " + s + "    Value: " + hash[s]);


            x = from k in hash.Values orderby k ascending select k;
            foreach (string s in x)
                Console.WriteLine("Value: " + s);

            Console.WriteLine();

            x = from k in hash.Values orderby k descending select k;
            foreach (string s in x)
                Console.WriteLine("Value: " + s);
		}
		#endregion

        #region "5.9 Creating a Hashtable with Max and Min Value Boundaries"
        public static void TestMaxMinValueHash()
        {
            MaxMinValueDictionary<int, int> Table = new MaxMinValueDictionary<int, int>(100, 200);
            Table.Add(1, 100);
            Table.Add(2, 200);
            Table.Add(3, 150);
            Table.Add(4, 200);
            Table[2] = 100;
            Table[2] = 500;
            Table.Add(5, 200);
            //Table.Add(6,20);
            //Table.Add(7,2000);

            Table.Remove(1);
            Table.Remove(2);
            Table.Remove(3);

            Table.Clear();
        }



        [Serializable]
        public class MaxMinValueDictionary<T, U>
            where U : IComparable<U>
        {
            protected Dictionary<T, U> internalDictionary = null;

            public MaxMinValueDictionary(U minValue, U maxValue)
            {
                this.minValue = minValue;
                this.maxValue = maxValue;
                internalDictionary = new Dictionary<T, U>();
            }

            protected U minValue = default(U);
            protected U maxValue = default(U);

            public int Count
            {
                get { return (internalDictionary.Count); }
            }

            public Dictionary<T, U>.KeyCollection Keys
            {
                get { return (internalDictionary.Keys); }
            }

            public Dictionary<T, U>.ValueCollection Values
            {
                get { return (internalDictionary.Values); }
            }

            public U this[T key]
            {
                get
                {
                    return (internalDictionary[key]);
                }
                set
                {
                    if (value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0)
                    {
                        internalDictionary[key] = value;
                    }
                    else
                    {
                        throw (new ArgumentOutOfRangeException("value", value,
                        "Value must be within the range " + minValue + " to " + maxValue));
                    }
                }
            }

            public void Add(T key, U value)
            {
                if (value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0)
                {
                    internalDictionary.Add(key, value);
                }
                else
                {
                    throw (new ArgumentOutOfRangeException("value", value,
                    "Value must be within the range " + minValue + " to " + maxValue));
                }
            }

            public bool ContainsKey(T key)
            {

                return (internalDictionary.ContainsKey(key));
            }

            public bool ContainsValue(U value)
            {
                return (internalDictionary.ContainsValue(value));
            }

            public override bool Equals(object obj)
            {
                return (internalDictionary.Equals(obj));
            }

            public IEnumerator GetEnumerator()
            {
                return (internalDictionary.GetEnumerator());
            }

            public override int GetHashCode()
            {
                return (internalDictionary.GetHashCode());
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                internalDictionary.GetObjectData(info, context);
            }

            public void OnDeserialization(object sender)
            {
                internalDictionary.OnDeserialization(sender);
            }

            public override string ToString()
            {
                return (internalDictionary.ToString());
            }

            public bool TryGetValue(T key, out U value)
            {
                return (internalDictionary.TryGetValue(key, out value));
            }

            public void Remove(T key)
            {
                internalDictionary.Remove(key);
            }

            public void Clear()
            {
                internalDictionary.Clear();
            }
        }
        #endregion

        #region "5.10 Storing Snapshots of Lists in an Array"
		public static void TestListSnapshot()
		{
			Queue<int> someQueue = new Queue<int>();
			someQueue.Enqueue(1);
			someQueue.Enqueue(2);
			someQueue.Enqueue(3);
			
			int[] queueSnapshot = TakeSnapshotOfList<int>(someQueue);
			foreach (int i in queueSnapshot)
			{
				Console.WriteLine(i.ToString());
			}
		}

        public static T[] TakeSnapshotOfList<T>(IEnumerable<T> theList)
        {
            T[] snapshot = theList.ToArray();
            return (snapshot);
        }
        #endregion
		
        #region "5.11 Persisting a Collection Between Application Sessions"
		public static void TestSerialization()
		{
			ArrayList HT = new ArrayList() {"Zero","One","Two"};
			
			foreach (object O in HT)
				Console.WriteLine(O.ToString());
			SaveObj<ArrayList>(HT, "HT.data");
				
			ArrayList HTNew = new ArrayList();
			HTNew = RestoreObj<ArrayList>("HT.data");
			foreach (object O in HTNew)
				Console.WriteLine(O.ToString());
				
			if (HT == HTNew)
				Console.WriteLine("Same reference");
			else
				Console.WriteLine("Different reference");
				
			if (HT[0] == HTNew[0])
				Console.WriteLine("Same [0] reference");
			else
				Console.WriteLine("Different [0] reference");


			Console.WriteLine();
			List<int> test = new List<int>() {1,2};
			foreach (int i in test)
				Console.WriteLine(i.ToString());
			SaveObj<List<int>>(test, "TEST.DATA");
			List<int> testNew = new List<int>();
			testNew = RestoreObj<List<int>>("TEST.DATA");
			foreach (int i in testNew)
				Console.WriteLine(i.ToString());

			Console.WriteLine();
			Dictionary<int, int> testD = new Dictionary<int, int>() {{1,1},{2,2}};
			foreach (KeyValuePair<int,int> kvp in testD)
				Console.WriteLine(kvp.Key + " : " + kvp.Value);
			SaveObj<Dictionary<int, int>>(testD, "TEST.DATA");
			Dictionary<int, int> testDNew = new Dictionary<int, int>();
			testDNew = RestoreObj<Dictionary<int, int>>("TEST.DATA");
			foreach (KeyValuePair<int, int> kvp in testDNew)
				Console.WriteLine(kvp.Key + " : " + kvp.Value);
		}


		public static void SaveObj<T>(T obj, string dataFile)
		{
			using (FileStream FS = File.Create(dataFile))
			{
			    BinaryFormatter binSerializer = new BinaryFormatter();
			    binSerializer.Serialize(FS, obj);
			}
		}
		
		public static T RestoreObj<T>(string dataFile)
		{
		    T obj = default(T);
		    
			using (FileStream FS = File.OpenRead(dataFile))
			{
			    BinaryFormatter binSerializer = new BinaryFormatter();
			    obj = (T)binSerializer.Deserialize(FS);
			}
			
			return (obj);
		}
		#endregion
        
		#region "5.12 Testing Every Element In An Array or List<T>"
		public static void TestArrayForNulls()
		{
			// Create a List of strings
			List<string> strings = new List<string>() {"one",null,"three","four"};

			// Determine if there are no null values in the List
			string str = strings.TrueForAll(delegate(string val) 
			{
				if (val == null)
					return false;
				else
					return true;
			}).ToString();
			
			// Display the results
			Console.WriteLine(str);
        }
        #endregion

        #region "5.13 Performing an Action on Each Element in an Array or List<T>"
        // The Data class
		public class Data
		{
			public Data(int v)
			{
				val = v;
			}
			
			public int val = 0;
		}
		
		public static void TestArrayForEach()
		{
			// Create and populate a List of Data objects
            List<Data> numbers = new List<Data>() {new Data(1), new Data(2), new Data(3), new Data(4)};

			// Display them
			foreach (Data d in numbers)
				Console.WriteLine(d.val);

			// Add 2 to all Data.val integer values
			numbers.ForEach(delegate(Data obj)
			{
                obj.val += 2;
			});

			// Display them
			foreach (Data d in numbers)
				Console.WriteLine(d.val);

			// Total val integer values in all Data objects in the List
			int total = 0;
			numbers.ForEach(delegate(Data obj)
			{
				total += obj.val;
			});
			
			// Display total
			Console.WriteLine("Total: " + total);
		}
	#endregion 

		#region "5.14 Creating a Read Only Array"
		public static void TestReadOnlyArray()
		{
			// Create and populate a List of strings
			List<string> strings = new List<string>() {"1","2","3","4"};
			
			// Create a read-only strings List
			IList<string> readOnlyStrings = strings.AsReadOnly();

			// Display them
			foreach (string s in readOnlyStrings)
				Console.WriteLine(s);
				
			// These actions are not allowed and will throw a
			//     System.NotSupportedException: Collection is read-only.
			//readOnlyStrings.Add("NEW");
			//readOnlyStrings.Remove("1");
			//readOnlyStrings[0] = "NEW";
			
			// However, changing the value in the original List is allowed
			strings[0] = "one";
			strings[1] = null;
			
			// In additon, the readOnlyStrings reference may be pointed at a new List object
			readOnlyStrings = new List<string>();

			// Display them
			foreach (string s in readOnlyStrings)
				Console.WriteLine(s);
			
		}
	#endregion
		
        #region "BONUS: Displaying an Array’s Data as a Delimited String"
		public static void TestDisplayDataAsDelStr()
		{
			string[] Numbers = {"one", "two", "three", "four", "five", "six"} ;
			
			string DelimitedStr = ConvertCollectionToDelStr(Numbers, ',');
			Console.WriteLine(DelimitedStr);
		}
		
		public static string ConvertArrayToDelStr(Array theArray, string delimiter)
		{
			string delimitedData = "";
			
			for (int index = 0; index < theArray.Length; index++)
			{
				if (theArray.GetValue(index).ToString().Equals(delimiter.ToString()))
				{
					throw (new ArgumentException("Cannot have a delimiter character as an element of the array.", "delimiter"));
				}

				delimitedData += theArray.GetValue(index);
				
				// Add the delimiting string unless we are on the last item in the array
				if (index < (theArray.Length - 1))
				{
					delimitedData += delimiter;
				}
			}
			
			return (delimitedData);
		}
		
		public static string ConvertCollectionToDelStr(ICollection theCollection, char delimiter)
		{
			string delimitedData = "";

			foreach (string strData in theCollection)
			{
				if (strData.Equals(delimiter.ToString()))
				{
					throw (new ArgumentException("Cannot have a delimiter character as an element of the array.", "theCollection"));
				}
				
				delimitedData += strData + delimiter;
			}
			
			// Return the constructed string minus the final appended delimiter char
			return (delimitedData.TrimEnd(delimiter));
		}
        #endregion
    }
}
