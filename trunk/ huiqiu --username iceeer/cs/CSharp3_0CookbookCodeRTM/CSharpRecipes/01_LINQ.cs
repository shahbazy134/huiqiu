using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Xml.XPath;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using System.Diagnostics;
using System.Data.Linq;
using CSharpRecipes.Properties;
using CSharpRecipes.ProductsTableAdapters;
using Microsoft.Win32;
using System.Configuration;
using CSharpRecipes.Config;

namespace CSharpRecipes
{
    // recipe 1.5
    public static class LinqExtensions
    {
        public static decimal? WeightedMovingAverage(this IEnumerable<decimal?> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            decimal aggregate = 0.0M;
            decimal weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
            {
                return new decimal?(aggregate / count);
            }
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<double?> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
            {
                return new double?(aggregate / count);
            }
            return null;
        }

        public static float? WeightedMovingAverage(this IEnumerable<float?> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            float aggregate = 0.0F;
            float weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
            {
                return new float?(aggregate / count);
            }
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<short?> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
            {
                return new double?(aggregate / count);
            }
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<int?> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
            {
                return new double?(aggregate / count);
            }
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<long?> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
            {
                return new double?(aggregate / count);
            }
            return null;
        }

        public static decimal WeightedMovingAverage(this IEnumerable<decimal> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            decimal weight;
            decimal aggregate = 0.0M;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
            {
                return aggregate / count;
            }
            else
                return 0.0M;
        }

        public static double WeightedMovingAverage(this IEnumerable<double> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
            {
                return aggregate / count;
            }
            else
                return 0.0;
        }

        public static float WeightedMovingAverage(this IEnumerable<float> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            float weight;
            float aggregate = 0.0F;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
            {
                return aggregate / count;
            }
            else
                return 0.0F;
        }

        public static double WeightedMovingAverage(this IEnumerable<short> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
            {
                return aggregate / count;
            }
            else
                return 0.0;
        }

        public static double WeightedMovingAverage(this IEnumerable<int> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
            {
                return aggregate / count;
            }
            else
                return 0.0;
        }

        public static double WeightedMovingAverage(this IEnumerable<long> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
            {
                return aggregate / count;
            }
            else
                return 0.0;
        }


