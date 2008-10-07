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
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.Practices.ServiceFactory.ServiceContracts
{
	internal partial class ServiceContractDslCommandSet
	{
		internal override void OnMenuValidate(object sender, EventArgs e)
		{
			if(this.CurrentServiceContractDslDocData != null && this.CurrentServiceContractDslDocData.Store != null)
			{
				List<ModelElement> elementList = new List<ModelElement>();
				FullDepthElementWalker elementWalker = new FullDepthElementWalker(new ModelElementVisitor(elementList), new EmbeddingReferenceVisitorFilter(), false);
				foreach(object selectedObject in this.CurrentSelection)
				{
					// Build list of elements embedded beneath the selected root.
					ModelElement element = GetValidationTarget(selectedObject);
					if(element != null && !elementList.Contains(element))
					{
						elementWalker.DoTraverse(element);
					}
				}

				this.CurrentServiceContractDslDocData.ValidationController.Validate(elementList, ValidationCategories.Menu);
			}
		}

		internal override void OnMenuValidateModel(object sender, EventArgs e)
		{
			if(this.CurrentServiceContractDslDocData != null && this.CurrentServiceContractDslDocData.Store != null)
			{
				List<ModelElement> elementList = new List<ModelElement>();
				FullDepthElementWalker elementWalker = new FullDepthElementWalker(new ModelElementVisitor(elementList), new EmbeddingReferenceVisitorFilter(), false);

				ServiceContractModel model = this.CurrentServiceContractDslDocData.Store.ElementDirectory.FindElements<ServiceContractModel>()[0];

				elementWalker.DoTraverse(model);

				this.CurrentServiceContractDslDocData.ValidationController.Validate(elementList, ValidationCategories.Menu);
			}
		}

		private static ModelElement GetValidationTarget(object selectedObject)
		{
			ModelElement element = null;
			PresentationElement presentation = selectedObject as PresentationElement;
			if(presentation != null)
			{
				if(presentation.Subject != null)
				{
					element = presentation.Subject;
				}
			}
			else
			{
				element = selectedObject as ModelElement;
			}
			return element;
		}
	}
}