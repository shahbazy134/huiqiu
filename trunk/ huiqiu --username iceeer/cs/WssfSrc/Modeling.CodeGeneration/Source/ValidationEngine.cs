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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.ServiceFactory.Validation;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using System.Globalization;
using System.IO;
using Microsoft.Practices.RecipeFramework.Services;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions;
using Microsoft.Practices.Modeling.Common;
using System.Reflection;

namespace Microsoft.Practices.Modeling.CodeGeneration
{
	public static class ValidationEngine
	{
		private static FileConfigurationSource configSource;
		private static ValidationContext currentValidationContext;
		private static ValidationMemory validationMemory = new ValidationMemory();
		private const string validationErrorCode = "Validation";
		private const string commonRuleSet = "Common";

		public static ValidationContext CurrentValidationContext
		{
			get { return currentValidationContext; }
		}

		private static IConfigurationSource GetConfig(ModelElement currentElement)
		{
			Guard.ArgumentNotNull(currentElement, "currentElement");

			if(configSource == null)
			{
				string rulePath = GetConfigurationRulePath();
				configSource = new FileConfigurationSource(rulePath);
			}
			return configSource;
		}

		public static string GetConfigurationRulePath()
		{
			return Path.Combine(new ConfigurationService(ServiceFactoryGuidancePackage.PackageGuid).BasePath, "ruleset.config");
		}

		public static void Validate(ValidationElementState state, ValidationContext context, ModelElement currentElement, string ruleSet)
		{
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(currentElement, "currentElement");
			Guard.ArgumentNotNull(ruleSet, "ruleSet");

			if(state == ValidationElementState.FirstElement)
			{
				validationMemory.Reset();
				//	Add all elements that the DSL validation will explicitly validate so that they don't
				//	get validated twice as the object graph gets walked.
				foreach(ModelElement element in context.ValidationSubjects)
				{
					validationMemory.Add(element.Id);
				}
			}
			currentValidationContext = context;

			Debug.WriteLine("Initiating validation on " + ModelElementToString(currentElement) + " with ruleset '" + ruleSet + "'...");
			try
			{
				if(validationMemory.IsValidated(currentElement.Id) &&
					(state != ValidationElementState.FirstElement))
				{
					Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "{0} Skipping", ModelElementToString(currentElement)));
					return;
				}

				IConfigurationSource config = ValidationEngine.GetConfig(currentElement);

				validationMemory.Add(currentElement.Id);

				DoValidate(config, currentElement, context, commonRuleSet);
				DoValidate(config, currentElement, context, ruleSet);
			}
			catch(Exception ex)
			{
				context.LogFatal(ex.ToString(), validationErrorCode, currentElement);
				throw;
			}
		}

		public static bool IsValidated(Guid elementId)
		{
			return validationMemory.IsValidated(elementId);
		}

		public static string ModelElementToString(ModelElement element)
		{
			Guard.ArgumentNotNull(element, "element");

			string name;
			DomainClassInfo.TryGetName(element, out name);
			name = name ?? "no name";
			string type = element.GetType().Name;
			return String.Format(CultureInfo.CurrentUICulture, "{0} '{1}' {2}", type, name, element.Id);
		}

		public static string GetUniquePropertyValue(object element, string propertyName)
		{
			Guard.ArgumentNotNull(element, "element");
			Guard.ArgumentNotNull(propertyName, "propertyName");

			PropertyInfo property = element.GetType().GetProperty(propertyName);
			if (property == null)
			{
				return null;
			}
			return property.GetValue(element, null) as string;
		}

		private static void WriteResultsToLog(ValidationContext context, ValidationResults results)
		{
			Debug.Assert(context != null);
			Debug.Assert(results != null);

			foreach(ValidationResult result in results)
			{
				ModelElement[] elements = new ModelElement[context.ValidationSubjects.Count];
				context.ValidationSubjects.CopyTo(elements, 0);
				context.LogError(result.Message, validationErrorCode, elements);
			}
		}

		private static void DoValidate(IConfigurationSource config, ModelElement element, ValidationContext context, string ruleSet)
		{
			Validator validator = ValidationFactory.CreateValidatorFromConfiguration(element.GetType(), ruleSet, config);

			ValidationResults results = validator.Validate(element);

			Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "{0} {1}", ModelElementToString(element), (results.IsValid ? "Succeeded" : "Failed")));

			if(!results.IsValid)
			{
				WriteResultsToLog(context, results);
			}
		}
	}
}