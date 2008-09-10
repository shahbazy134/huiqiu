/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.CSharp;
using Microsoft.PowerCommands.Commands.UI;
using Microsoft.PowerCommands.Linq;
using Microsoft.PowerCommands.Mvp;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that extracts a text selection and creates the corresponding constant definition for the selected text
    /// </summary>
    [Guid("DD2ADE52-CB4E-415A-B9C1-C3183BC8DDB5")]
    [DisplayName("Extract Constant...")]
    internal class ExtractToConstantCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidExtractToConstantCommand = 0x757A;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractToConstantCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ExtractToConstantCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(ExtractToConstantCommand).GUID,
                    (int)ExtractToConstantCommand.cmdidExtractToConstantCommand))
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
                    DynamicCommand.Dte.ActiveDocument.Selection != null &&
                    !string.IsNullOrEmpty(((TextSelection)DynamicCommand.Dte.ActiveDocument.Selection).Text) &&
                    DynamicCommand.Dte.ActiveDocument.ProjectItem != null &&
                    DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel != null)
                {
                    Project project = DynamicCommand.Dte.ActiveDocument.ProjectItem.ContainingProject;
                    FileCodeModel codeModel = DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel;

                    if(project.Kind == PrjKind.prjKindCSharpProject || project.Kind == PrjKind.prjKindVBProject)
                    {
                        TextSelection selection = DynamicCommand.Dte.ActiveDocument.Selection as TextSelection;

                        if(project.Kind == PrjKind.prjKindVBProject)
                        {
                            if(codeModel.CodeElements.Count > 0 &&
                                codeModel.CodeElements.Item(1).Kind != vsCMElement.vsCMElementModule)
                            {
                                //Activated if there is some valid text selected
                                return (GetConstantRefType(selection.Text) != vsCMTypeRef.vsCMTypeRefVoid);
                            }
                        }
                        else
                        {
                            //Activated if there is some valid text selected
                            return (GetConstantRefType(selection.Text) != vsCMTypeRef.vsCMTypeRefVoid);
                        }
                    }
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            TextSelection selection = DynamicCommand.Dte.ActiveDocument.Selection as TextSelection;

            if(selection != null &&
                !string.IsNullOrEmpty(selection.Text) &&
                DynamicCommand.Dte.ActiveDocument.ProjectItem != null &&
                DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel != null)
            {
                string constantValue = selection.Text;
                FileCodeModel codeModel = DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel;

                ExtractToConstantModel model;
                ExtractToConstantView view;
                ExtractToConstantPresenter presenter;

                MvpFactory.Create<ExtractToConstantModel, IModalView, ExtractToConstantView, ExtractToConstantPresenter>(
                    out model, out view, out presenter);

                try
                {
                    CodeClass @class =
                        codeModel.CodeElementFromPoint(selection.ActivePoint, vsCMElement.vsCMElementClass) as CodeClass;

                    model.Identifier = GetConstantIdentifier(codeModel, @class, constantValue);
                    model.CodeDomProvider = GetCodeDomProvider(codeModel);
                    model.CodeClass = @class;

                    if((bool)view.ShowDialog())
                    {
                        vsCMTypeRef constantRefType = GetConstantRefType(constantValue);

                        if(@class != null)
                        {
                            selection.ReplaceText(constantValue, model.Identifier, (int)vsFindOptions.vsFindOptionsNone);

                            int variableCount =
                                new CodeElementIterator(@class.Children).OfType<CodeVariable>().Count();

                            CodeVariable var =
                                @class.AddVariable(model.Identifier, constantRefType, variableCount, TranslateAccess(model.SelectedVisibility), null);

                            var.IsConstant = true;
                            var.InitExpression = constantValue;
                        }
                    }
                }
                catch(COMException)
                {
                    //Ocurred when right clicking out of a class because of vsCMElement.vsCMElementClass
                }
            }
        }

        private static vsCMAccess TranslateAccess(string visibility)
        {
            if(visibility.Equals("Private", StringComparison.OrdinalIgnoreCase))
            {
                return vsCMAccess.vsCMAccessPrivate;
            }
            else if(visibility.Equals("Public", StringComparison.OrdinalIgnoreCase))
            {
                return vsCMAccess.vsCMAccessPublic;
            }
            else if(visibility.Equals("Protected", StringComparison.OrdinalIgnoreCase))
            {
                return vsCMAccess.vsCMAccessProtected;
            }
            else if(visibility.Equals("Internal", StringComparison.OrdinalIgnoreCase))
            {
                return vsCMAccess.vsCMAccessProject;
            }
            else
            {
                return vsCMAccess.vsCMAccessPrivate;
            }
        }

        private static CodeDomProvider GetCodeDomProvider(FileCodeModel codeModel)
        {
            CodeDomProvider provider;

            if(codeModel.Language.Equals(CodeModelLanguageConstants.vsCMLanguageCSharp, StringComparison.OrdinalIgnoreCase))
            {
                provider = new CSharpCodeProvider();
            }
            else if(codeModel.Language.Equals(CodeModelLanguageConstants.vsCMLanguageVB, StringComparison.OrdinalIgnoreCase))
            {
                provider = new VBCodeProvider();
            }
            else
            {
                provider = new CSharpCodeProvider();
            }

            return provider;
        }

        private static string GetConstantIdentifier(FileCodeModel codeModel, CodeClass @class, string constantValue)
        {
            CodeDomProvider provider = GetCodeDomProvider(codeModel);
            string identifier = SanitizeIdentifier(constantValue);

            identifier = provider.CreateValidIdentifier(identifier);

            if(!provider.IsValidIdentifier(identifier) || IdentifierAlreadyExists(@class, identifier))
            {
                identifier = string.Empty;
            }

            return identifier;
        }

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

        private static string SanitizeIdentifier(string identifier)
        {
            string sanitizedIdentifier = identifier.Replace("\"", "");
            sanitizedIdentifier = sanitizedIdentifier.Replace("'", "");

            return sanitizedIdentifier;
        }

        private static vsCMTypeRef GetConstantRefType(string constantValue)
        {
            bool result;
            short result1;
            int result2;
            long result3;
            double result4;
            float result5;
            decimal result6;

            if(constantValue.StartsWith("\"", StringComparison.OrdinalIgnoreCase) &&
                constantValue.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                return vsCMTypeRef.vsCMTypeRefString;
            }
            else if(constantValue.StartsWith("'", StringComparison.OrdinalIgnoreCase) &&
                constantValue.EndsWith("'", StringComparison.OrdinalIgnoreCase))
            {
                return vsCMTypeRef.vsCMTypeRefChar;
            }
            else if(Boolean.TryParse(constantValue, out result))
            {
                return vsCMTypeRef.vsCMTypeRefBool;
            }
            else if(short.TryParse(constantValue, out result1))
            {
                return vsCMTypeRef.vsCMTypeRefShort;
            }
            else if(int.TryParse(constantValue, out result2))
            {
                return vsCMTypeRef.vsCMTypeRefInt;
            }
            else if(long.TryParse(constantValue, out result3))
            {
                return vsCMTypeRef.vsCMTypeRefLong;
            }
            else if(double.TryParse(constantValue, out result4))
            {
                return vsCMTypeRef.vsCMTypeRefDouble;
            }
            else if(float.TryParse(constantValue, out result5))
            {
                return vsCMTypeRef.vsCMTypeRefFloat;
            }
            else if(decimal.TryParse(constantValue, out result6))
            {
                return vsCMTypeRef.vsCMTypeRefDecimal;
            }
            else
            {
                return vsCMTypeRef.vsCMTypeRefVoid;
            }
        }
        #endregion
    }
}