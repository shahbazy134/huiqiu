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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation;
using Microsoft.Practices.ServiceFactory.Helpers;

namespace ServicesGuidancePackage.Tests.CreateTranslator.Presentation
{
    [TestClass]
	public class CreateTranslatorMappingPresenterFixture
    {
        MockCreateTranslatorMappingPage view;
        CreateTranslatorMappingPresenter presenter;
        Type firstType;
        Type secondType;
        PropertyInfo firstNameProperty = typeof(FirstType).GetProperty("FirstName");
        PropertyInfo nameFirstProperty = typeof(SecondType).GetProperty("NameFirst");
        PropertyInfo phoneNumberProperty = typeof(FirstType).GetProperty("PhoneNumber");

        [TestInitialize]
        public void SetUp()
        {
            view = new MockCreateTranslatorMappingPage();
			presenter = new CreateTranslatorMappingPresenter(view);

            firstType = typeof(FirstType);
            secondType = typeof(SecondType);

            presenter.Initialize(firstType, secondType);
        }

        [TestMethod]
        public void InitializingPageShouldDisableAddMappingButton()
        {
            Assert.IsFalse(view.IsAddMappingButtonEnabled());
        }

        [TestMethod]
        public void InitializingPageShouldDisableRemoveMappingButton()
        {
            Assert.IsFalse(view.IsRemoveMappingButtonEnabled());
        }

        [TestMethod]
        public void InitializingPageShouldFillFirstListBoxWithPublicProperties()
        {
            Assert.AreEqual(2, view.FirstTypeListCount());
            Assert.AreEqual("FirstName (String)", view.FirstTypeList[0].ToString());
            Assert.AreEqual("PhoneNumber (Int64)", view.FirstTypeList[1].ToString());
        }

        [TestMethod]
        public void InitializingPageShouldFillSecondListBoxWithPublicProperties()
        {
            Assert.AreEqual(1, view.SecondTypeListCount());
            Assert.AreEqual("NameFirst (String)", view.SecondTypeList[0].ToString());
        }

        [TestMethod]
        public void CreateMappingsButtonIsEnabledIfAnItemIsSelectedInBothListBoxes()
        {
            presenter.ElementSelectedForMapping(new object(), new object());
            Assert.IsTrue(view.IsAddMappingButtonEnabled());
        }

        [TestMethod]
        public void CreateMappingsButtonIsDisabledIfItemIsNotSelectedInEitherListBox()
        {
            presenter.ElementSelectedForMapping(new object(), null);
            Assert.IsFalse(view.IsAddMappingButtonEnabled());

            presenter.ElementSelectedForMapping(null, new object());
            Assert.IsFalse(view.IsAddMappingButtonEnabled());
        }

        [TestMethod]
        public void ClickingCreateMappingButtonAddsMappingToMappingListBox()
        {
            presenter.CreateMappingButtonClicked(new PublicPropertyListItem(firstNameProperty),
                new PublicPropertyListItem(nameFirstProperty));
            Assert.AreEqual(1, view.MappedPropertiesCount());
        }

        [TestMethod]
        public void CreatingMappingRemovesMappedItemsFromPropertyLists()
        {
            PublicPropertyListItem firstProperty = new PublicPropertyListItem(firstNameProperty);
            PublicPropertyListItem secondProperty = new PublicPropertyListItem(nameFirstProperty);

            presenter.CreateMappingButtonClicked(firstProperty, secondProperty);

            Assert.IsFalse(view.FirstTypeList.Contains(firstProperty));
            Assert.IsFalse(view.SecondTypeList.Contains(secondProperty));
        }

        [TestMethod]
        public void SelectingMappingEnablesRemoveMappingButton()
        {
            PublicPropertyListItem firstProperty = new PublicPropertyListItem(firstNameProperty);
            PublicPropertyListItem secondProperty = new PublicPropertyListItem(nameFirstProperty);
            presenter.CreateMappingButtonClicked(firstProperty, secondProperty);

            presenter.MappedPropertySelected();

            Assert.IsTrue(view.IsRemoveMappingButtonEnabled());
        }

        [TestMethod]
        public void RemovingSelectedMappingReaddsBothPropertiesIntoUpperLists()
        {
            PublicPropertyListItem firstProperty = new PublicPropertyListItem(firstNameProperty);
            PublicPropertyListItem secondProperty = new PublicPropertyListItem(nameFirstProperty);
            MappedProperty mappedProperty = new MappedProperty(firstProperty, secondProperty);

            presenter.CreateMappingButtonClicked(firstProperty, secondProperty);
            presenter.MappedPropertySelected();

            presenter.RemoveMappingButtonClicked(mappedProperty);

            int indexOfBottomOfList = 1;
            Assert.AreEqual(2, view.FirstTypeListCount());
            Assert.AreEqual(firstProperty, view.FirstTypeList[indexOfBottomOfList]);
            Assert.AreEqual(1, view.SecondTypeListCount());
            Assert.AreEqual(secondProperty, view.SecondTypeList[0]);
        }

