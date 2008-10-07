using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using System.Security.Principal;

namespace CSharpRecipes
{
    public static class IteratorExtensionMethods
    {
        public static IEnumerable<T> EveryNthItem<T>(this IEnumerable<T> enumerable, int step)
        {
            int current = 0;
            foreach (T item in enumerable)
            {
                ++current;
                if (current % step == 0)
                    yield return item;
            }
        }
    }

	public partial class IteratorsAndPartialTypesAndPartialMethods
	{
        #region "6.1 Creating an Iterator on a Generic Type"
        public static void TestShoppingCart()
        {
            //Create ShoppingList object and fill it with data
            ShoppingList<string> shoppingCart = new ShoppingList<string>(){
                "item1","item2","item3","item4","item5","item6"};

            // Display all data in ShoppingCart object
            foreach (string item in shoppingCart)
            {
                Console.WriteLine(item);
            }
        }


        public class ShoppingList<T> : IEnumerable<T>
        {
            private List<T> _items = new List<T>();

            public void Add(T name)
            {
                _items.Add(name);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        #endregion

        #region "6.2 Creating an Iterator on a Non-Generic Type"
        public static void TestStampCollection()
        {
            //Create a StampCollection and fill it with stamps
            StampCollection stamps = new StampCollection() {
                new Stamp(1998,"Louisiana Duck"),
                new Stamp(1968,"Goethals Memorial"),
                new Stamp(1909,"Carmine Hudson"),
                new Stamp(1936,"Hotel Corner Card")};

            foreach (Stamp stamp in stamps)
            {
                Console.WriteLine(stamp);
            }
        }

        public class Stamp
        {
            public Stamp(int year, string name)
            {
                this.Year = year;
                this.Name = name;
            }
            public int Year { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return this.Year + ":" + this.Name;
            }
        }

        public class StampCollection : IEnumerable
        {
            private Dictionary<string, Stamp> _stamps =
                new Dictionary<string, Stamp>();

            public void Add(Stamp stamp)
            {
                _stamps.Add(stamp.Name, stamp);
            }

            public IEnumerator GetEnumerator()
            {
                // Return all stamps in the stamp collection
                // in order of publication
                var orderedStamps = from Stamp stamp in _stamps.Values
                                    orderby stamp.Year
                                    select stamp;
                foreach (Stamp stamp in orderedStamps)
                {
                    yield return stamp;
                }
            }
        }
        #endregion
        
        #region "6.3 Creating Custom Enumerators"
		public static void TestIterators()
		{
	        Container<int> container = new Container<int>();

	        // Create test data
	        List<int> testData = new List<int>(){
	            -1,1,2,3,4,5,6,7,8,9,10,200,500};

	        // Add test data to Container object
	        container.Clear();
            container.AddRange(testData);

            // Iterate over Container object
            foreach (int i in container)
            {
                Console.WriteLine(i);
            }

	        Console.WriteLine();
	        foreach (int i in container.GetReverseOrderEnumerator())
	        {
		        Console.WriteLine(i);
	        }

	        Console.WriteLine();
	        foreach (int i in container.GetForwardStepEnumerator(2))
	        {
		        Console.WriteLine(i);
	        }

	        Console.WriteLine();
	        foreach (int i in container.GetReverseStepEnumerator(3))
	        {
		        Console.WriteLine(i);
	        }
		}

        public class Container<T> : IEnumerable<T>
        {
            public Container() {}

            private List<T> _internalList = new List<T>();

            // This iterator iterates over each element from first to last
            public IEnumerator<T> GetEnumerator()
            {
                return _internalList.GetEnumerator();
            }

            // This iterator iterates over each element from last to first
            public IEnumerable<T> GetReverseOrderEnumerator()
            {
                foreach (T item in ((IEnumerable<T>)_internalList).Reverse())
                {
	                yield return item;
                }
            }

            // This iterator iterates over each element from first to last stepping 
            // over a predefined number of elements
            public IEnumerable<T> GetForwardStepEnumerator(int step)
            {
                foreach (T item in _internalList.EveryNthItem(step))
                {
                    yield return item;
                }
            }

            // This iterator iterates over each element from last to first stepping 
            // over a predefined number of elements
            public IEnumerable<T> GetReverseStepEnumerator(int step)
            {
                foreach (T item in ((IEnumerable<T>)_internalList).Reverse().EveryNthItem(step))
                {
                    yield return item;
                }
            }

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            public void Clear()
            {
                _internalList.Clear();
            }

            public void Add(T item)
            {
                _internalList.Add(item);
            }

            public void AddRange(ICollection<T> collection)
            {
                _internalList.AddRange(collection);
            }
        }


        #endregion

		#region "6.4 Implementing iterators with LINQ"
        public static void TestIteratorsAndLinq()
        {
            //Create SectionalList and fill it with data
            SectionalList<int> sectionalList = new SectionalList<int>() {
                12,26,95,37,50,33,81,54};

            // Display all data in SectionalList
	        Console.WriteLine("\r\nGetEnumerator iterator");
	        foreach (int i in sectionalList)
	        {
                Console.Write(i + ":");
            }
            Console.WriteLine("");

	        Console.WriteLine("\r\nGetFirstHalf iterator");
	        foreach (int i in sectionalList.GetFirstHalf())
	        {
                Console.Write(i + ":");
            }
            Console.WriteLine("");

	        Console.WriteLine("\r\nGetSecondHalf iterator");
            foreach (int i in sectionalList.GetSecondHalf())
	        {
		        Console.Write(i + ":");
	        }
            Console.WriteLine("");

            Console.WriteLine("\r\nGetFilteredValues iterator");
            // make a predicate test for even numbers
            Func<int, bool> predicate = item => (item % 2 == 0);
            foreach (int i in sectionalList.GetFilteredValues(predicate))
            {
                Console.Write(i + ":");
            }
            Console.WriteLine("");

            Console.WriteLine("\r\nGetReverseFilteredValues iterator");
            foreach (int i in sectionalList.GetNonFilteredValues(predicate))
            {
                Console.Write(i + ":");
            }
            Console.WriteLine("");

        }


        public class SectionalList<T> : IEnumerable<T>
        {
	        private List<T> _items = new List<T>();

	        public void Add(T item)
	        {
		        _items.Add(item);
	        }

            public IEnumerator<T> GetEnumerator()
            {
                return _items.GetEnumerator();
            }
            
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

	        public IEnumerable<T> GetFirstHalf()
	        {
		        foreach(T item in _items.Take(_items.Count / 2))
		        {
			        yield return item;
		        }
	        }
	        public IEnumerable<T> GetSecondHalf()
	        {
                foreach (T item in _items.Skip(_items.Count / 2))
                {
                    yield return item;
                }
            }

            public IEnumerable<T> GetFilteredValues(Func<T, bool> predicate)
            {
                foreach (T item in _items.TakeWhile(predicate))
                {
                    yield return item;
                }
            }

            public IEnumerable<T> GetNonFilteredValues(Func<T, bool> predicate)
            {
                foreach (T item in _items.SkipWhile(predicate))
                {
                    yield return item;
                }
            }
        }
		#endregion

		#region "6.5 Forcing an Iterator to Stop Iterating"
        public static void TestYieldBreak()
        {
            //Create UpperLimitList and fill it with data
            UpperLimitList<string> gatePasses = new UpperLimitList<string>() {
                "A","B","C","D","E","F","G"};

            Console.WriteLine("Gates allowed before limit set");
            foreach (string gatePass in gatePasses)
            {
                Console.Write(gatePass + ":");
            }
            Console.WriteLine("");

            // only give out 5 gate passes
            gatePasses.UpperLimit = 5;

            Console.WriteLine("Gates allowed after limit set");
            foreach (string gatePass in gatePasses)
	        {
		        Console.Write(gatePass + ":");
	        }
            Console.WriteLine("");
        }


        public class UpperLimitList<T> : IEnumerable<T>
        {
	        private List<T> _items = new List<T>();
	        private bool noMoreItemsCanBeAdded;
	        private int upperLimit = -1;

	        public int UpperLimit
	        {
		        get {return (upperLimit);}
		        set {upperLimit = value;}
	        }

	        public void Add(T name)
	        {
		        _items.Add(name);
	        }

	        public IEnumerator<T> GetEnumerator()
	        {
		        for (int index = 0; index < _items.Count; index++)
		        {
			        if (noMoreItemsCanBeAdded)
			        {
				        yield break;
			        }
			        else
			        {
				        // Perform some action that may or may not set noMoreItemsCanBeAdded to true
				        if (upperLimit >= 0 && index >= upperLimit-1)
				        {
					        noMoreItemsCanBeAdded = true;
				        }

				        yield return (_items[index]);
			        }
		        }
	        }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
		#endregion

		#region "6.6 Dealing with Finally Blocks and Iterators"
        public static void TestFinallyAndIterators()
        {
	        //Create a StringSet object and fill it with data
            StringSet strSet = 
                new StringSet() 
                    {"item1", 
                     "item2", 
                     "item3", 
                     "item4", 
                     "item5"};

            //// Use the GetEnumerator iterator.
            //foreach (string s in strSet)
            //{
            //    Console.WriteLine(s);
            //}


	        // Display all data in StringSet object
	        try
	        {
		        foreach (string s in strSet)
		        {
			        try
			        {
                        Console.WriteLine(s);
                        // Force an exception here
                        //throw new Exception();
			        }
			        catch (Exception)
			        {
				        Console.WriteLine("In foreach catch block");
			        }
			        finally
			        {
				        // Executed on each iteration
				        Console.WriteLine("In foreach finally block");
			        }
		        }
	        }
	        catch (Exception)
	        {
		        Console.WriteLine("In outer catch block");
	        }
	        finally
	        {
		        // Executed on each iteration
		        Console.WriteLine("In outer finally block");
	        }

			/*
			This code is executed in this fashion when an exception occurs in the iterator:
			 - In iterator finally block
			 - In outer catch block
			 - In outer finally block

			This code is executed in this fashion when NO exception occurs in the iterator:
			 - item1
			 - In foreach finally block
			 - item2
			 - In foreach finally block
			 - item3
			 - In foreach finally block
			 - item4
			 - In foreach finally block
			 - item5
			 - In foreach finally block
			 - item6
			 - In foreach finally block
			 - In iterator finally block
			 - In outer finally block

			This code is executed in this fashion when an exception occurs in the foreach loop:
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In iterator finally block
			 - In outer finally block
			*/
		}


        public class StringSet : IEnumerable<string>
        {
	        private List<string> _items = new List<string>();

	        public void Add(string value)
	        {
		        _items.Add(value);
	        }

            public IEnumerator<string> GetEnumerator()
            {
                try
                {
                    for (int index = 0; index < _items.Count; index++)
                    {
                        // Force an exception here
                        if(index == 1) throw new Exception();
                        yield return (_items[index]);
                    }
                }
                // Cannot use catch blocks in an iterator
                finally
                {
                    // Only executed at end of foreach loop (including on yield break)
                    Console.WriteLine("In iterator finally block");
                }
            }

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }
		#endregion

        #region "6.7 Implementing Nested foreach Functionality In A Class"
        public static void CreateNestedObjects()
        {
            Group<Group<Item>> hierarchy = 
                new Group<Group<Item>>("root") {
                    new Group<Item>("subgroup1"){
                        new Item("item1",100),
                        new Item("item2",200)},
                    new Group<Item>("subgroup2"){
                        new Item("item3",300),
                        new Item("item4",400)}};

            IEnumerator enumerator = ((IEnumerable)hierarchy).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(((Group<Item>)enumerator.Current).Name);
                foreach (Item i in ((Group<Item>)enumerator.Current))
                {
                    Console.WriteLine(i.Name);
                }
            }

            // Read back the data
            DisplayNestedObjects(hierarchy);
        }

        //topLevelGroup.Count: 2
        //topLevelGroupName:  root
        //        subGroup.SubGroupName:  subgroup1
        //        subGroup.Count: 2
        //                item.Name:     item1
        //                item.Location: 100
        //                item.Name:     item2
        //                item.Location: 200
        //        subGroup.SubGroupName:  subgroup2
        //        subGroup.Count: 2
        //                item.Name:     item3
        //                item.Location: 300
        //                item.Name:     item4
        //                item.Location: 400

        private static void DisplayNestedObjects(Group<Group<Item>> topLevelGroup)
        {
            Console.WriteLine("topLevelGroup.Count: " + topLevelGroup.Count);
            Console.WriteLine("topLevelGroupName:  " + topLevelGroup.Name);

            // Outer foreach to iterate over all objects in the 
            // topLevelGroup object
            foreach (Group<Item> subGroup in topLevelGroup)
            {
                Console.WriteLine("\tsubGroup.SubGroupName:  " + subGroup.Name);
                Console.WriteLine("\tsubGroup.Count: " + subGroup.Count);

                // Inner foreach to iterate over all Item objects in the 
                // current SubGroup object
                foreach (Item item in subGroup)
                {
                    Console.WriteLine("\t\titem.Name:     " + item.Name);
                    Console.WriteLine("\t\titem.Location: " + item.Location);
                }
            }
        }


        public class Group<T> : IEnumerable<T>
        {
            public Group(string name) 
            {
                this.Name = name;
            }

            private List<T> _groupList = new List<T>();

            public string Name { get; set; }

            public int Count
            {
                get { return _groupList.Count; }
            }

            public void Add(T group)
            {
                _groupList.Add(group);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
                //return new GroupEnumerator<T>(_groupList.ToArray());
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _groupList.GetEnumerator();
            }
        }


        public class Item
        {
            public Item(string name, int location)
            {
                this.Name = name;
                this.Location = location;
            }
            public string Name { get; set; }
            public int Location { get; set; }
        }

        public class GroupEnumerator<T> : IEnumerator
        {
            public T[] _items;

            int position = -1;

            public GroupEnumerator(T[] list)
            {
                _items = list;
            }

            public bool MoveNext()
            {
                position++;
                return (position < _items.Length);
            }

            public void Reset()
            {
                position = -1;
            }

            public object Current
            {
                get
                {
                    try
                    {
                        return _items[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }


        #endregion

        #region "6.8 Organizing your interface implementations"
		// see the "PartialClassInterface" project
		#endregion

		#region "6.9 Generate designer code that is no longer in your main code paths (Partial classes / UserControl?)"
		// see the "PartialClassAddin" project
		#endregion

        #region "6.10 Adding hooks to generated entities"
        public static void TestPartialMethods()
        {
            Console.WriteLine("Start entity work");
            GeneratedEntity entity = new GeneratedEntity("FirstEntity");
            entity.FirstName = "Bob";
            entity.State = "NH";
            GeneratedEntity secondEntity = new GeneratedEntity("SecondEntity");
            entity.FirstName = "Jay";
            secondEntity.FirstName = "Steve";
            secondEntity.State = "MA";
            entity.FirstName = "Barry";
            secondEntity.State = "WA";
            secondEntity.FirstName = "Matt";
            Console.WriteLine("End entity work");
        }

        //OUTPUT
        //Start entity work
        //Changed property (FirstName) for entity FirstEntity from  to Bob
        //Changed property (State) for entity FirstEntity from  to NH
        //Changed property (FirstName) for entity FirstEntity from Bob to Jay
        //Changed property (FirstName) for entity SecondEntity from  to Steve
        //Changed property (State) for entity SecondEntity from  to MA
        //Changed property (FirstName) for entity FirstEntity from Jay to Barry
        //Changed property (State) for entity SecondEntity from MA to WA
        //Changed property (FirstName) for entity SecondEntity from Steve to Matt
        //End entity work

        public partial class GeneratedEntity
        {
            public GeneratedEntity(string entityName)
            {
                this.EntityName = entityName;
            }

            partial void ChangingProperty(string name, string originalValue, string newValue);

            public string EntityName { get; private set; }

            private string _FirstName;
            public string FirstName
            {
                get { return _FirstName; }
                set
                {
                    ChangingProperty("FirstName",_FirstName,value);
                    _FirstName = value;
                }
            }

            private string _State;
            public string State
            {
                get { return _State; }
                set
                {
                    ChangingProperty("State",_State,value);
                    _State = value;
                }
            }
        }

        public partial class GeneratedEntity
        {
            partial void ChangingProperty(string name, string originalValue, string newValue)
            {
                Console.WriteLine("Changed property ({0}) for entity {1} from {2} to {3}",
                    name, this.EntityName, originalValue, newValue);
            }
        }
        #endregion // "6.10  Adding hooks to generated entities"
    }
}
