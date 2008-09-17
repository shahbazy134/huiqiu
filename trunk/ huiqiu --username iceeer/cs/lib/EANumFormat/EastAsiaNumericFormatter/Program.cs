using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Formatters;
using System.Diagnostics;
using System.Globalization;

namespace EastAsiaNumericFormatterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // The code produces different debug output with different culture settings:
            // 
            // Chinese-Simplified:
            // The representation for number 123.45 in Standard format of current language is 壹佰贰拾叁点肆伍.
            // The representation for number 123.45 in Japanese Standard format is 壱百弐拾参.

            Console.WriteLine(string.Format(new EastAsiaNumericFormatter()
                , "The representation for number 123.45 in Standard format of current language is {0:L}", 123.45));
            Console.WriteLine("The representation for number 123.45 in Japanese Standard format is "
                + EastAsiaNumericFormatter.FormatWithCulture("L", 123.45, null, new CultureInfo("ja")));
            Console.ReadLine();
        }

    }
}
