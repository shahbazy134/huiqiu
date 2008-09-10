/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Web;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.PowerCommands.Common;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that send a codesnippet by email
    /// </summary>
    [Guid("F359CFC9-D628-46B4-AA78-99893E4E056C")]
    [DisplayName("Email CodeSnippet")]
    internal class EmailCodeSnippetCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidEmailCodeSnippetCommand = 0x3DAC;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailCodeSnippetCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public EmailCodeSnippetCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(EmailCodeSnippetCommand).GUID,
                    (int)EmailCodeSnippetCommand.cmdidEmailCodeSnippetCommand))
        {
        }
        #endregion

        #region Private Implementation
        /// <summary>
        /// Determines whether this instance can execute the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can execute the specified command; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                if(DynamicCommand.Dte.ActiveDocument != null &&
                    DynamicCommand.Dte.ActiveDocument.ProjectItem != null)
                {
                    TextSelection selection = DynamicCommand.Dte.ActiveDocument.Selection as TextSelection;

                    string text = selection.Text;

                    if(string.IsNullOrEmpty(text))
                    {
                        //Select all document content
                        EditPoint point = selection.ActivePoint.CreateEditPoint();
                        point.EndOfDocument();
                        text = point.GetLines(1, point.Line + 1);
                    }

                    return !string.IsNullOrEmpty(text);
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            if(DynamicCommand.Dte.ActiveDocument != null &&
                DynamicCommand.Dte.ActiveDocument.ProjectItem != null)
            {
                try
                {
                    TextSelection selection = DynamicCommand.Dte.ActiveDocument.Selection as TextSelection;

                    string text = selection.Text;

                    if(string.IsNullOrEmpty(text))
                    {
                        //Select all document content
                        EditPoint point = selection.ActivePoint.CreateEditPoint();
                        point.EndOfDocument();
                        text = point.GetLines(1, point.Line + 1);
                    }

                    string body = HttpUtility.UrlEncode(text.Replace("+", "*space*"));
                    body = body.Replace("+", "%20").Replace("*space*", "+");

                    System.Diagnostics.Process.Start(
                        string.Format(
                        CultureInfo.CurrentCulture,
                        "mailto:?subject=CodeSnippet from {0}&body={1}",
                        Dte.ActiveDocument.Name,
                        body));
                }
                catch
                {
                    VSShellHelper.ShowMessageBox(
                        Microsoft.PowerCommands.Properties.Resources.Error,
                        Microsoft.PowerCommands.Properties.Resources.ExceptionLaunchingDefaultMailClient,
                        string.Empty,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}