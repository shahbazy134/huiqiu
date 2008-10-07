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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Serialization;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests
{
	/// <summary>
	/// Summary description for GenericSerializerFixture
	/// </summary>
	[TestClass]
	public class GenericSerializerFixture
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotSerializeWithNullFirstParameter()
		{
			GenericSerializer.Serialize<SerializableType>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotSerializeWithAllParameterNull()
		{
			GenericSerializer.Serialize<SerializableType>(null, null, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotSerializeWithFirstParameterNullAndSecondParameterValid()
		{
			Type[] types = { };
			GenericSerializer.Serialize<SerializableType>(null, types);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeSerializeWithNullFirstParameter()
		{
			string dummy = null;
			GenericSerializer.Deserialize<SerializableType>(dummy);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeSerializeWithBothParameterNull()
		{
			string dummy = null;
			Type[] types = null;
			GenericSerializer.Deserialize<SerializableType>(types, dummy);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeSerializeWithFirstParameterNullAndSecondParameterValid()
		{
			string dummy = null;
			Type[] types = { };
			GenericSerializer.Deserialize<SerializableType>(types, dummy);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeSerializeWithInvalidParameters4()
		{
			string dummy = "dummy";
			Type[] types = null;
			GenericSerializer.Deserialize<SerializableType>(types, dummy);
		}

		[TestMethod]
		public void ShouldSerializeASerializableType()
		{
			SerializableType type = new SerializableType();
			type.Field = 1;

			string typeRepresentation = Serialize<SerializableType>(type);
			string typeRepresentation1 = GenericSerializer.Serialize<SerializableType>(type);

			Assert.AreEqual(typeRepresentation, typeRepresentation1, "Not Equal");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldNotSerializeANotSerializableType()
		{
			NotSerializableType type = new NotSerializableType();
			type.Field = new Hashtable();

			string typeRepresentation = GenericSerializer.Serialize<NotSerializableType>(type);
		}

		[TestMethod]
		public void ShouldSerializeASerializableTypeWithOtherReferences()
		{
			Type[] types = { typeof(SerializableType1) };
			SerializableType2 type = new SerializableType2();
			SerializableType1 type1 = new SerializableType1();
			type1.Field1 = 1;
			type.Field = type1;

			string typeRepresentation = Serialize<SerializableType2>(type, types);
			string typeRepresentation1 = GenericSerializer.Serialize<SerializableType2>(type, types);
			Assert.AreEqual(typeRepresentation, typeRepresentation1, "Not Equal");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldNotSerializeASerializableTypeWithoutSpecifyingReferences()
		{
			SerializableType2 type = new SerializableType2();
			SerializableType1 type1 = new SerializableType1();
			type1.Field1 = 1;
			type.Field = type1;

			string typeRepresentation = GenericSerializer.Serialize<SerializableType2>(type);
		}

		[TestMethod]
		public void ShouldDeSerializeASerializableType()
		{
			SerializableType type = new SerializableType();
			type.Field = 1;
			string typeRepresentation = GenericSerializer.Serialize<SerializableType>(type);

			SerializableType type1 = GenericSerializer.Deserialize<SerializableType>(typeRepresentation);

			Assert.AreEqual(type.Field, type1.Field, "Not Equal");
		}

		[TestMethod]
		public void ShouldDeSerializeASerializableTypeWithOtherReferences()
		{
			Type[] types = { typeof(SerializableType1) };
			SerializableType2 type = new SerializableType2();
			SerializableType1 type1 = new SerializableType1();
			type1.Field1 = 1;
			type.Field = type1;
			string typeRepresentation = GenericSerializer.Serialize<SerializableType2>(type, types);

			SerializableType2 type3 = GenericSerializer.Deserialize<SerializableType2>(types, typeRepresentation);

			Assert.AreEqual(type.Field.GetType(), type3.Field.GetType(), "Not Equal");
		}

		[TestMethod]
		public void ShouldNotDeSerializeASerializableTypeWithoutSpecifyingReferences()
		{
			Type[] types = { typeof(SerializableType1) };
			SerializableType2 type = new SerializableType2();
			SerializableType1 type1 = new SerializableType1();
			type1.Field1 = 1;
			type.Field = type1;
			string typeRepresentation = GenericSerializer.Serialize<SerializableType2>(type, types);

			SerializableType2 type3 = GenericSerializer.Deserialize<SerializableType2>(typeRepresentation);

			Assert.AreNotEqual(type.Field.GetType(), type3.Field.GetType(), "Equal");
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void DeserializationShouldReturnNullIfFileDoesntExist()
		{
			FileInfo info = new FileInfo(Path.Combine(Environment.CurrentDirectory, @"FooDumy.foo"));
			SerializableType serializableType = GenericSerializer.Deserialize<SerializableType>(info);

			Assert.IsNull(serializableType, "Not null");
		}

		[TestMethod]
		public void ShouldSerializeToFile()
		{
			SerializableType type = new SerializableType();
			type.Field = 1;
			FileInfo info = new FileInfo(@"C:\FooDumy.foo");
			string typeRepresentation = Serialize<SerializableType>(type);
			GenericSerializer.Serialize<SerializableType>(type, info, null);

			Assert.IsTrue(File.Exists(info.FullName), "File Doesn't exist");

			string typeRepresentation1 = File.ReadAllText(info.FullName);

			Assert.AreEqual(typeRepresentation, typeRepresentation1, "Not Equal");

			File.Delete(info.FullName);
		}

		[TestMethod]
		public void ShouldSerializeAndDeserializeFromFile()
		{
			SerializableType type = new SerializableType();
			type.Field = 1;
			FileInfo info = new FileInfo(@"C:\FooDumy.foo");

			GenericSerializer.Serialize<SerializableType>(type, info, null);

			Assert.IsTrue(File.Exists(info.FullName), "File Doesn't exist");

			SerializableType type1 = GenericSerializer.Deserialize<SerializableType>(info);

			Assert.AreEqual(type.Field, type1.Field, "Not Equal");

			File.Delete(info.FullName);
		}

		#region Private Implementation
		private string Serialize<TypeToSerialize>(TypeToSerialize obj)
		{
			Type[] types = { };
			return Serialize<TypeToSerialize>(obj, types);
		}

		private string Serialize<TypeToSerialize>(TypeToSerialize obj, Type[] types)
		{
			string text;
			XmlSerializer serializer = new XmlSerializer(typeof(TypeToSerialize), types);

			using(MemoryStream stream = new MemoryStream())
			{
				serializer.Serialize(stream, obj);
				stream.Position = 0;
				text = new StreamReader(stream).ReadToEnd();
			}

			return text;
		}

		private TypeToSerialize DeSerialize<TypeToSerialize>(string representation)
		{
			Type[] types = { };
			return DeSerialize<TypeToSerialize>(representation, types);
		}

		private TypeToSerialize DeSerialize<TypeToSerialize>(string representation, Type[] types)
		{
			TypeToSerialize obj;

			XmlSerializer serializer = new XmlSerializer(typeof(TypeToSerialize), types);

			using(MemoryStream stream = new MemoryStream())
			{
				StreamWriter writer = new StreamWriter(stream);
				writer.Write(representation);
				writer.Flush();
				stream.Position = 0;
				obj = (TypeToSerialize)serializer.Deserialize(stream);
			}

			return obj;
		}
		#endregion

		#region Dummy classes
		[Serializable]
		public class SerializableType
		{
			private int field;

			public int Field
			{
				get { return field; }
				set { field = value; }
			}
		}

		[Serializable]
		public class SerializableType2
		{
			private object field;

			public object Field
			{
				get { return field; }
				set { field = value; }
			}
		}

		[Serializable]
		public class SerializableType1
		{
			private int field1;

			public int Field1
			{
				get { return field1; }
				set { field1 = value; }
			}
		}

		public class NotSerializableType
		{
			private Hashtable field;

			public Hashtable Field
			{
				get { return field; }
				set { field = value; }
			}
		}
		#endregion
	}
}