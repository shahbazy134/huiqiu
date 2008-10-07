using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

//Done deliberately in 11.8 to show overload of == in Set
#pragma warning disable 1718 //Comparison made to same variable; did you mean to compare something else? 

namespace CSharpRecipes
{
	public class DataStructsAndAlgorithms
	{
		#region 11.1 Creating a Hash Code for a Data Type	
		public static void CreateHashCodeDataType()
		{
			SimpleClass simpleClass = new SimpleClass("foo");

			Hashtable hashTable = new Hashtable();
			hashTable.Add(simpleClass, 100);

			Dictionary<SimpleClass, int> dict = new Dictionary<SimpleClass, int>();
			dict.Add(simpleClass, 100);
		}

        public class SimpleClass
        {
            private string text = "";

            public SimpleClass(string inputText)
            {
                text = inputText;
            }

            public override int GetHashCode()
            {
                return (ShiftAndAddHash(text));
            }

            public int ShiftAndAddHash(string strValue)
            {
                int hashCode = 0;

                foreach (char c in strValue)
                {
                    hashCode = (hashCode << 5) + (int)c + (hashCode >> 2);
                }

                return (hashCode);
            }
        }

		public int SimpleHash(params int[] values)
		{
			int hashCode = 0;
			if (values != null)
			{
				foreach (int val in values)
				{
					hashCode ^= val;
				}
			}

			return (hashCode);
		}

		public int FoldingHash(params long[] values)
		{
			int hashCode = 0;
			if (values != null)
			{
				int tempLowerVal = 0;
				int tempUpperVal = 0;
				foreach (long val in values)
				{
					tempLowerVal = (int)(val & 0x000000007FFFFFFF);
					tempUpperVal = (int)((val >> 32) & 0xFFFFFFFF);
					hashCode^= tempLowerVal ^ tempUpperVal;
				}
			}

			return (hashCode);
		}

		public int ContainedObjHash(params object[] values)
		{
			int hashCode = 0;
			if (values != null)
			{
				foreach (int val in values)
				{
					hashCode ^= val.GetHashCode();
				}
			}

			return (hashCode);
		}

		public int CryptoHash(string strValue)
		{
			int hashCode = 0;
			if (strValue != null)
			{
				byte[] encodedUnHashedString = 
					Encoding.Unicode.GetBytes(strValue);

                byte[] key = new byte[16];
                RandomNumberGenerator.Create().GetBytes(key);

                MACTripleDES hashingObj = new MACTripleDES(key);
				byte[] code = 
					hashingObj.ComputeHash(encodedUnHashedString);

				// use the BitConverter class to take the 
				// first 4 bytes and use them as an int for
				// the hash code
				hashCode = BitConverter.ToInt32(code,0);    
			}

			return (hashCode);
		}

		public int CryptoHash(long intValue)
		{
			int hashCode = 0;
			byte[] encodedUnHashedString = 
				Encoding.Unicode.GetBytes(intValue.ToString());

			SHA256Managed hashingObj = new SHA256Managed();
			byte[] code = hashingObj.ComputeHash(encodedUnHashedString);

			// use the BitConverter class to take the 
			// first 4 bytes and use them as an int for
			// the hash code
			hashCode = BitConverter.ToInt32(code,0);    

			return (hashCode);
		}

		public int ShiftAndAddHash (string strValue)
		{
			int hashCode = 0;
			
			foreach (char c in strValue)
			{
			    hashCode = (hashCode << 5) + (int)c + (hashCode >> 2);
			}
			
			return (hashCode);
		}

		public int CalcHash(short someShort, int someInt, long someLong,
			float someFloat, object someObject)
		{
			int hashCode = 7;
			hashCode = hashCode * 31 + (int)someShort;
			hashCode = hashCode * 31 + someInt;
			hashCode = hashCode * 31 + 
				(int)(someLong ^ (someLong >> 32));
			long someFloatToLong = (long)someFloat;
			hashCode = hashCode * 31 + 
				(int)(someFloatToLong ^ (someFloatToLong >> 32));

			if (someObject != null)
			{
				hashCode = hashCode * 31 + 
					someObject.GetHashCode();
			}

			return (hashCode);
		}

		public int ConcatStringGetHashCode(int[] someIntArray)
		{
			int hashCode = 0;
			StringBuilder hashString = new StringBuilder();

			if (someIntArray != null)
			{
				foreach (int i in someIntArray)
				{
					hashString.Append(i.ToString() + "^");
				}
			}
			hashCode = hashString.GetHashCode();

			return (hashCode);
		}



		#endregion

		#region 11.2 Creating a Priority Queue	

