using System;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSharpRecipes
{
	public class ThreadingSync
	{
        #region "18.1 Creating Per-Thread Static Fields"
        public static void PerThreadStatic()
        {
            TestStaticField();
        }

        public class Foo
        {
            [ThreadStaticAttribute()]
            public static string bar = "Initialized string";
        }

        private static void TestStaticField()
        {
            ThreadStaticField.DisplayStaticFieldValue();

            Thread newStaticFieldThread = 
                new Thread(ThreadStaticField.DisplayStaticFieldValue);

            newStaticFieldThread.Start();

            ThreadStaticField.DisplayStaticFieldValue();
        }

        private class ThreadStaticField
        {
            [ThreadStaticAttribute()]
            public static string bar = "Initialized string";

            public static void DisplayStaticFieldValue()
            {
                string msg = 
                    string.Format("{0} contains static field value of: {1}", 
                        Thread.CurrentThread.GetHashCode(),
                        ThreadStaticField.bar);
                Console.WriteLine(msg);
            }
        }


        #endregion

        #region "18.2 Providing Thread Safe Access To Class Members"
        public static void ThreadSafeAccess()
        {
            DeadLock deadLock = new DeadLock();
            lock(deadLock)
            {
                Thread thread = new Thread(deadLock.Method1);
                thread.Start();

                // Do some time consuming task here
            }

            int num = 0;
            if(Monitor.TryEnter(MonitorMethodAccess.SyncRoot,250))
            {
                MonitorMethodAccess.ModifyNumericField(10);
                num = MonitorMethodAccess.ReadNumericField();
                Monitor.Exit(MonitorMethodAccess.SyncRoot);
            }
            Console.WriteLine(num);

        }

        public static class NoSafeMemberAccess
        {
            private static int numericField = 1;

            public static void IncrementNumericField() 
            {
                ++numericField;
            }

            public static void ModifyNumericField(int newValue) 
            {
                numericField = newValue;
            }

            public static int ReadNumericField()
            {
                return (numericField);
            }
        }

        public static class SaferMemberAccess
        {
            private static int numericField = 1;
            private static object syncObj = new object();

            public static void IncrementNumericField() 
            {
                lock(syncObj)
                {
                    ++numericField;
                }
            }

            public static void ModifyNumericField(int newValue) 
            {
                lock (syncObj)
                {
                    numericField = newValue;
                }
            }

            public static int ReadNumericField()
            {
                lock (syncObj)
                {
                    return (numericField);
                }
            }
        }

        public class DeadLock
        {
            private object syncObj = new object();

            public void Method1()
            {
                lock(syncObj)
                {
                    // Do something
                }
            }
        }

        public static class MonitorMethodAccess
        {
            private static int numericField = 1;
            private static object syncObj = new object();

            public static object SyncRoot
            {
                get { return syncObj; }
            }

            public static void IncrementNumericField() 
            {
                if (Monitor.TryEnter(syncObj, 250))
                {
                    try
                    {
                        ++numericField;
                    }
                    finally
                    {
                        Monitor.Exit(syncObj);
                    }
                }
            }

            public static void ModifyNumericField(int newValue) 
            {
                if (Monitor.TryEnter(syncObj, 250))
                {
                    try
                    {
                        numericField = newValue;
                    }
                    finally
                    {
                        Monitor.Exit(syncObj);
                    }
                }
            }

            public static int ReadNumericField()
            {
                if (Monitor.TryEnter(syncObj, 250))
                {
                    try
                    {
                        return (numericField);
                    }
                    finally
                    {
                        Monitor.Exit(syncObj);
                    }
                }

                return (-1);
            }
            [MethodImpl (MethodImplOptions.Synchronized)]
            public static void MySynchronizedMethod()
            {
            }

        }

        
        #endregion

        #region "18.3 Preventing Silent Thread Termination"
        public static void PreventSilentTermination()
        {
            MainThread mt = new MainThread();
            mt.CreateNewThread();
        }

        public class MainThread
        {
            public void CreateNewThread()
            {
                // Spawn new thread to do concurrent work
                Thread newWorkerThread = new Thread(Worker.DoWork);
                newWorkerThread.Start();
            }
        }

        public class Worker
        {
            // Method called by ThreadStart delegate to do concurrent work
            public static void DoWork ()
            {
                try
                {
                    // Do thread work here
                    throw new Exception("Boom!");
                }
                catch(Exception e) 
                {
                    // Handle thread exception here
                    Console.WriteLine(e.ToString());
                    // Do not re-throw exception
                }
                finally
                {
                    // Do thread cleanup here
                }
            }
        }
        #endregion

        #region "18.4 Being Notified of the Completion of an Asynchronous Delegate"
        public static void CompletionAsyncDelegate()
        {
            AsyncAction2 aa2 = new AsyncAction2();
            aa2.CallbackAsyncDelegate();
        }

        public delegate int AsyncInvoke();
        
        public class TestAsyncInvoke
        {
            public static int Method1()
            {
                Console.WriteLine("Invoked Method1 on Thread {0}",
                    Thread.CurrentThread.ManagedThreadId);
                return (1);
            }
        }

        public class AsyncAction2
        {
            public void CallbackAsyncDelegate()
            {
                AsyncCallback callBack = DelegateCallback;

                AsyncInvoke method1 = TestAsyncInvoke.Method1;
                Console.WriteLine("Calling BeginInvoke on Thread {0}",
                    Thread.CurrentThread.ManagedThreadId);
                IAsyncResult asyncResult = method1.BeginInvoke(callBack, method1);

                // No need to poll or use the WaitOne method here, so return to the calling method.
                return;
            }

            private static void DelegateCallback(IAsyncResult iresult)
            {
                Console.WriteLine("Getting callback on Thread {0}",
                    Thread.CurrentThread.ManagedThreadId);
                AsyncResult asyncResult = (AsyncResult)iresult;
                AsyncInvoke method1 = (AsyncInvoke)asyncResult.AsyncDelegate;

                int retVal = method1.EndInvoke(asyncResult);
                Console.WriteLine("retVal (Callback): " + retVal);
            }
        }

        public delegate int AsyncInvoke2();

        public class TestAsyncInvoke2
        {
            public static int Method1()
            {
                Console.WriteLine("Invoked Method1 on Thread {0}",
                    Thread.CurrentThread.ManagedThreadId);
                return (1);
            }
        }

        #endregion

        #region "18.5 Storing Thread Specific Data Privately"
        public static void StoreThreadDataPrivately()
        {
            HandleClass.Run();
        }

        public class ApplicationData
        {
            // Application data is stored here
            public int Data { get; set; }
        }

        public class HandleClass
        {
            public static void Run()
            {
                // Create structure instance and store it in the named data slot
                ApplicationData appData = new ApplicationData();
                Thread.SetData(Thread.GetNamedDataSlot("appDataSlot"), appData);

                // Call another method that will use this structure
                HandleClass.MethodB();

                // When done, free this data slot
                Thread.FreeNamedDataSlot("appDataSlot");
            }

            public static void MethodB()
            {
                // Get the instance from the named data slot
                ApplicationData storedAppData = (ApplicationData)Thread.GetData(
                    Thread.GetNamedDataSlot("appDataSlot"));

                // Modify the ApplicationData 

                // When finished modifying this data, store the changes back into
                // into the named data slot
                Thread.SetData(Thread.GetNamedDataSlot("appDataSlot"), 
                    storedAppData);

                // Call another method that will use this structure
                HandleClass.MethodC();
            }

            public static void MethodC()
            {
                // Get the instance from the named data slot
                ApplicationData storedAppData = 
                    (ApplicationData)Thread.GetData(Thread.GetNamedDataSlot("appDataSlot"));

                // Modify the data

                // When finished modifying this data, store the changes back into
                // the named data slot
                Thread.SetData(Thread.GetNamedDataSlot("appDataSlot"), storedAppData);
            }
        }

        #endregion

		#region "18.6 Granting multiple access to resources with a Semaphore"
		// Xbox360 with 4 ports, group of software developers want access.  4 players get access initially
		// Players die after random time, new players pop up in queue after waiting on semaphore

        public class Xbox360Player
        {
	        public class PlayerInfo
	        {
		        public ManualResetEvent Dead {get; set;}
		        public string Name {get; set;}
	        }

			// Death Modes for Players
			private static string[] _deaths = new string[7]{"bought the farm",
																"choked on a rocket",
																"shot their own foot",
																"was captured",
																"fallen to their death",
																"died of lead poisoning",
																"failed to dodge a genrade",
																};

			/// <summary>
			/// Thread function
			/// </summary>
			/// <param name="info">Xbox360Player.Data item with Xbox reference and handle</param>
            public static void JoinIn(object info)
            {
	            // open up the semaphore by name so we can act on it
                using (Semaphore Xbox360 = Semaphore.OpenExisting("Xbox360"))
                {

                    // get the data object
                    PlayerInfo player = (PlayerInfo)info;

                    // Each player notifies the Xbox360 they want to play
                    Console.WriteLine("{0} is waiting to play!", player.Name);

                    // they wait on the Xbox360 (semaphore) until it lets them
                    // have a controller
                    Xbox360.WaitOne();

                    // The Xbox360 has chosen the player! (or the semaphore has 
                    // allowed access to the resource...)
                    Console.WriteLine("{0} has been chosen to play. " +
                        "Welcome to your doom {0}. >:)", player.Name);

                    // figure out a random value for how long the player lasts
                    System.Random rand = new Random(500);
                    int timeTillDeath = rand.Next(100, 1000);

                    // simulate the player is busy playing till they die
                    Thread.Sleep(timeTillDeath);

                    // figure out how they died
                    rand = new Random();
                    int deathIndex = rand.Next(6);

                    // notify of the player's passing
                    Console.WriteLine("{0} has {1} and gives way to another player",
                        player.Name, _deaths[deathIndex]);

                    // if all ports are open, everyone has played and the game is over
                    int semaphoreCount = Xbox360.Release();
                    if (semaphoreCount == 3)
                    {
                        Console.WriteLine("Thank you for playing, the game has ended.");
                        // set the Dead event for the player
                        player.Dead.Set();
                        // close out the semaphore
                        Xbox360.Close();
                    }
                }
            }
		}		
 
        public class Halo3Session
        {
	        // A semaphore that simulates a limited resource pool.
	        private static Semaphore _Xbox360;

            public static void Play()
            {
	            // An Xbox360 has 4 controller ports so 4 people can play at a time
	            // We use 4 as the max an zero to start with as we want Players
	            // to queue up at first until the Xbox360 boots and loads the game
	            //
                using (_Xbox360 = new Semaphore(0, 4, "Xbox360"))
                {
                    using (ManualResetEvent GameOver =
                        new ManualResetEvent(false))
                    {
                        //
                        // 9 Players log in to play
                        //
                        List<Xbox360Player.PlayerInfo> players =
                            new List<Xbox360Player.PlayerInfo>() {
                                new Xbox360Player.PlayerInfo { Name="Igor", Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="AxeMan", Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="Dr. Death",Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="HaPpyCaMpEr",Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="Executioner",Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="FragMan",Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="Beatdown",Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="Stoney",Dead=GameOver},
                                new Xbox360Player.PlayerInfo { Name="Pwned",Dead=GameOver}
                                };
                             
                        foreach (Xbox360Player.PlayerInfo player in players)
                        {
                            Thread t = new Thread(Xbox360Player.JoinIn);

                            // put a name on the thread
                            t.Name = player.Name;
                            // fire up the player
                            t.Start(player);
                        }

                        // Wait for the Xbox360 to spin up and load Halo3 (3 seconds)
                        Console.WriteLine("Xbox360 initializing...");
                        Thread.Sleep(3000);
                        Console.WriteLine(
                            "Halo3 loaded & ready, allowing 4 players in now...");

                        // The Xbox360 has the whole semaphore count.  We call 
                        // Release(4) to open up 4 slots and
                        // allows the waiting players to enter the Xbox360(semaphore)
                        // up to four at a time.
                        //
                        _Xbox360.Release(4);

                        // wait for the game to end...
                        GameOver.WaitOne();
                    }
                }
            }
		}	
		#endregion

		#region "18.7 Synchronizing multiple processes with the Mutex"
		// see the MutexFun project
		#endregion

		#region "18.8 Using events to make threads cooperate"
		public static void TestResetEvent()
		{
			// We have a diner with a cook who can only serve up one meal at a time 
			Cook Mel = new Cook();

			// Make up 5 waitresses and tell them to get orders
			for (int i = 0; i < 5; i++)
			{
				Thread t = new Thread(Waitress.PlaceOrder);
				// The Waitress places the order and then waits for the order
				t.Start(Cook.OrderReady);
			}

			// now we can go through and let people in
			for (int i = 0; i < 5; i++)
			{
				// make the waitressess wait...
				Thread.Sleep(2000);
				// ok, next waitress, pickup!
				Mel.CallWaitress();
			}
		}

		public class Cook
		{
			public static AutoResetEvent OrderReady = new AutoResetEvent(false);

			public void CallWaitress()
			{
				// we call Set on the AutoResetEvent and don't have to 
				// call Reset like we would with ManualResetEvent to fire it 
				// off again.  This sets the event that the waitress is waiting for
				// in GetInLine
				OrderReady.Set();
			}
		}

		public class Waitress
		{
			public static void PlaceOrder(object signal)
			{
				// cast the AutoResetEvent so the waitress can wait for the
				// order to be ready
				AutoResetEvent OrderReady = (AutoResetEvent)signal;
				// wait for the order...
				OrderReady.WaitOne();
				// order is ready....
				Console.WriteLine("Waitress got order!");
			}
		}

		#endregion

		#region "18.9 Get the naming rights for your events"
		public static void TestManualNamedEvent()
		{
            // make a named manual reset event
            EventWaitHandle ewhSuperBowl = 
                new EventWaitHandle(false, // not initially signalled
                                    EventResetMode.ManualReset, 
                                    @"Champs");

            // spin up three threads to listen for the event
            for (int i = 0; i < 3; i++)
            {
	            Thread t = new Thread(ManualFan);
	            // The fans wait anxiously...
                t.Name = "Fan " + i;
	            t.Start();
            }
            // play the game
            Thread.Sleep(10000);
            // notify people
            Console.WriteLine("Patriots win the SuperBowl!");
            // signal all fans
            ewhSuperBowl.Set();
            // close the event
            ewhSuperBowl.Close();

		}

        public static void TestAutoNamedEvent()
        {
            // make a named auto reset event
            EventWaitHandle ewhSuperBowl =
                new EventWaitHandle(false, // not initially signalled
                                    EventResetMode.AutoReset,
                                    @"Champs");

            // spin up three threads to listen for the event
            for (int i = 0; i < 3; i++)
            {
                Thread t = new Thread(AutoFan, i);
                // The fans wait anxiously...
                t.Name = "Fan " + i;
                t.Start();
            }
            // play the game
            Thread.Sleep(10000);
            // notify people
            Console.WriteLine("Patriots win the SuperBowl!");
            // signal 1 fan at a time
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Notify fans");
                ewhSuperBowl.Set();
            }
            // close the event
            ewhSuperBowl.Close();
        }


        public static void ManualFan()
        {
	        // open the event by name 
            EventWaitHandle ewhSuperBowl = 
                new EventWaitHandle(false, 
                                    EventResetMode.ManualReset, 
                                    @"Champs");

	        // wait for the signal
            ewhSuperBowl.WaitOne();
	        // shout out
	        Console.WriteLine("\"They're great!\" says {0}",Thread.CurrentThread.Name);
	        // close the event
            ewhSuperBowl.Close();
        }

        public static void AutoFan()
        {
            // open the event by name 
            EventWaitHandle ewhSuperBowl =
                new EventWaitHandle(false,
                                    EventResetMode.ManualReset,
                                    @"Champs");

            // wait for the signal
            ewhSuperBowl.WaitOne();
            // shout out
            Console.WriteLine("\"Yahoo!\" says {0}", Thread.CurrentThread.Name);
            // close the event
            ewhSuperBowl.Close();
        }
		#endregion

		#region "18.10 Performing atomic operations amongst threads"
		public static void TestInterlocked()
		{
			int i = 0;
			long l = 0;
			Interlocked.Increment(ref i); // i = 1
			Interlocked.Decrement(ref i); // i = 0
			Interlocked.Increment(ref l); // l = 1
			Interlocked.Decrement(ref i); // l = 0

			Interlocked.Add(ref i, 10); // i = 10;
			Interlocked.Add(ref l, 100); // l = 100;

			string name = "Mr. Ed";
			Interlocked.Exchange(ref name, "Barney");

			double runningTotal = 0.0;
			double startingTotal = 0.0;
			double calc = 0.0;
			for (i = 0; i < 10; i++)
			{
				do
				{
					// store of the original total
					startingTotal = runningTotal;
					// do an intense calculation
					calc = runningTotal + i * Math.PI * 2 / Math.PI;
				}
				// check to make sure runningTotal wasn't modified
				// and replace it with calc if not.  If it was, 
				// run through the loop until we get it current
				while (startingTotal !=
					Interlocked.CompareExchange(
						ref runningTotal, calc, startingTotal));
			}

			// have one less credible auction
			Auction Shadys = new Auction("Shady's");
			// have one credible auction
			Auction UpperCrust = new Auction("UpperCrust");

			// Make up 3 "reckless" small bidders, 3 small "safe" bidders
			// Make up 3 "reckless" big bidders, 3 big "safe" bidders
			Thread [] handles = new Thread[12];
			for (i = 0; i < 12; i++)
			{
				Thread t = null;
				switch(i)
				{
					// small time reckless bidders
					case 0:
					case 1:
					case 2:
					{
						t = new Thread(new ThreadStart(Shadys.Bid));			
					}
					break;
					// small time safe bidders
					case 3:
					case 4:
					case 5:
					{
						t = new Thread(new ThreadStart(UpperCrust.BidSafe));			
					}
					break;
					// big time reckless bidders
					case 6:
					case 7:
					case 8:
					{
						t = new Thread(new ThreadStart(Shadys.BigBid));			
					}
					break;
					// big time safe bidders
					case 9:
					case 10:
					case 11:
					{
						t = new Thread(new ThreadStart(UpperCrust.BigBidSafe));			
					}
					break;
				}
				// store the thread reference for later use
				handles[i] = t;

				// Start up the thread
				t.Start();
			}

			// join to each thread so we wait for all to finish
			for(i=0;i<12;i++)
				handles[i].Join();

			Console.WriteLine(
				"Shady's Auction House had {0} bids for a total of ${1} dollars",
				Shadys.BidCount,Shadys.BidTotal);
			Console.WriteLine(
				"UpperCrust Auction House had {0} bids for a total of ${1} dollars",
				UpperCrust.BidCount,UpperCrust.BidTotal);
		}

		/// <summary>
		/// Class to simulate an auction house
		/// </summary>
		public class Auction
		{
			#region Public Data
            private int _bidCount;
            private long _bidTotal;

			// number of bids taken
			public int BidCount 
            { 
                get { return _bidCount; }
                set { _bidCount = value; }
            } 
			// total amount of bids
			public long BidTotal 
            { 
                get { return _bidTotal; }
                set { _bidTotal = value; }
            } 
			#endregion

			#region Private Data
			// name of auction house
			private string _name;
			#endregion

			#region CTOR
			/// <summary>
			/// Constructor that takes the name
			/// </summary>
			/// <param name="name">name of the house</param>
			public Auction(string name)
			{
				_name = name;
			}
			#endregion

			#region Private Methods
			/// <summary>
			/// Wait for a random period of time from 0 - 2 seconds
			/// </summary>
			private void Wait()
			{
				// wait some random time to do work
				Random r = new Random();
				int milli = r.Next(2000);
				Thread.Sleep(milli);
			}
			#endregion

			#region Public Thread Methods
			public void Bid()
			{
				// wait some random time to do work
				Wait();
				
				// increase the bid by 1 without regard to 
				// thread safety
				BidCount++;
				BidTotal++;

				// notify of the bid
				Console.WriteLine(_name + " Bid by thread " + Thread.CurrentThread.ManagedThreadId);
			}

			public void BidSafe()
			{
				// wait some random time to do work
				Wait();
				
				// increase the bid by 1 taking threading into account
				Interlocked.Increment(ref _bidCount);
				Interlocked.Increment(ref _bidTotal);

				// notify of the bid
				Console.WriteLine(_name + " BidSafe by thread " + Thread.CurrentThread.ManagedThreadId);
			}

			public void BigBid()
			{
				// wait some random time to do work
				Wait();

				// increase the bid by 10 without regard to 
				// thread safety
				BidCount += 1;
				BidTotal += 10;

				// notify of the bid
				Console.WriteLine(_name + " BigBid by thread " + Thread.CurrentThread.ManagedThreadId);
			}

			public void BigBidSafe()
			{
				// wait some random time to do work
				Wait();

				// increase the bid by 10 taking threading into account
				Interlocked.Add(ref _bidCount, 1);
				Interlocked.Add(ref _bidTotal,10);
				
				// notify of the bid
				Console.WriteLine(_name + " BigBidSafe by thread " + Thread.CurrentThread.ManagedThreadId);
			}
			#endregion
		}
		#endregion

        #region "18.11 Optimizing Read-Mostly Access"

        static Developer _dev = new Developer(15);
        static bool _end = false;

        /// <summary>
        /// </summary>
        public static void TestReaderWriterLockSlim()
        {
            LaunchTeam(_dev);
            Thread.Sleep(10000);
        }

        private static void LaunchTeam(Developer dev)
        {
            LaunchManager("CTO", dev);
            LaunchManager("Director", dev);
            LaunchManager("Project Manager", dev);
            LaunchDependent("Product Manager", dev);
            LaunchDependent("Test Engineer", dev);
            LaunchDependent("Technical Communications Professional", dev);
            LaunchDependent("Operations Staff", dev);
            LaunchDependent("Support Staff", dev);
        }

        public class TaskInfo
        {
            private Developer _dev;
            public string Name { get; set; }
            public Developer Developer
            {
                get { return _dev; }
                set { _dev = value; }
            }
        }

        private static void LaunchManager(string name, Developer dev)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CreateManagerOnThread),
                new TaskInfo() { Name = name, Developer = dev });
        }

        private static void LaunchDependent(string name, Developer dev)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CreateDependentOnThread),
                new TaskInfo() { Name = name, Developer = dev });
        }

        private static void CreateManagerOnThread(object objInfo)
        {
            TaskInfo taskInfo = (TaskInfo)objInfo;
            Console.WriteLine("Added " + taskInfo.Name + " to the project...");
            TaskManager mgr = new TaskManager(taskInfo.Name, taskInfo.Developer);
        }

        private static void CreateDependentOnThread(object objInfo)
        {
            TaskInfo taskInfo = (TaskInfo)objInfo;
            Console.WriteLine("Added " + taskInfo.Name + " to the project...");
            TaskDependent dep = new TaskDependent(taskInfo.Name, taskInfo.Developer);
        }

        public class Task 
        {
            public Task(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
            public int Priority { get; set; }
            public bool Status { get; set; }

            public override string ToString()
            {
                return this.Name;
            }

            public override bool Equals(object obj)
            {
                Task task = obj as Task;
                if(task != null)
                    return this.Name == task.Name;
                return false;
            }

            public override int GetHashCode()
            {
                return this.Name.GetHashCode();
            }
        }

        public class Developer
        {
            /// <summary>
            /// Dictionary for the tasks
            /// </summary>
            private List<Task> _tasks = new List<Task>();
            private ReaderWriterLockSlim _rwlSlim = new ReaderWriterLockSlim();
            private System.Threading.Timer _timer;
            private int _maxTasks;

            public Developer(int maxTasks)
            {
                // the maximum number of tasks before the developer quits
                _maxTasks = maxTasks;
                // do some work every 1/4 second
                _timer = new Timer(new TimerCallback(DoWork), null, 1000, 250);            
            }

            ~Developer()
            {
                _timer.Dispose();
            }


            // Execute a task
            protected void DoWork(Object stateInfo)
            {
                ExecuteTask();
                try
                {
                    _rwlSlim.EnterWriteLock();
                    // if we finished all tasks, go on vacation!
                    if (_tasks.Count == 0)
                    {
                        _end = true;
                        Console.WriteLine("Developer finished all tasks, go on vacation!");
                        return;
                    }

                    if (!_end)
                    {
                        // if we have too many tasks quit
                        if (_tasks.Count > _maxTasks)
                        {
                            // get the number of unfinished tasks
                            var query = from t in _tasks
                                        where t.Status == false
                                        select t;
                            int unfinishedTaskCount = query.Count<Task>();

                            _end = true;
                            Console.WriteLine("Developer has too many tasks, quitting! " +
                                unfinishedTaskCount + " tasks left unfinished.");
                        }
                    }
                    else
                        _timer.Dispose();
                }
                finally
                {
                    _rwlSlim.ExitWriteLock();
                }
            }

            public void AddTask(Task newTask)
            {
                try
                {
                    _rwlSlim.EnterWriteLock();
                    // if we already have this task (unique by name) 
                    // then just accept the add as sometimes people
                    // give you the same task more than once :)
                    var taskQuery = from t in _tasks
                                    where t == newTask
                                    select t;
                    if (taskQuery.Count<Task>() == 0)
                    {
                        Console.WriteLine("Task " + newTask.Name + " was added to developer");
                        _tasks.Add(newTask);
                    }
                }
                finally
                {
                    _rwlSlim.ExitWriteLock();
                }
            }

            /// <summary>
            /// Increase the priority of the task
            /// </summary>
            /// <param name="taskName">name of the task</param>
            public void IncreasePriority(string taskName)
            {
                try
                {
                    _rwlSlim.EnterUpgradeableReadLock();
                    var taskQuery = from t in _tasks
                                    where t.Name == taskName
                                    select t;
                    if(taskQuery.Count<Task>()>0)
                    {
                        Task task = taskQuery.First<Task>();
                        _rwlSlim.EnterWriteLock();
                        task.Priority++;
                        Console.WriteLine("Task " + task.Name + 
                            " priority was increased to " + task.Priority + 
                            " for developer");
                        _rwlSlim.ExitWriteLock();
                    }
                }
                finally
                {
                    _rwlSlim.ExitUpgradeableReadLock();
                }
            }

            /// <summary>
            /// Allows people to check if the task is done
            /// </summary>
            /// <param name="taskName">name of the task</param>
            /// <returns>False if the taks is undone or not in the list, true if done</returns>
            public bool IsTaskDone(string taskName)
            {
                try
                {
                    _rwlSlim.EnterReadLock();
                    var taskQuery = from t in _tasks
                                    where t.Name == taskName
                                    select t;
                    if (taskQuery.Count<Task>() > 0)
                    {
                        Task task = taskQuery.First<Task>();
                        Console.WriteLine("Task " + task.Name + " status was reported.");
                        return task.Status;
                    }
                }
                finally
                {
                    _rwlSlim.ExitReadLock();
                }
                return false;
            }

            private void ExecuteTask()
            {
                // look over the tasks and do the highest priority 
                var queryResult =   from t in _tasks
                                    where t.Status == false
                                    orderby t.Priority
                                    select t;
                if (queryResult.Count<Task>() > 0)
                {
                    // do the task
                    Task task = queryResult.First<Task>();
                    task.Status = true;
                    task.Priority = -1;
                    Console.WriteLine("Task " + task.Name + " executed by developer.");
                }
            }
        }

        public class TaskManager : TaskDependent
        {
            private System.Threading.Timer _mgrTimer;

            public TaskManager(string name, Developer taskExecutor) :
                base(name, taskExecutor)
            {
                // intervene every 2 seconds
                _mgrTimer = new Timer(new TimerCallback(Intervene), null, 0, 2000);
            }

            ~TaskManager()
            {
                _mgrTimer.Dispose();
            }

            // Intervene in the plan
            protected void Intervene(Object stateInfo)
            {
                ChangePriority();
                // developer ended, kill timer
                if (_end)
                {
                    _mgrTimer.Dispose();
                    _developer = null;
                }
            }

            public void ChangePriority()
            {
                if (_tasks.Count > 0)
                {
                    int taskIndex = _rnd.Next(0, _tasks.Count - 1);
                    Task checkTask = _tasks[taskIndex];
                    // make those developers work faster on some random task!
                    if (_developer != null)
                    {
                        _developer.IncreasePriority(checkTask.Name);
                        Console.WriteLine(Name + " intervened and changed priority for task " + checkTask.Name);
                    }
                }
            }
        }

        public class TaskDependent
        {
            protected List<Task> _tasks = new List<Task>();
            protected Developer _developer;
            protected Random _rnd = new Random();
            private Timer _taskTimer;
            private Timer _statusTimer;

            public TaskDependent(string name, Developer taskExecutor)
            {
                Name = name;
                _developer = taskExecutor;
                // add work every 1 second
                _taskTimer = new Timer(new TimerCallback(AddWork), null, 0, 1000);
                // check status every 3 seconds
                _statusTimer = new Timer(new TimerCallback(CheckStatus), null, 0, 3000);
            }

            ~TaskDependent()
            {
                _taskTimer.Dispose();
                _statusTimer.Dispose();
            }

            // Add more work to the developer
            protected void AddWork(Object stateInfo)
            {
                SubmitTask();
                // developer ended, kill timer
                if (_end)
                {
                    _taskTimer.Dispose();
                    _developer = null;
                }
            }

            // Check Status of work with the developer
            protected void CheckStatus(Object stateInfo)
            {
                CheckTaskStatus();
                // developer ended, kill timer
                if (_end)
                {
                    _statusTimer.Dispose();
                    _developer = null;
                }
            }

            public string Name { get; set; }

            public void SubmitTask()
            {
                int taskId = _rnd.Next(10000);
                string taskName = "(" + taskId + " for " + Name + ")";
                Task newTask = new Task(taskName);
                if (_developer != null)
                {
                    _developer.AddTask(newTask);
                    _tasks.Add(newTask);
                }
            }

            public void CheckTaskStatus()
            {
                if (_tasks.Count > 0)
                {
                    int taskIndex = _rnd.Next(0, _tasks.Count - 1);
                    Task checkTask = _tasks[taskIndex];
                    if (_developer != null &&
                        _developer.IsTaskDone(checkTask.Name))
                    {
                        Console.WriteLine("Task " + checkTask.Name + " is done for " + Name);
                        // remove it from the todo list
                        _tasks.Remove(checkTask);
                    }
                }
            }
        }

        #endregion // "18.11 - Optimizing Read-Mostly Access"
    }
}
