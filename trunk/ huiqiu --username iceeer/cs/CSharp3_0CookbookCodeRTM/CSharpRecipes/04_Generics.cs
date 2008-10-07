using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.ComponentModel; // ListSortDirection
using System.Runtime.InteropServices; // ReversibleSortedList
using System.Diagnostics; // DebuggerDisplay
using System.Linq;

namespace CSharpRecipes
{
	public static class Generics
	{
		#region "4.1 Deciding When and Where to Use Generics "
		// See this recipe in the book for more information
		#endregion

		#region "4.2 Understanding generic class types"
		public static void TestGenericClassInstanceCounter()
		{
			// regular class
			FixedSizeCollection A = new FixedSizeCollection(5);
			Console.WriteLine(A);
			FixedSizeCollection B = new FixedSizeCollection(5);
			Console.WriteLine(B);
			FixedSizeCollection C = new FixedSizeCollection(5);
			Console.WriteLine(C);

			// generic class
			FixedSizeCollection<bool> gA = new FixedSizeCollection<bool>(5);
			Console.WriteLine(gA);
			FixedSizeCollection<int> gB = new FixedSizeCollection<int>(5);
			Console.WriteLine(gB);
			FixedSizeCollection<string> gC = new FixedSizeCollection<string>(5);
			Console.WriteLine(gC);
			FixedSizeCollection<string> gD = new FixedSizeCollection<string>(5);
			Console.WriteLine(gD);

            bool b1 = true;
            bool b2 = false;
            bool bHolder = false;

            // add to the standard class (as object)
            A.AddItem(b1);
            A.AddItem(b2);
            // add to the generic class (as bool)
            gA.AddItem(b1);
            gA.AddItem(b2);

            Console.WriteLine(A);
            Console.WriteLine(gA);

            // have to cast or get error CS0266: 
            // Cannot implicitly convert type 'object' to 'bool'...
            bHolder = (bool)A.GetItem(1);
            // no cast necessary
            bHolder = gA.GetItem(1);

            int i1 = 1;
            int i2 = 2;
            int i3 = 3;
            int iHolder = 0;

            // add to the standard class (as object)
            B.AddItem(i1);
            B.AddItem(i2);
            B.AddItem(i3);
            // add to the generic class (as int)
            gB.AddItem(i1);
            gB.AddItem(i2);
            gB.AddItem(i3);

            Console.WriteLine(B);
            Console.WriteLine(gB);

            // have to cast or get error CS0266: 
            // Cannot implicitly convert type 'object' to 'int'...
            iHolder = (int)B.GetItem(1);
            // no cast necessary
            iHolder = gB.GetItem(1);

            string s1 = "s1";
            string s2 = "s2";
            string s3 = "s3";
            string sHolder = "";

            // add to the standard class (as object)
            C.AddItem(s1);
            C.AddItem(s2);
            C.AddItem(s3);
            // add an int to the string instance, perfectly OK
            C.AddItem(i1);

            // add to the generic class (as string)
            gC.AddItem(s1);
            gC.AddItem(s2);
            gC.AddItem(s3);
            // try to add an int to the string instance, denied by compiler
            // error CS1503: Argument '1': cannot convert from 'int' to 'string'
            //gC.AddItem(i1);

            Console.WriteLine(C);
            Console.WriteLine(gC);

            // have to cast or get error CS0266: 
            // Cannot implicitly convert type 'object' to 'string'...
            sHolder = (string)C.GetItem(1);
            // no cast necessary
            sHolder = gC.GetItem(1);
            // try to get a string into an int, error
            // error CS0029: Cannot implicitly convert type 'string' to 'int'
            //iHolder = gC.GetItem(1);
        }

        public class FixedSizeCollection
        {
            /// <summary>
            /// Constructor that increments static counter
            /// and sets the maximum number of items
            /// </summary>
            /// <param name="maxItems"></param>
            public FixedSizeCollection(int maxItems)
            {
                FixedSizeCollection.InstanceCount++;
                this.Items = new object[maxItems];
            }

