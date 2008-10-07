//===============================================================================
// Microsoft patterns & practices
// Web Service Software Factory
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Microsoft.Practices.FxCop.Rules.WcfSemantic
{
    /// <summary/>
    public static class Utilities
    {
        /// <summary>
        /// Returns the rules assembly file in FxCop rules folder or the assembly location.
        /// </summary>
        /// <returns></returns>
        public static string GetRulesAssemblyLocation()
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // check of we find this asm in the FxCop rules folder
            string rulesFolder = Path.GetFullPath(Path.Combine(
                Environment.GetEnvironmentVariable("VS90COMNTOOLS", EnvironmentVariableTarget.Process),
                "..\\..\\Team Tools\\Static Analysis Tools\\FxCop\\Rules"));

            if (Directory.Exists(rulesFolder))
            {
                string rulesAsm = Path.Combine(rulesFolder, Path.GetFileName(thisAssemblyPath));
                if (File.Exists(rulesAsm))
                {
                    return rulesAsm;
                }

            }
            return thisAssemblyPath;
        }
    }
}
