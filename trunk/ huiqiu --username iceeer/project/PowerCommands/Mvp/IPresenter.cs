/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System.Collections;

namespace Microsoft.PowerCommands.Mvp
{
    /// <summary>
    /// Interface for presenters
    /// </summary>
	public interface IPresenter
	{
        /// <summary>
        /// Gets the command bindings.
        /// </summary>
        /// <value>The command bindings.</value>
        ICollection CommandBindings
        {
            get;
        }
	}
}