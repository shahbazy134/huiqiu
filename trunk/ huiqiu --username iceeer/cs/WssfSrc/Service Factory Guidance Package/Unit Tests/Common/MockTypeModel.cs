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
using System.Reflection;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace ServicesGuidancePackage.Tests.Common
{
	public class MockTypeModel : ITypeModel
	{
		private string fullName;
		private string name;
		private List<string> attributes = new List<string>();
        private List<string> members = new List<string>();
		private bool isClass;
		private bool isInterface;
		private bool isPublic;
        private object typeModel;

        public MockTypeModel() : this(new object())
        {
        }

        public MockTypeModel(object typeModel)
        {
            this.typeModel = typeModel;
            if(typeModel != null)
            {
                Type type = typeModel.GetType();
                this.isClass = type.IsClass;
                this.isInterface = type.IsInterface;
                this.isPublic = type.IsPublic;
                fullName = type.FullName;
                name = type.Name;
                foreach (object attribute in type.GetCustomAttributes(true))
                {
                    attributes.Add(attribute.GetType().FullName);
                }
                foreach (MemberInfo member in type.GetMembers())
                {
                    members.Add(member.Name);
                }
            }
        }

        #region ITypeModel Members

        public string FullName
		{
			get { return fullName; }
			set { fullName = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool HasAttribute(string attributeFullName)
		{
			return attributes.Contains(attributeFullName);
		}

		public bool HasAttribute(string attributeFullName, bool inherited)
		{
			return attributes.Contains(attributeFullName);
		}

        public bool HasAttributePropertyValue(string attributeFullName, string propertyName, object propertyValue)
        {
            if(HasAttribute(attributeFullName))
            {
                return InternalHasAttributePropertyValue(typeModel.GetType(), attributeFullName, propertyName, propertyValue);           
            }
            foreach (Type interfaceType in typeModel.GetType().GetInterfaces())
            {
                if (InternalHasAttributePropertyValue(interfaceType, attributeFullName, propertyName, propertyValue))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasMember(string memberName)
        {
            return members.Contains(memberName);
        }

		public void AddAttribute(string name)
		{
			attributes.Add(name);
		}

        public void AddMember(string name)
        {
            members.Add(name);
        }

		public bool IsClass
		{
			get { return isClass; }
			set { isClass = value; }
		}

		public bool IsInterface
		{
			get { return isInterface; }
			set { isInterface = value; }
		}

		public bool IsPublic
		{
			get { return isPublic; }
			set { isPublic = value; }
		}

        public object TypeModel
        {
            get { return typeModel; }
        }

        #endregion

        private bool InternalHasAttributePropertyValue(Type type,
            string attributeFullName, string propertyName, object propertyValue)
        {
            foreach (Attribute attribute in type.GetCustomAttributes(true))
            {
                PropertyInfo property = attribute.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    return property.GetValue(attribute, new object[0]).Equals(propertyValue);
                }
            }
            return false;
        }
    }
}