            /// <summary>
            /// Add an item to the class whose type 
            /// is unknown as only object can hold any type
            /// </summary>
            /// <param name="item">item to add</param>
            /// <returns>the index of the item added</returns>
            public int AddItem(object item)
            {
                if (this.ItemCount < this.Items.Length)
                {
                    this.Items[this.ItemCount] = item;
                    return this.ItemCount++;
                }
                else
                    throw new Exception("Item queue is full");
            }

            /// <summary>
            /// Get an item from the class
            /// </summary>
            /// <param name="index">the index of the item to get</param>
            /// <returns>an item of type object</returns>
            public object GetItem(int index)
            {
                if (index >= this.Items.Length &&
                    index >= 0)
                    throw new ArgumentOutOfRangeException("index");

                return this.Items[index];
            }

            #region Properties
            /// <summary>
            /// Static instance counter hangs off of the Type for 
            /// StandardClass 
            /// </summary>
            public static int InstanceCount { get; set; }

            /// <summary>
            /// The count of the items the class holds
            /// </summary>
            public int ItemCount { get; private set; }

            /// <summary>
            /// The items in the class
            /// </summary>
            private object[] Items { get; set; }
            #endregion // Properties

            /// <summary>
            /// ToString override to provide class detail
            /// </summary>
            /// <returns>formatted string with class details</returns>
            public override string ToString()
            {
                return "There are " + FixedSizeCollection.InstanceCount.ToString() +
                    " instances of " + this.GetType().ToString() +
                    " and this instance contains " + this.ItemCount + " items...";
            }
        }

        /// <summary>
        /// A generic class to show instance counting
        /// </summary>
        /// <typeparam name="T">the type parameter used for the array storage</typeparam>
        public class FixedSizeCollection<T>
        {
            /// <summary>
            /// Constructor that increments static counter and sets up internal storage
            /// </summary>
            /// <param name="items"></param>
            public FixedSizeCollection(int items)
            {
                FixedSizeCollection<T>.InstanceCount++;
                this.Items = new T[items];
            }

            /// <summary>
            /// Add an item to the class whose type 
            /// is determined by the instantiating type
            /// </summary>
            /// <param name="item">item to add</param>
            /// <returns>the zero-based index of the item added</returns>
            public int AddItem(T item)
            {
                if (this.ItemCount < this.Items.Length)
                {
                    this.Items[this.ItemCount] = item;
                    return this.ItemCount++;
                }
                else
                    throw new Exception("Item queue is full");
            }

            /// <summary>
            /// Get an item from the class
            /// </summary>
            /// <param name="index">the zero-based index of the item to get</param>
            /// <returns>an item of the instantiating type</returns>
            public T GetItem(int index)
            {
                if (index >= this.Items.Length &&
                    index >= 0)
                    throw new ArgumentOutOfRangeException("index");

                return this.Items[index];
            }

            #region Properties
            /// <summary>
            /// Static instance counter hangs off of the 
            /// instantiated Type for 
            /// GenericClass
            /// </summary>
            public static int InstanceCount { get; set; }

            /// <summary>
            /// The count of the items the class holds
            /// </summary>
            public int ItemCount { get; private set; }

            /// <summary>
            /// The items in the class
            /// </summary>
            private T[] Items { get; set; }
            #endregion // Properties

            /// <summary>
            /// ToString override to provide class detail
            /// </summary>
            /// <returns>formatted string with class details</returns>
            public override string ToString()
            {
                return "There are " + FixedSizeCollection<T>.InstanceCount.ToString() +
                    " instances of " + this.GetType().ToString() +
                    " and this instance contains " + this.ItemCount + " items...";
            }
        }

		#endregion

