using System;
using System.Collections.Generic;
using System.Text;

namespace CheckManagedCppExceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            string sep = "-----------------------------------------";
            ManagedCppException.ThrowAnException tae = new ManagedCppException.ThrowAnException();
            try
            {
                tae.ThrowManagedException();
            }
            catch
            {
                Console.WriteLine("Caught Managed Exception with base catch block");
            }

            Console.WriteLine(sep);

            try
            {
                tae.ThrowManagedException();
            }
            catch(Exception e)
            {
                Console.WriteLine("Caught Managed Exception with System.Exception catch block:\n {0}",e.ToString());
            }

            Console.WriteLine(sep);

            try
            {
                tae.ThrowUnmanagedException();
            }
            catch
            {
                Console.WriteLine("Caught Unmanaged Exception with base catch block");
            }

            Console.WriteLine(sep);

            try
            {
                tae.ThrowUnmanagedException();
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught Unmanaged Exception with System.Exception catch block:\n {0}", e.ToString());
            }

            Console.WriteLine(sep);

            try
            {
                tae.ThrowBoxedException();
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught Unmanaged Exception with System.Exception catch block:\n {0}", e.ToString());
            }

            Console.ReadLine();
        }

        static string Meth()
        {
            throw new NotImplementedException();
        }
    }
}
