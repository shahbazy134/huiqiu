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
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.Modeling.Dsl.Service;
using Microsoft.VisualStudio.Modeling;
using System.Diagnostics;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.Practices.ServiceFactory.DataContracts;
using Microsoft.Practices.ServiceFactory.ServiceContracts;
using Microsoft.Practices.ServiceFactory.HostDesigner;

namespace Microsoft.Practices.ServiceFactory.Validation
{
	/// <summary>
	/// Validate that the ServiceContract Model Serializer Type and the DataContract Message parts are compliant with the Technology
	/// </summary>
	[ConfigurationElementType(typeof(CustomValidatorData))]
	public class ImplementationTechnologyAndSerializerCrossModelValidator : CrossModelReferenceValidator
	{
		private const string crossModelReferenceValidatorMessageKeyName = "crossModelReferenceValidatorMessage";
		private string currentMessageTemplate;

		/// <summary>
		/// Initializes a new instance of the <see cref="CrossDataContractModelTIandPMTValidator"/> class.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		public ImplementationTechnologyAndSerializerCrossModelValidator(NameValueCollection attributes)
			: base(attributes)
		{
			if(attributes == null)
			{
				return;
			}

			currentMessageTemplate = String.IsNullOrEmpty(attributes.Get(crossModelReferenceValidatorMessageKeyName)) ?
				Resources.ServiceAndServiceImplementationTechnologyCrossModelValidator :
				attributes.Get(crossModelReferenceValidatorMessageKeyName);
		}

		/// <summary>
		/// Does the validate.
		/// </summary>
		/// <param name="objectToValidate">The object to validate.</param>
		/// <param name="currentTarget">The current target.</param>
		/// <param name="key">The key.</param>
		/// <param name="validationResults">The validation results.</param>
		protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
		{
			this.MessageTemplate = currentMessageTemplate;	

			base.DoValidate(objectToValidate, currentTarget, key, validationResults);

			if(!validationResults.IsValid)
			{
				return;
			}

			DataContractMessagePart part = currentTarget as DataContractMessagePart;

			if(part == null)
			{
				return;
			}

            if (part.Message.ServiceContractModel.ImplementationTechnology == null)
            {
                return;
            }

			string dcImplementationTechnology = GetDcImplementationTechnology(currentTarget as ModelElement, objectToValidate);

			if(IsASMX(part.Message.ServiceContractModel.ImplementationTechnology.Name))
			{
				if(part.Message.ServiceContractModel.SerializerType.Equals(SerializerType.XmlSerializer))
				{
					if(IsWCF(dcImplementationTechnology))
					{
						this.LogValidationResult(
							validationResults,
							string.Format(CultureInfo.CurrentCulture, Resources.ImplementationTechnologyAndSerializerValidatorMessage, part.Name),
							currentTarget,
							key);
					}
				}
			}
			else if(IsWCF(part.Message.ServiceContractModel.ImplementationTechnology.Name))
			{
                if (part.Message.ServiceContractModel.SerializerType.Equals(SerializerType.DataContractSerializer))
                {
                    if (IsASMX(dcImplementationTechnology))
                    {
                        this.LogValidationResult(
                            validationResults,
                            string.Format(CultureInfo.CurrentCulture, Resources.ImplementationTechnologyAndSerializerValidatorMessage, part.Name),
                            currentTarget,
                            key);
                    }
                }
                else
                {
                    if (IsWCF(dcImplementationTechnology))
                    {
                        this.LogValidationResult(
                            validationResults,
                            Resources.InvalidExtensionAndSerializerCrossModelMessage,
                            currentTarget,
                            key);
                    }
                }
			}
		}

		/// <summary>
		/// Gets the default message template.
		/// </summary>
		/// <value>The default message template.</value>
		protected override string DefaultMessageTemplate
		{
			get { return Resources.ServiceAndServiceImplementationTechnologyCrossModelValidator; }
		}

		private bool IsASMX(string implementationTechnology)
		{
			return implementationTechnology.StartsWith("ASMX", StringComparison.OrdinalIgnoreCase);
		}

		private bool IsWCF(string implementationTechnology)
		{
			return implementationTechnology.StartsWith("WCF", StringComparison.OrdinalIgnoreCase);
		}

		private string GetDcImplementationTechnology(ModelElement modelElement, string moniker)
		{
			ModelElement referencedElement = base.ResolveReference(modelElement, moniker);

			foreach(ModelElement mel in referencedElement.Store.ElementDirectory.AllElements)
			{
				if(mel is DataContractModel)
				{
					DataContractModel dcm = (DataContractModel)mel;

                    if (dcm.ImplementationTechnology == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return dcm.ImplementationTechnology.Name;
                    }
				}
			}

			return string.Empty;
		}
	}
}