		#region "4.3 Replacing the ArrayList with its Generic Counterpart"
		public static void UseNonGenericArrayList()
		{
			Console.WriteLine("\r\nUseNonGenericList");

			// Create and populate an ArrayList
			ArrayList numbers = new ArrayList();
			numbers.Add(1);    // Causes a boxing operation to occur
			numbers.Add(2);    // Causes a boxing operation to occur

			// Display all integers in the ArrayList
			// Causes an unboxing operation to occur on each iteration
			foreach (int i in numbers)
			{
				Console.WriteLine(i);
			}

			numbers.Clear();

			Console.WriteLine(numbers.IsReadOnly);
			Console.WriteLine(numbers.IsFixedSize);
			Console.WriteLine(numbers.IsSynchronized);
			Console.WriteLine(numbers.SyncRoot);
		}

		public static void UseGenericList()
		{
			Console.WriteLine("\r\nUseGenericList");

			// Create and populate a List
			List<int> numbers = new List<int>();
			numbers.Add(1);
			numbers.Add(2);

			// Display all integers in the ArrayList
			foreach (int i in numbers)
			{
				Console.WriteLine(i);
			}

			numbers.Clear();

			Console.WriteLine(((IList<int>)numbers).IsReadOnly);
			Console.WriteLine(((IList)numbers).IsFixedSize);
			Console.WriteLine(((IList)numbers).IsSynchronized);
			Console.WriteLine(((IList)numbers).SyncRoot);
		}

		public static void TestAdapterVSConstructor()
		{
			Console.WriteLine("\r\nAdapter Test");

			ArrayList al = new ArrayList();
			al.Add(new object());
			ArrayList adapter = ArrayList.Adapter(al);
			Console.WriteLine("al " + al[0].GetHashCode());
			Console.WriteLine("adapter " + adapter[0].GetHashCode());

			List<object> oldList = new List<object>();
			oldList.Add(new object());
			List<object> newList = new List<object>(oldList);
			Console.WriteLine("oldList " + oldList[0].GetHashCode());
			Console.WriteLine("newList " + newList[0].GetHashCode());
		}

		public static void TestCloneVSGetRange()
		{
			Console.WriteLine("\r\nClone Test");

			ArrayList al = new ArrayList();
			al.Add(new object());
			ArrayList clone = (ArrayList)al.Clone();
			Console.WriteLine("al " + al[0].GetHashCode());
			Console.WriteLine("clone " + clone[0].GetHashCode());

			List<object> oldList = new List<object>();
			oldList.Add(new object());
			List<object> newList = oldList.GetRange(0, oldList.Count);
			Console.WriteLine("oldList " + oldList[0].GetHashCode());
			Console.WriteLine("newList " + newList[0].GetHashCode());
		}

		public static void TestGenericRepeat()
		{
			List<int> numberList = new List<int>();
			numberList.Repeat(100, 3);

			foreach (int i in numberList)
				Console.WriteLine(i);
		}

		public static void Repeat<T>(this List<T> list, T obj, int count)
		{
			if (count < 0)
			{
				throw (new ArgumentException("The count parameter must be greater or equal to zero."));
			}

			for (int index = 0; index < count; index++)
			{
				list.Add(obj);
			}
		}

		public static void CloneGenericList()
		{
			Console.WriteLine("\r\nCloneGenericList");

			List<int> numbers = new List<int>();
			numbers.Add(1);
			numbers.Add(2);

			List<int> cloneNumbers = new List<int>(numbers);
			Console.WriteLine("cloneNumbers[0] " + cloneNumbers[0].GetHashCode());

			Console.WriteLine(((IList<int>)numbers).IsReadOnly);
			Console.WriteLine(((IList)numbers).IsFixedSize);
			Console.WriteLine(((IList)numbers).IsSynchronized);
			Console.WriteLine(((IList)numbers).SyncRoot);
		}
		#endregion

		#region "4.4 Replacing the Stack and Queue with Their Generic Counterparts"

