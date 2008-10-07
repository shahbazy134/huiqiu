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

using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Microsoft.Practices.ServiceFactory.DataContracts
{
	internal partial class DataContractDslCommandSet
	{
		internal override void OnMenuValidate(object sender, EventArgs e)
		{
			if(this.CurrentDataContractDslDocData != null && this.CurrentDataContractDslDocData.Store != null)
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

				this.CurrentDataContractDslDocData.ValidationController.Validate(elementList, ValidationCategories.Menu);
			}
		}

		internal override void OnMenuValidateModel(object sender, EventArgs e)
		{
			if(this.CurrentDataContractDslDocData != null && this.CurrentDataContractDslDocData.Store != null)
			{
				List<ModelElement> elementList = new List<ModelElement>();
				FullDepthElementWalker elementWalker = new FullDepthElementWalker(new ModelElementVisitor(elementList), new EmbeddingReferenceVisitorFilter(), false);

				DataContractModel model = this.CurrentDataContractDslDocData.Store.ElementDirectory.FindElements<DataContractModel>()[0];

				elementWalker.DoTraverse(model);

				this.CurrentDataContractDslDocData.ValidationController.Validate(elementList, ValidationCategories.Menu);
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