		public static void CreatePriorityQueue()
		{
			// Create List of messages
			List<string> msgs = new List<string>();
			msgs.Add("foo");
			msgs.Add("This is a longer message.");
			msgs.Add("bar");
			msgs.Add(@"Message with odd characters
                   !@#$%^&*()_+=-0987654321~|}{[]\\;:?/>.<,");
			msgs.Add(@"<                                                                   
                      >");
			msgs.Add("<text>one</text><text>two</text><text>three</text>" + 
				"<text>four</text>");
			msgs.Add("");
			msgs.Add("1234567890");

			// Create a Priority Queue with the appropriate comparer
			//CompareObjs<string> comparer = new CompareObjs<string>();
			CompareStrLen<string> comparer = new CompareStrLen<string>();
			PriorityQueue<string> pqueue = new PriorityQueue<string>(comparer);

			// Add all messages from the List to the priority queue
			foreach (string msg in msgs)
			{
				pqueue.Enqueue(msg);
			}

			// Display messages in the queue in order of priority
			foreach (string msg in pqueue)
			{
				Console.WriteLine("Msg: " + msg);
			}

			Console.WriteLine("pqueue.Count == " + pqueue.Count);
			//pqueue.Clear();
			//Console.WriteLine("pqueue.Count == " + pqueue.Count);

			Console.WriteLine("pqueue.IndexOf('bar') == " + pqueue.IndexOf("bar"));
			Console.WriteLine("pqueue.IndexOf('_bar_') == " + pqueue.IndexOf("_bar_"));

			Console.WriteLine("pqueue.Contains('bar') == " + pqueue.Contains("bar"));
			Console.WriteLine("pqueue.Contains('_bar_') == " + pqueue.Contains("_bar_"));

			Console.WriteLine("pqueue.BinarySearch('bar') == " + pqueue.BinarySearch("bar"));
			Console.WriteLine("pqueue.BinarySearch('_bar_') == " + pqueue.BinarySearch("_bar_"));
			
			// Dequeue messages starting with the smallest
			int currCount = pqueue.Count;
			for (int index = 0; index < currCount; index++)
			{
				Console.WriteLine("pqueue.DequeueLargest(): " + 
				                  pqueue.DequeueLargest().ToString());
			}
		
		}

		public class PriorityQueue<T> : IEnumerable
		{
			public PriorityQueue() {}
			public PriorityQueue(IComparer<T> icomparer)
			{
				specialComparer = icomparer;
			}


			private List<T> internalQueue = new List<T>();
			protected IComparer<T> specialComparer = null;


            protected List<T> InternalQueue
            {
                get {return internalQueue;}
            }
            
			public int Count
			{
				get {return (internalQueue.Count);}
			}

			public void Clear()
			{
				internalQueue.Clear();
			}

			public object Clone()
			{
				// Make a new PQ and give it the same comparer
				PriorityQueue<T> newPQ = new PriorityQueue<T>(specialComparer);
				newPQ.CopyTo(internalQueue.ToArray(),0);
				return newPQ;    
			}

			public int IndexOf(T item)
			{
				return (internalQueue.IndexOf(item));
			}

			public bool Contains(T item)
			{
				return (internalQueue.Contains(item));
			}

			public int BinarySearch(T item)
			{
				return (internalQueue.BinarySearch(item, specialComparer));
			}

			public bool Contains(T item, IComparer<T> comparer)
			{
				if (internalQueue.BinarySearch(item, comparer) >= 0)
				{
					return (true);
				}
				else
				{
					return (false);
				}
			}

			public void CopyTo(T[] array, int index)
			{
				internalQueue.CopyTo(array, index);
			}

			public T[] ToArray()
			{
				return (internalQueue.ToArray());
			}

			public void TrimExcess()
			{
				internalQueue.TrimExcess();
			}

			public void Enqueue(T item)
			{
				internalQueue.Add(item);
				internalQueue.Sort(specialComparer);
			}

			public T DequeueLargest()
			{
				T item = internalQueue[internalQueue.Count - 1];
				internalQueue.RemoveAt(internalQueue.Count - 1);

				return (item);
			}

			public T PeekLargest()
			{
				return (internalQueue[internalQueue.Count - 1]);
			}

			public IEnumerator GetEnumerator()
			{
				return (internalQueue.GetEnumerator());
			}
		}

		public class CompareStrLen<T> : IComparer<T>
			where T: IComparable<T>
		{
			public int Compare(T obj1, T obj2)
			{
				int result = 0;
				if ((obj1 is string) && (obj2 is string))
				{
					result = CompareStrings(obj1 as string, obj2 as string);
				}
				else
				{
					// Default to the objects comparison algorithm
					result = Compare(obj1, obj2);
				}
				return (result);
			}

			private int CompareStrings(string str1, string str2)
			{
				if (str1.Length == str2.Length)
				{
					return (0);
				}
				else if (str1.Length > str2.Length)
				{
					return (1);
				}
				else
				{
					return (-1);
				}
			}

			public bool Equals(T item1, T item2)
			{
				return (item1.Equals(item2));
			}

			public int GetHashCode(T obj)
			{
				return (obj.GetHashCode());
			}
		}

		public class CompareObjs<T> : IComparer<T>
			where T: IComparable<T>
		{
			public int Compare(T obj1, T obj2)
			{
				int result = 0;

				if ((!obj1.Equals(null)) && (!obj2.Equals(null)))
				{
					result = obj1.CompareTo(obj2);
				}
				
				return (result);
			}

			public bool Equals(T item1, T item2)
			{
				return (item1.Equals(item2));
			}

			public int GetHashCode(T item)
			{
				return (item.GetHashCode());
			}
		}
		#endregion

		#region	11.3 Creating a One-to-Many Map (MultiMap)		
		public static void TestMultiMap()
		{
			string s = "foo";

			// Create and populate a MultiMap object
			MultiMap<int, string> myMap = new MultiMap<int, string>();
			myMap.Add(0, "zero");
			myMap.Add(1, "one");
			myMap.Add(2, "two");
			myMap.Add(3, "three");
			myMap.Add(3, "duplicate three");
			myMap.Add(3, "duplicate three");
			myMap.Add(4, null);
			myMap.Add(5, s);
			myMap.Add(6, s);

			// Display contents
			foreach (KeyValuePair<int, List<string>> entry in myMap)
			{
				Console.Write("Key: " + entry.Key.ToString() + "\tValue: ");
				foreach (string str in myMap[entry.Key])
				{
					Console.Write(str + " : ");
				}
				Console.WriteLine();
			}

			// Obtain values through the indexer
			Console.WriteLine();
			Console.WriteLine("((ArrayList) myMap[3])[0]: " + myMap[3][0]);
			Console.WriteLine("((ArrayList) myMap[3])[1]: " + myMap[3][1]);

			// Add items to MultiMap using a List
			List<string> testArray = new List<string>();
			testArray.Add("BAR");
			testArray.Add("BAZ");
			myMap[10] = testArray;
			myMap[10] = testArray;

			// Remove items from MultiMap
			myMap.Remove(0);
			myMap.Remove(1);

			// Display MultiMap
			Console.WriteLine();
			Console.WriteLine("myMap.Count: " + myMap.Count);
			foreach (KeyValuePair<int, List<string>> entry in myMap)
			{
				Console.Write("entry.Key: " + entry.Key.ToString() + 
					"\tentry.Value(s): ");
				foreach (string str in myMap[entry.Key])
				{
					if (str == null)
					{
						Console.Write("null : ");
					}
					else
					{
						Console.Write(str + " : ");
					}
				}
				Console.WriteLine();
			}

			// Determine if the map contains the key or the value
			Console.WriteLine();
			Console.WriteLine("myMap.ContainsKey(2): " + myMap.ContainsKey(2));
			Console.WriteLine("myMap.ContainsValue(two): " + 
				myMap.ContainsValue("two"));

			Console.WriteLine("Contains Key 2: " + myMap.ContainsKey(2));
			Console.WriteLine("Contains Key 12: " + myMap.ContainsKey(12));

			Console.WriteLine("Contains Value two: " + myMap.ContainsValue("two"));
			Console.WriteLine("Contains Value BAR: " + myMap.ContainsValue("BAR"));

			// Clear all items from MultiMap
			myMap.Clear();
		}
/* ORIGINAL DATA
Key: 4	Value: 
Key: 5	Value: foo : 
Key: 6	Value: foo : 
Key: 0	Value: zero : 
Key: 1	Value: one : 
Key: 2	Value: two : 
Key: 3	Value: three : duplicate three : duplicate three : 

((ArrayList) myMap[3])[0]: three
((ArrayList) myMap[3])[1]: duplicate three

myMap.Count: 6
entry.Key: 2	entry.Value(s): two : 
entry.Key: 3	entry.Value(s): three : duplicate three : duplicate three : 
entry.Key: 4	entry.Value(s): 
entry.Key: 5	entry.Value(s): foo : 
entry.Key: 6	entry.Value(s): foo : 
entry.Key: 10	entry.Value(s): BAR : BAZ : 

myMap.ContainsKey(2): True
myMap.ContainsValue(two): True
*/

		public class MultiMap<T,U>		// T is the MultiMap key and U is the MultiMap value
		{
			private Dictionary<T, List<U>> map = new Dictionary<T, List<U>>();


			public List<U> this[T key] 
			{
				get {return (map[key]);}
				set {map[key] = value;}
			}

			public void Add(T key, U item)
			{
				AddSingleMap(key, item);
			}

			public void Clear()
			{
				map.Clear();
			}

			public int Count
			{
				get {return (map.Count);}
			}

			public bool ContainsKey (T key)
			{
				return (map.ContainsKey(key));
			}

			public bool ContainsValue(U item)
			{
				if (item == null)
				{
					foreach (KeyValuePair<T, List<U>> kvp in map)
					{
						if (((List<U>)kvp.Value).Count == 0)
						{
							return (true);
						}
					}

					return (false);
				}
				else
				{
					foreach (KeyValuePair<T, List<U>> kvp in map)
					{
						if (((List<U>)kvp.Value).Contains(item))
						{
							return (true);
						}
					}

					return (false);
				}
			}
	
			public Dictionary<T, List<U>>.Enumerator GetEnumerator()
			{
				return (map.GetEnumerator());
			}

			public void Remove(T key)
			{
				RemoveSingleMap(key);
			}

			protected void AddSingleMap(T key, U item)
			{
				// Search for key in map Hashtable
				if (map.ContainsKey(key))
				{
					if (item == null)
					{
						throw (new ArgumentNullException("item", 
							"Cannot map a null to this key"));
					}
					else
					{
						// Add value to List in map
						List<U> values = (List<U>)map[key];

						// Add this value to this existing key
						values.Add(item);
					}
				}
				else
				{
					if (item == null)
					{
						// Create new key and mapping to an empty List
						map.Add(key, new List<U>());
					}
					else
					{
						List<U> values = new List<U>();
						values.Add(item);

						// Create new key and mapping to its value
						map.Add(key, values);
					}
				}
			}

			protected void RemoveSingleMap(T key)
			{
				if (this.ContainsKey(key))
				{
					// Remove the key from KeysTable
					map.Remove(key);
				}
				else
				{
					throw (new ArgumentOutOfRangeException("key", key.ToString(), 
						"This key does not exists in the map."));
				}
			}
		}


		#endregion

		#region 11.4 Creating a Binary Search Tree	
		public static void TestBinaryTree()
		{
			BinaryTree<string> tree = new BinaryTree<string>("d");
			tree.AddNode("a");
			tree.AddNode("b");
			tree.AddNode("f");
			tree.AddNode("e");
			tree.AddNode("c");
			tree.AddNode("g");
    
			tree.Print();
			tree.Print();

			Console.WriteLine("tree.TreeSize: " + tree.TreeSize);            
			Console.WriteLine("tree.GetRoot().DepthFirstSearch(a).NumOfChildren: " + 
				tree.GetRoot().DepthFirstSearch("b").Children);            
			Console.WriteLine("tree.GetRoot().DepthFirstSearch(a).NumOfChildren: " +
                tree.GetRoot().DepthFirstSearch("a").Children);            
			Console.WriteLine("tree.GetRoot().DepthFirstSearch(g).NumOfChildren: " +
                tree.GetRoot().DepthFirstSearch("g").Children);            

			Console.WriteLine("tree.SearchDepthFirst(a): " + 
				tree.SearchDepthFirst("a").Value.ToString());
			Console.WriteLine("tree.SearchDepthFirst(b): " + 
				tree.SearchDepthFirst("b").Value.ToString());
			Console.WriteLine("tree.SearchDepthFirst(c): " + 
				tree.SearchDepthFirst("c").Value.ToString());
			Console.WriteLine("tree.SearchDepthFirst(d): " + 
				tree.SearchDepthFirst("d").Value.ToString());
			Console.WriteLine("tree.SearchDepthFirst(e): " + 
				tree.SearchDepthFirst("e").Value.ToString());
			Console.WriteLine("tree.SearchDepthFirst(f): " + 
				tree.SearchDepthFirst("f").Value.ToString());

			tree.GetRoot().RemoveLeftNode();
			tree.Print();

			tree.GetRoot().RemoveRightNode();
			tree.Print();
		}
/*  ORIGINAL DATA
a
	Contains Left:  NULL
	Contains Right: b
b
	Contains Left:  NULL
	Contains Right: c
c
	Contains Left:  NULL
	Contains Right: NULL
d
	Contains Left: a
	Contains Right: f
e
	Contains Left:  NULL
	Contains Right: NULL
f
	Contains Left: e
	Contains Right: g
g
	Contains Left:  NULL
	Contains Right: NULL
a
	Contains Left:  NULL
	Contains Right: b
b
	Contains Left:  NULL
	Contains Right: c
c
	Contains Left:  NULL
	Contains Right: NULL
d
	Contains Left: a
	Contains Right: f
e
	Contains Left:  NULL
	Contains Right: NULL
f
	Contains Left: e
	Contains Right: g
g
	Contains Left:  NULL
	Contains Right: NULL
tree.TreeSize: 7
tree.GetRoot().DepthFirstSearch(a).NumOfChildren: 1
tree.GetRoot().DepthFirstSearch(a).NumOfChildren: 2
tree.GetRoot().DepthFirstSearch(g).NumOfChildren: 0
tree.SearchDepthFirst(a): a
tree.SearchDepthFirst(b): b
tree.SearchDepthFirst(c): c
tree.SearchDepthFirst(d): d
tree.SearchDepthFirst(e): e
tree.SearchDepthFirst(f): f
d
	Contains Left:  NULL
	Contains Right: f
e
	Contains Left:  NULL
	Contains Right: NULL
f
	Contains Left: e
	Contains Right: g
g
	Contains Left:  NULL
	Contains Right: NULL
d
	Contains Left:  NULL
	Contains Right: NULL
*/

		public static void TestManagedTreeWithNoBinaryTreeClass()
		{
			// Create the root node
			BinaryTreeNode<string> topLevel = new BinaryTreeNode<string>("d");

			// Create all nodes that will be added to the tree
			BinaryTreeNode<string> one = new BinaryTreeNode<string>("b");
			BinaryTreeNode<string> two = new BinaryTreeNode<string>("c");
			BinaryTreeNode<string> three = new BinaryTreeNode<string>("a");
			BinaryTreeNode<string> four = new BinaryTreeNode<string>("e");
			BinaryTreeNode<string> five = new BinaryTreeNode<string>("f");
			BinaryTreeNode<string> six = new BinaryTreeNode<string>("g");

			// Add nodes to tree through the root
			topLevel.AddNode(three);
			topLevel.AddNode(one);
			topLevel.AddNode(five);
			topLevel.AddNode(four);
			topLevel.AddNode(two);
			topLevel.AddNode(six);

			// Print the tree starting at the root node
			topLevel.PrintDepthFirst();

			// Print the tree starting at node ‘Three’
			three.PrintDepthFirst();

			// Display the number of child nodes of various nodes in the tree
            Console.WriteLine("topLevel.NumOfChildren: " + topLevel.Children);
            Console.WriteLine("one.NumOfChildren: " + one.Children);
            Console.WriteLine("three.NumOfChildren: " + three.Children);
            Console.WriteLine("six.NumOfChildren: " + six.Children);

			// Search the tree using the depth first searching method
			Console.WriteLine("topLevel.DepthFirstSearch(a): " +
					topLevel.DepthFirstSearch("a").Value.ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(b): " +
					topLevel.DepthFirstSearch("b").Value.ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(c): " +
					topLevel.DepthFirstSearch("c").Value.ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(d): " +
					topLevel.DepthFirstSearch("d").Value.ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(e): " +
					topLevel.DepthFirstSearch("e").Value.ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(f): " +
					topLevel.DepthFirstSearch("f").Value.ToString());

			// Remove the left child node from the root node and display the entire tree
			topLevel.RemoveLeftNode();
			topLevel.PrintDepthFirst();

			// Remove all nodes from the tree except for the root and display the tree
			topLevel.RemoveRightNode();
			topLevel.PrintDepthFirst();
		}
/*  ORIGINAL DATA
a
	Contains Left:  NULL
	Contains Right: b
b
	Contains Left:  NULL
	Contains Right: c
c
	Contains Left:  NULL
	Contains Right: NULL
d
	Contains Left: a
	Contains Right: f
e
	Contains Left:  NULL
	Contains Right: NULL
f
	Contains Left: e
	Contains Right: g
g
	Contains Left:  NULL
	Contains Right: NULL
a
	Contains Left:  NULL
	Contains Right: b
b
	Contains Left:  NULL
	Contains Right: c
c
	Contains Left:  NULL
	Contains Right: NULL
topLevel.NumOfChildren: 6
one.NumOfChildren: 1
three.NumOfChildren: 2
six.NumOfChildren: 0
topLevel.DepthFirstSearch(a): a
topLevel.DepthFirstSearch(b): b
topLevel.DepthFirstSearch(c): c
topLevel.DepthFirstSearch(d): d
topLevel.DepthFirstSearch(e): e
topLevel.DepthFirstSearch(f): f
d
	Contains Left:  NULL
	Contains Right: f
e
	Contains Left:  NULL
	Contains Right: NULL
f
	Contains Left: e
	Contains Right: g
g
	Contains Left:  NULL
	Contains Right: NULL
d
	Contains Left:  NULL
	Contains Right: NULL
	*/


		public class BinaryTree<T> 
			where T: IComparable<T>
		{
			public BinaryTree() {}

			public BinaryTree(T value, int index) 
			{
				BinaryTreeNode<T> node = new BinaryTreeNode<T>(value, index);
				root = node;
				counter = 1;
			}

			// Use this .ctor when you need to flatten this tree
			public BinaryTree(T value) 
			{
				BinaryTreeNode<T> node = new BinaryTreeNode<T>(value);
				root = node;
				counter = 1;
			}


			private int counter = 0;				// Number of nodes in tree
            private BinaryTreeNode<T> root = null;  // Pointer to root node in this tree


			public void AddNode(T value, int index)
			{
				BinaryTreeNode<T> node = new BinaryTreeNode<T>(value, index);
				++counter;

				if (root == null)
				{
					root = node;
				}
				else
				{
					root.AddNode(node);
				}
			}

			// Use this method to add a node 
			//    when you need to flatten this tree
			public int AddNode(T value)
			{
				BinaryTreeNode<T> node = new BinaryTreeNode<T>(value);
				++counter;

				if (root == null)
				{
					root = node;
				}
				else
				{
					root.AddNode(node);
				}

				return (counter - 1);
			}

			public BinaryTreeNode<T> SearchDepthFirst(T value)
			{
				return (root.DepthFirstSearch(value));
			}

			public void Print()
			{
				root.PrintDepthFirst();
			}

			public BinaryTreeNode<T> GetRoot()
			{
				return (root);
			}

			public int TreeSize
			{
				get {return (counter);}
			}        
		}

		public class BinaryTreeNode<T>
			where T: IComparable<T>
		{
			public BinaryTreeNode() {}

			public BinaryTreeNode(T value)
			{
				nodeValue = value;
			}

			// These 2 ctors Added to allow tree to be flattened
			public BinaryTreeNode(int index) 
			{
				nodeIndex = index;
			}

			public BinaryTreeNode(T value, int index)
			{
				nodeValue = value;
				nodeIndex = index;
			}


            private int nodeIndex = 0;         // Added to allow tree to be flattened
            private T nodeValue = default(T);
            private BinaryTreeNode<T> leftNode = null;     //  leftNode.Value < Value
            private BinaryTreeNode<T> rightNode = null;    //  rightNode.Value >= Value


            public int Children
            {
                get
                {
                    int currCount = 0;
                    if (leftNode != null)
                    {
                        ++currCount;
                        currCount += leftNode.Children;
                    }

                    if (rightNode != null)
                    {
                        ++currCount;
                        currCount += rightNode.Children;
                    }

                    return (currCount);
                }
            }


			public int Index
			{
				get {return (nodeIndex);}
			}

			public BinaryTreeNode<T> Left
			{
				get {return (leftNode);}
			}

			public BinaryTreeNode<T> Right
			{
				get {return (rightNode);}
			}

			public T Value 
			{
				get {return (nodeValue);}
			}

			public void AddNode(BinaryTreeNode<T> node)
			{
				if (node.nodeValue.CompareTo(nodeValue) < 0)
				{
					if (leftNode == null)
					{
						leftNode = node;
					}
					else
					{
						leftNode.AddNode(node);
					}
				}
				else if (node.nodeValue.CompareTo(nodeValue) >= 0)
				{
					if (rightNode == null)
					{
						rightNode = node;
					}
					else
					{
						rightNode.AddNode(node);
					}
				}
			}

			public bool AddUniqueNode(BinaryTreeNode<T> node)
			{
				bool isUnique = true;

				if (node.nodeValue.CompareTo(nodeValue) < 0)
				{
					if (leftNode == null)
					{
						leftNode = node;
					}
					else
					{
						leftNode.AddNode(node);
					}
				}
				else if (node.nodeValue.CompareTo(nodeValue) > 0)
				{
					if (rightNode == null)
					{
						rightNode = node;
					}
					else
					{
						rightNode.AddNode(node);
					}
				}
				else   //node.nodeValue.CompareTo(nodeValue) = 0
				{
					isUnique = false;
					// Could throw exception here as well...
				}

				return (isUnique);
			}

			public BinaryTreeNode<T> DepthFirstSearch(T targetObj)
			{
				// NOTE: foo.CompareTo(bar) == -1   -->   (foo < bar)
				BinaryTreeNode<T> retObj = null;
				int comparisonResult = targetObj.CompareTo(nodeValue);

				if (comparisonResult  == 0)
				{
					retObj = this;
				}
				else if (comparisonResult > 0)
				{
					if (rightNode != null)
					{
						retObj = rightNode.DepthFirstSearch(targetObj);
					}
				}
				else if (comparisonResult < 0)
				{
					if (leftNode != null)
					{
						retObj = leftNode.DepthFirstSearch(targetObj);
					}
				}

				return (retObj);
			}

			public void PrintDepthFirst()
			{
				if (leftNode != null)
				{
					leftNode.PrintDepthFirst();
				}

				Console.WriteLine(this.nodeValue.ToString());

				try
				{
					Console.WriteLine("\tContains Left: " + 
						leftNode.nodeValue.ToString());
				}
				catch
				{
					Console.WriteLine("\tContains Left:  NULL");
				}
				try
				{
					Console.WriteLine("\tContains Right: " + 
						rightNode.nodeValue.ToString());
				}
				catch
				{
					Console.WriteLine("\tContains Right: NULL");
				}

				if (rightNode != null)
				{
					rightNode.PrintDepthFirst();
				}
			}


            public List<T> CopyToList()
            {
                List<T> tempList = new List<T>();
                if (leftNode != null)
                {
                    tempList.AddRange(leftNode.CopyToList());
                    tempList.Add(leftNode.nodeValue);
                }
                if (rightNode != null)
                {
                    tempList.Add(rightNode.nodeValue);
                    tempList.AddRange(rightNode.CopyToList());
                }
                return (tempList);
            }

			public void RemoveLeftNode()
			{
				leftNode = null;
			}

			public void RemoveRightNode()
			{
				rightNode = null;
			}
		}
		#endregion

		#region 11.5 Creating an N-Ary Tree
		public static void TestNTree()
		{
			NTree<string> topLevel = new NTree<string>(3);
			NTreeNodeFactory<string> nodeFactory = new NTreeNodeFactory<string>(topLevel);

			NTreeNodeFactory<string>.NTreeNode<string> one = nodeFactory.CreateNode("One");
			NTreeNodeFactory<string>.NTreeNode<string> two = nodeFactory.CreateNode("Two");
			NTreeNodeFactory<string>.NTreeNode<string> three = nodeFactory.CreateNode("Three");
			NTreeNodeFactory<string>.NTreeNode<string> four = nodeFactory.CreateNode("Four");
			NTreeNodeFactory<string>.NTreeNode<string> five = nodeFactory.CreateNode("Five");
			NTreeNodeFactory<string>.NTreeNode<string> six = nodeFactory.CreateNode("Six");
			NTreeNodeFactory<string>.NTreeNode<string> seven = nodeFactory.CreateNode("Seven");
			NTreeNodeFactory<string>.NTreeNode<string> eight = nodeFactory.CreateNode("Eight");
			NTreeNodeFactory<string>.NTreeNode<string> nine = nodeFactory.CreateNode("Nine");

			topLevel.AddRoot(one);
			Console.WriteLine("topLevel.GetRoot().CountChildren: " + 
				topLevel.GetRoot().CountChildren());

			topLevel.GetRoot().AddNode(two);
			topLevel.GetRoot().AddNode(three);
			topLevel.GetRoot().AddNode(four);

			topLevel.GetRoot().Children[0].AddNode(five);
			topLevel.GetRoot().Children[0].AddNode(eight);
			topLevel.GetRoot().Children[0].AddNode(nine);
			topLevel.GetRoot().Children[1].AddNode(six);
			topLevel.GetRoot().Children[1].Children[0].AddNode(seven);

			Console.WriteLine("Display Entire tree:");
			topLevel.GetRoot().PrintDepthFirst();

			Console.WriteLine("Display tree from node [two]:");
			topLevel.GetRoot().Children[0].PrintDepthFirst();

			Console.WriteLine("Depth First Search:");
			Console.WriteLine("topLevel.DepthFirstSearch(One): " + 
				topLevel.GetRoot().DepthFirstSearch("One").Value().ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(Two): " + 
				topLevel.GetRoot().DepthFirstSearch("Two").Value().ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(Three): " + 
				topLevel.GetRoot().DepthFirstSearch("Three").Value().ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(Four): " + 
				topLevel.GetRoot().DepthFirstSearch("Four").Value().ToString());
			Console.WriteLine("topLevel.DepthFirstSearch(Five): " + 
				topLevel.GetRoot().DepthFirstSearch("Five").Value().ToString());

			Console.WriteLine("\r\n\r\nBreadth First Search:");
			Console.WriteLine("topLevel.BreadthFirstSearch(One): " + 
				topLevel.GetRoot().BreadthFirstSearch("One").Value().ToString());
			Console.WriteLine("topLevel.BreadthFirstSearch(Two): " + 
				topLevel.GetRoot().BreadthFirstSearch("Two").Value().ToString());
			Console.WriteLine("topLevel.BreadthFirstSearch(Three): " + 
				topLevel.GetRoot().BreadthFirstSearch("Three").Value().ToString());
			Console.WriteLine("topLevel.BreadthFirstSearch(Four): " + 
				topLevel.GetRoot().BreadthFirstSearch("Four").Value().ToString());
		}
/*
topLevel.GetRoot().CountChildren: 0
Display Entire tree:
this: One
	childNodes[0]:  Two
	childNodes[1]:  Three
	childNodes[2]:  Four
this: Two
	childNodes[0]:  Five
	childNodes[1]:  Eight
	childNodes[2]:  Nine
this: Five
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Eight
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Nine
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Three
	childNodes[0]:  Six
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Six
	childNodes[0]:  Seven
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Seven
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Four
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
Display tree from node [two]:
this: Two
	childNodes[0]:  Five
	childNodes[1]:  Eight
	childNodes[2]:  Nine
this: Five
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Eight
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
this: Nine
	childNodes[0]:  NULL
	childNodes[1]:  NULL
	childNodes[2]:  NULL
Depth First Search:
topLevel.DepthFirstSearch(One): One
topLevel.DepthFirstSearch(Two): Two
topLevel.DepthFirstSearch(Three): Three
topLevel.DepthFirstSearch(Four): Four
topLevel.DepthFirstSearch(Five): Five


Breadth First Search:
topLevel.BreadthFirstSearch(One): One
topLevel.BreadthFirstSearch(Two): Two
topLevel.BreadthFirstSearch(Three): Three
topLevel.BreadthFirstSearch(Four): Four
*/


		public class NTree<T>
			where T : IComparable<T>
		{
			public NTree() 
			{
				maxChildren = int.MaxValue;
			}

			public NTree(int maxNumChildren) 
			{
				maxChildren = maxNumChildren;
			}


			// The root node of the tree
			private NTreeNodeFactory<T>.NTreeNode<T> root = null;
			// The maximum number of child nodes that a parent node may contain
            private int maxChildren = 0;


			public void AddRoot(NTreeNodeFactory<T>.NTreeNode<T> node)
			{
				root = node;
			}

			public NTreeNodeFactory<T>.NTreeNode<T> GetRoot()
			{
				return (root);
			}

			public int MaxChildren
			{
				get {return (maxChildren);}
			}
		}

		public class NTreeNodeFactory<T>
			where T : IComparable<T>
		{
			public NTreeNodeFactory(NTree<T> root) 
			{
				maxChildren = root.MaxChildren;
			}


			private int maxChildren = 0;


			public int MaxChildren
			{
				get {return (maxChildren);}
			}

			public NTreeNode<T> CreateNode(T value)
			{
				return (new NTreeNode<T>(value, maxChildren));
			}


			// Nested Node class
			public class NTreeNode<U>
				where U : IComparable<U>
			{
				public NTreeNode(U value, int maxChildren) 
				{
					if (!value.Equals(null))
					{
						nodeValue = value;
					}

					childNodes = new NTreeNode<U>[maxChildren];
				}


				protected U nodeValue = default(U);
				protected NTreeNode<U>[] childNodes = null;


				public int NumOfChildren
				{
					get {return (CountChildren());}
				}

				public int CountChildren()
				{
					int currCount = 0;

					for (int index = 0; index <= childNodes.GetUpperBound(0); index++)
					{
						if (childNodes[index] != null)
						{
							++currCount;
							currCount += childNodes[index].CountChildren();
						}
					}

					return (currCount);
				}

				public int CountImmediateChildren()
				{
					int currCount = 0;

					for (int index = 0; index <= childNodes.GetUpperBound(0); index++)
					{
						if (childNodes[index] != null)
						{
							++currCount;
						}
					}

					return (currCount);
				}


				public NTreeNode<U>[] Children
				{
					get {return (childNodes);}
				}

				public NTreeNode<U> GetChild(int index)
				{
					return (childNodes[index]);
				}

				public U Value()
				{
					return (nodeValue);
				}

				public void AddNode(NTreeNode<U> node)
				{
					int numOfNonNullNodes = CountImmediateChildren();

					if (numOfNonNullNodes < childNodes.Length)
					{
						childNodes[numOfNonNullNodes] = node;
					}
					else
					{
						throw (new InvalidOperationException("Cannot add more children to this node."));
					}
				}

				public NTreeNode<U> DepthFirstSearch(U targetObj)
				{
					NTreeNode<U> retObj = default(NTreeNode<U>);

					if (targetObj.CompareTo(nodeValue) == 0)
					{
						retObj = this;
					}
					else
					{
						for (int index=0; index<=childNodes.GetUpperBound(0); index++)
						{
							if (childNodes[index] != null)
							{
								retObj = childNodes[index].DepthFirstSearch(targetObj);
								if (retObj != null)
								{
									break;
								}
							}
						}
					}

					return (retObj);
				}

				public NTreeNode<U> BreadthFirstSearch(U targetObj)
				{
					Queue<NTreeNode<U>> row = new Queue<NTreeNode<U>>();
					row.Enqueue(this);

					while (row.Count > 0)
					{
						// Get next node in queue
						NTreeNode<U> currentNode = row.Dequeue();

						// Is this the node we are looking for?
						if (targetObj.CompareTo(currentNode.nodeValue) == 0)
						{
							return (currentNode);
						}

						for (int index = 0; 
							index < currentNode.CountImmediateChildren(); 
							index++)
						{
							if (currentNode.Children[index] != null)
							{
								row.Enqueue(currentNode.Children[index]);
							}
						}
					}

					return (null);
				}

				public void PrintDepthFirst()
				{
					Console.WriteLine("this: " + nodeValue.ToString());

					for (int index = 0; index < childNodes.Length; index++)
					{
						if (childNodes[index] != null)
						{
							Console.WriteLine("\tchildNodes[" + index + "]:  " + 
								childNodes[index].nodeValue.ToString());
						}
						else
						{
							Console.WriteLine("\tchildNodes[" + index + "]:  NULL");
						}
					}

					for (int index = 0; index < childNodes.Length; index++)
					{
						if (childNodes[index] != null)
						{
							childNodes[index].PrintDepthFirst();
						}
					}
				}

				public void RemoveNode(int index)
				{
					// Remove node from array and Compact the array
					if (index < childNodes.GetLowerBound(0) || 
						index > childNodes.GetUpperBound(0))
					{
						throw (new ArgumentOutOfRangeException("index", index, 
							"Array index out of bounds."));
					}
					else if (index < childNodes.GetUpperBound(0))
					{
						Array.Copy(childNodes, index + 1, childNodes, index, 
							childNodes.Length - index - 1);
					}

					childNodes.SetValue(null, childNodes.GetUpperBound(0));
				}
			}
		}
		#endregion
		
		#region 11.6 Creating a Set Object
        public static void TestSet()
        {
            HashSet<int> set1 = new HashSet<int>();
            HashSet<int> set2 = new HashSet<int>();
            HashSet<int> set3 = new HashSet<int>();

            set1.Add(1);
            set1.Add(2);
            set1.Add(3);
            set1.Add(4);
            set1.Add(5);
            set1.Add(6);

            set2.Add(-10);
            set2.Add(2);
            set2.Add(40);

            set3.Add(3);
            set3.Add(6);

            foreach (int o in set2)
            {
                Console.WriteLine(o.ToString());
            }

            Console.WriteLine("set1.Contains(2): " + set1.Contains(2));
            Console.WriteLine("set1.Contains(0): " + set1.Contains(0));

            Console.WriteLine("\r\nset1.Count: " + set1.Count);
            Console.WriteLine();
            Console.WriteLine("set1.DisplaySet: " + DisplaySet(set1));
            Console.WriteLine("set2.DisplaySet: " + DisplaySet(set2));
            Console.WriteLine("set3.DisplaySet: " + DisplaySet(set3));
            Console.WriteLine();
            set1.UnionWith(set2);
            Console.WriteLine("set1.UnionWith(set2): " +
                DisplaySet(set1));
            set1.IntersectWith(set2);
            Console.WriteLine("set1.IntersectWith(set2): " +
                DisplaySet(set1));
            set1.SymmetricExceptWith(set2);
            Console.WriteLine("set1.SymmetricExceptWith(set2): " +
                DisplaySet(set1));
            Console.WriteLine("set1.Equals(set2): " + set1.Equals(set2));
            Console.WriteLine("set1 == set2: " + (set1 == set2));
            Console.WriteLine("set1 != set2: " + (set1 != set2));
            Console.WriteLine("set1.IsSubsetOf(set2): " + set1.IsSubsetOf(set2));
            Console.WriteLine("set1.IsSupersetOf(set2): " + set1.IsSupersetOf(set2));
            Console.WriteLine();
            set2.UnionWith(set1);
            Console.WriteLine("set2.UnionWith(set1): " +
                DisplaySet(set2));
            set2.IntersectWith(set1);
            Console.WriteLine("set2.IntersectWith(set1): " +
                DisplaySet(set2));
            set2.ExceptWith(set1);
            Console.WriteLine("set2.ExceptWith(set1): " +
                DisplaySet(set2));
            Console.WriteLine("set2.Equals(set1): " + set2.Equals(set1));
            Console.WriteLine("set2 == set1): " + (set2 == set1));
            Console.WriteLine("set2 != set1): " + (set2 != set1));
            Console.WriteLine("set2.IsSubsetOf(set1): " + set2.IsSubsetOf(set1));
            Console.WriteLine("set2.IsSupersetOf(set1): " + set2.IsSupersetOf(set1));
            Console.WriteLine();
            set3.UnionWith(set1);
            Console.WriteLine("set3.UnionWith(set1): " +
                DisplaySet(set3));
            set3.IntersectWith(set1);
            Console.WriteLine("set3.IntersectWith(set1): " +
                DisplaySet(set3));
            set3.ExceptWith(set1);
            Console.WriteLine("set3.ExceptWith(set1): " +
                DisplaySet(set3));
            Console.WriteLine("set3.Equals(set1): " + set3.Equals(set1));
            Console.WriteLine("set3 == set1: " + (set3 == set1));
            Console.WriteLine("set3 != set1: " + (set3 != set1));
            Console.WriteLine("set3.IsSubsetOf(set1): " + set3.IsSubsetOf(set1));
            Console.WriteLine("set3.IsSupersetOf(set1): " + set3.IsSupersetOf(set1));
            Console.WriteLine("set1.IsSubsetOf(set3): " + set1.IsSubsetOf(set3));
            Console.WriteLine("set1.IsSupersetOf(set3): " + set1.IsSupersetOf(set3));
            Console.WriteLine();
            set3.UnionWith(set2);
            Console.WriteLine("set3.UnionWith(set2): " +
                DisplaySet(set3));
            set3.IntersectWith(set2);
            Console.WriteLine("set3.IntersectWith(set2): " +
                DisplaySet(set3));
            set3.ExceptWith(set2);
            Console.WriteLine("set3.ExceptWith(set2): " +
                DisplaySet(set3));
            Console.WriteLine("set3.Equals(set2): " + set3.Equals(set2));
            Console.WriteLine("set3 == set2: " + (set3 == set2));
            Console.WriteLine("set3 != set2: " + (set3 != set2));
            Console.WriteLine("set3.IsSubsetOf(set2): " + set3.IsSubsetOf(set2));
            Console.WriteLine("set3.IsSupersetOf(set2): " + set3.IsSupersetOf(set2));
            Console.WriteLine();
            Console.WriteLine("set3.Equals(set3): " + set3.Equals(set3));
            Console.WriteLine("set3 == set3: " + (set3 == set3));
            Console.WriteLine("set3 != set3: " + (set3 != set3));
            Console.WriteLine("set3.IsSubsetOf(set3): " + set3.IsSubsetOf(set3));
            Console.WriteLine("set3.IsSupersetOf(set3): " + set3.IsSupersetOf(set3));
        }

        public static string DisplaySet(HashSet<int> set)
        {
            if (set.Count == 0)
            {
                return ("{}");
            }
            else
            {
                StringBuilder displayStr = new StringBuilder("{ ");

                foreach (int i in set)
                {
                    displayStr.Append(i);
                    displayStr.Append(", ");
                }

                displayStr.Remove(displayStr.Length - 2, 2);
                displayStr.Append(" }");

                return (displayStr.ToString());
            }
        }
        
        
        public static void TestSet_Deprecated()
		{
			Set<int> set1 = new Set<int>();
			Set<int> set2 = new Set<int>();
			Set<int> set3 = new Set<int>();

			set1.Add(1);
			set1.Add(2);
			set1.Add(3);
			set1.Add(4);
			set1.Add(5);
			set1.Add(6);

			set2.Add(-10);
			set2.Add(2);
			set2.Add(40);

			set3.Add(3);
			set3.Add(6);

			foreach (int o in set2)
			{
				Console.WriteLine(o.ToString());
			}

			Console.WriteLine("set1.Contains(2): " + set1.Contains(2));
			Console.WriteLine("set1.Contains(0): " + set1.Contains(0));

			Console.WriteLine("\r\nset1.Count: " + set1.Count);
			Console.WriteLine();
			Console.WriteLine("set1.DisplaySet: " + set1.DisplaySet());
			Console.WriteLine("set2.DisplaySet: " + set2.DisplaySet());
			Console.WriteLine("set3.DisplaySet: " + set3.DisplaySet());
			Console.WriteLine();
			Console.WriteLine("set1.UnionOf(set2): " + 
				set1.UnionOf(set2).DisplaySet());
			Console.WriteLine("set1.IntersectionOf(set2): " + 
				set1.IntersectionOf(set2).DisplaySet());
			Console.WriteLine("set1.DifferenceOf(set2): " + 
				set1.DifferenceOf(set2).DisplaySet());
			Console.WriteLine("set1 | set2: " + (set1 | set2).DisplaySet());
			Console.WriteLine("set1 & set2: " + (set1 & set2).DisplaySet());
			Console.WriteLine("set1 ^ set2: " + (set1 ^ set2).DisplaySet());
			Console.WriteLine("set1.Equals(set2): " + set1.Equals(set2));
			Console.WriteLine("set1 == set2: " + (set1 == set2));
			Console.WriteLine("set1 != set2: " + (set1 != set2));
			Console.WriteLine("set1.IsSubsetOf(set2): " + set1.IsSubsetOf(set2));
			Console.WriteLine("set1.IsSupersetOf(set2): " + set1.IsSupersetOf(set2));
			Console.WriteLine();
			Console.WriteLine("set2.UnionOf(set1): " + 
				set2.UnionOf(set1).DisplaySet());
			Console.WriteLine("set2.IntersectionOf(set1): " + 
				set2.IntersectionOf(set1).DisplaySet());
			Console.WriteLine("set2.DifferenceOf(set1): " + 
				set2.DifferenceOf(set1).DisplaySet());
			Console.WriteLine("set2.Equals(set1): " + set2.Equals(set1));
			Console.WriteLine("set2 == set1): " + (set2 == set1));
			Console.WriteLine("set2 != set1): " + (set2 != set1));
			Console.WriteLine("set2.IsSubsetOf(set1): " + set2.IsSubsetOf(set1));
			Console.WriteLine("set2.IsSupersetOf(set1): " + set2.IsSupersetOf(set1));
			Console.WriteLine();
			Console.WriteLine("set3.UnionOf(set1): " + 
				set3.UnionOf(set1).DisplaySet());
			Console.WriteLine("set3.IntersectionOf(set1): " + 
				set3.IntersectionOf(set1).DisplaySet());
			Console.WriteLine("set3.DifferenceOf(set1): " + 
				set3.DifferenceOf(set1).DisplaySet());
			Console.WriteLine("set3.Equals(set1): " + set3.Equals(set1));
			Console.WriteLine("set3 == set1: " + (set3 == set1));
			Console.WriteLine("set3 != set1: " + (set3 != set1));
			Console.WriteLine("set3.IsSubsetOf(set1): " + set3.IsSubsetOf(set1));
			Console.WriteLine("set3.IsSupersetOf(set1): " + set3.IsSupersetOf(set1));
			Console.WriteLine("set1.IsSubsetOf(set3): " + set1.IsSubsetOf(set3));
			Console.WriteLine("set1.IsSupersetOf(set3): " + set1.IsSupersetOf(set3));
			Console.WriteLine();
			Console.WriteLine("set3.UnionOf(set2): " + 
				set3.UnionOf(set2).DisplaySet());
			Console.WriteLine("set3.IntersectionOf(set2): " + 
				set3.IntersectionOf(set2).DisplaySet());
			Console.WriteLine("set3.DifferenceOf(set2): " + 
				set3.DifferenceOf(set2).DisplaySet());
			Console.WriteLine("set3 | set2: " + (set3 | set2).DisplaySet());
			Console.WriteLine("set3 & set2: " + (set3 & set2).DisplaySet());
			Console.WriteLine("set3 ^ set2: " + (set3 ^ set2).DisplaySet());
			Console.WriteLine("set3.Equals(set2): " + set3.Equals(set2));
			Console.WriteLine("set3 == set2: " + (set3 == set2));
			Console.WriteLine("set3 != set2: " + (set3 != set2));
			Console.WriteLine("set3.IsSubsetOf(set2): " + set3.IsSubsetOf(set2));
			Console.WriteLine("set3.IsSupersetOf(set2): " + set3.IsSupersetOf(set2));
			Console.WriteLine();
			Console.WriteLine("set3.Equals(set3): " + set3.Equals(set3));
			Console.WriteLine("set3 == set3: " + (set3 == set3));
			Console.WriteLine("set3 != set3: " + (set3 != set3));
			Console.WriteLine("set3.IsSubsetOf(set3): " + set3.IsSubsetOf(set3));
			Console.WriteLine("set3.IsSupersetOf(set3): " + set3.IsSupersetOf(set3));

			Console.WriteLine("set1[1]: " + set1[1].ToString());
			set1[1] = 100;

			set1.RemoveAt(1);
			set1.RemoveAt(2);
			Console.WriteLine("set1: " + set1.DisplaySet());
		}
/*  ORIGINAL DATA DEPRECATED
-10
2
40
set1.Contains(2): True
set1.Contains(0): False

set1.Count: 6

set1.DisplaySet: { 1, 2, 3, 4, 5, 6 }
set2.DisplaySet: { -10, 2, 40 }
set3.DisplaySet: { 3, 6 }

set1.UnionOf(set2): { 1, 2, 3, 4, 5, 6, -10, 40 }
set1.IntersectionOf(set2): { 2 }
set1.DifferenceOf(set2): { -10, 40, 1, 3, 4, 5, 6 }
set1 | set2: { 1, 2, 3, 4, 5, 6, -10, 40 }
set1 & set2: { 2 }
set1 ^ set2: { -10, 40, 1, 3, 4, 5, 6 }
set1.Equals(set2): False
set1 == set2: False
set1 != set2: True
set1.IsSubsetOf(set2): False
set1.IsSupersetOf(set2): False

set2.UnionOf(set1): { 1, 2, 3, 4, 5, 6, -10, 40 }
set2.IntersectionOf(set1): { 2 }
set2.DifferenceOf(set1): { 1, 3, 4, 5, 6, -10, 40 }
set2.Equals(set1): False
set2 == set1): False
set2 != set1): True
set2.IsSubsetOf(set1): False
set2.IsSupersetOf(set1): False

set3.UnionOf(set1): { 1, 2, 3, 4, 5, 6 }
set3.IntersectionOf(set1): { 3, 6 }
set3.DifferenceOf(set1): { 1, 2, 4, 5 }
set3.Equals(set1): False
set3 == set1: False
set3 != set1: True
set3.IsSubsetOf(set1): True
set3.IsSupersetOf(set1): False
set1.IsSubsetOf(set3): False
set1.IsSupersetOf(set3): True

set3.UnionOf(set2): { -10, 2, 40, 3, 6 }
set3.IntersectionOf(set2): {}
set3.DifferenceOf(set2): { -10, 2, 40, 3, 6 }
set3 | set2: { -10, 2, 40, 3, 6 }
set3 & set2: {}
set3 ^ set2: { -10, 2, 40, 3, 6 }
set3.Equals(set2): False
set3 == set2: False
set3 != set2: True
set3.IsSubsetOf(set2): False
set3.IsSupersetOf(set2): False

set3.Equals(set3): True
set3 == set3: True
set3 != set3: False
set3.IsSubsetOf(set3): True
set3.IsSupersetOf(set3): True
set1[1]: 2
set1: { 1, 3, 5, 6 }
*/

        // DEPRECATED (Use HashSet if possible)
		public class Set<T>
		{
			private List<T> internalSet = new List<T>();

			public int Count
			{
				get {return (internalSet.Count);}
			}

			public T this[int index] 
			{
				get 
				{
					return (internalSet[index]);
				}
				set 
				{
					if (internalSet.Contains(value))
					{
						throw (new ArgumentException(
							   "Duplicate object cannot be added to this set."));
					}
					else
					{
						internalSet[index] = value;
					}
				}
			}

			public void Add(T obj)
			{
				if (internalSet.Contains(obj))
				{
					throw (new ArgumentException(
						   "Duplicate object cannot be added to this set."));
				}
				else
				{
					internalSet.Add(obj);
				}
			}

			public void Remove(T obj)
			{
				if (internalSet.Contains(obj))
				{
					throw (new ArgumentException("Object cannot be removed from " +
						   "this set because it does not exist in this set."));
				}
				else
				{
					internalSet.Remove(obj);
				}
			}

			public void RemoveAt(int index)
			{
				internalSet.RemoveAt(index);
			}

			public bool Contains(T obj)
			{
				return (internalSet.Contains(obj));
			}

			public static Set<T> operator |(Set<T> lhs, Set<T> rhs)
			{
				return (lhs.UnionOf(rhs));
			}

			public Set<T> UnionOf(Set<T> set) 
			{
				Set<T> unionSet = new Set<T>();
				Set<T> sourceSet = null;
				Set<T> mergeSet = null;

				if (set.Count > this.Count)   // An optimization
				{
					sourceSet = set;
					mergeSet = this;
				}
				else
				{
					sourceSet = this;
					mergeSet = set;
				}

				// Initialize unionSet with the entire SourceSet
				for (int index = 0; index < sourceSet.Count; index++)
				{
					unionSet.Add(sourceSet.internalSet[index]);
				}

				// mergeSet OR sourceSet
				for (int index = 0; index < mergeSet.Count; index++)
				{
					if (!sourceSet.Contains(mergeSet.internalSet[index])) 
					{
						unionSet.Add(mergeSet.internalSet[index]);
					}
				}

				return (unionSet);
			}

			public static Set<T> operator &(Set<T> lhs, Set<T> rhs)
			{
				return (lhs.IntersectionOf(rhs));
			}

			public Set<T> IntersectionOf(Set<T> set)
			{
				Set<T> intersectionSet = new Set<T>();
				Set<T> sourceSet = null;
				Set<T> mergeSet = null;

				if (set.Count > this.Count)   // An optimization
				{
					sourceSet = set;
					mergeSet = this;
				}
				else
				{
					sourceSet = this;
					mergeSet = set;
				}

				// mergeSet AND sourceSet
				for (int index = 0; index < mergeSet.Count; index++)
				{
					if (sourceSet.Contains(mergeSet.internalSet[index])) 
					{
						intersectionSet.Add(mergeSet.internalSet[index]);
					}
				}

				return (intersectionSet);
			}

			public static Set<T> operator ^(Set<T> lhs, Set<T> rhs)
			{
				return (lhs.DifferenceOf(rhs));
			}

			public Set<T> DifferenceOf(Set<T> set)
			{
				Set<T> differenceSet = new Set<T>();

				// mergeSet XOR sourceSet
				for (int index = 0; index < set.Count; index++)
				{
					if (!this.Contains(set.internalSet[index])) 
					{
						differenceSet.Add(set.internalSet[index]);
					}
				}

				for (int index = 0; index < this.Count; index++)
				{
					if (!set.Contains(internalSet[index])) 
					{
						differenceSet.Add(internalSet[index]);
					}
				}

				return (differenceSet);
			}

			public static bool operator ==(Set<T> lhs, Set<T> rhs)
			{
				return (lhs.Equals(rhs));
			}

			public static bool operator !=(Set<T> lhs, Set<T> rhs)
			{
				return (!lhs.Equals(rhs));
			}

			public override bool Equals(object obj)
			{
				// Since we are overriding Equals we must use the same signature (i.e. the object type)
				bool isEquals = false;

				if (obj != null)
				{
					if (obj is Set<T>)
					{
						if (this.Count == ((Set<T>)obj).Count)
						{
							if (this.IsSubsetOf((Set<T>)obj) && 
								((Set<T>)obj).IsSubsetOf(this))
							{
								isEquals = true;
							}
						}
					}
				}

				return (isEquals);
			}

			public override int GetHashCode()
			{
				return (internalSet.GetHashCode());
			}

			public bool IsSubsetOf(Set<T> set)
			{
				for (int index = 0; index < this.Count; index++)
				{
					if (!set.Contains(internalSet[index])) 
					{
						return (false);
					}
				}

				return (true);
			}

			public bool IsSupersetOf(Set<T> set)
			{
				for (int index = 0; index < set.Count; index++)
				{
					if (!this.Contains(set.internalSet[index])) 
					{
						return (false);
					}
				}

				return (true);
			}

			public string DisplaySet()
			{
				if (this.Count == 0)
				{
					return ("{}");
				}
				else
				{
					StringBuilder displayStr = new StringBuilder("{ ");

					for (int index = 0; index < (this.Count - 1); index++)
					{
						displayStr.Append(internalSet[index]);
						displayStr.Append(", ");
					}

					displayStr.Append(internalSet[internalSet.Count - 1]);
					displayStr.Append(" }");

					return (displayStr.ToString());
				}
			}

			public IEnumerator<T> GetEnumerator()
			{
				for (int cntr = 0; cntr < internalSet.Count; cntr++)
				{
					yield return (internalSet[cntr]);
				}
			}
			
			
			//public IEnumerator GetEnumerator()
			//{
			//    return(new SetEnumerator(this));
			//}
			//
			//
			//// Nested enumerator class
			//public class SetEnumerator : IEnumerator
			//{
			//    public SetEnumerator(Set theSet)
			//    {
			//        setObj = theSet;
			//    }
			//    private Set setObj;
			//    private int index = -1;
			//    public bool MoveNext()
			//    {
			//        index++;
			//        if (index >= setObj.Count)
			//        {
			//            return(false);
			//        }
			//        else
			//        {
			//            return(true);
			//        }
			//    }
			//    public void Reset()
			//    {
			//        index = -1;
			//    }
			//    public object Current
			//    {
			//        get{return(setObj[index]);}
			//    }
			//}
		}

		#endregion
	}
}
