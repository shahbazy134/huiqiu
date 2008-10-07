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
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.RecipeFramework.Library;
using EnvDTE;
using System.ComponentModel.Design;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.Common.Services;
using System.IO;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using System.Globalization;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.Practices.Modeling.CodeGeneration;

namespace Microsoft.Practices.ServiceFactory.Actions
{
	[ServiceDependency(typeof(DTE))]
	[ServiceDependency(typeof(ITypeResolutionService))]
	[ServiceDependency(typeof(IServiceProvider))]
	public abstract class ActionBase : ConfigurableAction
	{
		private ExpressionEvaluationService evaluator;
		private DTE dte;
		private string basePath;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionBase"/> class.
		/// </summary>
		protected ActionBase()
		{
			this.evaluator = new ExpressionEvaluationService();
		}

		/// <summary>
		/// When implemented by a class, allows descendants to
		/// perform processing whenever the component is being sited.
		/// </summary>
		protected override void  OnSited()
		{
 			 base.OnSited();
			 this.dte = this.GetService<DTE>();
			 TypeResolutionService service = (TypeResolutionService)this.GetService<ITypeResolutionService>();
			 this.basePath = service.BasePath;
		}

		/// <summary>
		/// See <see cref="M:Microsoft.Practices.RecipeFramework.IAction.Execute"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override void Execute()
		{
			try
			{
				Logger.Clear();
				OnExecute();
			}
			catch (Exception e)
			{
				HandleException(e);
			}
			finally
			{
				RestoreStatusBar();
			}
		}

		/// <summary>
		/// Called when [execute].
		/// </summary>
		protected virtual void OnExecute()
		{
		}

		protected virtual void HandleException(Exception e)
		{
			LogException(e);
		}

		/// <summary>
		/// See <see cref="M:Microsoft.Practices.RecipeFramework.IAction.Undo"/>.
		/// </summary>
		public override void Undo()
		{
			OnUndo();
		}

		/// <summary>
		/// Called when [undo].
		/// </summary>
		protected virtual void OnUndo()
		{
		}

		/// <summary>
		/// Determines whether the current model is valid (Executes all the validation rules on that model).
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if [is valid model]; otherwise, <c>false</c>.
		/// </returns>
		protected virtual bool IsValidModel()
		{
			ModelingDocView docView = DesignerHelper.GetModelingDocView(this.Site);
			IValidationControllerAccesor accesor = docView.DocData as IValidationControllerAccesor;
			if (accesor != null)
			{
                return ModelValidator.ValidateModelElement(docView.DocData.RootElement, accesor.Controller);
			}
			return true;
		}

		protected object Evaluate(string parameter)
		{
			IDictionaryService service = (IDictionaryService)this.GetService(typeof(IDictionaryService));
			return this.evaluator.Evaluate(parameter, new ServiceAdapterDictionary(service));
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <returns></returns>
		protected TInterface GetService<TInterface, TImpl>()
		{
			return (TInterface)GetService(typeof(TImpl));
		}

		/// <summary>
		/// Gets the DTE.
		/// </summary>
		/// <value>The DTE.</value>
		protected DTE Dte
		{
			get { return this.dte; }
		}

		/// <summary>
		/// Gets the base path.
		/// </summary>
		/// <value>The base path.</value>
		protected string BasePath
		{
			get { return basePath; }
		}

		/// <summary>
		/// Gets the templates root path.
		/// </summary>
		/// <value>The templates root path.</value>
		protected string TemplatesRootPath
		{
			get
			{
				return Path.Combine(this.BasePath, "Templates");
			}
		}

		protected string GetTemplatePath(string templatePath)
		{
			return Path.Combine(this.TemplatesRootPath, templatePath);
		}

		protected void ClearLogs()
		{
			Logger.Clear();
		}

		protected void LogException(Exception e)
		{
			ClearLogs();
			LogEntry entry = new LogEntry(
				e.Message,
				Properties.Resources.LogErrorTitle,
				TraceEventType.Error,
				1000);

			Logger.Write(entry);
		}

				/// <summary>
		/// Shows the message in output window.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The args.</param>
		protected void Trace(string message, params object[] args)
		{
			Trace(message, TraceEventType.Information, args);
		}

		/// <summary>
		/// Shows the message in output window.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The args.</param>
		protected void Trace(string message, TraceEventType logSeverity, params object[] args)
		{
			LogEntry entry = new LogEntry(
				args.Length == 0 ? message : string.Format(CultureInfo.CurrentCulture, message, args),
				Properties.Resources.LogInformationTitle,
				logSeverity,
				1000);

			Logger.Write(entry);
		}

		/// <summary>
		/// Show a progress bar in the VS status bar.
		/// </summary>
		/// <param name="label">The label to show in the status bar.</param>
		/// <param name="amountCompleted">The amount completed.</param>
		/// <param name="total">The total.</param>
		protected void Progress(string label, int amountCompleted, int total)
		{
			this.dte.StatusBar.Progress(true, label, amountCompleted, total);
		}

		/// <summary>
		/// Updates the status.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="args">The args.</param>
		protected void UpdateStatus(string text, params object[] args)
		{
			if (args != null)
			{
				this.dte.StatusBar.Text = String.Format(CultureInfo.CurrentCulture, text, args);
			}
			else
			{
				this.dte.StatusBar.Text = text;
			}
		}

		private void RestoreStatusBar()
		{
			this.dte.StatusBar.Progress(false, string.Empty, 0, 0);
			UpdateStatus(Properties.Resources.IdleStatusBarMessage);
		}
	}
}