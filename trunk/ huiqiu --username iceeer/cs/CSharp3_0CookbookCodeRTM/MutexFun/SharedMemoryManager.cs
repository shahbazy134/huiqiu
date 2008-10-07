using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace MutexFun
{
    /// <summary>
    /// Class for sending objects through shared memory using a mutex
    /// to synchronize access to the shared memory
    /// </summary>
    public class SharedMemoryManager<TransferItemType> : IDisposable 
    {
	    #region Consts
	    const int INVALID_HANDLE_VALUE = -1;
	    const int FILE_MAP_WRITE = 0x0002;
	    /// <summary>
	    /// Define from Win32 API
	    /// </summary>
	    const int ERROR_ALREADY_EXISTS = 183;
	    #endregion

	    #region Private members
	    IntPtr _handleFileMapping = IntPtr.Zero;
	    IntPtr _ptrToMemory = IntPtr.Zero;
	    uint _memRegionSize = 0;
	    string _memoryRegionName;
        bool disposed = false;
        int _sharedMemoryBaseSize = 0;
	    Mutex _mtxSharedMem = null;

	    #endregion

        #region Construction / Cleanup
        public SharedMemoryManager(string name,int sharedMemoryBaseSize)
        {
            // can only be built for seralizable objects 
            if (!typeof(TransferItemType).IsSerializable)
                throw new ArgumentException(
                    string.Format("Object {0} is not serializeable.",
                        typeof(TransferItemType)));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (sharedMemoryBaseSize <= 0)
                throw new ArgumentOutOfRangeException("sharedMemoryBaseSize",
                    "Shared Memory Base Size must be a value greater than zero");

            // set name of the region
            _memoryRegionName = name;
            // save base size
            _sharedMemoryBaseSize = sharedMemoryBaseSize;
            // set up the memory region size
            _memRegionSize = (uint)(_sharedMemoryBaseSize + sizeof(int));
            // set up the shared memory section
            SetupSharedMemory();
        }

        private void SetupSharedMemory()
        {
            // Grab some storage from the page file
            _handleFileMapping =
                PInvoke.CreateFileMapping((IntPtr)INVALID_HANDLE_VALUE,
                                    IntPtr.Zero,
                                    PInvoke.PageProtection.ReadWrite,
                                    0,
                                    _memRegionSize,
                                    _memoryRegionName);

            if (_handleFileMapping == IntPtr.Zero)
            {
                throw new Win32Exception(
                    "Could not create file mapping");
            }

            // check the error status
            int retVal = Marshal.GetLastWin32Error();
            if (retVal == ERROR_ALREADY_EXISTS)
            {
                // we opened one that already existed
                // make the mutex not the initial owner
                // of the mutex since we are connecting 
                // to an existing one
                _mtxSharedMem = new Mutex(false,
                    string.Format("{0}mtx{1}", 
                        typeof(TransferItemType), _memoryRegionName));
            }
            else if (retVal == 0)
            {
                // we opened a new one
                // make the mutex the initial owner
                _mtxSharedMem = new Mutex(true,
                    string.Format("{0}mtx{1}", 
                        typeof(TransferItemType), _memoryRegionName));
            }
            else
            {
                // something else went wrong..
                throw new Win32Exception(retVal, "Error creating file mapping");
            }

            // map the shared memory
            _ptrToMemory = PInvoke.MapViewOfFile(_handleFileMapping,
                                            FILE_MAP_WRITE,
                                            0, 0, IntPtr.Zero);

            if (_ptrToMemory == IntPtr.Zero)
            {
                retVal = Marshal.GetLastWin32Error();
                throw new Win32Exception(retVal, "Could not map file view");
            }

            retVal = Marshal.GetLastWin32Error();
            if (retVal != 0 && retVal != ERROR_ALREADY_EXISTS)
            {
                // something else went wrong..
                throw new Win32Exception(retVal, "Error mapping file view");
            }
        }

	    ~SharedMemoryManager()
	    {
		    // make sure we close
		    Dispose(false);
	    }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                CloseSharedMemory();
            }
            disposed = true;
        }

        private void CloseSharedMemory()
        {
            if (_ptrToMemory != IntPtr.Zero)
            {
                // close map for shared memory
                PInvoke.UnmapViewOfFile(_ptrToMemory);
                _ptrToMemory = IntPtr.Zero;
            }
            if (_handleFileMapping != IntPtr.Zero)
            {
                // close handle 
                PInvoke.CloseHandle(_handleFileMapping);
                _handleFileMapping = IntPtr.Zero;
            }
        }

        public void Close()
        {
            CloseSharedMemory();
        }
        #endregion

        #region Properties
        public int SharedMemoryBaseSize
        {
            get { return _sharedMemoryBaseSize; } 
        }
        #endregion

        #region Public Methods
        /// <summary>
	    /// Send a serializeable object through the shared memory
	    /// and wait for it to be picked up
	    /// </summary>
	    /// <param name="transferObject"></param>
        public void SendObject(TransferItemType transferObject)
        {
	        // create a memory stream, initialize size
            using (MemoryStream ms = new MemoryStream())
            {
                // get a formatter to serialize with
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    // serialize the object to the stream
                    formatter.Serialize(ms, transferObject);

                    // get the bytes for the serialized object 
                    byte[] bytes = ms.ToArray();

                    // check that this object will fit
                    if(bytes.Length + sizeof(int) > _memRegionSize)
                    {
                        string fmt = 
                            "{0} object instance serialized to {1} bytes " +
                            "which is too large for the shared memory region";
                        
                        string msg = 
                            string.Format(fmt,
                                typeof(TransferItemType),bytes.Length);

                        throw new ArgumentException(msg, "transferObject");
                    }

                    // write out how long this object is
                    Marshal.WriteInt32(this._ptrToMemory, bytes.Length);

                    // write out the bytes
                    Marshal.Copy(bytes, 0, this._ptrToMemory, bytes.Length);
                }
                finally
                {
                    // signal the other process using the mutex to tell it
                    // to do receive processing
                    _mtxSharedMem.ReleaseMutex();

                    // wait for the other process to signal it has received
                    // and we can move on
                    _mtxSharedMem.WaitOne();
                }
            }
        }

	    /// <summary>
	    /// Wait for an object to hit the shared memory and then deserialize it
	    /// </summary>
	    /// <returns>object passed</returns>
        public TransferItemType ReceiveObject()
        {
	        // wait on the mutex for an object to be queued by the sender
	        _mtxSharedMem.WaitOne();

	        // get the count of what is in the shared memory
	        int count = Marshal.ReadInt32(_ptrToMemory);
	        if (count <= 0)
	        {
		        throw new InvalidDataException("No object to read");
	        }

	        // make an array to hold the bytes
	        byte[] bytes = new byte[count];

	        // read out the bytes for the object
            Marshal.Copy(_ptrToMemory, bytes, 0, count);

	        // set up the memory stream with the object bytes
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                // set up a binary formatter
                BinaryFormatter formatter = new BinaryFormatter();

                // get the object to return
                TransferItemType item;
                try
                {
                    item = (TransferItemType)formatter.Deserialize(ms);
                }
                finally
                {
                    // signal that we received the object using the mutex
                    _mtxSharedMem.ReleaseMutex();
                }
                // give them the object
                return item;
            }
        }
	    #endregion
    }

    public class PInvoke
    {
        #region PInvoke defines
        [Flags]
        public enum PageProtection : uint
        {
            NoAccess = 0x01,
            Readonly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            Guard = 0x100,
            NoCache = 0x200,
            WriteCombine = 0x400,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile,
            IntPtr lpFileMappingAttributes, PageProtection flProtect,
            uint dwMaximumSizeHigh,
            uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint
            dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,
            IntPtr dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
        #endregion
    }
}