        public static void UseGenericStack()
        {
            // Create a generic Stack object.
            Stack<int> numericStack = new Stack<int>();

            // Populate Stack.
            numericStack.Push(1);
            numericStack.Push(2);
            numericStack.Push(3);

            // De-populate Stack and display items.
            Console.WriteLine(numericStack.Pop().ToString());
            Console.WriteLine(numericStack.Pop().ToString());
            Console.WriteLine(numericStack.Pop().ToString());
        }

        public static void UseNonGenericStack()
        {
            // Create a nongeneric Stack object.
            Stack numericStack = new Stack();

            // Populate Stack (causing a boxing operation to occur).
            numericStack.Push(1);
            numericStack.Push(2);
            numericStack.Push(3);

            // De-populate Stack and display items (causing an unboxing operation to occur).
            Console.WriteLine(numericStack.Pop().ToString());
            Console.WriteLine(numericStack.Pop().ToString());
            Console.WriteLine(numericStack.Pop().ToString());
        }

        public static void CloneStack()
        {
            // Create a generic Stack object.
            Stack<int> numericStack = new Stack<int>();

            // Populate Stack.
            numericStack.Push(1);
            numericStack.Push(2);
            numericStack.Push(3);

            // Clone the numericStack object.
            Stack<int> clonedNumericStack = new Stack<int>(numericStack);

            // This does a simple peek at the values not a pop.
            foreach (int i in clonedNumericStack)
            {
                Console.WriteLine("foreach: " + i.ToString());
            }

            // De-populate Stack and display items.
            Console.WriteLine(clonedNumericStack.Pop().ToString());
            Console.WriteLine(clonedNumericStack.Pop().ToString());
            Console.WriteLine(clonedNumericStack.Pop().ToString());
        }


        public static void UseGenericQueue()
        {
            // Create a generic Queue object.
            Queue<int> numericQueue = new Queue<int>();

            // Populate Queue.
            numericQueue.Enqueue(1);
            numericQueue.Enqueue(2);
            numericQueue.Enqueue(3);

            // De-populate Queue and display items.
            Console.WriteLine(numericQueue.Dequeue());
            Console.WriteLine(numericQueue.Dequeue());
            Console.WriteLine(numericQueue.Dequeue());
        }

        public static void UseNonGenericQueue()
        {
            // Create a nongeneric Queue object.
            Queue numericQueue = new Queue();

            // Populate Queue (causing a boxing operation to occur).
            numericQueue.Enqueue(1);
            numericQueue.Enqueue(2);
            numericQueue.Enqueue(3);

            // De-populate Queue and display items (causing an unboxing operation to occur)
            Console.WriteLine(numericQueue.Dequeue());
            Console.WriteLine(numericQueue.Dequeue());
            Console.WriteLine(numericQueue.Dequeue().ToString());
        }

        public static void CloneQueue()
        {
            // Create a generic Queue object.
            Queue<int> numericQueue = new Queue<int>();

            // Populate Queue.
            numericQueue.Enqueue(1);
            numericQueue.Enqueue(2);
            numericQueue.Enqueue(3);

            // Create a clone of the numericQueue.
            Queue<int> clonedNumericQueue = new Queue<int>(numericQueue);

            // This does a simple peek at the values not a dequeue.
            foreach (int i in clonedNumericQueue)
            {
                Console.WriteLine("foreach: " + i.ToString());
            }

            // De-populate Queue and display items.
            Console.WriteLine(clonedNumericQueue.Dequeue().ToString());
            Console.WriteLine(clonedNumericQueue.Dequeue().ToString());
            Console.WriteLine(clonedNumericQueue.Dequeue().ToString());
        }
        #endregion

