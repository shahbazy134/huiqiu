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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Configuration;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests.Mocks;
using Microsoft.Practices.UnitTestLibrary;
using System.IO;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests
{
	[TestClass]
	public class ProjectMappingManagerFixture
	{
		private const string WcfMappingTableName = "WCF";
		private Guid ProjectId1 = new Guid("4A216B22-B2B2-4851-AFFA-B7A5AF147645");

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		[DeploymentItem("Microsoft.Practices.Common.dll")]
		[DeploymentItem("Microsoft.Practices.ComponentModel.dll")]
		public void GetProjectsInRolesWithFirstArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectsInRoles(null, new List<Role>());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectsInRolesWithSecondArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectsInRoles("Foo", null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectsInRolesWithBothArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectsInRoles(null, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldNotReturnProjectInRolesIfMappingTableDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ProjectMappingManager.Instance.GetProjectsInRoles("Foo", new List<Role>());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectRolesWithFirstArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectRoles(null, Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectRolesWithSecondArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectRoles("Foo", Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectRolesWithBothArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectRoles(null, Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldNotReturnRolesIfMappingTableDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ProjectMappingManager.Instance.GetProjectRoles("Foo", Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingNotFoundException))]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldNotReturnRolesIfProjectDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ProjectMappingManager.Instance.GetProjectRoles(WcfMappingTableName, Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectPathWithFirstArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectPath(null, Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectPathWithSecondArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectPath("Foo", Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectPathWithBothArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectPath(null, Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldNotReturnProjectPathIfMappingTableDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectPath("Foo", Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingNotFoundException))]
		public void ShouldNotReturnProjectPathIfProjectDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectPath(WcfMappingTableName, Guid.NewGuid());
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnProjectInRolesOneProject()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			List<Role> roles = new List<Role>();
			roles.Add(new Role("MessageContractRole"));

			ReadOnlyCollection<Guid> projectIds =
				ProjectMappingManager.Instance.GetProjectsInRoles(WcfMappingTableName, roles);

			Assert.IsNotNull(projectIds, "Null");
			Assert.AreEqual(projectIds.Count, 1, "Not Equal");
			Assert.AreEqual(projectIds[0], new Guid("BC9E7634-206C-43f4-81F3-5CA0D6DDBA99"), "Not Equal");
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnProjectInRolesMultipleProjects()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			List<Role> roles = new List<Role>();
			roles.Add(new Role("DataContractRole"));
			roles.Add(new Role("ServiceContractRole"));

			ReadOnlyCollection<Guid> projectIds =
				ProjectMappingManager.Instance.GetProjectsInRoles(WcfMappingTableName, roles);

			Assert.IsNotNull(projectIds, "Null");
			Assert.AreEqual(projectIds.Count, 2, "Not Equal");
			Assert.AreEqual(projectIds[0], new Guid("4A216B22-B2B2-4851-AFFA-B7A5AF147645"), "Not Equal");
			Assert.AreEqual(projectIds[1], new Guid("DE91F8D4-0BB1-4768-ACF3-204ABB481AFD"), "Not Equal");
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnRoles()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ReadOnlyCollection<Role> roles =
				ProjectMappingManager.Instance.GetProjectRoles(WcfMappingTableName, new Guid("4A216B22-B2B2-4851-AFFA-B7A5AF147645"));

			Assert.IsNotNull(roles, "Null");
			Assert.AreEqual(roles.Count, 2, "Not Equal");
			Assert.AreEqual(roles[0].Name, "DataContractRole", "Not Equal");
			Assert.AreEqual(roles[1].Name, "ServiceContractRole", "Not Equal");
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnProjectPath()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			string projectPath = ProjectMappingManager.Instance.GetProjectPath(WcfMappingTableName, new Guid("4A216B22-B2B2-4851-AFFA-B7A5AF147645"));

			Assert.AreEqual(projectPath, @"\", "Not Equal");
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnProjectMappingTableNames()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ReadOnlyCollection<string> mappingTableNames =
				ProjectMappingManager.Instance.GetMappingTableNames();

			Assert.IsNotNull(mappingTableNames, "Null");
			Assert.AreEqual(mappingTableNames.Count, 2, "Not Equal");
			Assert.AreEqual(mappingTableNames[0], WcfMappingTableName, "Not Equal");
			Assert.AreEqual(mappingTableNames[1], "ASMX", "Not Equal");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotAddAProjectMappingEntryWithNullFirstParameter()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.AddProjectMappingEntry(null, new ProjectMapping.Configuration.ProjectMappingEntry());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotAddAProjectMappingEntryWithNullSecondParameter()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.AddProjectMappingEntry("Foo", null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotAddAProjectMappingEntryWithWithBothParameterNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.AddProjectMappingEntry(null, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		public void ShouldNotAddAProjectMappingEntryIfMappingTableThatDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.AddProjectMappingEntry("Foo", new ProjectMapping.Configuration.ProjectMappingEntry());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ProjectMappingAlreadyExistsException))]
		public void ShouldNotAddAProjectMappingEntryThatAlreadyExists()
		{
			Guid projectGuid = new Guid("4A216B22-B2B2-4851-AFFA-B7A5AF147645");

			ProjectMapping.Configuration.ProjectMappingEntry projectMapping =
				new ProjectMapping.Configuration.ProjectMappingEntry(
					projectGuid,
					@"\Foo", "FooName");

			projectMapping.Roles.Add(new Role("FooRole"));
			
			ProjectMappingManager.Instance.AddProjectMappingEntry(WcfMappingTableName, projectMapping);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeleteAProjectMappingEntryWithNullFirstParameter()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.DeleteProjectMappingEntry(null, Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeleteAProjectMappingEntryWithNullSecondParameter()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.DeleteProjectMappingEntry("Foo", Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeleteAProjectMappingEntryWithBothParameterNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.DeleteProjectMappingEntry(null, Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		public void ShouldNotDeleteAProjectMappingEntryIfMappingTableThatDoesntExist()
		{
			ProjectMappingManager.Instance.DeleteProjectMappingEntry("Foo", Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingNotFoundException))]
		public void ShouldNotDeleteAProjectMappingEntryIfProjectThatDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.DeleteProjectMappingEntry(WcfMappingTableName, Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldNotDeleteAProjectMappingTableEntryWithParameterNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.DeleteProjectMappingTableEntry(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		public void ShouldNotDeleteAProjectMappingTableEntryIfMappingTableThatDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.DeleteProjectMappingTableEntry("Foo");
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableAlreadyExistsException))]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldNotAddAProjectMappingTableEntryThatAlreadyExists()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ProjectMappingTable projectMappingTable = new ProjectMappingTable(WcfMappingTableName);

			ProjectMappingManager.Instance.AddProjectMappingTableEntry(projectMappingTable);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableAlreadyExistsException))]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldAddAProjectMappingTableEntry()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ProjectMappingTable projectMappingTable = new ProjectMappingTable("FOOEntry");

			ProjectMappingManager.Instance.AddProjectMappingTableEntry(projectMappingTable);
			ProjectMappingManager.Instance.AddProjectMappingTableEntry(projectMappingTable);
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		public void ShouldDeleteAProjectMappingTableEntry()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ProjectMappingTable projectMappingTable = new ProjectMappingTable("FOOEntry1");
			ProjectMappingManager.Instance.AddProjectMappingTableEntry(projectMappingTable);

			ProjectMappingManager.Instance.DeleteProjectMappingTableEntry("FOOEntry1");
			ProjectMappingManager.Instance.DeleteProjectMappingTableEntry("FOOEntry1");
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldAddAProjectMappingEntry()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			Guid projectGuid = new Guid("0D0429A4-2E7B-413b-92DB-2F5E048667C8");

			ProjectMapping.Configuration.ProjectMappingEntry projectMapping =
				new ProjectMapping.Configuration.ProjectMappingEntry(
					projectGuid,
                    @"\Foo", "FooName");

			projectMapping.Roles.Add(new Role("FooRole"));

			ProjectMappingManager.Instance.AddProjectMappingEntry(WcfMappingTableName, projectMapping);

			string projectPath = ProjectMappingManager.Instance.GetProjectPath(WcfMappingTableName, projectGuid);
			ReadOnlyCollection<Role> roles = ProjectMappingManager.Instance.GetProjectRoles(WcfMappingTableName, projectGuid);

			Assert.AreEqual(roles.Count, 1, "Not Equal");
			Assert.AreEqual(roles[0].Name, "FooRole", "Not Equal");
			Assert.AreEqual(projectPath, @"\Foo", "Not Equal");
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldDeleteAProjectMappingEntry()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			Guid projectGuid = new Guid("A168E8C3-8CCD-47cd-AE2B-BE0F85F66782");

			ProjectMapping.Configuration.ProjectMappingEntry projectMapping =
				new ProjectMapping.Configuration.ProjectMappingEntry(
					projectGuid,
                    @"\Foo", "FooName");

			ProjectMappingTable projectMappingTable = new ProjectMappingTable("FooEntry2");
			ProjectMappingManager.Instance.AddProjectMappingTableEntry(projectMappingTable);

			ProjectMappingManager.Instance.AddProjectMappingEntry("FooEntry2", projectMapping);

			ProjectMappingManager.Instance.DeleteProjectMappingEntry("FooEntry2", projectGuid);

			Assert.IsNull(ProjectMappingManager.Instance.GetProjectMappingEntry("FooEntry2", projectGuid));
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingNotFoundException))]
		public void ShouldNotDeleteAProjectMappingEntryTwice()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			Guid projectGuid = new Guid("A168E8C3-8CCD-47cd-AE2B-BE0F85F66782");

			ProjectMapping.Configuration.ProjectMappingEntry projectMapping =
				new ProjectMapping.Configuration.ProjectMappingEntry(
					projectGuid,
                    @"\Foo", "FooName");

			ProjectMappingTable projectMappingTable = new ProjectMappingTable("FooEntry2");
			ProjectMappingManager.Instance.AddProjectMappingTableEntry(projectMappingTable);

			ProjectMappingManager.Instance.AddProjectMappingEntry("FooEntry2", projectMapping);

			ProjectMappingManager.Instance.DeleteProjectMappingEntry("FooEntry2", projectGuid);
			ProjectMappingManager.Instance.DeleteProjectMappingEntry("FooEntry2", projectGuid);
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReadSampleFileCorrectly()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			Assert.AreEqual(2, ProjectMappingManager.Instance.GetMappingTableNames().Count);

			ProjectMappingTable table = ProjectMappingManager.Instance.GetMappingTable("WCF");
			Assert.AreEqual(4, table.ProjectMappings.Count);

			ProjectMappingEntry entry = table.ProjectMappings[0];
			Assert.AreEqual("4A216B22-B2B2-4851-AFFA-B7A5AF147645", entry.ProjectId);
			Assert.AreEqual(@"\", entry.ProjectPath);
			Assert.AreEqual(2, entry.Roles.Count);
			Assert.AreEqual("DataContractRole", entry.Roles[0].Name);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetMappingTableWithArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetMappingTable(null);
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnNullIfMappingTableDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			Assert.IsNull(ProjectMappingManager.Instance.GetMappingTable("Foo"));
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnMappingTableIfExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingTable table = ProjectMappingManager.Instance.GetMappingTable(WcfMappingTableName);

			Assert.IsNotNull(table);
			Assert.AreEqual<string>(WcfMappingTableName, table.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectMappingEntryWithFirstArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectMappingEntry(null, Guid.NewGuid());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectMappingEntryWithSecondArgumentEmpty()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectMappingEntry("Foo", Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProjectMappingEntryWithBothArgumentNull()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMappingManager.Instance.GetProjectMappingEntry(null, Guid.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ProjectMappingTableNotFoundException))]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldNotReturnProjectMappingIfMappingTableDoesntExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");

			ProjectMappingManager.Instance.GetProjectMappingEntry("Foo", Guid.NewGuid());
		}

		[TestMethod]
		[DeploymentItem(@"ProjectMapping.RecipeFramework.Extensions.Tests.xml")]
		public void ShouldReturnProjectMappingIfMappingTableExist()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.RecipeFramework.Extensions.Tests.xml");
			ProjectMapping.Configuration.ProjectMappingEntry mapping = ProjectMappingManager.Instance.GetProjectMappingEntry(WcfMappingTableName, ProjectId1);

			Assert.IsNotNull(mapping);
			Assert.AreEqual<Guid>(ProjectId1, new Guid(mapping.ProjectId));
		}
	}
}