using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;

namespace Microsoft.PowerCommands.Commands.UI
{
    /// <summary>
    /// View for the extract to Clear List command
    /// </summary>
    public partial class ClearListView : Window
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearListView"/> class.
        /// </summary>
        public ClearListView()
        {
            InitializeComponent();
            this.DataContext = new ClearListModel();
            this.CommandBindings.Add(new CommandBinding(DoClearAndRestart, OnDoClearAndRestart, OnCanDoClearAndRestart));
            this.CommandBindings.Add(new CommandBinding(DoCancel, OnDoCancel));
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ClearListModel Model
        {
            get
            {
                return (ClearListModel)this.DataContext;
            }
        } 
        #endregion

        /// <summary>
        /// Fired when the ClearAndRestart button is clicked
        /// </summary>
        public static readonly RoutedCommand DoClearAndRestart = new RoutedCommand("DoClearAndRestart", typeof(ClearListView));
        /// <summary>
        /// Fired when the Cancel button is clicked
        /// </summary>
        public static readonly RoutedCommand DoCancel = new RoutedCommand("DoCancel", typeof(ClearListView));

        #region Private Implementation
        private void OnDoClearAndRestart(object sender, ExecutedRoutedEventArgs e)
        {
            Model.SelectedListEntries.Clear();
            Model.SelectedListEntries.AddRange(lstEntries.SelectedItems.Cast<KeyValue>());
            this.DialogResult = true;
            this.Close();
        }

        private void OnCanDoClearAndRestart(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lstEntries.SelectedItems.Count > 0;
        }

        private void OnDoCancel(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if((bool)chkSelectAll.IsChecked)
            {
                lstEntries.SelectAll();
            }
            else
            {
                lstEntries.UnselectAll();
            }
        }

        private void chkEntry_Click(object sender, RoutedEventArgs e)
        {
            bool areAllItemChecked = true;

            foreach(KeyValue dataItem in this.Model.ListEntries)
            {
                ListBoxItem lbitem = (ListBoxItem)lstEntries.ItemContainerGenerator.ContainerFromItem(dataItem);

                if(lbitem != null)
                {
                    CheckBox chkEntry = (CheckBox)GetChildHelper(lbitem, "chkEntry");

                    if(!(bool)chkEntry.IsChecked)
                    {
                        areAllItemChecked = false;
                        break;
                    }
                }
            }

            chkSelectAll.IsChecked = areAllItemChecked;
        }

        private object GetChildHelper(ListBoxItem lbitem, string name)
        {
            Border border = VisualTreeHelper.GetChild(lbitem, 0) as Border;
            ContentPresenter contentPresenter = VisualTreeHelper.GetChild(border, 0) as ContentPresenter;
            return lstEntries.ItemTemplate.FindName(name, contentPresenter);
        } 
        #endregion
    }
}