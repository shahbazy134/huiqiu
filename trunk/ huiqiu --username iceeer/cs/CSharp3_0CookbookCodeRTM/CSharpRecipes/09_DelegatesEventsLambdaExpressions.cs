using System;
using System.Linq;
using System.Security;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections.ObjectModel;



namespace CSharpRecipes
{
	public static class DelegatesEventsLambdaExpressions
    {
        #region "9.0 Introduction"
        // declare the delegate
        public delegate int IncreaseByANumber(int j);
        // Defines a delegate for multiple
        public delegate int MultipleIncreaseByANumber(int j, int k, int l);

        // set up a method to implement the delegate functionality
        static public int MultiplyByANumber(int j) {
            return j * 42;
        }

        public static void ExecuteCSharp1_0()
        {
            // create the delegate instance
            IncreaseByANumber increase =
               new IncreaseByANumber(
                   DelegatesEventsLambdaExpressions.MultiplyByANumber);

            // invoke the method and print 420 to the console
            Console.WriteLine(increase(10));
        }

        public static void ExecuteCSharp2_0()
        {
            // create the delegate instance
            IncreaseByANumber increase =
               new IncreaseByANumber(
                delegate(int j)
                {
                    return j * 42;
                });

            // invoke the method and print 420 to the console
            Console.WriteLine(increase(10));
        }

        public static void ExecuteCSharp3_0()
        {
            // declare the lambda expression
            IncreaseByANumber increase = j => j * 42;
            // invoke the method and print 420 to the console
            Console.WriteLine(increase(10));

            MultipleIncreaseByANumber multiple = (j, k, l) => ((j * 42) / k) % l;
            Console.WriteLine(multiple(10, 11, 12));
        }
        #endregion

        #region "9.1 Controlling When and If a Delegate Fires Within a Multicast Delegate"

        public static void InvokeInReverse()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances =
                    myDelegateInstance1 + 
                    myDelegateInstance2 + 
                    myDelegateInstance3;

            Console.WriteLine("Fire delegates in reverse");
            Delegate[] delegateList = allInstances.GetInvocationList();
            foreach (Func<int> instance in delegateList.Reverse())
            {
                instance();
            }
        }	

        public static void InvokeEveryOtherOperation()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances = //myDelegateInstance1;
                    myDelegateInstance1 + 
                    myDelegateInstance2 + 
                    myDelegateInstance3;

