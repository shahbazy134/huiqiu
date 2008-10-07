using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Text;


[assembly: ObfuscateAssembly(true, StripAfterObfuscation=true)]

namespace CSharpRecipes
{
    public static class ReflectionExt
    {
        #region 13.6 Extension methods
        public static string GetBaseTypeDisplay(this Type type)
        {
            IEnumerable<string> baseTypes =
                (from t in type.GetBaseTypes()
                 select t.Name).Reverse();
            StringBuilder builder = new StringBuilder();
            foreach (string typeName in baseTypes)
            {
                if (builder.Length == 0)
                    builder.Append(typeName);
                else
                    builder.AppendFormat("<-{0}", typeName);
            }
            return builder.ToString();
        }

        private static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            Type current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
        #endregion
        
        #region 13.7 Extension method
        public static IEnumerable<Type> ListSubClassesForType(this Assembly asm,
                                                              Type baseclassType)
        {
            return from type in asm.GetTypes()
                   where type.IsSubclassOf(baseclassType)
                   select type;
        }
        #endregion
    }
    
    
	public class Reflection
	{
		#region "13.1 Listing Referenced Assemblies"	
		public static void ListImportedAssemblies()
		{
            string file = GetProcessPath();
			
			//ProcessModuleCollection pmc = curProc.Modules;
			//foreach (ProcessModule pm in pmc)
			//{
			//    Console.WriteLine("ModuleName: " + pm.ModuleName);
			//    Console.WriteLine("FileName: " + pm.FileName);
			//    Console.WriteLine("ModuleName: " + pm.FileVersionInfo.FileName);
			//}
			
			StringCollection assemblies = new StringCollection();
			Reflection.BuildDependentAssemblyList(file,assemblies);
			Console.WriteLine("Assembly {0} has a dependency tree of these assemblies:\r\n",file);
			foreach(string name in assemblies)
			{
				Console.WriteLine("\t{0}\r\n",name);
			}
		}

