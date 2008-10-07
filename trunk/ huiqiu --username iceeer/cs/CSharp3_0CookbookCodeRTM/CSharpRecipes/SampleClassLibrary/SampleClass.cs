using System;
using System.IO;

namespace SampleClassLibrary
{
    public class SampleClass
    {
        public SampleClass()
        {
        }

        public bool TestMethod1(string text)
        {
            Console.WriteLine(text);
            return (true);
        }

        public bool TestMethod2(string text, int n)
        {
            Console.WriteLine(text + "invoked with {0}",n);
            return (true);
        }
    }
}