            Delegate[] delegateList = allInstances.GetInvocationList();
            Console.WriteLine("Invoke every other delegate");
            foreach (Func<int> instance in delegateList.EveryOther())
            {
                // invoke the delegate
                int retVal = instance();
                Console.WriteLine("Delegate returned " + retVal);
            }
        }

        static IEnumerable<T> EveryOther<T>(this IEnumerable<T> enumerable)
        {
            bool retNext = true;
            foreach (T t in enumerable) 
            { 
                if (retNext) yield return t; 
                retNext = !retNext; 
            }
        }

        public class TestInvokeIntReturn
        {
            public static int Method1()
            {
                Console.WriteLine("Invoked Method1");
                return 1;
            }

            public static int Method2()
            {
                Console.WriteLine("Invoked Method2");
                return 2;
            }

            public static int Method3()
            {
                //throw (new Exception("Method1"));
                //throw (new SecurityException("Method3"));
                Console.WriteLine("Invoked Method3");
                return 3;
            }
        }

        public static void InvokeWithTest()
        {
            Func<bool> myDelegateInstanceBool1 = TestInvokeBoolReturn.Method1;
            Func<bool> myDelegateInstanceBool2 = TestInvokeBoolReturn.Method2;
            Func<bool> myDelegateInstanceBool3 = TestInvokeBoolReturn.Method3;

            Func<bool> allInstancesBool = 
                    myDelegateInstanceBool1 +
                    myDelegateInstanceBool2 +
                    myDelegateInstanceBool3;

            Console.WriteLine(
                "Invoke individually (Call based on previous return value):");
            foreach (Func<bool> instance in allInstancesBool.GetInvocationList())
            {
                if (!instance())
                    break;
            }
        }

        public class TestInvokeBoolReturn
        {
            public static bool Method1()
            {
                Console.WriteLine("Invoked Method1");
                return true;
            }

            public static bool Method2()
            {
                Console.WriteLine("Invoked Method2");
                return false;
            }

            public static bool Method3()
            {
                Console.WriteLine("Invoked Method3");
                return true;
            }
        }
        #endregion

        #region "9.2 Obtaining Return Values from Each Delegate in a Multicast Delegate"
        public static void TestIndividualInvokesReturnValue()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances =
                    myDelegateInstance1 +
                    myDelegateInstance2 +
                    myDelegateInstance3;

            Console.WriteLine("Invoke individually (Obtain each return value):");
            foreach (Func<int> instance in allInstances.GetInvocationList())
            {
                int retVal = instance();
                Console.WriteLine("\tOutput: " + retVal);
            }
        }
        #endregion

        #region "9.3 Handling Exceptions Individually for Each Delegate in a Multicast Delegate"
        [Serializable]
        public class MulticastInvocationException : Exception
        {
            private List<Exception> _invocationExceptions;

            public MulticastInvocationException()
                : base()
            {
            }

            public MulticastInvocationException(IEnumerable<Exception> invocationExceptions)
            {
                _invocationExceptions = new List<Exception>(invocationExceptions);
            }

            public MulticastInvocationException(string message)
                : base(message)
            {
            }

            public MulticastInvocationException(string message, Exception innerException) :
                base(message,innerException)
            {
            }

            protected MulticastInvocationException(SerializationInfo info, StreamingContext context) :
                base(info, context)
            {
                _invocationExceptions =
                    (List<Exception>)info.GetValue("InvocationExceptions", 
                        typeof(List<Exception>));
            }

            [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter = true)]
            public override void GetObjectData(
               SerializationInfo info, StreamingContext context)
            {
                info.AddValue("InvocationExceptions", this.InvocationExceptions);
                base.GetObjectData(info, context);
            }

            public ReadOnlyCollection<Exception> InvocationExceptions
            {
                get { return new ReadOnlyCollection<Exception>(_invocationExceptions); }
            }
        }

        public static void TestIndividualInvokesExceptions()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances =
                    myDelegateInstance1 +
                    myDelegateInstance2 +
                    myDelegateInstance3;

            Console.WriteLine("Invoke individually (handle exceptions):");
            
            // Create an instance of a wrapper exception to hold any exceptions
            // encountered during the invocations of the delegate instances
            List<Exception> invocationExceptions = new List<Exception>();
            
            foreach (Func<int> instance in allInstances.GetInvocationList())
            {
                try
                {
                    int retVal = instance();
                    Console.WriteLine("\tOutput: " + retVal);
                }
                catch (Exception ex)
                {
                    // Display and log the exception and continue
                    Console.WriteLine(ex.ToString());
                    EventLog myLog = new EventLog();
                    myLog.Source = "MyApplicationSource";
                    myLog.WriteEntry("Failure invoking " +
                        instance.Method.Name + " with error " +
                        ex.ToString(),
                        EventLogEntryType.Error);
                    // add this exception to the list
                    invocationExceptions.Add(ex);
                }
            }
            // if we caught any exceptions along the way, throw our
            // wrapper exception with all of them in it.
            if (invocationExceptions.Count > 0)
            {
                throw new MulticastInvocationException(invocationExceptions);
            }
        }
		#endregion
		
		#region "9.4 Converting a Synchronous Delegate to an Asynchronous Delegate"

        public delegate void SyncDelegateTypeSimple();
        public delegate int SyncDelegateType(string message);

        public static void TestSimpleSyncDelegate()
        {
            SyncDelegateTypeSimple sdtsInstance = TestSyncDelegateTypeSimple.Method1;
            sdtsInstance();
        }

        public static void TestSimpleAsyncDelegate()
        {
            AsyncCallback callBack = new AsyncCallback(DelegateSimpleCallback);

            SyncDelegateTypeSimple sdtsInstance = TestSyncDelegateTypeSimple.Method1;

            IAsyncResult asyncResult = 
                sdtsInstance.BeginInvoke(callBack, null);

            Console.WriteLine("WORKING...");
        }

        public static void TestComplexSyncDelegate()
        {
            SyncDelegateType sdtInstance = TestSyncDelegateType.Method1;

            int retVal = sdtInstance("Synchronous call");

            Console.WriteLine("Sync: " + retVal);
        }

        public static void TestCallbackAsyncDelegate()
        {
            AsyncCallback callBack = 
                new AsyncCallback(DelegateCallback);

            SyncDelegateType sdtInstance = TestSyncDelegateType.Method1;

            IAsyncResult asyncResult = 
                sdtInstance.BeginInvoke("Asynchronous call", callBack, null);

            Console.WriteLine("WORKING...");
        }

        // The callback that gets called when TestSyncDelegateTypeSimple.Method1 
        // is finished processing
        private static void DelegateSimpleCallback(IAsyncResult iResult)
        {
            AsyncResult result = (AsyncResult)iResult;
            SyncDelegateTypeSimple sdtsInstance =
                (SyncDelegateTypeSimple)result.AsyncDelegate;

            sdtsInstance.EndInvoke(result);
            Console.WriteLine("Simple callback run");
        }

        // The callback that gets called when TestSyncDelegateType.Method1 
        // is finished processing
        private static void DelegateCallback(IAsyncResult iResult)
        {
            AsyncResult result = (AsyncResult)iResult;
            SyncDelegateType sdtInstance =
                (SyncDelegateType)result.AsyncDelegate;

            int retVal = sdtInstance.EndInvoke(result);
            Console.WriteLine("retVal (Callback): " + retVal);
        }

        // The class and method that is invoked through the SyncDelegateTypeSimple delegate
        public class TestSyncDelegateTypeSimple
        {
            public static void Method1()
            {
                Console.WriteLine("Invoked Method1");
            }
        }


        public class TestSyncDelegateType
        {
            public static int Method1(string message)
            {
                Console.WriteLine("Invoked Method1 with message: " + message);
                return 1;
            }
        }
        #endregion

        #region "9.5 An Advanced Interface Search Mechanism"
	    public static void FindSpecificInterfaces()
	    {
            // set up the interfaces to search for
            Type[] interfaces = {
                typeof(System.ICloneable),
                typeof(System.Collections.ICollection),
                typeof(System.IAppDomainSetup) };

            // set up the type to examine
            Type searchType = typeof(System.Collections.ArrayList);

            var matches = from t in searchType.GetInterfaces()
                          join s in interfaces on t equals s
                          select s;

            Console.WriteLine("Matches found:");
            foreach (Type match in matches)
            {
                Console.WriteLine(match.ToString());
            }


            // A filter to search for all implemented interfaces that are defined 
            // within a particular namespace (in this case the System.Collections namespace):
            var collectionsInterfaces = from type in searchType.GetInterfaces()
                                       where type.Namespace == "System.Collections"
                                       select type;
            foreach (Type t in collectionsInterfaces)
            {
                Console.WriteLine("Implemented interface in System.Collections: " + t);
            }

            // A filter to search for all implemented interfaces that contain a method called Add, 
            // which returns an Int32 value:
            var addInterfaces = from type in searchType.GetInterfaces()
                                       from method in type.GetMethods()
                                       where (method.Name == "Add") && 
                                             (method.ReturnType == typeof(int))
                                       select type;
            foreach (Type t in addInterfaces)
            {
                Console.WriteLine("Implemented interface with int Add() method: " + t);
            }

            // A filter to search for all implemented interfaces that are loaded from the 
            // Global Assembly Cache (GAC):
            var gacInterfaces = from type in searchType.GetInterfaces()
                                where type.Assembly.GlobalAssemblyCache
                                select type;
            foreach (Type t in gacInterfaces)
            {
                Console.WriteLine("Implemented interface loaded from GAC: " + t);
            }

            // A filter to search for all implemented interfaces that are defined within an 
            // assembly with the version number 2.0.0.0:
            var versionInterfaces = from type in searchType.GetInterfaces()
                                    where type.Assembly.GlobalAssemblyCache
                                    select type;
            foreach (Type t in versionInterfaces)
            {
                Console.WriteLine("Implemented interface from assembly with version 2.0.0.0: " + t);
            }
		}

        #endregion
            
        #region "9.6 Observing Additions and Modifications to a Dictionary"
        public static void TestObserverPattern()
        {
            Dictionary<int, string> dictionary1 = new Dictionary<int, string>();
            Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
            Dictionary<int, string> dictionary3 = new Dictionary<int, string>();

            // Create three observable dictionary instances
            var obsDict1 = dictionary1.MakeObservableDictionary();
            var obsDict2 = dictionary2.MakeObservableDictionary();
            var obsDict3 = dictionary3.MakeObservableDictionary();

            // Create an observer for the three subject objects
            var observer = new ObservableDictionaryObserver<int, string>();

            // Register the three subjects with the observer
            observer.Register(obsDict1);
            observer.Register(obsDict2);
            observer.Register(obsDict3);

            // hook up the approval events for adding or changing 
            observer.ApproveAdd += 
                new ObservableDictionaryObserver<int, string>.
                    Approval(SeekApproval);
            observer.ApproveChange +=
                new ObservableDictionaryObserver<int, string>.
                    Approval(SeekApproval);

            // Use the observable instances
            obsDict1.Add(1, "one");
            obsDict2.Add(2, "two");
            obsDict3.Add(3, "three");

            // Insure the approval process worked
            Debug.Assert(obsDict1.Count == 1);
            Debug.Assert(obsDict2.Count == 1);
            // this should be empty as the value was more than three characters
            Debug.Assert(obsDict3.Count == 0);

            // Unregister the observable instances
            observer.Unregister(obsDict3);
            observer.Unregister(obsDict2);
            observer.Unregister(obsDict1);

            ///////////////////////////////////////////////////////////////
            // Now do it with a different type of dictionary
            ///////////////////////////////////////////////////////////////
            // Create two observable SortedList instances
            SortedList<string, bool> sortedList1 = new SortedList<string, bool>();
            SortedList<string, bool> sortedList2 = new SortedList<string, bool>();

            var obsSortedList1 = sortedList1.MakeObservableDictionary();
            var obsSortedList2 = sortedList2.MakeObservableDictionary();

            // Create an observer for the two subject objects
            ObservableDictionaryObserver<string, bool> listObserver =
                new ObservableDictionaryObserver<string, bool>();

            // Register the three subjects with the observer
            listObserver.Register(obsSortedList1);
            listObserver.Register(obsSortedList2);

            // hook up the approval events for adding or changing 
            listObserver.ApproveAdd +=
                new ObservableDictionaryObserver<string, bool>.
                    Approval(ApprovePositive);
            listObserver.ApproveChange +=
                new ObservableDictionaryObserver<string, bool>.
                    Approval(ApprovePositive);

            // Use the observable instances
            obsSortedList1.Add("Item 1",true);
            obsSortedList2.Add("Item 2", false);

            // Insure the approval process worked
            Debug.Assert(obsSortedList1.Count == 1);
            // this should be empty as only true values are taken
            Debug.Assert(obsSortedList2.Count == 0);

            // Unregister the observable instances
            listObserver.Unregister(obsSortedList2);
            listObserver.Unregister(obsSortedList1);
        }

        static bool SeekApproval(object sender, 
                ObservableDictionaryEventArgs<int, string> args)
        {
            // only allow strings of no more than 3 characters in
            // our dictionary
            string value = args.Value.ToString();
            if (value.Length <= 3)
                return true;
            return false;
        }

        static bool ApprovePositive(object sender, 
                ObservableDictionaryEventArgs<string, bool> args)
        {
            // only allow positive values
            return args.Value;
        }


        // The observer object that will observe a registered ObservableDictionary object
        public class ObservableDictionaryObserver<TKey,TValue>
        {
            public ObservableDictionaryObserver() { }

            // set up delegate/events for approving an addition or change
            public delegate bool Approval(object sender,
                    ObservableDictionaryEventArgs<TKey,TValue> e);

            public Approval ApproveAdd { get; set; }
            public Approval ApproveChange { get; set; }

            public void Register(ObservableDictionary<TKey, TValue> dictionary)
            {
                // hook up to the ObservableDictionary instance events
                dictionary.AddingEntry +=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnAddingListener);
                dictionary.AddedEntry +=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnAddedListener);
                dictionary.ChangingEntry +=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnChangingListener);
                dictionary.ChangedEntry +=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnChangedListener);
            }

            public void Unregister(ObservableDictionary<TKey,TValue> dictionary)
            {
                // Unhook from the ObservableDictionary instance events
                dictionary.AddingEntry -=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnAddingListener);
                dictionary.AddedEntry -=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnAddedListener);
                dictionary.ChangingEntry -=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnChangingListener);
                dictionary.ChangedEntry -=
                    new EventHandler<ObservableDictionaryEventArgs<TKey, TValue>>(OnChangedListener);
            }

            private void CheckApproval(Approval approval, 
                    ObservableDictionaryEventArgs<TKey,TValue> args)
            {
                // check everyone who wants to approve
                foreach (Approval approvalInstance in 
                                approval.GetInvocationList())
                {
                    if (!approvalInstance(this,args))
                    {
                        // if any of the concerned parties
                        // refuse, then no add.  Adds by default
                        args.KeepChanges = false;
                        break;
                    }
                }
            }

            private void OnAddingListener(object sender, 
                        ObservableDictionaryEventArgs<TKey,TValue> args)
            {
                // see if anyone hooked up for approval
                if (ApproveAdd != null)
                {
                    CheckApproval(ApproveAdd, args);
                }

                Debug.WriteLine("[NOTIFY] Before Add...: Add Approval = " +
                                    args.KeepChanges.ToString());
            }

            private void OnAddedListener(object sender,
                    ObservableDictionaryEventArgs<TKey, TValue> args)
            {
                Debug.WriteLine("[NOTIFY] ...After Add:  Item approved for adding: " +
                                    args.KeepChanges.ToString());
            }

            private void OnChangingListener(object sender,
                    ObservableDictionaryEventArgs<TKey, TValue> args)
            {
                // see if anyone hooked up for approval
                if (ApproveChange != null)
                {
                    CheckApproval(ApproveChange, args);
                }

                Debug.WriteLine("[NOTIFY] Before Change...: Change Approval = " +
                                    args.KeepChanges.ToString());
            }

            private void OnChangedListener(object sender,
                    ObservableDictionaryEventArgs<TKey, TValue> args)
            {
                Debug.WriteLine("[NOTIFY] ...After Change:  Item approved for change: " +
                                    args.KeepChanges.ToString());
            }
        }

        public class ObservableDictionary<TKey,TValue> : IDictionary<TKey,TValue>
        {
            IDictionary<TKey, TValue> _internalDictionary;
            public ObservableDictionary(IDictionary<TKey,TValue> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _internalDictionary = dictionary;
            }

            #region Events and Event Initiation

            public event EventHandler<ObservableDictionaryEventArgs<TKey,TValue>> AddingEntry;
            public event EventHandler<ObservableDictionaryEventArgs<TKey, TValue>> AddedEntry;
            public event EventHandler<ObservableDictionaryEventArgs<TKey, TValue>> ChangingEntry;
            public event EventHandler<ObservableDictionaryEventArgs<TKey, TValue>> ChangedEntry;

            protected virtual bool OnAdding(ObservableDictionaryEventArgs<TKey,TValue> e)
            {
                if (AddingEntry != null)
                {
                    AddingEntry(this, e);
                    return (e.KeepChanges);
                }

                return (true);
            }

            protected virtual void OnAdded(ObservableDictionaryEventArgs<TKey, TValue> e)
            {
                if (AddedEntry != null)
                {
                    AddedEntry(this, e);
                }
            }

            protected virtual bool OnChanging(ObservableDictionaryEventArgs<TKey, TValue> e)
            {
                if (ChangingEntry != null)
                {
                    ChangingEntry(this, e);
                    return (e.KeepChanges);
                }

                return (true);
            }

            protected virtual void OnChanged(ObservableDictionaryEventArgs<TKey, TValue> e)
            {
                if (ChangedEntry != null)
                {
                    ChangedEntry(this, e);
                }
            }
            #endregion // Events and Event Initiation

            #region Interface implementations
            #region IDictionary<TKey,TValue> Members

            public ICollection<TValue> Values
            {
                get { return _internalDictionary.Values; }
            }

            public ICollection<TKey> Keys
            {
                get { return _internalDictionary.Keys; }
            }

            public TValue this[TKey key]
            {
                get
                {
                    TValue value;
                    if (_internalDictionary.TryGetValue(key, out value))
                        return value;
                    else
                    {
                        return default(TValue);
                    }
                }
                set
                {
                    // see if this key is there to be changed, if not add it
                    if (_internalDictionary.ContainsKey(key))
                    {
                        ObservableDictionaryEventArgs<TKey, TValue> args =
                            new ObservableDictionaryEventArgs<TKey, TValue>(key, value);

                        if (OnChanging(args))
                        {
                            _internalDictionary[key] = value;
                        }
                        else
                        {
                            Debug.WriteLine("Change of value cannot be performed");
                        }

                        OnChanged(args);
                    }
                    else
                    {
                        Debug.WriteLine("Item did not exist, adding");
                        _internalDictionary.Add(key, value);
                    }
                }
            }

            public void Add(TKey key, TValue value)
            {
                ObservableDictionaryEventArgs<TKey, TValue> args =
                    new ObservableDictionaryEventArgs<TKey, TValue>(key, value);
                if (OnAdding(args))
                {
                    this._internalDictionary.Add(key, value);
                }
                else
                {
                    Debug.WriteLine("Addition of key/value cannot be performed");
                }

                OnAdded(args);
            }

            public bool ContainsKey(TKey key)
            {
                return _internalDictionary.ContainsKey(key);
            }

            public bool Remove(TKey key)
            {
                return _internalDictionary.Remove(key);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return _internalDictionary.TryGetValue(key, out value);
            }

            #endregion

            #region ICollection<KeyValuePair<TKey,TValue>> Members

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                _internalDictionary.Add(item.Key, item.Value);
            }

            public void Clear()
            {
                _internalDictionary.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return _internalDictionary.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                _internalDictionary.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _internalDictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return _internalDictionary.IsReadOnly; }
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return _internalDictionary.Remove(item);
            }

            #endregion

            #region IEnumerable<KeyValuePair<TKey,TValue>> Members

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _internalDictionary.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _internalDictionary.GetEnumerator();
            }

            #endregion
            #endregion // Interface implementations
        }

        public static ObservableDictionary<TKey, TValue> MakeObservableDictionary<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return new ObservableDictionary<TKey, TValue>(dictionary);
        }

        public class ObservableDictionaryEventArgs<TKey, TValue> : EventArgs
        {
            TKey _key;
            TValue _value;

            public ObservableDictionaryEventArgs(TKey key, TValue value)
            {
                _key = key;
                _value = value;
                this.KeepChanges = true;
            }

            public bool KeepChanges { get; set; }
            public TKey Key { get { return _key; } }
            public TValue Value { get { return _value; } }
        }

        #endregion

		#region "9.7 Using lambda expressions"
		public static void TestUsingLambdaExpressions()
		{
			OldWay ow = new OldWay();
			ow.WorkItOut();

			LambdaWay iw = new LambdaWay();
			iw.WorkItOut();

			DirectAssignmentWay diw = new DirectAssignmentWay();
			diw.WorkItOut();

			GenericWay gw = new GenericWay();
			gw.WorkItOut();

			GenericEventConsumer gec = new GenericEventConsumer();
			gec.Test();

            OuterVars ov = new OuterVars();
            ov.SeeOuterWork();
		}

        class OuterVars
        {
            public void SeeOuterWork()
            {
                int count = 0;
                int total = 0;
                Func<int> countUp = () => count++; 
                for(int i=0;i<10;i++)
                {
                    total += countUp();
                }
                Debug.WriteLine("Total = " + total);
            }
        }

        class OldWay
        {
	        // declare delegate
	        delegate int DoWork(string work);

	        // have a method to create an instance of and call the delegate
	        public void WorkItOut()
	        {
		        // declare instance
		        DoWork dw = new DoWork(DoWorkMethodImpl);
		        // invoke delegate
		        int i = dw("Do work the old way");
	        }

	        // Have a method that the delegate is tied to with a matching signature 
	        // so that it is invoked when the delegate is called
	        public int DoWorkMethodImpl(string s)
	        {
		        Console.WriteLine(s);
		        return s.GetHashCode();
	        }
        }

        class LambdaWay
        {
	        // declare delegate
	        delegate int DoWork(string work);

	        // have a method to create an instance of and call the delegate
	        public void WorkItOut()
	        {
		        // declare instance
		        DoWork dw = s =>
		        {
			        Console.WriteLine(s);
			        return s.GetHashCode();
		        };
		        // invoke delegate
		        int i = dw("Do some inline work");
	        }
        }

        class DirectAssignmentWay
        {
	        // declare delegate
	        delegate int DoWork(string work);

	        // have a method to create an instance of and call the delegate
	        public void WorkItOut()
	        {
		        // declare instance and assign method
		        DoWork dw = DoWorkMethodImpl;
		        // invoke delegate
		        int i = dw("Do some direct assignment work");
	        }
	        // Have a method that the delegate is tied to with a matching signature 
	        // so that it is invoked when the delegate is called
	        public int DoWorkMethodImpl(string s)
	        {
		        Console.WriteLine(s);
		        return s.GetHashCode();
	        }
        }

        class GenericWay
        {
	        // have a method to create two instances of and call the delegates
	        public void WorkItOut()
	        {
		        Func<string,string> dwString = s =>
		        {
			        Console.WriteLine(s);
			        return s;
		        };

		        // invoke string delegate
		        string retStr = dwString("Do some generic work");

		        Func<int,int> dwInt = i =>
		        {
			        Console.WriteLine(i);
			        return i;
		        };

		        // invoke int delegate
		        int j = dwInt(5);

	        }
        }

        public class GenericEventArgs<T> : EventArgs
        {
            public GenericEventArgs(T value)
            {
                this.Value = value;
            }

            public T Value { get ; set; }
        }

		public class GenericEvent
		{
			// declare generic events
            public event EventHandler<GenericEventArgs<string>> DoingStringWork;
            public event EventHandler<GenericEventArgs<int>> DoingIntWork;

			public void WorkItOut()
			{
				DoingStringWork(this, new GenericEventArgs<string>("String work"));
				DoingIntWork(this, new GenericEventArgs<int>(5));
			}
		}

		public class GenericEventConsumer
		{
			public void Test()
			{
				GenericEvent ge = new GenericEvent();
				ge.DoingStringWork += new EventHandler<GenericEventArgs<string>>(ge_DoingStringWork);
				ge.DoingIntWork += new EventHandler<GenericEventArgs<int>>(ge_DoingIntWork);
			}
            void ge_DoingIntWork(object sender, GenericEventArgs<int> workArgs)
			{
				throw new NotImplementedException();
			}
			void ge_DoingStringWork(object sender, GenericEventArgs<string> workArgs)
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region "9.8 Set up event Handlers without the mess"
        public class IWantToKnowNow
        {
	        public static void Test()
	        {
		        eNewspaper DailyBitFlash = new eNewspaper();
		        DailyBitFlash.NewsEvent +=
				           new EventHandler<NewsEventArgs>(BreakingNews);
		        DailyBitFlash.SubscriptionExpired +=
				           new EventHandler<ExpiredEventArgs>(Expired);

		        // send breaking news
		        DailyBitFlash.TransmitBreakingNews("Patriots win!");
		        DailyBitFlash.TransmitBreakingNews("4 more years for W.");
		        DailyBitFlash.TransmitBreakingNews("VS2008 & .NET 3.5 Rocks LA");

		        // offer expired subscriber great deal
		        string offer = "You're subscription expired!";
		        offer += "Act now and get 98 weeks for $19.95!";
		        DailyBitFlash.NotifySubscriptionExpired(offer);

		        IWantToKnowThen.TryMe();
	        }

	        private static void BreakingNews(object src, NewsEventArgs nea)
	        {
		        Console.WriteLine(nea.LatestNews);
	        }

	        private static void Expired(object src, ExpiredEventArgs eea)
	        {
		        Console.WriteLine(eea.NewSubscriptionOffer);
	        }

        }

        public class NewsEventArgs : EventArgs
        {
	        private string _latestNews;

	        public NewsEventArgs(string latestNews)
	        {
		        _latestNews = latestNews;
	        }
	        public string LatestNews
	        {
		        get { return _latestNews; }
	        }
        }

		public class ExpiredEventArgs : EventArgs
		{
			private string _offer;

			public ExpiredEventArgs(string newOffer)
			{
				_offer = newOffer;
			}
			public string NewSubscriptionOffer
			{
				get { return _offer; }
			}
		}

		public class eNewspaper
		{
			public event EventHandler<NewsEventArgs> NewsEvent;

            public void TransmitBreakingNews(string news)
            {
	            // Copy to a temporary variable to be thread-safe.
	            EventHandler<NewsEventArgs> breakingNews = NewsEvent;
	            if (breakingNews != null)
		            breakingNews(this, new NewsEventArgs(news));
            }

			// find out when your subscription expires
			public event EventHandler<ExpiredEventArgs> SubscriptionExpired;

			public void NotifySubscriptionExpired(string newOffer)
			{
                EventHandler<ExpiredEventArgs> subscriptionExpired =
                    SubscriptionExpired;
				if (subscriptionExpired != null)
					subscriptionExpired(this, new ExpiredEventArgs(newOffer));
			}
		}

        public class IWantToKnowThen
        {
	        public static void TryMe()
	        {
		        OldNewspaper DailyPaperFlash = new OldNewspaper();
		        DailyPaperFlash.NewsEvent +=
			        new OldNewspaper.NewsEventHandler(StaleNews);
		        DailyPaperFlash.SubscriptionExpired +=
			        new OldNewspaper.SubscriptionExpiredEventHandler(Expired);

		        // send news
		        DailyPaperFlash.TransmitStaleNews("Patriots win third super bowl!");
		        DailyPaperFlash.TransmitStaleNews("W takes office amongst recount.");
		        DailyPaperFlash.TransmitStaleNews("VS2005 is sooo passe");

		        // offer expired subscriber great deal
		        string offer = "You're subscription expired!";
		        offer += "Act now and get 24 weeks for $19.95!";
		        DailyPaperFlash.NotifySubscriptionExpired(offer);
	        }

	        private static void StaleNews(object src, NewsEventArgs nea)
	        {
		        Console.WriteLine(nea.LatestNews);
	        }

	        private static void Expired(object src, OldExpiredEventArgs eea)
	        {
		        Console.WriteLine(eea.NewSubscriptionOffer);
	        }
        }

		public class OldExpiredEventArgs : EventArgs
		{
			private string _offer;

			public OldExpiredEventArgs(string newOffer)
			{
				_offer = newOffer;
			}
			public string NewSubscriptionOffer
			{
				get { return _offer; }
			}
		}

        public class OldNewspaper
        {
	        // get the news
	        public delegate void NewsEventHandler(Object sender, NewsEventArgs e);
            public event NewsEventHandler NewsEvent;

	        public void TransmitStaleNews(string news)
	        {
                // Copy to a temporary variable to be thread-safe.
                NewsEventHandler newsEvent = NewsEvent;
                if (newsEvent != null)
                    newsEvent(this, new NewsEventArgs(news));
	        }

	        // find out when your subscription expires
	        public delegate void SubscriptionExpiredEventHandler(Object sender, OldExpiredEventArgs e);
	        public event SubscriptionExpiredEventHandler SubscriptionExpired;

	        public void NotifySubscriptionExpired(string newOffer)
	        {
                SubscriptionExpiredEventHandler subscriptionExpired =
                    SubscriptionExpired;
		        if (subscriptionExpired != null)
			        subscriptionExpired(this, new OldExpiredEventArgs(newOffer));
	        }

        }
		#endregion

		#region "9.9 Using different parameter modifiers in lambda expressions"
		public static void TestParameterModifiers()
		{
			ParameterMods pm = new ParameterMods();
			pm.WorkItOut();

			OldParams op = new OldParams();
			op.WorkItOut();

			OuterVariablesParameterModifiers ovpm = new OuterVariablesParameterModifiers();
			ovpm.TestParams();
		}

		class ParameterMods
		{
			// declare out delegate
			delegate int DoOutWork(out string work);
			// declare ref delegate
			delegate int DoRefWork(ref string work);
			// declare params delegate
			delegate int DoParamsWork(params object[] workItems);
			// declare simulated params delegate
			delegate int DoNonParamsWork(object[] workItems);

			// have a method to create an instance of and call the delegate
			public void WorkItOut()
			{
				// declare instance and assign method
				DoOutWork dow = (out string s) =>
				{
					s = "WorkFinished";
					Console.WriteLine(s);
					return s.GetHashCode();
				};
				// invoke delegate
				string work;
				int i = dow(out work);
				Console.WriteLine(work);

				// declare instance and assign method
				DoRefWork drw = (ref string s) =>
				{
					Console.WriteLine(s);
					s = "WorkFinished";
					return s.GetHashCode();
				};
				// invoke delegate
				work = "WorkStarted";
				i = drw(ref work);
				Console.WriteLine(work);

				// Done as an anonymous method you get CS1670 "params is not valid in this context"
				//DoParamsWork dpw = delegate (params object[] workItems)
                // Done as a lambda expression you get CS1525 "Invalida expression term 'params'"
                //DoParamsWork dpw = (params object[] workItems) =>
                //{
                //    foreach (object o in workItems)
                //    {
                //        Console.WriteLine(o.ToString());
                //        return o.GetHashCode();
                //    }
                //};

	            // All we have to do is omit the params keyword.
	            DoParamsWork dpw = workItems =>
	            {
	                foreach (object o in workItems)
	                {
	                    Console.WriteLine(o.ToString());
	                }
	                return workItems.GetHashCode();
	            };

                i = dpw("Hello", "42", "bar");

				// Work around params not being valid by using object[]
				// as it gives an unbounded number of method parameters
				// we just don't get the benefit of having the compiler 
				// create the object array for us implicitly
				DoNonParamsWork dnpw = (object[] items) =>
				{
					foreach (object o in items)
					{
						Console.WriteLine(o.ToString());
					}
					if (items.Length > 0)
						return items[0].GetHashCode();
					else
						return -1;
				};
				// invoke delegate
				i = dnpw(new object[]{"WorkItem1", 5, 65.99, true});
			}
		}

		class OldParams
		{
			// declare delegate
			delegate int DoWork(params string [] workItems);

			// have a method to create an instance of and call the delegate
			public void WorkItOut()
			{
				// declare instance
				DoWork dw = new DoWork(DoWorkMethodImpl);
				string[] items = new string[3];
				items[0] = "item 0";
				items[1] = "item 1";
				items[2] = "item 2";
				// invoke delegate
				int i = dw(items);

				items = new string[1];
				items[0] = "item 0";
				// invoke delegate
				i = dw(items);

			}

			// Have a method that the delegate is tied to with a matching signature 
			// so that it is invoked when the delegate is called
			public int DoWorkMethodImpl(params string [] items)
			{
				foreach (string s in items)
				{
					Console.WriteLine(s);
				}
				return items.GetHashCode();
			}
		}

		class OuterVariablesParameterModifiers
		{
			// declare delegate
			delegate int DoWork(string work);

			public void TestParams(params string[] items)
			{
				// declare instance
				DoWork dw = s =>
				{
					Console.WriteLine(s);
					foreach (string item in items)
					{
						Console.WriteLine(item);
					}
					return s.GetHashCode();
				};
				// invoke delegate
				int i = dw("DoWorkMethodImpl1");
			}

            //public void TestOut(out string outStr)
            //{
            //    // declare instance
            //    DoWork dw = s =>
            //    {
            //        Console.WriteLine(s);
            //        // Causes error CS1628: 
            //        // "Cannot use ref or out parameter 'outStr' inside an 
            //        // anonymous method, lambda expression, or query expression"
            //        //outStr = s;
            //        return s.GetHashCode();
            //    };
            //    // invoke delegate
            //    int i = dw("DoWorkMethodImpl1");
            //}

            //public void TestRef(ref string refStr)
            //{
            //    // declare instance
            //    DoWork dw = s =>
            //    {
            //        Console.WriteLine(s);
            //        // Causes error CS1628: 
            //        // "Cannot use ref or out parameter 'refStr' inside an 
            //        // anonymous method, lambda expression, or query expression"
            //        //refStr = s;
            //        return s.GetHashCode();
            //    };
            //    // invoke delegate
            //    int i = dw("DoWorkMethodImpl1");
            //}
		}

		#endregion

		#region "9.10 Gaining closures in C#"
		/// <summary>
		/// A class to represent the Sales Weasels of the world...
		/// </summary>
        class SalesWeasel
        {
	        #region CTOR
            public SalesWeasel()
            {
            }

	        public SalesWeasel(string name,
						        decimal annualQuota,
						        decimal commissionRate)
	        {
		        this.Name = name;
		        this.AnnualQuota = annualQuota;
		        this.CommissionRate = commissionRate;
	        }
	        #endregion //CTOR

            #region Private Members
            decimal _commission;
            #endregion Private Members

            #region Properties
            public string Name { get; set; }

	        public decimal AnnualQuota { get; set; }

	        public decimal CommissionRate { get; set; }

	        public decimal Commission 
	        {
		        get { return _commission; }
		        set
		        {
			        _commission = value;
			        this.TotalCommission += _commission;
		        }
	        }

	        public decimal TotalCommission {get; private set; }
	        #endregion // Properties
        }

		delegate void CalculateEarnings(SalesWeasel weasel);

		static CalculateEarnings GetEarningsCalculator(decimal quarterlySales,
														decimal bonusRate)
		{
			return salesWeasel =>
			{
				// figure out the weasel's quota for the quarter
                decimal quarterlyQuota = (salesWeasel.AnnualQuota / 4);
				// did they make quota for the quarter?
				if (quarterlySales < quarterlyQuota)
				{
					// didn't make quota, no commission
                    salesWeasel.Commission = 0;
				}
				// check for bonus level performance (200% of quota)
				else if (quarterlySales > (quarterlyQuota * 2.0m))
				{
                    decimal baseCommission = quarterlyQuota * salesWeasel.CommissionRate;
                    salesWeasel.Commission = (baseCommission +
							((quarterlySales - quarterlyQuota) *
                            (salesWeasel.CommissionRate * (1 + bonusRate))));
				}
				else // just regular commission
				{
                    salesWeasel.Commission = salesWeasel.CommissionRate * quarterlySales;
				}
			};
		}


        public class QuarterlyEarning
        {
            public string Name { get; set; }
            public decimal Earnings { get; set; }
            public decimal Rate { get; set; }
        }

		public static void TestClosure()
		{
            // set up the sales weasels...
            SalesWeasel[] weasels = {
                new SalesWeasel { Name="Chas", AnnualQuota=100000m, CommissionRate=0.10m },
                new SalesWeasel { Name="Ray", AnnualQuota=200000m, CommissionRate=0.025m },
                new SalesWeasel { Name="Biff", AnnualQuota=50000m, CommissionRate=0.001m }};

    
            QuarterlyEarning[] quarterlyEarnings = 
                           { new QuarterlyEarning(){ Name="Q1", Earnings = 65000m, Rate = 0.1m },
                             new QuarterlyEarning(){ Name="Q2", Earnings = 20000m, Rate = 0.1m },
                             new QuarterlyEarning(){ Name="Q3", Earnings = 37000m, Rate = 0.1m },
                             new QuarterlyEarning(){ Name="Q4", Earnings = 110000m, Rate = 0.15m } };

            var calculators = from e in quarterlyEarnings 
                              select new 
                              { 
                                  Calculator = 
                                      GetEarningsCalculator(e.Earnings, e.Rate), 
                                  QuarterlyEarning = e
                              };

            decimal annualEarnings = 0;
            foreach (var c in calculators)
            {
                WriteQuarterlyReport(c.QuarterlyEarning.Name, 
                    c.QuarterlyEarning.Earnings, c.Calculator, weasels);
                annualEarnings += c.QuarterlyEarning.Earnings;
            }

            // Let's see who is worth keeping...
            WriteCommissionReport(annualEarnings, weasels);

			//Console.ReadLine();
		}

		static void WriteQuarterlyReport(string quarter,
										decimal quarterlySales,
										CalculateEarnings eCalc,
										SalesWeasel[] weasels)
		{
			Console.WriteLine("{0} Sales Earnings on Quarterly Sales of {1}:",
				quarter, quarterlySales.ToString("C"));
			foreach (SalesWeasel weasel in weasels)
			{
				// calc commission
				eCalc(weasel);
				// report
				Console.WriteLine("	 SalesWeasel {0} made a commission of : {1}",
					weasel.Name, weasel.Commission.ToString("C"));
			}
		}

        static void WriteCommissionReport(decimal annualEarnings,
								        SalesWeasel[] weasels)
        {
	        decimal revenueProduced = ((annualEarnings) / weasels.Length);
	        Console.WriteLine("");
	        Console.WriteLine("Annual Earnings were {0}",
		        annualEarnings.ToString("C"));
	        Console.WriteLine("");
            var whoToCan = from weasel in weasels
                           select new
                           {
                               // if his commission is more than 20% 
                               // of what he produced can him
                               CanThem = (revenueProduced * 0.2m) <
                                           weasel.TotalCommission,
                               weasel.Name,
                               weasel.TotalCommission
                           };

	        foreach (var weaselInfo in whoToCan)
	        {
		        Console.WriteLine("    Paid {0} {1} to produce {2}",
                    weaselInfo.Name,
                    weaselInfo.TotalCommission.ToString("C"),
			        revenueProduced.ToString("C"));
                if (weaselInfo.CanThem)
		        {
			        Console.WriteLine("        FIRE {0}!", weaselInfo.Name);
		        }
	        }
        }
		#endregion

		#region "9.11 Performing Multiple Operations on a List using Functors"
		public static void TestFunctors()
		{
			// No, none of these are real tickers...
			// OU81
			// C#4VR
			// PCKD
			// BTML
			// NOVB
			// MGDCD
			// GNRCS
			// FNCTR
			// LMBDA
			// PCLS

            StockPortfolio tech = new StockPortfolio() {
                {"OU81", -10.5},
                {"C#4VR", 2.0},
                {"PCKD", 12.3},
                {"BTML", 0.5},
                {"NOVB", -35.2},
                {"MGDCD", 15.7},
                {"GNRCS", 4.0},
                {"FNCTR", 9.16},
                {"LMBDA", 9.12},
                {"PCLS", 6.11}};

            tech.PrintPortfolio("Starting Portfolio");
            // sell the worst 3 performers
            var worstPerformers = tech.GetWorstPerformers(3);
            Console.WriteLine("Selling the worst performers:");
            worstPerformers.DisplayStocks();
            tech.SellStocks(worstPerformers);
            tech.PrintPortfolio("After Selling Worst 3 Performers");

            //Output:
            //Starting Portfolio
            //  (OU81) lost 10.5%
            //  (C#4VR) gained 2%
            //  (PCKD) gained 12.3%
            //  (BTML) gained 0.5%
            //  (NOVB) lost 35.2%
            //  (MGDCD) gained 15.7%
            //  (GNRCS) gained 4%
            //  (FNCTR) gained 9.16%
            //  (LMBDA) gained 9.12%
            //  (PCLS) gained 6.11%
            //Selling the worst performers:
            //  (NOVB) lost 35.2%
            //  (OU81) lost 10.5%
            //  (BTML) gained 0.5%
            //After Selling Worst 3 Performers
            //  (C#4VR) gained 2%
            //  (PCKD) gained 12.3%
            //  (MGDCD) gained 15.7%
            //  (GNRCS) gained 4%
            //  (FNCTR) gained 9.16%
            //  (LMBDA) gained 9.12%
            //  (PCLS) gained 6.11%        
        }

        public class Stock
        {
            public double GainLoss { get; set; }
            public string Ticker { get; set; }
        }

        public static void DisplayStocks(this IEnumerable<Stock> stocks)
        {
            var gainLoss = from stock in stocks
                           select new
                           {
                               Result = stock.GainLoss < 0 ? "lost" : "gained",
                               stock.Ticker,
                               stock.GainLoss
                           };

            foreach (var s in gainLoss)
            {
                Console.WriteLine("  ({0}) {1} {2}%", s.Ticker, s.Result,
                    System.Math.Abs(s.GainLoss));
            }
        }

        public class StockPortfolio : IEnumerable<Stock>
        {
            List<Stock> _stocks;

            public StockPortfolio()
            {
                _stocks = new List<Stock>();
            }

            public void Add(string ticker, double gainLoss)
            {
                _stocks.Add(new Stock() {Ticker=ticker, GainLoss=gainLoss});
            }

            public IEnumerable<Stock> GetWorstPerformers(int topNumber)
            {
                return _stocks.OrderBy(
                            (Stock stock) => stock.GainLoss).Take(topNumber);
            }

            public void SellStocks(IEnumerable<Stock> stocks)
            {
                foreach(Stock s in stocks)
                    _stocks.Remove(s);
            }

            public void PrintPortfolio(string title)
            {
                Console.WriteLine(title);
                _stocks.DisplayStocks();
            }

            #region IEnumerable<Stock> Members

            public IEnumerator<Stock> GetEnumerator()
            {
                return _stocks.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }
        #endregion
    }
}
