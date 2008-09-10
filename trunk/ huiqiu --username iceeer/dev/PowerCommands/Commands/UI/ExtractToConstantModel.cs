/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EnvDTE;
using System.CodeDom.Compiler;
using Microsoft.PowerCommands.Linq;

namespace Microsoft.PowerCommands.Commands.UI
{
    /// <summary>
    /// Class that represents the model for the ExtractToConstantPresenter
    /// </summary>
    public class ExtractToConstantModel : IDataErrorInfo
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractToConstantModel"/> class.
        /// </summary>
        public ExtractToConstantModel()
        {
            this.Visibility = new List<string>() { "Private", "Public", "Protected", "Internal" };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the code class.
        /// </summary>
        /// <value>The code class.</value>
        public CodeClass CodeClass { get; set; }

        /// <summary>
        /// Gets or sets the code DOM provider.
        /// </summary>
        /// <value>The code DOM provider.</value>
        public CodeDomProvider CodeDomProvider { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the selected visibility.
        /// </summary>
        /// <value>The selected visibility.</value>
        public string SelectedVisibility { get; set; }

        private IEnumerable<string> visibility;

        /// <summary>
        /// Gets or sets the visibility.
        /// </summary>
        /// <value>The visibility.</value>
        public IEnumerable<string> Visibility
        {
            get
            {
                return this.visibility.OrderBy(visibility => visibility);
            }

            private set
            {
                visibility = value;
            }
        }
        #endregion

        #region IDataErrorInfo Members

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value></value>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified column name.
        /// </summary>
        /// <value></value>
        public string this[string columnName]
        {
            get
            {
                string result = null;
                Error = null;

                if(columnName == "Identifier")
                {
                    if(string.IsNullOrEmpty(this.Identifier))
                    {
                        result = "String empty";
                        Error = "Invalid";
                    }
                    else if(this.CodeDomProvider != null)
                    {
                        if(!this.CodeDomProvider.IsValidIdentifier(this.Identifier))
                        {
                            result = "Invalid identifier";
                            Error = "Invalid";
                        }
                        else if(IdentifierAlreadyExists(this.CodeClass, this.Identifier))
                        {
                            result = "The identifier already exists";
                            Error = "Invalid";
                        }
                    }
                }

                return result;
            }
        }

        #endregion

        #region Private Implementation
        private static bool IdentifierAlreadyExists(CodeClass @class, string identifier)
        {
            CodeElementIterator iterator =
                new CodeElementIterator(@class.Children);

            if(iterator
                 .OfType<CodeVariable>()
                 .SingleOrDefault(
                     variable => variable.Name.Equals(
                         identifier, System.StringComparison.OrdinalIgnoreCase)) != null)
            {
                return true;
            }
            else if(iterator
                 .OfType<CodeEnum>()
                 .SingleOrDefault(
                     variable => variable.Name.Equals(
                         identifier, System.StringComparison.OrdinalIgnoreCase)) != null)
            {
                return true;
            }
            else if(iterator
                 .OfType<CodeProperty>()
                 .SingleOrDefault(
                     variable => variable.Name.Equals(
                         identifier, System.StringComparison.OrdinalIgnoreCase)) != null)
            {
                return true;
            }
            else if(iterator
                 .OfType<CodeFunction>()
                 .SingleOrDefault(
                     variable => variable.Name.Equals(
                         identifier, System.StringComparison.OrdinalIgnoreCase)) != null)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}