		public static void BuildDependentAssemblyList(string path, 
			StringCollection assemblies)
		{
			// maintain a list of assemblies the original one needs
			if(assemblies == null)
				assemblies = new StringCollection();

			// have we already seen this one?
			if(assemblies.Contains(path)==true)
				return;

            try
            {
                Assembly asm = null;
                
                // look for common path delimiters in the string 
                // to see if it is a name or a path
                if ((path.IndexOf(@"\", 0, path.Length, StringComparison.Ordinal) != -1) ||
                    (path.IndexOf("/", 0, path.Length, StringComparison.Ordinal) != -1))
                {
                    // load the assembly from a path
                    asm = Assembly.LoadFrom(path);
                }
                else
                {
                    // try as assembly name
                    asm = Assembly.Load(path);
                }
                
                // add the assembly to the list
                if (asm != null)
                {
                    assemblies.Add(path);
                }
                
                // get the referenced assemblies
                AssemblyName[] imports = asm.GetReferencedAssemblies();
                
                // iterate
                foreach (AssemblyName asmName in imports)
                {
                    // now recursively call this assembly to get the new modules 
                    // it references
                    BuildDependentAssemblyList(asmName.FullName, assemblies);
                }
            }
            catch (FileLoadException fle)
            {
                // just let this one go...
                Console.WriteLine(fle);
            }
		}

		#endregion 

		#region "13.2 Listing Exported Types"	
		public static void ListExportedTypes()
		{
            string file = GetProcessPath();
            ListExportedTypes(file);
		}

		public static void ListExportedTypes(string path)
		{
			// load the assembly
			Assembly asm = Assembly.LoadFrom(path);
			Console.WriteLine("Assembly: {0} imports:",path);
			// get the exported types
			Type[] types = asm.GetExportedTypes();
			foreach (Type t in types)
			{
				Console.WriteLine ("\tExported Type: {0}",t.FullName);
			}
		}

		#endregion 

		#region "13.3 Finding Overridden Methods"	
		public abstract class BaseOverrides
		{
			public abstract void Foo(string str, int i);

			public abstract void Foo(long l, double d, byte[] bytes);
		}

		public class DerivedOverrides : BaseOverrides
		{
			public override void Foo(string str, int i)
			{
			}

			public override void Foo(long l, double d, byte[] bytes)
			{
			}
		}

        private static string GetProcessPath()
        {
            // fix the path so that if running under the debugger we get the original file
            string processName = Process.GetCurrentProcess().MainModule.FileName;
            int index = processName.IndexOf("vshost", StringComparison.Ordinal);
            if (index != -1)
            {
                string first = processName.Substring(0, index);
                int numChars = processName.Length - (index + 7);
                string second = processName.Substring(index + 7, numChars);

                processName = first + second;
            }
            return processName;
        }

		public static void FindOverriddenMethods()
		{
            string path = GetProcessPath();

			// try the easier one
			FindMethodOverrides(path,"CSharpRecipes.Reflection+DerivedOverrides");

			// try the signature findmethodoverrides
			FindMethodOverrides(path, 
				"CSharpRecipes.Reflection+DerivedOverrides", 
				"Foo", 
				new Type[3] {typeof(long), typeof(double), typeof(byte[])});
		}

		public static void FindMethodOverrides(string asmPath, string typeName)
		{
			Assembly asm = Assembly.LoadFrom(asmPath);
			Type asmType = asm.GetType(typeName);

			Console.WriteLine("---[" + asmType.FullName + "]---");

			// get the methods that match this type
			MethodInfo[] methods = asmType.GetMethods(BindingFlags.Instance | 
				BindingFlags.NonPublic | BindingFlags.Public | 
				BindingFlags.Static | BindingFlags.DeclaredOnly);

		    var mis = from ms in methods
		              where ms != ms.GetBaseDefinition()
                      select ms.GetBaseDefinition();
		             
		    foreach (MethodInfo mi in mis)
		    {
			    Console.WriteLine();
				Console.WriteLine("Current Method:  " + mi.ToString());

                Console.WriteLine("Base Type FullName:  " + mi.DeclaringType.FullName);
                Console.WriteLine("Base Method:  " + mi.ToString());

                // list the types of this method
                foreach (ParameterInfo pi in mi.GetParameters())
                {
                    Console.WriteLine("\tParam {0}: {1}", pi.Name, pi.ParameterType.ToString());                       
                }
            }
		}

		public static void FindMethodOverrides(string asmPath, string typeName, 
			string methodName, Type[] paramTypes)
		{
			Console.WriteLine(Environment.NewLine + "For [Type] Method:  [" + typeName + "] " + methodName);

			Assembly asm = Assembly.LoadFrom(asmPath);
			
			// GetType should throw an exception if the type cannot be found
			//   and it should also ignore the case of the typeName
			Type asmType = asm.GetType(typeName,true,true);
			MethodInfo method = asmType.GetMethod(methodName, paramTypes);

			if (method != null)
			{
				MethodInfo baseDef = method.GetBaseDefinition();
				if (baseDef != method)
				{
					Console.WriteLine("Base Type FullName:  " + 
						baseDef.DeclaringType.FullName);
					Console.WriteLine("Base Method:  " + baseDef.ToString());
					
					bool foundMatch = false;

                    var match = from p in baseDef.GetParameters()
                                join op in paramTypes 
                                    on p.ParameterType.UnderlyingSystemType equals op.UnderlyingSystemType
                                select p;

                    foundMatch = match.Any();
                                
                    foreach (ParameterInfo pi in match)
                    {
                        // list the params so we can see which one we got
                        Console.WriteLine("\tParam {0}: {1}",
                            pi.Name, pi.ParameterType.ToString());
                    }            

					// we found the one we were looking for
					if(foundMatch == true)
					{
						Console.WriteLine("Found Match!");
					}
				}
			}
			Console.WriteLine();
		}
		#endregion 

		#region "13.4 Finding Members in an Assembly"	
		public static void FindMembersInAssembly()
		{
            string file = GetProcessPath();
            FindMemberInAssembly(file, "FindMembersInAssembly");
		}

		public static void FindMemberInAssembly(string asmPath, string memberName)
		{
            var members = from asm in Assembly.LoadFrom(asmPath).GetTypes()
                          from ms in asm.GetMember(memberName, MemberTypes.All, 
					          BindingFlags.Public | BindingFlags.NonPublic |
                              BindingFlags.Static | BindingFlags.Instance)
                          select ms;

    		foreach (MemberInfo member in members)
			{
				Console.WriteLine("Found " + member.MemberType + ":  " + 
					member.ToString() + " IN " + 
					member.DeclaringType.FullName);
			}
		}

		#endregion 

		#region "13.5 Determining and Obtaining Types Nested Within a Type"	
		public static void ObtainNestedTypes()
		{
            string path = GetProcessPath();
            DisplayNestedTypes(path);
		}

		public static void DisplayNestedTypes(string asmPath)
		{
            var names = from t in Assembly.LoadFrom(asmPath).GetTypes()
                        from t2 in t.GetNestedTypes(BindingFlags.Instance |
                                    BindingFlags.Static |
                                    BindingFlags.Public |
                                    BindingFlags.NonPublic)
			            where !t2.IsEnum && !t2.IsInterface 
			            select t2.FullName;

            foreach (string name in names)
			{
			    Console.WriteLine(name);
			}
		}

		#endregion 

		#region "13.6 Displaying the Inheritance Hierarchy for a Type"	
		public static void DisplayInheritanceHierarchy()
		{
			DisplayInheritanceHierarchyType();
		}

        public static void DisplayInheritanceHierarchyType()
        {
            Process current = Process.GetCurrentProcess();
            // get the path of the current module
            string asmPath = current.MainModule.FileName;
            // a specific type
            DisplayInheritanceChain(asmPath, "CSharpRecipes.Reflection+DerivedOverrides");
            // all types in the assembly
            DisplayInheritanceChain(asmPath);
        }

        public static void DisplayInheritanceChain(string asmPath)
        {
            Assembly asm = Assembly.LoadFrom(asmPath);
            var typeInfos = from Type type in asm.GetTypes()
                            select new
                            {
                                FullName = type.FullName,
                                BaseTypeDisplay = type.GetBaseTypeDisplay()
                            };

            foreach (var typeInfo in typeInfos)
            {
                // Recurse over all base types
                Console.WriteLine("Derived Type: " + typeInfo.FullName);
                Console.WriteLine("Base Type List: " + typeInfo.BaseTypeDisplay);
                Console.WriteLine();
            }
        }

        public static void DisplayInheritanceChain(string asmPath, string baseType)
        {
            Assembly asm = Assembly.LoadFrom(asmPath);
            string typeDisplay = asm.GetType(baseType).GetBaseTypeDisplay();
            Console.WriteLine(typeDisplay);
        }
        #endregion 

		#region "13.7 Finding the Subclasses of a Type"	
		public static void FindSubclasses()
		{
			FindSubclassOfType();
		}

        public static void FindSubclassOfType()
        {
            Process current = Process.GetCurrentProcess();
            // get the path of the current module
            string asmPath = current.MainModule.FileName;
            Assembly asm = Assembly.LoadFrom(asmPath);
            Type type = Type.GetType("CSharpRecipes.Reflection+BaseOverrides");
            IEnumerable<Type> subClasses = asm.ListSubClassesForType(type);

            // write out the subclasses for this type    
            if (subClasses.Count() > 0)
            {
                Console.WriteLine("{0} is subclassed by:", type.FullName);
                foreach (Type t in subClasses)
                {
                    Console.WriteLine("\t{0}", t.FullName);
                }
            }
        }
		#endregion 

		#region "13.8 Finding All Serializable Types Within an Assembly"	
		public static void FindSerializableTypes()
		{
			FindSerializable();
		}


        public static void FindSerializable()
        {
            Process current = Process.GetCurrentProcess();
            // get the path of the current module
            string asmPath = current.MainModule.FileName;
            IEnumerable<string> typeNames = GetSerializableTypeNames(asmPath);
            // write out the serializable types in the assembly
            if (typeNames.Count() > 0)
            {
                Console.WriteLine("{0} has serializable types:", asmPath);
                foreach (string typeName in typeNames)
                {
                    Console.WriteLine("\t{0}", typeName);
                }
            }
        }

        public static IEnumerable<string> GetSerializableTypeNames(string asmPath)
        {
            Assembly asm = Assembly.LoadFrom(asmPath);
            return from type in asm.GetTypes()
                   where type.IsSerializable
                   select type.FullName;
        }
        #endregion 

        #region "13.9 Dynamically Invoking Members"
        public static void DynamicInvocation()
        {
            TestDynamicInvocation();
        }

        public static void TestDynamicInvocation()
        {
            XDocument xdoc = XDocument.Load(@"..\..\SampleClassLibrary\SampleClassLibraryTests.xml");
            DynamicInvoke(xdoc, @"SampleClassLibrary.dll");
        }

        public static void DynamicInvoke(XDocument xdoc, string asmPath)
        {
            var test = from t in xdoc.Root.Elements("Test")
                       select new
                       {
                           typeName = (string)t.Attribute("className").Value,
                           methodName = (string)t.Attribute("methodName").Value,
                           argument = from p in t.Elements("Argument")
                                       select new { arg = p.Value }
                       };

            // Load the assembly
            Assembly asm = Assembly.LoadFrom(asmPath);

            foreach (var elem in test)
            {
                // create the actual type
                Type dynClassType = asm.GetType(elem.typeName, true, false);

                // Create an instance of this type and verify that it exists
                object dynObj = Activator.CreateInstance(dynClassType);
                if (dynObj != null)
                {
                    // Verify that the method exists and get its MethodInfo obj
                    MethodInfo invokedMethod = dynClassType.GetMethod(elem.methodName);
                    if (invokedMethod != null)
                    {
                        // Create the argument list for the dynamically invoked methods
                        object[] arguments = new object[elem.argument.Count()];
                        int index = 0;

                        // for each parameter, add it to the list
                        foreach (var arg in elem.argument)
                        {
                            // get the type of the parameter
                            Type paramType =
                                invokedMethod.GetParameters()[index].ParameterType;

                            // change the value to that type and assign it
                            arguments[index] =
                                Convert.ChangeType(arg.arg, paramType);
                            index++;
                        }

                        // Invoke the method with the parameters
                        object retObj = invokedMethod.Invoke(dynObj, arguments);

                        Console.WriteLine("\tReturned object: " + retObj);
                        Console.WriteLine("\tReturned object: " + retObj.GetType().FullName);
                    }
                }
            }
        }
        #endregion

        #region "13.10 Determining If a Type or Method is Generic"

        // ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/P_System_Type_ContainsGenericParameters.htm
        // ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.VisualStudio.v80.en/dv_fxadvance/html/f93b03b0-1778-43fc-bc6d-35983d210e74.htm
        // ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.VisualStudio.v80.en/dv_fxadvance/html/a0c3c0c5-8178-4673-8107-9fdde780a669.htm


        public static void TestIsGeneric()
        {
            string path = GetProcessPath();

            // load the assembly
            Assembly asm = Assembly.LoadFrom(path);
            Console.WriteLine("Assembly: {0}:", path);

            //foreach (Type tp in asm.GetTypes())
            //{
            //    if (tp.FullName.StartsWith("CSharpRecipes.DataStructsAndAlgorithms+"))
            //        Console.WriteLine("Name == " + tp.FullName);
            //}
            //Console.WriteLine();
            //Console.WriteLine();

            // get the type
            Type t = asm.GetType("CSharpRecipes.DataStructsAndAlgorithms+PriorityQueue`1");

            bool genericType = IsGenericType(t);

            bool genericMethod = false;
            foreach (MethodInfo mi in t.GetMethods())
                genericMethod = IsGenericMethod(mi);
            GetGenericMemberInfo(t);
        }

        public static bool IsGenericType(Type type)
        {
            Console.WriteLine("Type HasGenericArguments: {0}", type.IsGenericType);

            return (type.IsGenericType);
        }

        public static bool IsGenericMethod(MethodInfo mi)
        {
            Console.WriteLine("Method Name: {0}", mi.Name);
            Console.WriteLine("Method HasGenericArguments: {0}", mi.IsGenericMethod);

            return (mi.IsGenericMethod);
        }

        public static void GetGenericMemberInfo(Type t)
        {

            Console.WriteLine("Type HasGenericArguments: {0}", t.IsGenericType);
            Console.WriteLine("Type IsGenericTypeDefinition: {0}", t.IsGenericTypeDefinition);

            Console.WriteLine("Type FullName: {0}", t.FullName);
            Console.WriteLine("Type ContainsGenericParameters: {0}", t.ContainsGenericParameters);
            Console.WriteLine("Type IsGenericParameter: {0}", t.IsGenericParameter);
            if (t.IsGenericParameter)
            {
                Console.WriteLine("Type DeclaringMethod.Name: {0}", t.DeclaringMethod.Name);
                Console.WriteLine("Type GenericParameterAttributes: {0}", t.GenericParameterAttributes);
                Console.WriteLine("Type GenericParameterPosition: {0}", t.GenericParameterPosition);
                foreach (Type innerType in t.GetGenericParameterConstraints())
                    Console.WriteLine("\tType GetGenericParameterConstraints().FullName: {0}", innerType.FullName);
            }
            foreach (Type innerType in t.GetGenericArguments())
                Console.WriteLine("\tType GetGenericArguments().FullName: {0}", innerType.FullName);
            Console.WriteLine("Type GetGenericTypeDefinition().FullName: {0}", t.GetGenericTypeDefinition().FullName);
            Console.WriteLine("Type Name: {0}", t.Name);
        }
        #endregion

		#region "13.11 Access Local Variables Information"
		public static void TestGetLocalVars()
		{
            string file = GetProcessPath();

			// Get all local var info for the CSharpRecipes.Reflection.GetLocalVars method
			System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo> vars = GetLocalVars(file, "CSharpRecipes.Reflection", "GetLocalVars");
		}

		public static System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo> GetLocalVars(string asmPath, string typeName, string methodName)
		{
			Assembly asm = Assembly.LoadFrom(asmPath);
			Type asmType = asm.GetType(typeName);
			MethodInfo mi = asmType.GetMethod(methodName);
			MethodBody mb = mi.GetMethodBody();

			System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo> vars = (System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo>)mb.LocalVariables;
			
			// Display information about each local variable
			foreach (LocalVariableInfo lvi in vars)
			{
				Console.WriteLine("IsPinned: " + lvi.IsPinned);
				Console.WriteLine("LocalIndex: " + lvi.LocalIndex);
				Console.WriteLine("LocalType.Module: " + lvi.LocalType.Module);
				Console.WriteLine("LocalType.FullName: " + lvi.LocalType.FullName);
				Console.WriteLine("ToString(): " + lvi.ToString());
			}
			
			return (vars);
		}
		#endregion

		#region "13.12 Creating a Generic Type"
		public static void CreateMultiMap()
		{
			// Get the type we want to construct
			Type typeToConstruct = Type.GetType("CSharpRecipes.DataStructsAndAlgorithms+MultiMap`2");
			// Get the type arguments we want to construct our type with
			Type[] typeArguments = {typeof(int), typeof(string)};
			// Bind these type arguments to our generic type
			Type newType = typeToConstruct.MakeGenericType(typeArguments);
			// Construct our type
			DataStructsAndAlgorithms.MultiMap<int, string> mm = (DataStructsAndAlgorithms.MultiMap<int, string>)Activator.CreateInstance(newType);
			
			// Test our newly constructed type
			Console.WriteLine("Count == " + mm.Count);
			mm.Add(1, "test1");
			Console.WriteLine("Count == " + mm.Count);
		}
		#endregion
		
		#region "13.B BONUS -- Access MSIL via Reflection
		public static void TestGetIL()
		{
            string file = GetProcessPath();

			byte[] IL = GetIL(file, "CSharpRecipes.Reflection", "DynamicInvocation");

			foreach (byte b in IL)
			{
				Console.WriteLine(string.Format("{0,00:00}", b));
				//Console.WriteLine(b.ToString("X"));
			}

			ResolveMember(IL);
		}

		public static byte[] GetIL(string asmPath, string typeName, string methodName)
		{
			Assembly asm = Assembly.LoadFrom(asmPath);
			Type asmType = asm.GetType(typeName);
			MethodInfo mi = asmType.GetMethod(methodName);

			foreach (MethodInfo testMI in asmType.GetMethods())
			{
				Console.WriteLine("\t-->  " + testMI.Name + "\t" + testMI.MetadataToken.ToString("X"));
			}

			MethodBody mb = mi.GetMethodBody();

			byte[] IL = mb.GetILAsByteArray();

			return (IL);
		}

		public static void ResolveMember(byte[] IL)
		{
			int currByte = 0;

			//Process current = Process.GetCurrentProcess();
			
			while (currByte < IL.Length)
			{
				switch (IL[currByte])
				{
					// A call instruction
					case 0x28:
						{
							// Get next four instrs
							int token = GetTokenFromBytes(IL[++currByte], IL[++currByte], IL[++currByte], IL[++currByte]);

							foreach (Module m in Assembly.GetExecutingAssembly().GetLoadedModules())
							{
								try
								{
									MemberInfo mi = m.ResolveMember(token);
									Console.WriteLine("Resolved to: " + mi.Name);
								}
								catch (ArgumentException)
								{
									// Not found in this module
								}
							}
							break;
						}
				}
				currByte++;
			}
		}

		public static int GetTokenFromBytes(byte b0, byte b1, byte b2, byte b3)
		{
			Console.WriteLine("\tb0 == " + b0.ToString("X"));
			Console.WriteLine("\tb1 == " + b1.ToString("X"));
			Console.WriteLine("\tb2 == " + b2.ToString("X"));
			Console.WriteLine("\tb3 == " + b3.ToString("X"));
			
			string token = b0.ToString("X");
			token = "0" + b1.ToString("X") + token;
			token = "0" + b2.ToString("X") + token;
			token = "0" + b3.ToString("X") + token;

			Console.WriteLine("MetaData Token(X): " + token.ToString());

			int intToken = int.Parse(token, System.Globalization.NumberStyles.HexNumber);

			Console.WriteLine("intToken == " + intToken.ToString("X"));

			return (intToken);
		}


		/*
				-->  DynamicInvocation  06000447
				-->  TestDynamicInvocation      06000448
		  
				00
				28	48	04	00	06
				0
				2A 
		  
		 
				ILDASM
				00  NOP
				28  (06)000448  CALL TestDynamicInvocation()
				00  NOP
				2A  RET
		*/

		/* THE ENQUEUE METHOD IL
			00   |                   nop
			02   |                   ldarg.0
			7B   | (0A)0005DF        ldfld
			03   |                   ldarg.1
			6F   | (0A)0001E1        callvirt
			00   |                   nop
			02   |                   ldarg.0
			7B   | (0A)0005DF        ldfld
			02   |                   ldarg.0
			7B   | (0A)0005E0        ldfld
			6F   | (0A)0005E8        callvirt
			00   |                   nop
			2A   |                   ret
		  
		  THIS METHOD RETURNS
			00
			02
			7B   | E0	05	00	0A
			03
			6F   |	E1	01	00	0A
			0
			2
			7B   |	E0	05	00	0A
			2
			7B   | 	E1	05	00	0A
			6F   | E9	05	00	0A
			0
			2A
		*/
		#endregion
	}
}
