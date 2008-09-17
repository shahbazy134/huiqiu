/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Windows;

namespace Microsoft.PowerCommands.Mvp
{
    /// <summary>
    /// Factory for MVPs
    /// </summary>
    public static class MvpFactory
    {
        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="IView">The type of the view.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="presenter">The presenter.</param>
        public static void Create<TModel, IView, TView, TPresenter>(out TModel model, out TView view, out TPresenter presenter)
            where TModel : new()
            where IView : class
            where TView : FrameworkElement, IView, new()
            where TPresenter : Presenter<TModel, IView>
        {
            model = new TModel();
            view = new TView();
            presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), model, view);

            AddCommandBindings(view, presenter);
            view.DataContext = model;
        }

        private static void AddCommandBindings(FrameworkElement view, IPresenter presenter)
        {
            view.CommandBindings.AddRange(presenter.CommandBindings);
        }
    }
}