		#region "4.5 Using a Linked List"
        public static void UseLinkedList()
        {
	        Console.WriteLine("\r\n\r\n");

	        // Create TodoItem objects to add to the linked list
            ToDoItem i1 = 
                new ToDoItem() { Name = "paint door", Comment = "Should be done third" };
	        ToDoItem i2 = 
                new ToDoItem() { Name = "buy door", Comment = "Should be done first" };
            ToDoItem i3 = 
                new ToDoItem() { Name = "assemble door", Comment = "Should be done second" };
            ToDoItem i4 =
                new ToDoItem() { Name = "hang door", Comment = "Should be done last" };

            // Create a new LinkedList object
            LinkedList<ToDoItem> todoList = new LinkedList<ToDoItem>();

	        // Add the items
	        todoList.AddFirst(i1);
	        todoList.AddFirst(i2);
	        todoList.AddBefore(todoList.Find(i1), i3);
	        todoList.AddAfter(todoList.Find(i1), i4);

	        // Display all items
	        foreach (ToDoItem tdi in todoList)
	        {
		        Console.WriteLine(tdi.Name + " : " + tdi.Comment);
	        }

	        // Display information from the first node in the linked list
	        Console.WriteLine("todoList.First.Value.Name == " + 
                todoList.First.Value.Name);

	        // Display information from the second node in the linked list
	        Console.WriteLine("todoList.First.Next.Value.Name == " + 
                todoList.First.Next.Value.Name);

	        // Display information from the next to last node in the linked list
	        Console.WriteLine("todoList.Last.Previous.Value.Name == " + 
                todoList.Last.Previous.Value.Name);
        }

        /// <summary>
        /// Todo list item
        /// </summary>
        public class ToDoItem
        {
            /// <summary>
            /// Name of the item
            /// </summary>
	        public string Name { get; set; }

            /// <summary>
            /// Comment for the item
            /// </summary>
	        public string Comment { get; set; }
        }
		#endregion

		#region "4.6 Creating a Value Type that can be Initialized to Null"
		public static void TestNullableStruct()
		{
			Console.WriteLine("\r\n\r\n");

			//int? myDBInt = null;
			//  OR
			Nullable<int> myDBInt = new Nullable<int>();
			Nullable<int> myTempDBInt = new Nullable<int>();


			if (myDBInt.HasValue)
				Console.WriteLine("Has a value:  " + myDBInt.Value);
			else
				Console.WriteLine("Does not have a value (NULL)");

			myDBInt = 100;

			if (myDBInt != null)
				Console.WriteLine("Has a value:  " + myDBInt.Value);
			else
				Console.WriteLine("Does not have a value (NULL)");

			if (myTempDBInt != null)
			{
				if (myTempDBInt < 100)
					Console.WriteLine("myTempDBInt < 100");
				else
					Console.WriteLine("myTempDBInt >= 100");
			}
			else
			{
				// Handle the null here
			}
		}
		#endregion

        #region "4.7 Reversing the Contents of a Sorted List"

		public static void TestReversibleSortedList()
		{
            SortedList<int, string> data = new SortedList<int, string>();
            data.Add(2, "two");
            data.Add(5, "five");
            data.Add(3, "three");
            data.Add(1, "one");

            foreach (KeyValuePair<int, string> kvp in data)
            {
                Debug.WriteLine("\t" + kvp.Key + "\t" + kvp.Value);
            }
            Debug.WriteLine("");

            // query ordering by descending
            var query = from d in data
                        orderby d.Key descending
                        select d;

            foreach (KeyValuePair<int, string> kvp in query)
            {
                Debug.WriteLine("\t" + kvp.Key + "\t" + kvp.Value);
            }
            Debug.WriteLine("");


            data.Add(4, "four");

            // requery ordering by descending
            query = from d in data
                    orderby d.Key descending
                    select d;

            foreach (KeyValuePair<int, string> kvp in query)
            {
                Debug.WriteLine("\t" + kvp.Key + "\t" + kvp.Value);
            }
            Debug.WriteLine("");

            // Just go against the original list for ascending
            foreach (KeyValuePair<int, string> kvp in data)
            {
                Debug.WriteLine("\t" + kvp.Key + "\t" + kvp.Value);
            }
		}
		#endregion

