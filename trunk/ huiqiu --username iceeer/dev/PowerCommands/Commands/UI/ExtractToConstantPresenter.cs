/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System.Windows.Input;
using Microsoft.PowerCommands.Mvp;

namespace Microsoft.PowerCommands.Commands.UI
{
    /// <summary>
    /// Presenter for the ExtractToConstantView
    /// </summary>
    public class ExtractToConstantPresenter : Presenter<ExtractToConstantModel, IModalView>
    {
        /// <summary>
        /// Fired when the Ok button is clicked
        /// </summary>
        static public RoutedCommand DoOk = new RoutedCommand("DoOk", typeof(ExtractToConstantPresenter));
        /// <summary>
        /// Fired when the Cancel button is clicked
        /// </summary>
        static public RoutedCommand DoCancel = new RoutedCommand("DoCancel", typeof(ExtractToConstantPresenter));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractToConstantPresenter"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        public ExtractToConstantPresenter(ExtractToConstantModel model, IModalView view)
            : base(model, view)
        {
            AddCommandBinding(new CommandBinding(DoOk, OnDoOk, OnCanDoOk));
            AddCommandBinding(new CommandBinding(DoCancel, OnDoCancel));
        }

        private void OnDoOk(object sender, ExecutedRoutedEventArgs e)
        {
            View.OK();
        }

        private void OnCanDoOk(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Model.Error == null);
        }

        private void OnDoCancel(object sender, ExecutedRoutedEventArgs e)
        {
            View.Cancel();
        }
    }
}