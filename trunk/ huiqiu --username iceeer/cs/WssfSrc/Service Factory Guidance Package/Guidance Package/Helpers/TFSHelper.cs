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
using EnvDTE;
using System.IO;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.Helpers
{
	/// <summary>
	/// Helper methods for Team Fundation System operations.
	/// </summary>
	public static class TFSHelper
	{
		/// <summary>
		/// Checks the out file.
		/// </summary>
		/// <param name="vs">The vs.</param>
		/// <param name="filePath">The file path.</param>
		public static void CheckOutFile(_DTE vs, string filePath)
		{
			Guard.ArgumentNotNull(vs, "vs");
			Guard.ArgumentNotNullOrEmptyString(filePath, "filePath");

			if (File.Exists(filePath))
			{
				if (vs.SourceControl.IsItemUnderSCC(filePath) &&
					!vs.SourceControl.IsItemCheckedOut(filePath))
				{
					bool checkout = vs.SourceControl.CheckOutItem(filePath);
					if (!checkout)
					{
						throw new CheckoutException(
							string.Format(CultureInfo.CurrentCulture, Properties.Resources.CheckoutException, filePath));
					}
				}
				else
				{
					// perform an extra check if the file is read only.
					if (IsReadOnly(filePath))
					{
						ResetReadOnly(filePath);
					}
				}
			}
		}

		private static bool IsReadOnly(string path)
		{
			return (File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
		}

		private static void ResetReadOnly(string path)
		{
			File.SetAttributes(path, File.GetAttributes(path) ^ FileAttributes.ReadOnly);
		}
	}
}