		#region "4.8 Making read-only collections the generic way"
		public static void MakeCollectionReadOnly()
		{
			Lottery tryYourLuck = new Lottery();
			// print out the results
			for (int i = 0; i < tryYourLuck.Results.Count; i++)
			{
				Console.WriteLine("Lottery Number " + i + " is " + tryYourLuck.Results[i]);
			}

            int[] items = { 0, 1, 2 };
            ReadOnlyCollection<int> readOnlyItems = 
                new ReadOnlyCollection<int>(items);

			// change it so we win!
			//tryYourLuck.Results[0]=29;

			// if the above line is uncommented, you get
			// Error	26	
			//   Property or indexer 
			// 'System.Collections.ObjectModel.ReadOnlyCollection<int>.this[int]' 
			// cannot be assigned to -- it is read only	
		}

        public class Lottery
        {
	        // make a list
	        List<int> _numbers;

	        public Lottery()
	        {
		        // pick the winning numbers
                _numbers = new List<int>(5) { 17, 21, 32, 44, 58 }; 
	        }

	        public ReadOnlyCollection<int> Results
	        {
		        // return a wrapped copy of the results
		        get { return new ReadOnlyCollection<int>(_numbers); }
	        }
        }

		#endregion

		#region "4.9 Replacing the Hashtable with its Generic Counterpart"
        public static void UseNonGenericHashtable()
        {
	        Console.WriteLine("\r\nUseNonGenericHashtable");

	        // Create and populate a Hashtable
            var numbers = new Hashtable() 
                { {1, "one"}, // Causes a boxing operation to occur for the key
                  {2, "two"} }; // Causes a boxing operation to occur for the key

	        // Display all key/value pairs in the Hashtable
	        // Causes an unboxing operation to occur on each iteration for the key
	        foreach (DictionaryEntry de in numbers)
	        {
		        Console.WriteLine("Key: " + de.Key + "\tValue: " + de.Value);
	        }

	        Console.WriteLine(numbers.IsReadOnly);
	        Console.WriteLine(numbers.IsFixedSize);
	        Console.WriteLine(numbers.IsSynchronized);
	        Console.WriteLine(numbers.SyncRoot);

	        numbers.Clear();
        }

        public static void UseGenericDictionary()
        {
	        Console.WriteLine("\r\nUseGenericDictionary");

	        // Create and populate a Dictionary
            Dictionary<int, string> numbers = new Dictionary<int, string>() 
                { { 1, "one" }, { 2, "two" } };

	        // Display all key/value pairs in the Dictionary
	        foreach (KeyValuePair<int, string> kvp in numbers)
	        {
		        Console.WriteLine("Key: " + kvp.Key + "\tValue: " + kvp.Value);
	        }

	        Console.WriteLine(((IDictionary)numbers).IsReadOnly);
	        Console.WriteLine(((IDictionary)numbers).IsFixedSize);
	        Console.WriteLine(((IDictionary)numbers).IsSynchronized);
	        Console.WriteLine(((IDictionary)numbers).SyncRoot);

	        numbers.Clear();
        }

		public static void CopyToGenericDictionary()
		{
			Console.WriteLine("\r\nCopyToGenericDictionary");

	        // Create and populate a Dictionary
            Dictionary<int, string> numbers = new Dictionary<int, string>() 
                { { 1, "one" }, { 2, "two" } };

			// Display all key/value pairs in the Dictionary
			foreach (KeyValuePair<int, string> kvp in numbers)
			{
				Console.WriteLine("Key: " + kvp.Key + "\tValue: " + kvp.Value);
			}

			// Create object array to hold copied information from Dictionary object
			KeyValuePair<int, string>[] objs = new KeyValuePair<int,string>[numbers.Count];

			// Calling CopyTo on a Dictionary
			// Copies all KeyValuePair objects in Dictionary object to objs[]
			((IDictionary)numbers).CopyTo(objs, 0);

			// Display all key/value pairs in the objs[]
			foreach (KeyValuePair<int, string> kvp in objs)
			{
				Console.WriteLine("Key: " + kvp.Key + "\tValue: " + kvp.Value);
			}
		}

