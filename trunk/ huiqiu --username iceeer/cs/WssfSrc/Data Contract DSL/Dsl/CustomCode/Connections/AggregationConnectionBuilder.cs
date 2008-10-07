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
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.DataContracts
{
	public static partial class AggregationConnectionBuilder
	{
		public static bool CanAcceptSource(ModelElement candidate)
		{
			if(candidate == null)
			{
				return false;
			}
			else if(candidate is DataContractBase)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool CanAcceptTarget(ModelElement candidate)
		{
			if(candidate == null)
			{
				return false;
			}
			else if(candidate is DataContract ||
				candidate is FaultContract)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool CanAcceptSourceAndTarget(ModelElement candidateSource, ModelElement candidateTarget)
		{
			if(candidateSource == null)
			{
				return false;
			}

			bool acceptSource = CanAcceptSource(candidateSource);
			// If the source wasn't accepted then there's no point checking targets.
			// If there is no target then the source controls the accept.
			if(!acceptSource || candidateTarget == null)
			{
				return acceptSource;
			}
			else // Check combinations
			{
				if(candidateSource is DataContractBase)
				{
					if(candidateTarget is DataContract ||
						candidateTarget is FaultContract)
					{
						if(HasNullReferences((DataContractBase)candidateSource, (Contract)candidateTarget))
						{
							return false;
						}
						
						return true;
					}
				}
			}
			return false;
		}

		private static bool HasNullReferences(DataContractBase candidateSource, Contract candidateTarget)
		{
			return candidateSource == null || candidateTarget == null;
		}

		public static ElementLink Connect(ModelElement source, ModelElement target)
		{
			return Connect(source, target, null);
		}

		public static ElementLink Connect(ModelElement source, ModelElement target, string targetDataElementName)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(target == null)
			{
				throw new ArgumentNullException("target");
			}

			if(CanAcceptSourceAndTarget(source, target))
			{
				if(source is DataContractBase)
				{
					if(target is DataContract ||
						target is FaultContract)
					{
						DataContractBase sourceAccepted = (DataContractBase)source;
						Contract targetAccepted = (Contract)target;
						ElementLink result = new DataContractBaseCanBeContainedOnContracts(sourceAccepted, targetAccepted);
						if(DomainClassInfo.HasNameProperty(result))
						{
							DomainClassInfo.SetUniqueName(result);
						}

						using(Transaction transaction = targetAccepted.Store.TransactionManager.BeginTransaction())
						{
							ModelElementReference dataElement = target.Store.ElementFactory.CreateElement(ModelElementReference.DomainClassId) as ModelElementReference;
							dataElement.ModelElementGuid = result.Id;
							dataElement.Type = sourceAccepted.Name;

							if(target is DataContract)
							{
								dataElement.Name = GetDataElementName(
										sourceAccepted.Name,
										targetAccepted.Name,
										((DataContract)target).DataMembers,
										targetDataElementName);

								((DataContract)target).DataMembers.Add(dataElement);
							}
							else if(target is FaultContract)
							{
								dataElement.Name = GetDataElementName(
										sourceAccepted.Name,
										targetAccepted.Name,
										((FaultContract)target).DataMembers,
										targetDataElementName);

								((FaultContract)target).DataMembers.Add(dataElement);
							}

							transaction.Commit();
						}

						return result;
					}
				}

			}
			
			Debug.Fail("Having agreed that the connection can be accepted we should never fail to make one.");
			throw new InvalidOperationException();
		}

		private static string GetDataElementName(string sourceName, string targetName, LinkedElementCollection<DataMember> dataElements, string defaultName)
		{
			// First check if we have duplicates in target DataElements
			string name = defaultName ?? sourceName;
			int suffix = 1;

			foreach(DataMember element in dataElements)
			{
				if (element is ModelElementReference &&
					element.Name == name)
				{
					name = sourceName + suffix.ToString(NumberFormatInfo.CurrentInfo);
					suffix++;
				}
			}

			// Second check if this is a self reference and therefore we need to get a new name (!= element name)
			for(; name == targetName; suffix++)
			{
				name = (defaultName ?? sourceName) + suffix.ToString(NumberFormatInfo.CurrentInfo);
			}

			return name; 
		}
	}
}