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
using Microsoft.Practices.Modeling.CodeGeneration;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.ServiceContracts
{
	public partial class DataContractMessagePart: ICrossModelingPropertyHolder
	{
		#region ICrossModelingPropertyHolder Members

		public System.Collections.ObjectModel.ReadOnlyCollection<string> Monikers
		{
			get 
			{
				IList<string> monikers = new List<string>();

				monikers.Add(this.Type);

				return new ReadOnlyCollection<string>(monikers);
			}
		}

		#endregion

        internal sealed partial class TypePropertyHandler : DomainPropertyValueHandler<DataContractMessagePart, System.String>
        {
            private const string melPrefix = "mel://";
            private const string typeName = "Data Contract";

            protected override void OnValueChanged(DataContractMessagePart element, string oldValue, string newValue)
            {
                if (!newValue.StartsWith(melPrefix))
                {
                    newValue = string.Concat(melPrefix, newValue);
                }
                
                if (!element.Store.InUndoRedoOrRollback)
                {
                    if (!IsModelReferenceValid(newValue))
                    {
                        // Log the error in the Error List
                        LogMessage(string.Format(CultureInfo.InvariantCulture, Properties.Resources.InvalidMoniker, typeName));
                        // Reset the property
                        element.typePropertyStorage = oldValue;
                        return;
                    }
                    else
                    {
                        if (oldValue == element.Type)
                        {
                            element.Type = newValue;
                        }
                    }
                }

                element.typePropertyStorage = newValue;
            }

            private void LogMessage(string message)
            {
                LogEntry entry = new LogEntry(
                    message,
                    string.Empty,
                    TraceEventType.Error,
                    0);

                Logger.Write(entry);
            }

            private bool IsModelReferenceValid(string moniker)
            {
                // filter out the schema part
                moniker = moniker.Replace(melPrefix, string.Empty);

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

                return true;
            }
        }
	}
}