        public static decimal? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            return source.Select<TSource, decimal?>(selector).WeightedMovingAverage();
        }
        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            return source.Select<TSource, double?>(selector).WeightedMovingAverage();
        }
        public static float? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            return source.Select<TSource, float?>(selector).WeightedMovingAverage();
        }
        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, short?> selector)
        {
            return source.Select<TSource, short?>(selector).WeightedMovingAverage();
        }
        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            return source.Select<TSource, int?>(selector).WeightedMovingAverage();
        }
        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            return source.Select<TSource, long?>(selector).WeightedMovingAverage();
        }
        public static decimal WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return source.Select<TSource, decimal>(selector).WeightedMovingAverage();
        }
        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return source.Select<TSource, double>(selector).WeightedMovingAverage();
        }
        public static float WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            return source.Select<TSource, float>(selector).WeightedMovingAverage();
        }
        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, short> selector)
        {
            return source.Select<TSource, short>(selector).WeightedMovingAverage();
        }
        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return source.Select<TSource, int>(selector).WeightedMovingAverage();
        }
        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            return source.Select<TSource, long>(selector).WeightedMovingAverage();
        }


        #region Extend Average
        public static double? Average(this IEnumerable<short?> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double aggregate = 0.0;
            int count = 0;
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    aggregate += nullable.GetValueOrDefault();
                    count++;
                }
            }
            if (count > 0)
            {
                return new double?(aggregate / count);
            }
            return null;
        }
        public static double Average(this IEnumerable<short> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double aggregate = 0.0;
            // use the count of the items from the source
            int count = source.Count();
            foreach (var value in source)
            {
                aggregate += value;
            }
            if (count > 0)
            {
                return aggregate / count;
            }
            else
                return 0.0;
        }
        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, short?> selector)
        {
            return source.Select<TSource, short?>(selector).Average();
        }
        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, short> selector)
        {
            return source.Select<TSource, short>(selector).Average();
        }
        #endregion // Extend Average

    }

    public static class LINQ
    {
        #region "1.1 Querying message queues for specififc messages
        public static void TestLinqMessageQueue()
        {
            // create and populate a queue
            string queuePath = @".\private$\LINQMQ";
            MessageQueue messageQueue = null;
            if (!MessageQueue.Exists(queuePath))
                messageQueue = MessageQueue.Create(queuePath);
            else
                messageQueue = new MessageQueue(queuePath);

            using (messageQueue)
            {
                BinaryMessageFormatter messageFormatter = new BinaryMessageFormatter();
                Type[] types = Assembly.GetExecutingAssembly().GetTypes();
                for (int i = 0; i < 10; i++)
                {
                    Message msg = new Message();
                    // label our message
                    msg.Label = i.ToString();
                    // override the default XML formatting with binary
                    // as it is faster (at the expense of legibility while debugging)
                    msg.Formatter = messageFormatter;
                    // make this message persist (causes message to be written
                    // to disk)
                    msg.Recoverable = true;
                    if (i < types.Length)
                        msg.Body = types[i].ToString();
                    else
                        msg.Body = types[i - types.Length].ToString();
                    messageQueue.Send(msg);
                }

                // Query the message queue for specific messages with the following criteria:
                // 1) the label must be greater than 5
                // 2) the name of the type in the message body must contain a 'D'
                // 3) the results should be in descending order by type name (from the body)

                var query = from Message msg in messageQueue
                            // The first assignment to msg.Formatter is so that we can touch the
                            // Message object.  It assigns the BinaryMessageFormatter  to each message
                            // instance so that it can be read to determine if it matches the criteria.
                            // This is done and then checks that the formatter was correctly assigned 
                            // by performing an equality check which satisfies the Where clause's need
                            // for a boolean result while still executing the assignment of the formatter.
                            where ((msg.Formatter = messageFormatter) == messageFormatter) &&
                                int.Parse(msg.Label) > 5 &&
                                msg.Body.ToString().Contains('D')
                            orderby msg.Body.ToString() descending
                            select msg;

                // check our results for messages with a label > 5 and containing a 'D' in the name
                foreach (var msg in query)
                {
                    Console.WriteLine("Label: " + msg.Label +
                        " Body: " + msg.Body);
                }

                // clean up the queue
                messageQueue.Purge();
            }
            // remove the queue
            MessageQueue.Delete(queuePath);


            // RESULTS look like this:
            //Label: 9 Body: CSharpRecipes.DelegatesEventsFunctionalProgramming+MultipleIncreaseWithTheAnswer
            //Label: 8 Body: CSharpRecipes.DelegatesEventsFunctionalProgramming+IncreaseWithTheAnswer
            //Label: 7 Body: CSharpRecipes.DelegatesEventsFunctionalProgramming
        }


        // Jay
        #endregion

        #region "1.2 Using set semantics with data"
        public class Employee
        {
            public string Name { get; set; }
            public override string ToString()
            {
                return this.Name;
            }
            public override bool Equals(object obj)
            {
                return this.GetHashCode().Equals(obj.GetHashCode());
            }
            public override int GetHashCode()
            {
                return this.Name.GetHashCode();
            }
        }

        public static void TestSetSemantics()
        {
            // Distinct
            string[] dailySecurityLog = {
                   "Bob logged in",
                   "Bob logged out",
                   "Bob logged in",
                   "Bill logged in",
                   "Melissa logged in",
                   "Bob logged out",
                   "Bill logged out",
                   "Bill logged in",
                   "Tim logged in",
                   "Scott logged in",
                   "Scott logged out",
                   "Dave logged in",
                   "Tim logged out",
                   "Bob logged in",
                   "Dave logged out"};

            IEnumerable<string> whoLoggedIn =
                dailySecurityLog.Where(logEntry => logEntry.Contains("logged in")).Distinct();
            Console.WriteLine("Everyone who logged in today:");
            foreach (string who in whoLoggedIn)
            {
                Console.WriteLine(who);
            }


            Employee[] project1 = { 
                       new Employee(){ Name = "Bob" }, 
                       new Employee(){ Name = "Bill" }, 
                       new Employee(){ Name = "Melissa" }, 
                       new Employee(){ Name = "Shawn" } };
            Employee[] project2 = { 
                       new Employee(){ Name = "Shawn" }, 
                       new Employee(){ Name = "Tim" }, 
                       new Employee(){ Name = "Scott" } };
            Employee[] project3 = { 
                       new Employee(){ Name = "Bob" }, 
                       new Employee(){ Name = "Dave" }, 
                       new Employee(){ Name = "Tim" },
                       new Employee(){ Name = "Shawn" } };

            // Union
            Console.WriteLine("Employees for all projects");
            var allProjectEmployees = project1.Union(project2.Union(project3));
            foreach (Employee employee in allProjectEmployees)
            {
                Console.WriteLine(employee);
            }

            // Intersect
            Console.WriteLine("Employees on every project");
            var everyProjectEmployees = project1.Intersect(project2.Intersect(project3));
            foreach (Employee employee in everyProjectEmployees)
            {
                Console.WriteLine(employee);
            }

            // Except
            var intersect1_3 = project1.Intersect(project3);
            var intersect1_2 = project1.Intersect(project2);
            var intersect2_3 = project2.Intersect(project3);
            var unionIntersect = intersect1_2.Union(intersect1_3).Union(intersect2_3);

            Console.WriteLine("Employees on only one project");
            var onlyProjectEmployees = allProjectEmployees.Except(unionIntersect);
            foreach (Employee employee in onlyProjectEmployees)
            {
                Console.WriteLine(employee);
            }
        }
        #endregion

        #region "1.3 Re-use parameterized queries with LINQ to SQL "
        public static void TestCompiledQuery()
        {
            //Func<Northwind, string, string, IQueryable<Employees>>
            var GetEmployees =
                    CompiledQuery.Compile((Northwind db, string ac, string ttl) => 
                               from employee in db.Employees
                               where employee.HomePhone.Contains(ac) &&
                                     employee.Title == ttl
                               select employee); 

            //                               where employee.HomePhone.Contains(string.Format("({0})",ac)) &&
            //System.NotSupportedException: Method 'System.String Format(System.String, System.Object)' has no supported translation to SQL.
            //   at System.Data.Linq.SqlClient.SqlMethodCallConverter.VisitMethodCall(SqlMethodCall mc)

            Northwind dataContext = new Northwind(Settings.Default.NorthwindConnectionString);
            dataContext.Log = Console.Out;

            foreach (var employee in GetEmployees(dataContext, "(206)", "Sales Representative"))
            {
                Console.WriteLine("{0} {1}",
                    employee.FirstName, employee.LastName);
            }

            foreach (var employee in GetEmployees(dataContext, "(71)", "Sales Manager"))
            {
                Console.WriteLine("{0} {1}",
                    employee.FirstName, employee.LastName);
            }
        }

        #endregion

        #region "1.4 Using LINQ with different cultures	
        public static void TestLinqForCulture()
        {

            // The Danish language treats the character "Æ" as an individual letter, 
            // sorting it after "Z" in the alphabet. 
            // The English language treats the character "Æ" as a 
            // special symbol, sorting it before the letter "A" in the alphabet.
            string[] names = { "Jello", "Apple", "Bar", "Æble", "Forsooth", "Orange", "Zanzibar" };

            // Create CultureInfo for Danish in Denmark.
            CultureInfo danish = new CultureInfo("da-DK");
            // Create CultureInfo for English in the U.S.
            CultureInfo american = new CultureInfo("en-US");

            CultureStringComparer comparer = new CultureStringComparer(danish,CompareOptions.None);
            var query = names.OrderBy(n => n, comparer);
            Console.WriteLine("Ordered by specific culture : " + comparer.CurrentCultureInfo.Name);
            foreach (string name in query)
            {
                Console.WriteLine(name);
            }

            comparer.CurrentCultureInfo = american;
            query = names.OrderBy(n => n, comparer);
            Console.WriteLine("Ordered by specific culture : " + comparer.CurrentCultureInfo.Name);
            foreach (string name in query)
            {
                Console.WriteLine(name);
            }


            query = from n in names
                    orderby n
                    select n;
            Console.WriteLine("Ordered by Thread.CurrentThread.CurrentCulture : " + Thread.CurrentThread.CurrentCulture.Name);
            foreach (string name in query)
            {
                Console.WriteLine(name);
            }


            // RESULTS look like this:
            //Ordered by specific culture : da-DK
            //Apple
            //Bar
            //Forsooth
            //Jello
            //Orange
            //Zanzibar
            //Æble
            //Ordered by specific culture : en-US
            //Æble
            //Apple
            //Bar
            //Forsooth
            //Jello
            //Orange
            //Zanzibar
            //Ordered by Thread.CurrentThread.CurrentCulture : en-US
            //Æble
            //Apple
            //Bar
            //Forsooth
            //Jello
            //Orange
            //Zanzibar
        }

        public class CultureStringComparer : IComparer<string>
        {
            private CultureStringComparer()
            {
            }

            public CultureStringComparer(CultureInfo cultureInfo, CompareOptions options)
            {
                if (cultureInfo == null)
                    throw new ArgumentNullException("cultureInfo");

                CurrentCultureInfo = cultureInfo;
                Options = options;
            }

            public int Compare(string x, string y)
            {
                return CurrentCultureInfo.CompareInfo.Compare(x, y, Options);
            }

            public CultureInfo CurrentCultureInfo { get; set; }

            public CompareOptions Options { get; set; }
        }

        #endregion

        #region "1.5 Adding extensions to types for use with LINQ"
        public static void TestWeightedMovingAverage()
        {
            decimal[] prices = new decimal[10] { 13.5M, 17.8M, 92.3M, 0.1M, 15.7M, 19.99M, 9.08M, 6.33M, 2.1M, 14.88M };
            Console.WriteLine(prices.WeightedMovingAverage());
            Console.WriteLine(prices.Average());

            double[] dprices = new double[10] { 13.5, 17.8, 92.3, 0.1, 15.7, 19.99, 9.08, 6.33, 2.1, 14.88 };
            Console.WriteLine(dprices.WeightedMovingAverage());
            Console.WriteLine(dprices.Average());

            float[] fprices = new float[10] { 13.5F, 17.8F, 92.3F, 0.1F, 15.7F, 19.99F, 9.08F, 6.33F, 2.1F, 14.88F };
            Console.WriteLine(fprices.WeightedMovingAverage());
            Console.WriteLine(fprices.Average());

            int[] iprices = new int[10] { 13, 17, 92, 0, 15, 19, 9, 6, 2, 14 };
            Console.WriteLine(iprices.WeightedMovingAverage());
            Console.WriteLine(iprices.Average());

            long[] lprices = new long[10] { 13, 17, 92, 0, 15, 19, 9, 6, 2, 14 };
            Console.WriteLine(lprices.WeightedMovingAverage());
            Console.WriteLine(lprices.Average());

            short[] sprices = new short[10] { 13, 17, 92, 0, 15, 19, 9, 6, 2, 14 };
            Console.WriteLine(sprices.WeightedMovingAverage());
            // System.Linq.Extensions doesn't implement Average for short but we do for them!
            Console.WriteLine(sprices.Average());
        }
        #endregion

        #region "1.6 Query and Join across data repositories	"
        public static void TestLinqToDataSet()
        {
            Northwind dataContext = new Northwind(Settings.Default.NorthwindConnectionString);
            var categories = new XElement("Categories",
                                from c in dataContext.Categories
                                select new XElement("Category",
                                   new XAttribute("CategoryID", c.CategoryID),
                                   new XAttribute("CategoryName", c.CategoryName),
                                   new XAttribute("Description", c.Description)));

            using (XmlWriter writer = XmlWriter.Create("Categories.xml"))
            {
                categories.WriteTo(writer);
            }


            XElement xmlCategories = XElement.Load("Categories.xml");
                        
            ProductsTableAdapter adapter = new ProductsTableAdapter();
            Products products = new Products();
            adapter.Fill(products._Products);

            var expr = from product in products._Products
                       where product.Units_In_Stock > 100
                       join xc in xmlCategories.Elements("Category")
                       on product.Category_ID equals int.Parse(xc.Attribute("CategoryID").Value)
                       select new
                       {
                           ProductName = product.Product_Name,
                           Category = xc.Attribute("CategoryName").Value,
                           CategoryDescription = xc.Attribute("Description").Value
                       };

            foreach (var productInfo in expr)
            {
                Console.WriteLine("ProductName: " + productInfo.ProductName +
                    " Category: " + productInfo.Category + 
                    " Category Description: " + productInfo.CategoryDescription);
            }

            //OUTPUT
            //ProductName: Grandma's Boysenberry Spread Category: Condiments Category Description: Sweet and savory sauces, relishes, spreads, and seasonings
            //ProductName: Gustaf's Knäckebröd Category: Grains/Cereals Category Description: Breads, crackers, pasta, and cereal
            //ProductName: Geitost Category: Dairy Products Category Description: Cheeses
            //ProductName: Sasquatch Ale Category: Beverages Category Description: Soft drinks, coffees, teas, beer, and ale
            //ProductName: Inlagd Sill Category: Seafood Category Description: Seaweed and fish
            //ProductName: Boston Crab Meat Category: Seafood Category Description: Seaweed and fish
            //ProductName: Pâté chinois Category: Meat/Poultry Category Description: Prepared meats
            //ProductName: Sirop d'érable Category: Condiments Category Description: Sweet and savory sauces, relishes, spreads, and seasonings
            //ProductName: Röd Kaviar Category: Seafood Category Description: Seaweed and fish
            //ProductName: Rhönbräu Klosterbier Category: Beverages Category Description: Soft drinks, coffees, teas, beer, and ale
        }
        #endregion 

        #region "1.7 Querying configuration files with LINQ"
        public static void TestQueryConfig()
        {
            CSharpRecipesConfigurationSection recipeConfig = 
                ConfigurationManager.GetSection("CSharpRecipesConfiguration") as CSharpRecipesConfigurationSection;

            var expr = from ChapterConfigurationElement chapter in recipeConfig.Chapters.OfType<ChapterConfigurationElement>()
                       where (chapter.Title.Contains("and")) && ((int.Parse(chapter.Number) % 2) == 0) 
                       select new 
                       { 
                           ChapterNumber = "Chapter " + chapter.Number,
                           chapter.Title
                       };

            foreach (var chapterInfo in expr)
            {
                Console.WriteLine(chapterInfo.ChapterNumber + ": " + chapterInfo.Title);
            }


            System.Configuration.Configuration machineConfig = 
                ConfigurationManager.OpenMachineConfiguration();

            var query = from ConfigurationSection section in machineConfig.Sections.OfType<ConfigurationSection>()
                        where section.SectionInformation.RequirePermission
                        select section;


            foreach (ConfigurationSection section in query)
            {
                Console.WriteLine(section.SectionInformation.Name);
            }

        }
        #endregion

        #region "1.8 Creating XML straight from a database	"
        public static void TestXmlFromDatabase()
        {
            Northwind dataContext = new Northwind(Settings.Default.NorthwindConnectionString);

            // Log the generated SQL to the console
            dataContext.Log = Console.Out;

            // select the top 5 customers whose contact is the owner and
            // those owners placed orders spending more than $10000 this year 

            // Generated SQL for query - output via DataContext.Log
            //SELECT [t10].[CompanyName], [t10].[ContactName], [t10].[Phone], [t10].[TotalSpend]
            //FROM (
            //    SELECT TOP (5) [t0].[Company Name] AS [CompanyName], [t0].[Contact Name] AS
            //[ContactName], [t0].[Phone], [t9].[value] AS [TotalSpend]
            //    FROM [Customers] AS [t0]
            //    OUTER APPLY (
            //        SELECT COUNT(*) AS [value]
            //        FROM [Orders] AS [t1]
            //        WHERE [t1].[Customer ID] = [t0].[Customer ID]
            //        ) AS [t2]
            //    OUTER APPLY (
            //        SELECT SUM([t8].[value]) AS [value]
            //        FROM (
            //            SELECT [t3].[Customer ID], [t6].[Order ID], 
            //                ([t7].[Unit Price] * 
            //                (CONVERT(Decimal(29,4),[t7].[Quantity]))) - ([t7].[Unit Price] * 
            //                    (CONVERT(Decimal(29,4),[t7].[Quantity])) * 
            //                        (CONVERT(Decimal(29,4),[t7].[Discount]))) AS [value], 
            //                [t7].[Order ID] AS [Order ID2], 
            //                [t3].[Contact Title] AS [ContactTitle], 
            //                [t5].[value] AS [value2], 
            //                [t6].[Customer ID] AS [CustomerID]
            //            FROM [Customers] AS [t3]
            //            OUTER APPLY (
            //                SELECT COUNT(*) AS [value]
            //                FROM [Orders] AS [t4]
            //                WHERE [t4].[Customer ID] = [t3].[Customer ID]
            //                ) AS [t5]
            //            CROSS JOIN [Orders] AS [t6]
            //            CROSS JOIN [Order Details] AS [t7]
            //            ) AS [t8]
            //        WHERE ([t0].[Customer ID] = [t8].[Customer ID]) AND ([t8].[Order ID] = [
            //t8].[Order ID2]) AND ([t8].[ContactTitle] LIKE @p0) AND ([t8].[value2] > @p1) AN
            //D ([t8].[CustomerID] = [t8].[Customer ID])
            //        ) AS [t9]
            //    WHERE ([t9].[value] > @p2) AND ([t0].[Contact Title] LIKE @p3) AND ([t2].[va
            //lue] > @p4)
            //    ORDER BY [t9].[value] DESC
            //    ) AS [t10]
            //ORDER BY [t10].[TotalSpend] DESC
            //-- @p0: Input String (Size = 0; Prec = 0; Scale = 0) [%Owner%]
            //-- @p1: Input Int32 (Size = 0; Prec = 0; Scale = 0) [0]
            //-- @p2: Input Decimal (Size = 0; Prec = 29; Scale = 4) [10000]
            //-- @p3: Input String (Size = 0; Prec = 0; Scale = 0) [%Owner%]
            //-- @p4: Input Int32 (Size = 0; Prec = 0; Scale = 0) [0]
            //-- Context: SqlProvider(SqlCE) Model: AttributedMetaModel Build: 3.5.20706.1

            var bigSpenders = new XElement("BigSpenders",
                        from top5 in 
                        (
                            (from customer in
                                 (
                                     from c in dataContext.Customers
                                     // get the customers where the contact is the owner 
                                     // and they placed orders
                                     where c.ContactTitle.Contains("Owner") 
                                        && c.Orders.Count > 0
                                     join orderData in
                                         (
                                             from c in dataContext.Customers
                                             // get the customers where the contact is the owner 
                                             // and they placed orders
                                             where c.ContactTitle.Contains("Owner") 
                                                && c.Orders.Count > 0
                                             from o in c.Orders
                                             // get the order details
                                             join od in dataContext.OrderDetails
                                                 on o.OrderID equals od.OrderID
                                             select new
                                             {
                                                 c.CompanyName,
                                                 c.CustomerID,
                                                 o.OrderID,
                                                 // have to calc order value from orderdetails
                                                 //(UnitPrice*Quantity as Total)- (Total*Discount) 
                                                 // as NetOrderTotal
                                                 NetOrderTotal = (
                                                     (((double)od.UnitPrice) * od.Quantity) -
                                                     ((((double)od.UnitPrice) * od.Quantity) * od.Discount))
                                             }
                                         )
                                     on c.CustomerID equals orderData.CustomerID
                                     into customerOrders
                                     select new
                                     {
                                         c.CompanyName,
                                         c.ContactName,
                                         c.Phone,
                                         // Get the total amount spent by the customer
                                         TotalSpend = customerOrders.Sum(order => order.NetOrderTotal)
                                     }
                                 )
                             // only worry about customers that spent > 10000
                             where customer.TotalSpend > 10000
                             orderby customer.TotalSpend descending
                             // only take the top 5 spenders
                             select customer).Take(5)
                        )
                        // format the data as XML
                        select new XElement("Customer",
                                   new XAttribute("companyName", top5.CompanyName),
                                   new XAttribute("contactName", top5.ContactName),
                                   new XAttribute("phoneNumber", top5.Phone),
                                   new XAttribute("amountSpent", top5.TotalSpend)));

            //var bigSpenders = 
            //            (from customer in 
            //            (
            //                from c in dataContext.Customers
            //                where c.ContactTitle.Contains("Owner") && c.Orders.Count > 0
            //                join orderData in
            //                    (
            //                        from c in dataContext.Customers
            //                        where c.ContactTitle.Contains("Owner") && c.Orders.Count > 0
            //                        from o in c.Orders
            //                        join od in dataContext.OrderDetails
            //                            on o.OrderID equals od.OrderID
            //                        select new
            //                        {
            //                            c.CompanyName,
            //                            c.CustomerID,
            //                            o.OrderID,
            //                            NetOrderTotal = (
            //                                (od.UnitPrice * (decimal)od.Quantity) -
            //                                ((od.UnitPrice * (decimal)od.Quantity) *
            //                                (decimal)od.Discount))
            //                        }
            //                    )
            //                on c.CustomerID equals orderData.CustomerID
            //                into customerOrders
            //                select new
            //                {
            //                    c.CompanyName,
            //                    c.ContactName,
            //                    c.Phone,
            //                    TotalNetOrders = customerOrders.Sum(order => order.NetOrderTotal)
            //                }
            //            )
            //           where customer.TotalNetOrders > 10000
            //           orderby customer.TotalNetOrders descending
            //           select customer).Take(5);

            using (XmlWriter writer = XmlWriter.Create("BigSpenders.xml"))
            {
                bigSpenders.WriteTo(writer);
            }

//<BigSpenders>
//  <Customer companyName="Folk och fä HB" contactName="Maria Larsson" 
//            phoneNumber="0695-34 67 21" amountSpent="39805.162472039461" />
//  <Customer companyName="White Clover Markets" contactName="Karl Jablonski" 
//            phoneNumber="(206) 555-4112" amountSpent="35957.604972146451" />
//  <Customer companyName="Bon app'" contactName="Laurence Lebihan" 
//            phoneNumber="91.24.45.40" amountSpent="22311.577472746558" />
//  <Customer companyName="LINO-Delicateses" contactName="Felipe Izquierdo" 
//            phoneNumber="(8) 34-56-12" amountSpent="20458.544984650609" />
//  <Customer companyName="Simons bistro" contactName="Jytte Petersen" 
//            phoneNumber="31 12 34 56" amountSpent="18978.777493602414" />
//</BigSpenders>

            Console.WriteLine(bigSpenders.ToString());
        }
        #endregion

        #region "1.9 Being selective about your query results"
        public static void TestTakeSkipWhile()
        {
            //Problem: You want to be able to get a dynamic subset of a query result

            Northwind dataContext = new Northwind(Settings.Default.NorthwindConnectionString);

            // find the products for all suppliers

            var query = 
                dataContext.Suppliers.GroupJoin(dataContext.Products, 
                    s => s.SupplierID, p => p.SupplierID,
                    (s, products) => new 
                    { 
                        s.CompanyName, 
                        s.ContactName, 
                        s.Phone, 
                        Products = products 
                    }).OrderByDescending(supplierData => supplierData.Products.Count())
                        .TakeWhile(supplierData => supplierData.Products.Count() > 3);
                        //.SkipWhile(supplierData =>
                            
                        

            Console.WriteLine("Suppliers that provide more than two products: {0}", query.Count());
            foreach (var supplierData in query)
            {
                Console.WriteLine("    Company Name : {0}",supplierData.CompanyName);
                Console.WriteLine("    Contact Name : {0}", supplierData.ContactName);
                Console.WriteLine("    Contact Phone : {0}", supplierData.Phone);
                Console.WriteLine("    Products Supplied : {0}", supplierData.Products.Count());
                foreach (var productData in supplierData.Products)
                {
                    Console.WriteLine("        Product: " + productData.ProductName);
                }
            }
        }
        #endregion
 
        #region "1.10 Using LINQ with types that don't support IEnumerable<T>"	
        public static void TestUsingNonIEnumT()
        {
            //IEnumerable
            //    ArrayList
            //    BitArray
            //    Hashtable
            //    Queue
            //    SortedList
            //    Stack
            //    System.Net.CredentialCache
            //    XmlNodeList
            //    XPathNodeIterator

            //ICollection
            //    System.Diagnostics.EVentlogentrycollection
            //    System.Net.CookieCollection
            //    System.Security.AccessControl.GenericAcl
            //    System.Security.PermissionSet

            // Make some XML with some types that you can use with LINQ 
            // that don't support IEnumerable<T> directly
            XElement xmlFragment = new XElement("NonGenericLinqableTypes",
                                    new XElement("IEnumerable",
                                        new XElement("System.Collections",
                                            new XElement("ArrayList"),
                                            new XElement("BitArray"),
                                            new XElement("Hashtable"),
                                            new XElement("Queue"),
                                            new XElement("SortedList"),
                                            new XElement("Stack")),
                                        new XElement("System.Net",
                                            new XElement("CredentialCache")),
                                        new XElement("System.Xml",
                                            new XElement("XmlNodeList")),
                                        new XElement("System.Xml.XPath",
                                            new XElement("XPathNodeIterator"))),
                                    new XElement("ICollection",
                                        new XElement("System.Diagnostics",
                                            new XElement("EventLogEntryCollection")),
                                        new XElement("System.Net",
                                            new XElement("CookieCollection")),
                                        new XElement("System.Security.AccessControl",
                                            new XElement("GenericAcl")),
                                        new XElement("System.Security",
                                            new XElement("PermissionSet"))));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFragment.ToString());

            // Select the names of the nodes under IEnumerable that have children and are
            // named System.Collections and contain a capital S and return that list in descending order
            var query = from node in doc.SelectNodes("/NonGenericLinqableTypes/IEnumerable/*").Cast<XmlNode>()
                        where node.HasChildNodes &&
                            node.Name == "System.Collections"
                        from XmlNode xmlNode in node.ChildNodes
                        where xmlNode.Name.Contains('S')
                        orderby xmlNode.Name descending
                        select xmlNode.Name;

            foreach (string name in query)
            {
                Console.WriteLine(name);
            }

            EventLog log = new EventLog("Application");
            query = from EventLogEntry entry in log.Entries
                    where entry.EntryType == EventLogEntryType.Error &&
                        entry.TimeGenerated > DateTime.Now.Subtract(new TimeSpan(6, 0, 0))
                    select entry.Message;

            Console.WriteLine("There were " + query.Count<string>() + 
                " Application Event Log error messages in the last 6 hours!");
            foreach (string message in query)
            {
                Console.WriteLine(message);
            }

            ArrayList stuff = new ArrayList();
            stuff.Add(DateTime.Now);
            stuff.Add(DateTime.Now);
            stuff.Add(1);
            stuff.Add(DateTime.Now);

            var expr = from item in stuff.OfType<DateTime>()
                       select item;
            foreach (DateTime item in expr)
            {
                Console.WriteLine(item);
            }
        }
        #endregion 
    }
}
