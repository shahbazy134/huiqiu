using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;

namespace ChineseConverterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
This code produces the following output with Office12  Chinese edition installed:

The simplified format of 北京時間 is 北京时间.
The traditional format of 软件 is 軟體.
This code produces the following output without Office12  Chinese edition installed:

The simplified format of 北京時間 is 北京时间.
The traditional format of 软件 is 軟件.*/

            string strSimplified = "软件";
            string strTraditional = "北京時間";

            Console.WriteLine("The simplified format of " + strTraditional + " is {0}.", ChineseConverter.Convert(strTraditional, ChineseConversionDirection.TraditionalToSimplified));//把繁体中文转换为简体中文。
            Console.WriteLine("The traditional format of " + strSimplified + " is {0}.", ChineseConverter.Convert(strSimplified, ChineseConversionDirection.SimplifiedToTraditional));//把简体中文转换为繁体中文。
            Console.ReadLine();

        }
    }
}