		public static void CloneGenericDictionary()
		{
			Console.WriteLine("\r\nCloneGenericDictionary");

	        // Create and populate a Dictionary
            Dictionary<int, string> numbers = 
                new Dictionary<int, string>() 
                { { 1, "one" }, { 2, "two" } };

			// Display all integers in the original Dictionary
			foreach (KeyValuePair<int, string> kvp in numbers)
			{
				Console.WriteLine("Original Key: " + kvp.Key + "\tValue: " + kvp.Value);
			}

			// Clone the Dictionary object
			Dictionary<int, string> clonedNumbers = 
                new Dictionary<int, string>(numbers);

			// Display all integers in the cloned Dictionary
			foreach (KeyValuePair<int, string> kvp in numbers)
			{
				Console.WriteLine("Cloned Key: " + kvp.Key + "\tValue: " + kvp.Value);
			}
		}
		#endregion

		#region "4.10 Using foreach With Generic Dictionary Types"
		public static void ShowForEachWithDictionary()
		{
	        // Create a Dictionary object and populate it
            Dictionary<int, string> myStringDict = new Dictionary<int, string>() 
                { { 1, "Foo" }, { 2, "Bar" }, { 3, "Baz" } };

			// Enumerate and display all key and value pairs
			foreach (KeyValuePair<int, string> kvp in myStringDict)
			{
				Console.WriteLine("key   " + kvp.Key);
				Console.WriteLine("Value " + kvp.Value);
				Console.WriteLine("kvp " + kvp.ToString());
			}

			// Using a DictionaryEntry object causes a compile-time error
            Hashtable myHashtable = new Hashtable() 
            { { 1, "Foo" }, { 2, "Bar" }, { 3, "Baz" } };
            foreach (DictionaryEntry de in myHashtable)
			{
			    Console.WriteLine("key   " + de.Key);
			    Console.WriteLine("Value " + de.Value);
			    Console.WriteLine("kvp " + de.ToString());
			}
		}
		#endregion

		#region "4.11 Constraining Type Arguments"
		public static void TestConversionCls()
		{
			Console.WriteLine("\r\n\r\n");

			Conversion<long> c = new Conversion<long>();
			//Console.WriteLine("long.MinValue:  " + c.ShowAsInt(long.MinValue));
			Console.WriteLine("-100:  " + c.ShowAsInt(-100));
			Console.WriteLine("0:  " + c.ShowAsInt(0));
			Console.WriteLine("100:  " + c.ShowAsInt(100));
			//Console.WriteLine("long.MaxValue:  " + c.ShowAsInt(long.MaxValue));
		}

		public static void TestComparableListCls()
		{
			Console.WriteLine("\r\n\r\n");

            ComparableList<int> cp = 
                new ComparableList<int>() { 100, 10 };

			Console.WriteLine("0 compare 1 == " + cp.Compare(0,1));
			Console.WriteLine("1 compare 0 == " + cp.Compare(1,0));
			Console.WriteLine("1 compare 1  == " + cp.Compare(1,1));
		}

