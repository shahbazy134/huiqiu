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
using System.Text;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.Modeling.Dsl.Service;

namespace Microsoft.Practices.Modeling.Dsl.Integration.Helpers
{
    /// <summary>
    /// Represents the method that loads the domain model of type T from a serialized file.
    /// </summary>
    public delegate T ModelLoader<T>(string modelPath, Store store, SerializationResult result) where T : ModelElement;

	/// <summary>
	/// Domain model helper class.
	/// </summary>
	public static class DomainModelHelper
	{
		/// <summary>
		/// Gets the surface area.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static bool IsDiagramSelected(IServiceProvider provider)
		{
			DiagramDocView docView = DesignerHelper.GetDiagramDocView(provider);

			if(docView != null)
			{
				if(docView.SelectionCount == 1)
				{
					foreach(object component in docView.GetSelectedComponents())
					{
						return component is Diagram;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the selected concept.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static TConcept GetSelectedConcept<TConcept>(IServiceProvider provider) where TConcept : class
		{
			ModelElement element = GetSelectedElement(provider);

			if(element is TConcept)
			{
				return element as TConcept;
			}

			return default(TConcept);
		}

		/// <summary>
		/// Gets the selected element.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static ModelElement GetSelectedElement(IServiceProvider provider)
		{
			ModelingDocView docView = DesignerHelper.GetModelingDocView(provider);

			if (docView != null)
			{
				if (docView.SelectionCount == 1)
				{
					foreach (object component in docView.GetSelectedComponents())
					{
						ShapeElement selectionShape = component as ShapeElement;
						ModelElement selectionElement = component as ModelElement;
						
						if (selectionShape != null)
						{
							return selectionShape.ModelElement;
						}
						else if (selectionElement != null)
						{
							return selectionElement;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the selected shape.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static ShapeElement GetSelectedShape(IServiceProvider provider)
		{
			DiagramDocView docView = DesignerHelper.GetDiagramDocView(provider);

			if(docView != null)
			{
				if(docView.SelectionCount == 1)
				{
					foreach(object component in docView.GetSelectedComponents())
					{
						ShapeElement selectedShape = component as ShapeElement;

						if(selectedShape != null)
						{
							return selectedShape;
						}
					}
				}
			}

			return null;
		}
		 
		/// <summary>
		/// Gets the selected elements.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static IList<object> GetSelectedElements(IServiceProvider provider)
		{
			ModelingDocView docView = DesignerHelper.GetModelingDocView(provider);

			if(docView != null)
			{
				if(docView.SelectionCount > 0)
				{
					IList<object> elements = new List<object>(docView.SelectionCount);

					foreach(object component in docView.GetSelectedComponents())
					{
						ShapeElement selectedShape = component as ShapeElement;
						ModelElement selectedElement = component as ModelElement;
						if (selectedShape != null)
						{
							elements.Add(selectedShape.ModelElement);
						}
						else if (selectedElement != null)
						{
							elements.Add(selectedElement);
						}
					}

					return elements;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the element.
		/// </summary>
		/// <param name="store">The store.</param>
		/// <returns></returns>
		public static T GetElement<T>(Store store) where T : ModelElement
		{
			T element = default(T);
			foreach (ModelElement mel in store.ElementDirectory.AllElements)
			{
				if (mel is T)
				{
					element = (T)mel;
					break;
				}
			}
			return element;
		}

		/// <summary>
		/// Gets the element.
		/// </summary>
		/// <param name="store">The store.</param>
		/// <param name="elementId">The element id.</param>
		/// <returns></returns>
		public static T GetElement<T>(Store store, Guid elementId) where T : ModelElement
		{
			return (T)store.ElementDirectory.GetElement(elementId);
		}

		/// <summary>
		/// Loads the domain model serialized in the specified path.
		/// </summary>
		/// <param name="modelPath">The model path.</param>
		/// <param name="loader">The model loader.</param>
		/// <returns></returns>
		public static TModel LoadModel<TDomainModel, TModel>(string modelPath, ModelLoader<TModel> loader)
			where TDomainModel : DomainModel
			where TModel : ModelElement
		{
			Store store = new Store(typeof(CoreDesignSurfaceDomainModel), typeof(TDomainModel));
			SerializationResult result = new SerializationResult();

			using (Transaction transaction = store.TransactionManager.BeginTransaction("Load", true))
			{
				TModel model = loader(modelPath, store, result);
				if (result.Failed)
				{
					throw new SerializationException(result);
				}
				transaction.Commit();
				return model;
			}
		}

		/// <summary>
		/// Add/update the linked element and remove any duplicate.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="element">The element.</param>
		/// <param name="renameOnExisting">if set to <c>true</c> [rename on existing].</param>
		public static void AddLinkedElement<T>(LinkedElementCollection<T> collection, T element)
			where T : ModelElement
		{
			RemoveDuplicateElement(collection, element);
			collection.Add(element);
		}

		/// <summary>
		/// Removes the duplicate element.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="element">The element.</param>
		/// <returns>True if a duplicate element was removed, False otherwise.</returns>
		public static bool RemoveDuplicateElement<T>(LinkedElementCollection<T> collection, T element)
			where T : ModelElement
		{
			T found = null;
			string name = GetNameProperty(element);
			// try finding a duplicate element (same name and != Id)
			if (!string.IsNullOrEmpty(name))
			{
				found = collection.Find(delegate(T match)
				{
					return name.Equals(GetNameProperty(match), StringComparison.OrdinalIgnoreCase); //&& match.Id != element.Id;
				});
			}

			if (found != null)
			{
				// Remove the duplicate
				collection.Remove(found);
				return true;
			}

			return false;
		}

		public static ModelElement ResolveModelReference(IDslIntegrationService dslIntegrationService, string moniker, bool openDocumentIfNeeded)
		{
			Guard.ArgumentNotNull(dslIntegrationService, "dslIntegrationService");
			Guard.ArgumentNotNull(moniker, "moniker");

			string validMoniker;
			string validMonikerData;
			string projectName;

			if (!IsModelReferenceValid(moniker, out validMoniker, out validMonikerData, out projectName))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, Properties.Resources.InvalidMelMonikerFormat, moniker));
			}

			try
			{
				return dslIntegrationService.ResolveExportedInstanceInDocument(validMonikerData, validMoniker, openDocumentIfNeeded);
			}
			catch (NullReferenceException)
			{
				// the project or the model file is missing or does not exists
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, Properties.Resources.ProjectNotFoundInMelMoniker, projectName, moniker));
			}
		}

		public static bool IsModelReferenceValid(string moniker)
		{
			string validMoniker;
			string validMonikerData;
			string projectName;

			return IsModelReferenceValid(moniker, out validMoniker, out validMonikerData, out projectName);
		}

		private static string GetNameProperty(ModelElement element)
		{
			PropertyInfo property = element.GetType().GetProperty("Name");
			if (property == null)
			{
				return null;
			}
			return property.GetValue(element, null) as string;
		}

		private static bool IsModelReferenceValid(string moniker, 
			out string validMoniker, out string validMonikerData, out string projectName)
		{
			validMoniker = null;
			validMonikerData = null;
			projectName = null;

			// filter out the schema part
			moniker = moniker.Replace("mel://", string.Empty);

			//Moniker format:
			//mel://[DSLNAMESPACE]\[MODELELEMENTTYPE]\[MODELELEMENT]@[PROJECT]\[MODELFILE]
			string[] data = moniker.Split('@');
			if (data.Length != 2)
			{
				return false;
			}

			string[] modelData = data[0].Split('\\');
			if (modelData.Length != 3)
			{
				return false;
			}

			string[] locationData = data[1].Split('\\');
			if (locationData.Length != 2)
			{
				return false;
			}

			validMoniker = moniker;
			validMonikerData = modelData[0];
			projectName = locationData[0];
			return true;
		}
	}
}
