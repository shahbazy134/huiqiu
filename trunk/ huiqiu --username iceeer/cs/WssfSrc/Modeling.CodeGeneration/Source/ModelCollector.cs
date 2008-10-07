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
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.Modeling.CodeGeneration
{
	public static class ModelCollector
	{
		public static IArtifactLinkContainer GetArtifacts(Store store)
		{
			ArtifactLinkCollector collector = new ArtifactLinkCollector();
			foreach (ModelElement element in store.ElementDirectory.AllElements)
			{
				if (typeof(IExtensibleObject).IsAssignableFrom(element.GetType()))
				{
					IArtifactLinkContainer links = GetArtifacts(element);
					collector.Collect(links);
				}
			}
			return collector;
		}

		public static IArtifactLinkContainer GetArtifacts(ModelElement modelElement)
		{
			IArtifactLinkContainer links = null;
			if (modelElement is IArtifactLinkContainer)
			{
				links = (IArtifactLinkContainer)modelElement;
			}
			else if (modelElement is IExtensibleObject)
			{
				IExtensibleObject extendedObject = (IExtensibleObject)modelElement;
				links = extendedObject.ObjectExtender as IArtifactLinkContainer;
			}
			return links;
		}

		public static IArtifactLinkContainer GetArtifacts(IServiceProvider serviceProvider)
		{
			ShapeElement selectedShape = DomainModelHelper.GetSelectedShape(serviceProvider);
			if (selectedShape != null &&
				selectedShape.ModelElement != null)
			{
				if (selectedShape is Diagram)
				{
					return GetArtifacts(selectedShape.ModelElement.Store);
				}
				else
				{
					return GetArtifacts(selectedShape.ModelElement);
				}
			}

			ModelElement selectedElement = DomainModelHelper.GetSelectedElement(serviceProvider);
			if (selectedElement != null)
			{
				return GetArtifacts(selectedElement);
			}
			return null;
		}

	}
}
