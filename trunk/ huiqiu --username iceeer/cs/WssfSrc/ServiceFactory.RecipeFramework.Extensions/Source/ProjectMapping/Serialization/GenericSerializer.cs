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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.VisualStudio.Shell.Design.Serialization;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Serialization
{
	public static class GenericSerializer
	{
		#region Public Implementation
		/// <summary>
		/// Deserializes the specified data.
		/// </summary>
		/// <param name="data">The string representation of the type.</param>
		/// <returns></returns>
		public static T Deserialize<T>(string data)
		{
			Guard.ArgumentNotNullOrEmptyString(data, "data");

			Type[] types = { };
			return Deserialize<T>(types, data);
		}

		/// <summary>
		/// Deserializes the specified types.
		/// </summary>
		/// <param name="types">The types.</param>
		/// <param name="data">The string representation of the type.</param>
		/// <returns></returns>
		public static T Deserialize<T>(Type[] types, string data)
		{
			Guard.ArgumentNotNull(types, "types");
			Guard.ArgumentNotNullOrEmptyString(data, "data");

			T obj;

			XmlSerializer serializer = new XmlSerializer(typeof(T), types);

			using(MemoryStream stream = new MemoryStream())
			{
				StreamWriter writer = new StreamWriter(stream);
				writer.Write(data);
				writer.Flush();
				stream.Position = 0;
				obj = (T)serializer.Deserialize(stream);
			}

			return obj;
		}

		/// <summary>
		/// Deserializes the specified file info.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <returns></returns>
		// FXCOP: FileInfo is more appropriate here as fileInfo refers to a file not a directory.
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static T Deserialize<T>(FileInfo fileInfo)
		{
			Guard.ArgumentNotNull(fileInfo, "fileInfo");

			Type[] types = { };
			return Deserialize<T>(types, fileInfo);
		}

		/// <summary>
		/// Deserializes the specified types.
		/// </summary>
		/// <param name="types">The types.</param>
		/// <param name="fileInfo">The file info.</param>
		/// <returns></returns>
		// FXCOP: FileInfo is more appropriate here as fileInfo refers to a file not a directory.
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static T Deserialize<T>(Type[] types, FileInfo fileInfo)
		{
			Guard.ArgumentNotNull(types, "types");
			Guard.ArgumentNotNull(fileInfo, "fileInfo");

			XmlSerializer serializer = new XmlSerializer(typeof(T), types);

			if(File.Exists(fileInfo.FullName))
			{
				using(StreamReader reader = new StreamReader(fileInfo.FullName))
				{
					return (T)serializer.Deserialize(reader);
				}
			}

			return default(T);
		}

		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
        [SuppressMessage("Microsoft.Naming","CA1720:IdentifiersShouldNotContainTypeNames")]
		public static string Serialize<T>(object obj)
		{
			Guard.ArgumentNotNull(obj, "obj");

			Type[] types = { };
			return Serialize<T>(obj, types);
		}

		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="types">The types.</param>
		/// <returns></returns>
        [SuppressMessage("Microsoft.Naming","CA1720:IdentifiersShouldNotContainTypeNames")]
		public static string Serialize<T>(object obj, Type[] types)
		{
			Guard.ArgumentNotNull(obj, "obj");
			Guard.ArgumentNotNull(types, "types");

			string text;
			XmlSerializer serializer = new XmlSerializer(typeof(T), types);

			using(MemoryStream stream = new MemoryStream())
			{
				serializer.Serialize(stream, obj);
				stream.Position = 0;
				text = new StreamReader(stream).ReadToEnd();
			}

			return text;
		}

		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="fileInfo">The file info.</param>
		// FXCOP: FileInfo is more appropriate here as fileInfo refers to a file not a directory.
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
		public static void Serialize<T>(object obj, FileInfo fileInfo, IServiceProvider serviceProvider)
		{
			Guard.ArgumentNotNull(obj, "obj");
			Guard.ArgumentNotNull(fileInfo, "fileInfo");

			Type[] types = { };
			Serialize<T>(obj, types, fileInfo, serviceProvider);
		}

		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="types">The types.</param>
		/// <param name="fileInfo">The file info.</param>
		// FXCOP: FileInfo is more appropriate here as fileInfo refers to a file not a directory.
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
		public static void Serialize<T>(object obj, Type[] types, FileInfo fileInfo, IServiceProvider serviceProvider)
		{
			Guard.ArgumentNotNull(obj, "obj");
			Guard.ArgumentNotNull(types, "types");
			Guard.ArgumentNotNull(fileInfo, "fileInfo");

			string text = Serialize<T>(obj, types);

			if(serviceProvider != null)
			{
				using (DocData docData = new DocData(serviceProvider, fileInfo.FullName))
				{
					docData.CheckoutFile(serviceProvider);

					File.WriteAllText(fileInfo.FullName, text);
				}
			}
			else
			{
				File.WriteAllText(fileInfo.FullName, text);
			}
		}
		#endregion
	}
}