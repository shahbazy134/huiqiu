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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using System.CodeDom.Compiler;
using System.IO;
using System.CodeDom;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using System.Globalization;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.Modeling.CodeGeneration.Strategies.TextTemplating
{
	public sealed class ModelInjectorDirectiveProcessor : DirectiveProcessor
	{
		System.CodeDom.Compiler.CodeDomProvider languageProvider;
		TextTemplateHost templateEngineHost;

		public override void FinishProcessingRun()
		{
			// Not implemented.
		}

		public override string GetClassCodeForProcessingRun()
		{
			CodeGeneratorOptions options = new CodeGeneratorOptions();

			StringWriter code = new StringWriter(CultureInfo.InvariantCulture);

			if (templateEngineHost.Model != null)
			{
				CodeMemberProperty modelProperty = new CodeMemberProperty();
				modelProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				modelProperty.Type = new CodeTypeReference(templateEngineHost.Model.GetType());
				modelProperty.Name = "Model";
				modelProperty.GetStatements.Add(new CodeMethodReturnStatement(
						 new CodeCastExpression(
												templateEngineHost.Model.GetType(),
												new CodePropertyReferenceExpression(
																new CodePropertyReferenceExpression(
																				new CodeTypeReferenceExpression(typeof(TextTemplateHost)),
																				 "Instance"),
												 "Model"))));
				languageProvider.GenerateCodeFromMember(modelProperty, code, options);
			}

			if (templateEngineHost.RootElement != null)
			{
				CodeMemberProperty rootProperty = new CodeMemberProperty();
				rootProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				rootProperty.Type = new CodeTypeReference(templateEngineHost.RootElement.GetType());
				rootProperty.Name = "RootElement";
				rootProperty.GetStatements.Add(new CodeMethodReturnStatement(
						 new CodeCastExpression(
												templateEngineHost.RootElement.GetType(),
												new CodePropertyReferenceExpression(
																new CodePropertyReferenceExpression(
																				new CodeTypeReferenceExpression(typeof(TextTemplateHost)),
																				 "Instance"),
												 "RootElement"))));
				languageProvider.GenerateCodeFromMember(rootProperty, code, options);
			}

			if (templateEngineHost.CurrentElement != null)
			{
				CodeMemberProperty currentProperty = new CodeMemberProperty();
				currentProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				currentProperty.Type = new CodeTypeReference(templateEngineHost.CurrentElement.GetType());
				currentProperty.Name = "CurrentElement";
				currentProperty.GetStatements.Add(new CodeMethodReturnStatement(
						 new CodeCastExpression(
												templateEngineHost.CurrentElement.GetType(),
												new CodePropertyReferenceExpression(
																new CodePropertyReferenceExpression(
																				new CodeTypeReferenceExpression(typeof(TextTemplateHost)),
																				 "Instance"),
												 "CurrentElement"))));
				languageProvider.GenerateCodeFromMember(currentProperty, code, options);
			}

			if (templateEngineHost.CurrentExtender != null)
			{
				CodeMemberProperty currentProperty = new CodeMemberProperty();
				currentProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				currentProperty.Type = new CodeTypeReference(templateEngineHost.CurrentExtender.GetType());
				currentProperty.Name = "CurrentExtender";
				currentProperty.GetStatements.Add(new CodeMethodReturnStatement(
					new CodeCastExpression(
						templateEngineHost.CurrentExtender.GetType(),
						new CodePropertyReferenceExpression(
							new CodePropertyReferenceExpression(
								new CodeTypeReferenceExpression(typeof(TextTemplateHost)),
								"Instance"),
						"CurrentExtender"))));
				languageProvider.GenerateCodeFromMember(currentProperty, code, options);
			}

			CodeMemberMethod currentMethod = new CodeMemberMethod();
			currentMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			currentMethod.ReturnType = new CodeTypeReference(typeof(ModelElement));
			currentMethod.Name = "ResolveModelReference";
			currentMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string),"melMoniker"));
			currentMethod.Statements.Add(new CodeMethodReturnStatement(
				new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodePropertyReferenceExpression(
							new CodeTypeReferenceExpression(typeof(TextTemplateHost)),"Instance"),
						"ResolveModelReference"),
					new CodeExpression[]{new CodeVariableReferenceExpression("melMoniker")})));
			languageProvider.GenerateCodeFromMember(currentMethod, code, options);

			currentMethod = new CodeMemberMethod();
			currentMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			currentMethod.ReturnType = new CodeTypeReference(typeof(bool));
			currentMethod.Name = "IsValid";
			currentMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IArtifactLink), "link"));
			currentMethod.Statements.Add(new CodeMethodReturnStatement(
				new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodePropertyReferenceExpression(
							new CodeTypeReferenceExpression(typeof(TextTemplateHost)), "Instance"),
						"IsValid"),
					new CodeExpression[] { new CodeVariableReferenceExpression("link") })));
			languageProvider.GenerateCodeFromMember(currentMethod, code, options);

			currentMethod = new CodeMemberMethod();
			currentMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			currentMethod.Name = "AddProjectReference";
			currentMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IArtifactLink), "link"));
			currentMethod.Statements.Add(new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodePropertyReferenceExpression(
							new CodeTypeReferenceExpression(typeof(TextTemplateHost)), "Instance"),
						"AddProjectReference"),
					new CodeExpression[] { new CodeVariableReferenceExpression("link") }));
			languageProvider.GenerateCodeFromMember(currentMethod, code, options);

			currentMethod = new CodeMemberMethod();
			currentMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			currentMethod.Name = "CancelOutput";
			currentMethod.Statements.Add(new CodeAssignStatement(
					new CodePropertyReferenceExpression(
						new CodePropertyReferenceExpression(
							new CodeTypeReferenceExpression(typeof(TextTemplateHost)), "Instance"),
							"GenerateOutput"), new CodePrimitiveExpression(false)));
			languageProvider.GenerateCodeFromMember(currentMethod, code, options);

			return code.ToString();
		}

		public override string[] GetImportsForProcessingRun()
		{
			// Not implemented.
			return null;
		}

		public override string GetPostInitializationCodeForProcessingRun()
		{
			return string.Empty;
		}

		public override string GetPreInitializationCodeForProcessingRun()
		{
			// Not implemented.
			return string.Empty;
		}

		public override string[] GetReferencesForProcessingRun()
		{
			return null;
		}

		public override bool IsDirectiveSupported(string directiveName)
		{
			Guard.ArgumentNotNullOrEmptyString(directiveName, "directiveName");
			return (directiveName.Equals("ModelInjector", StringComparison.OrdinalIgnoreCase));
		}

		public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
		{
			return;
		}

		public override void Initialize(ITextTemplatingEngineHost host)
		{
			templateEngineHost = (TextTemplateHost)host;
			base.Initialize(host);
		}

		public override void StartProcessingRun(CodeDomProvider languageProvider,
				string templateContents, CompilerErrorCollection errors)
		{
			this.languageProvider = languageProvider;
			base.StartProcessingRun(languageProvider, templateContents, errors);
		}
	}
}