        [TestMethod]
        public void RemovingSelectedMappingRemovesTheMappingFromMappingListBox()
        {
            PublicPropertyListItem firstProperty = new PublicPropertyListItem(firstNameProperty);
            PublicPropertyListItem secondProperty = new PublicPropertyListItem(nameFirstProperty);
            MappedProperty mappedProperty = new MappedProperty(firstProperty, secondProperty);

            presenter.CreateMappingButtonClicked(firstProperty, secondProperty);
            presenter.MappedPropertySelected();

            presenter.RemoveMappingButtonClicked(mappedProperty);

            Assert.AreEqual(0, view.MappedPropertyList.Count);
        }

        [TestMethod]
        public void RemovingSelectedMappingDisablesRemoveButton()
        {
            PublicPropertyListItem firstProperty = new PublicPropertyListItem(firstNameProperty);
            PublicPropertyListItem secondProperty = new PublicPropertyListItem(nameFirstProperty);
            MappedProperty mappedProperty = new MappedProperty(firstProperty, secondProperty);

            presenter.CreateMappingButtonClicked(firstProperty, secondProperty);
            presenter.MappedPropertySelected();

            presenter.RemoveMappingButtonClicked(mappedProperty);

            Assert.IsFalse(view.IsRemoveMappingButtonEnabled());
        }

        [TestMethod]
        public void AttemptingToMapTwoPropertiesThatAreUnmappableCausesErrorAndDoesNotDoMapping()
        {
            PublicPropertyListItem firstProperty = new PublicPropertyListItem(phoneNumberProperty);
            PublicPropertyListItem secondProperty = new PublicPropertyListItem(nameFirstProperty);
            MappedProperty mappedProperty = new MappedProperty(firstProperty, secondProperty);

            presenter.CreateMappingButtonClicked(firstProperty, secondProperty);

            Assert.IsTrue(view.BadMappingSeen);
        }

        private class FirstType
        {
            public string FirstName
            {
                get { return "John"; }
                set { }
            }

            public long PhoneNumber
            {
                get { return 17; }
                set { }
            }
        }

        private class SecondType
        {
            public string NameFirst
            {
                get { return "John"; }
                set { }
            }
        }

		public class MockCreateTranslatorMappingPage : ICreateTranslatorMappingView
        {
            bool addMappingButtonEnabled = true;
            bool removeMappingButtonEnabled = true;
            bool badMappingSeen = false;
            PublicPropertyCollection firstTypeList;
			PublicPropertyCollection secondTypeList;
            List<MappedProperty> mappedPropertiesList = new List<MappedProperty>();

            public bool IsAddMappingButtonEnabled() { return addMappingButtonEnabled; }
            public bool IsRemoveMappingButtonEnabled() { return removeMappingButtonEnabled; }
            public int FirstTypeListCount() { return firstTypeList.Count; }
            public int SecondTypeListCount() { return secondTypeList.Count; }
			public PublicPropertyCollection FirstTypeList { get { return firstTypeList; } }
			public PublicPropertyCollection SecondTypeList { get { return secondTypeList; } }
            public List<MappedProperty> MappedPropertyList { get { return mappedPropertiesList; } }
            public int MappedPropertiesCount() { return mappedPropertiesList.Count; }
            public bool BadMappingSeen { get { return badMappingSeen; } }

            public void AddFirstTypeListTypes(PublicPropertyCollection firstTypeListNames) { this.firstTypeList = firstTypeListNames; }
			public void AddSecondTypeListTypes(PublicPropertyCollection secondTypeListNames) { this.secondTypeList = secondTypeListNames; }
            public void AddPropertyToFirstList(PublicPropertyListItem property) { firstTypeList.Add(property); }
            public void AddPropertyToSecondList(PublicPropertyListItem property) { secondTypeList.Add(property); }

            public void AddMappingButtonEnabled(bool isEnabled) { addMappingButtonEnabled = isEnabled; }
            public void RemoveMappingButtonEnabled(bool isEnabled) { removeMappingButtonEnabled = isEnabled; }
            public void RemoveTypeFromFirstList(PublicPropertyListItem property) { firstTypeList.Remove(property); }
            public void RemoveTypeFromSecondList(PublicPropertyListItem property) { secondTypeList.Remove(property); }

            public void AddMappedProperty(MappedProperty mappedProperty) { mappedPropertiesList.Add(mappedProperty); }
            public void RemoveMappedProperty(MappedProperty mappedProperty) { mappedPropertiesList.Remove(mappedProperty); }

            public void NotifyUserOfInvalidMapping(MappedProperty badMapping) { badMappingSeen = true; }
        }
    }
}