		public static void TestDisposableListCls()
		{
			Console.WriteLine("\r\n\r\n");

			DisposableList<StreamReader> dl = new DisposableList<StreamReader>();

			// Create a few test objects
			StreamReader tr1 = new StreamReader("c:\\boot.ini");
			StreamReader tr2 = new StreamReader("c:\\autoexec.bat");
			StreamReader tr3 = new StreamReader("c:\\config.sys");

			// Add the test object to the DisposableList
			dl.Add(tr1);
			dl.Insert(0, tr2);

			Console.WriteLine("dl.IndexOf(tr3) == " + dl.IndexOf(tr3));

			dl.Add(tr3);

			Console.WriteLine("dl.Contains(tr1) == " + dl.Contains(tr1));

			StreamReader[] srArray = new StreamReader[3];
			dl.CopyTo(srArray, 0);
			Console.WriteLine("srArray[1].ReadLine() == " + srArray[1].ReadLine());

			Console.WriteLine("dl.Count == " + dl.Count);

			foreach(StreamReader sr in dl)
			{
				Console.WriteLine("sr.ReadLine() == " + sr.ReadLine());
			}

			Console.WriteLine("dl.IndexOf(tr3) == " + dl.IndexOf(tr3));

			Console.WriteLine("dl.IsReadOnly == " + dl.IsReadOnly);

			// Call Dispose before any of the disposable objects are removed from the DisposableList
			dl.RemoveAt(0);
			Console.WriteLine("dl.Count == " + dl.Count);

			dl.Remove(tr1);
			Console.WriteLine("dl.Count == " + dl.Count);

			dl.Clear();
			Console.WriteLine("dl.Count == " + dl.Count);
		}


		public class Conversion<T>
			where T : struct, IConvertible
		{
			public int ShowAsInt(T value)
			{
				return (value.ToInt32(NumberFormatInfo.CurrentInfo));
			}
		}

		public class ComparableList<T> : List<T>
			where T : IComparable<T>
		{
			public int Compare(int index1, int index2)
			{
				return (index1.CompareTo(index2));
			}
		}

		public class DisposableList<T> : IList<T>
			where T : class, IDisposable
		{
			private List<T> _items = new List<T>();

			// Private method that will dispose of items in the list
			private void Delete(T item)
			{
				item.Dispose();
			}

			// IList<T> Members
			public int IndexOf(T item)
			{
				return (_items.IndexOf(item));
			}

			public void Insert(int index, T item)
			{
				_items.Insert(index, item);
			}

			public T this[int index]
			{
				get	{return (_items[index]);}
				set	{_items[index] = value;}
			}

			public void RemoveAt(int index)
			{
				Delete(this[index]);
				_items.RemoveAt(index);
			}

			// ICollection<T> Members
			public void Add(T item)
			{
				_items.Add(item);
			}

			public bool Contains(T item)
			{
				return (_items.Contains(item));
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				_items.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get	{return (_items.Count);}
			}

			public bool IsReadOnly
			{
				get	{return (false);}
			}

			// IEnumerable<T> Members
			public IEnumerator<T> GetEnumerator()
			{
				return (_items.GetEnumerator());
			}

			// IEnumerable Members
			IEnumerator IEnumerable.GetEnumerator()
			{
				return (_items.GetEnumerator());
			}

			// Other members
			public void Clear()
			{
				for (int index = 0; index < _items.Count; index++)
				{
					Delete(_items[index]);
				}

				_items.Clear();
			}

			public bool Remove(T item)
			{
				int index = _items.IndexOf(item);

				if (index >= 0)
				{
					Delete(_items[index]);
					_items.RemoveAt(index);

					return (true);
				}
				else
				{
					return (false);
				}
			}
		}
		#endregion

		#region "4.12 Initializing Generic Variables to their Default Value"
		public static void ShowSettingFieldsToDefaults()
		{
			Console.WriteLine("\r\n\r\n");

			DefaultValueExample<int> dv = new DefaultValueExample<int>();

			// Check if the data is set to its defalut value, true is returned
			bool isDefault = dv.IsDefaultData();
			Console.WriteLine("Initial data: " + isDefault);

			// Set data
			dv.SetData(100);

			// Check again, this time a false is returned
			isDefault = dv.IsDefaultData();
			Console.WriteLine("Set data: " + isDefault);
		}

		public class DefaultValueExample<T>
		{
			T data = default(T);

			public bool IsDefaultData()
			{
				T temp = default(T);

				if (temp.Equals(data))
				{
					return (true);
				}
				else
				{
					return (false);
				}
			}

			public void SetData(T value)
			{
				data = value;
			}
		}
		#endregion

	}
}

