/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.PowerCommands.Mvp
{
    /// <summary>
    /// Generic Presenter
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="IView">The type of the view.</typeparam>
    public class Presenter<TModel, IView> : IPresenter
    {
        private List<CommandBinding> bindings = new List<CommandBinding>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Presenter&lt;TModel, IView&gt;"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        public Presenter(TModel model, IView view)
        {
            this.Model = model;
            this.View = view;
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        protected IView View { get; private set; }
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        protected TModel Model { get; private set; }

        /// <summary>
        /// Adds the command binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        protected void AddCommandBinding(CommandBinding binding)
        {
            bindings.Add(binding);
        }

        ICollection IPresenter.CommandBindings
        {
            get { return bindings; }
        }
    }
}