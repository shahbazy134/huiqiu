/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.PowerCommands.Services
{
    /// <summary>
    /// Class that represents a fixed capacity stack
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class FixedCapacityStack<TItem> : IEnumerable<TItem>, ICollection, IEnumerable
    {
        #region Fields
        private int capacity;
        private TItem[] array;
        private int currentIndex;
        [NonSerialized]
        private object syncRoot; 
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCapacityStack&lt;TItem&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public FixedCapacityStack(int capacity)
        {
            if(capacity == 0)
            {
                throw new ArgumentException(Properties.Resources.CapacityShouldBeGreaterThanZero, "capacity");
            }

            this.currentIndex = 0;
            this.capacity = capacity;
            this.array = new TItem[capacity];
        } 
        #endregion

        #region Public Implementation
        /// <summary>
        /// Pushes an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Push(TItem item)
        {
            if(this.currentIndex == this.array.Length)
            {
                TItem[] destinationArray = new TItem[this.capacity];
                Array.Copy(this.array, 1, destinationArray, 0, this.capacity - 1);
                this.array = destinationArray;
                this.currentIndex = this.array.Length - 1;
            }

            this.array[this.currentIndex++] = item;
        }

        /// <summary>
        /// Pops an item.
        /// </summary>
        /// <returns></returns>
        public TItem Pop()
        {
            if(this.currentIndex == 0)
            {
                throw new InvalidOperationException(Properties.Resources.EmptyStack);
            }

            TItem item = this.array[--this.currentIndex];
            this.array[this.currentIndex] = default(TItem);

            return item;
        }

        /// <summary>
        /// Peeks the current item.
        /// </summary>
        /// <returns></returns>
        public TItem Peek()
        {
            if(this.currentIndex == 0)
            {
                throw new InvalidOperationException(Properties.Resources.EmptyStack);
            }

            return this.array[this.currentIndex - 1];
        }

        /// <summary>
        /// Clears the stack.
        /// </summary>
        public void Clear()
        {
            Array.Clear(this.array, 0, this.capacity);
            this.currentIndex = 0;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.-or- <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.</returns>
        public int Count
        {
            get { return this.currentIndex; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.</returns>
        public object SyncRoot
        {
            get
            {
                if(this.syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this.syncRoot, new object(), null);
                }
                return this.syncRoot;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new Enumerator<TItem>((FixedCapacityStack<TItem>)this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return new Enumerator<TItem>((FixedCapacityStack<TItem>)this);
        } 
        #endregion

        /// <summary>
        /// Enumerator for the Circular Stack
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator<T> : IEnumerator<TItem>, IDisposable, IEnumerator
        {
            #region Fields
            private int index;
            private FixedCapacityStack<TItem> stack;
            private TItem currentElement; 
            #endregion

            #region Constructors
            internal Enumerator(FixedCapacityStack<TItem> stack)
            {
                this.stack = stack;
                this.index = -2;
                this.currentElement = default(TItem);
            } 
            #endregion

            #region Public Implementation
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.index = -1;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                bool flag;

                if(this.index == -2)
                {
                    this.index = this.stack.currentIndex - 1;
                    flag = this.index >= 0;
                    if(flag)
                    {
                        this.currentElement = this.stack.array[this.index];
                    }
                    return flag;
                }
                if(this.index == -1)
                {
                    return false;
                }
                flag = --this.index >= 0;
                if(flag)
                {
                    this.currentElement = this.stack.array[this.index];
                    return flag;
                }
                this.currentElement = default(TItem);
                return flag;
            }

            void IEnumerator.Reset()
            {
                this.index = -2;
                this.currentElement = default(TItem);
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            public TItem Current
            {
                get
                {
                    if(this.index == -2)
                    {
                        throw new InvalidOperationException(Properties.Resources.EnumNotStarted);
                    }
                    if(this.index == -1)
                    {
                        throw new InvalidOperationException(Properties.Resources.EnumEnded);
                    }
                    return this.currentElement;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if(this.index == -2)
                    {
                        throw new InvalidOperationException(Properties.Resources.EnumNotStarted);
                    }
                    if(this.index == -1)
                    {
                        throw new InvalidOperationException(Properties.Resources.EnumEnded);
                    }
                    return this.currentElement;
                }
            } 
            #endregion
        }
    }
}
