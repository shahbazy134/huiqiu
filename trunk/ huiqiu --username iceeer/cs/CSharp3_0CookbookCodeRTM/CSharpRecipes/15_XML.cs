using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.CodeDom;
using System.Linq;
using System.Xml.Linq;

namespace CSharpRecipes
{
	public class XML
	{
        #region "15.1 Reading and Accessing XML Data in Document Order"
        public static void AccessXml()
        {
            // New LINQ to XML syntax for constructing XML
            XDocument xDoc = new XDocument(
                                new XDeclaration("1.0", "UTF-8", "yes"),
                                new XComment("My sample XML"),
                                new XProcessingInstruction("myProcessingInstruction", "value"),
                                new XElement("Root",
                                    new XElement("Node1",
                                        new XAttribute("nodeId", "1"), "FirstNode"),
                                    new XElement("Node2",
                                        new XAttribute("nodeId", "2"), "SecondNode"),
                                    new XElement("Node3",
                                        new XAttribute("nodeId", "1"), "ThirdNode")
                                )
                             );

            // write out the XML to the console
            Console.WriteLine(xDoc.ToString());

            // create an XmlReader from the XDocument
            XmlReader reader = xDoc.CreateReader();
            reader.Settings.CheckCharacters = true;
            int level = 0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.CDATA:
                        Display(level, "CDATA: {0}", reader.Value);
                        break;
                    case XmlNodeType.Comment:
                        Display(level, "COMMENT: {0}", reader.Value);
                        break;
                    case XmlNodeType.DocumentType:
                        Display(level, "DOCTYPE: {0}={1}", reader.Name, reader.Value);
                        break;
                    case XmlNodeType.Element:
                        Display(level, "ELEMENT: {0}", reader.Name);
                        level++;
                        while (reader.MoveToNextAttribute())
                        {
                            Display(level, "ATTRIBUTE: {0}='{1}'", reader.Name, reader.Value);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        level--;
                        break;
                    case XmlNodeType.EntityReference:
                        Display(level, "ENTITY: {0}", reader.Name);
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        Display(level, "INSTRUCTION: {0}={1}", reader.Name, reader.Value);
                        break;
                    case XmlNodeType.Text:
                        Display(level, "TEXT: {0}", reader.Value);
                        break;
                    case XmlNodeType.XmlDeclaration:
                        Display(level, "DECLARATION: {0}={1}", reader.Name, reader.Value);
                        break;
                }
            }
        }

        private static void Display(int indentLevel, string format, params object[] args)
        {
            for (int i = 0; i < indentLevel; i++)
                Console.Write(" ");
            Console.WriteLine(format, args);
        }
        #endregion
 	
        #region "15.2 Reading XML on the Web"	
        public static void ReadXmlWeb()
        {
            // This requires you set up a virtual directory pointing
            // to the sample.xml file included with the sample code
            // prior to executing this
            string url = "http://localhost/xml/sample.xml";
            XDocument xDoc = XDocument.Load(url);
            var query = from e in xDoc.Descendants()
                        where e.NodeType == XmlNodeType.Element
                        select new
                        {
                            ElementName = e,
                            ElementValue = e.Value
                        };

            foreach(var elementInfo in query)
            {
                Console.WriteLine("Element Name: {0}, Value: {1}", 
                    elementInfo.ElementName,elementInfo.ElementValue);
            }
            Console.WriteLine();
            Console.WriteLine();

            using (XmlReader reader = XmlReader.Create(url))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.Write("\r\nElement Name: {0}, Value: ",
                                reader.Name);
                            break;
                        case XmlNodeType.Text:
                            Console.Write(reader.Value);
                            break;
                        case XmlNodeType.CDATA:
                            Console.Write(reader.Value);
                            break;
                    }
                }
            }
        }
        #endregion
	
        #region "15.3 Querying the Contents of an XML Document"	
        private static XDocument GetAClue()
        {
            return new XDocument(
                            new XDeclaration("1.0", "UTF-8", "yes"),
                            new XElement("Clue",
                                new XElement("Participant",
                                    new XAttribute("type", "Perpetrator"), "Professor Plum"),
                                new XElement("Participant",
                                    new XAttribute("type", "Witness"), "Colonel Mustard"),
                                new XElement("Participant",
                                    new XAttribute("type", "Witness"), "Mrs. White"),
                                new XElement("Participant",
                                    new XAttribute("type", "Witness"), "Mrs. Peacock"),
                                new XElement("Participant",
                                    new XAttribute("type", "Witness"), "Mr. Green"),
                                new XElement("Participant",
                                    new XAttribute("type", "Witness"), "Miss Scarlet"),
                                new XElement("Participant",
                                    new XAttribute("type", "Victim"), "Mr. Boddy")
                            )
                         );
        }

        public static void QueryXml()
        {
            XDocument xDoc = GetAClue();

            // set up the query looking for the married female participants 
            // who were witnesses
            var query = from p in xDoc.Root.Elements("Participant")
                        where p.Attribute("type").Value == "Witness" &&
                            p.Value.Contains("Mrs.")
                        orderby p.Value
                        select p.Value;

            // write out the nodes found (Mrs. Peacock and Mrs. White in this instance as it is sorted)
            foreach (string s in query)
            {
                Console.WriteLine(s);
            }

            // old way
            //using (StringReader reader = new StringReader(xmlFragment))
            //{
            //    // Instantiate an XPathDocument using the StringReader.  
            //    XPathDocument xpathDoc = new XPathDocument(reader);

            //    // get the navigator
            //    XPathNavigator xpathNav = xpathDoc.CreateNavigator();

            //    // set up the query looking for the married female participants 
            //    // who were witnesses
            //    string xpathQuery =
            //        "/Clue/Participant[attribute::type='Witness'][contains(text(),'Mrs.')]";
            //    XPathExpression xpathExpr = xpathNav.Compile(xpathQuery);

            //    // get the nodeset from the compiled expression
            //    XPathNodeIterator xpathIter = xpathNav.Select(xpathExpr);

            //    // write out the nodes found (Mrs. White and Mrs.Peacock in this instance)
            //    while (xpathIter.MoveNext())
            //    {
            //        Console.WriteLine(xpathIter.Current.Value);
            //    }
            //}
        }
        #endregion
	
        #region "15.4 Validating a static XML stream"	
        public static void ValidateXml()
        {
            // open the bookbad.xml file
            XDocument book = XDocument.Load(@"..\..\BookBad.xml");
            // create XSD schema collection with book.xsd
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null,@"..\..\Book.xsd");
            // wire up handler to get any validation errors
            book.Validate(schemas, settings_ValidationEventHandler);

            // create a reader to roll over the file so validation fires
            XmlReader reader = book.CreateReader();
            // report warnings as well as errors
            reader.Settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
            // use XML Schema
            reader.Settings.ValidationType = ValidationType.Schema;
            // roll over the XML
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    Console.Write("<{0}", reader.Name);
                    while (reader.MoveToNextAttribute())
                    {
                        Console.Write(" {0}='{1}'", reader.Name,
                            reader.Value);
                    }
                    Console.Write(">");
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    Console.Write(reader.Value);
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    Console.WriteLine("</{0}>", reader.Name);
                }
            }


            //// create XSD schema collection with book.xsd
            //XmlReaderSettings settings = new XmlReaderSettings();
            //// wire up handler to get any validation errors
            //settings.ValidationEventHandler += settings_ValidationEventHandler;

            //// set the validation type to schema (used to be XsdValidate property in Beta1)
            //settings.ValidationType = ValidationType.Schema;

            //// add book.xsd
            //settings.Schemas.Add(null, XmlReader.Create(@"..\..\Book.xsd"));
            //// make sure we added
            //if (settings.Schemas.Count > 0)
            //{
            //    // open the bookbad.xml file
            //    using (XmlReader reader = XmlReader.Create(@"..\..\BookBad.xml", settings))
            //    {
            //        // replaced validReader with reader for the whole loop
            //        while (reader.Read())
            //        {
            //            if (reader.NodeType == XmlNodeType.Element)
            //            {
            //                Console.Write("<{0}", reader.Name);
            //                while (reader.MoveToNextAttribute())
            //                {
            //                    Console.Write(" {0}='{1}'", reader.Name,
            //                        reader.Value);
            //                }
            //                Console.Write(">");
            //            }
            //            else if (reader.NodeType == XmlNodeType.Text)
            //            {
            //                Console.Write(reader.Value);
            //            }
            //            else if (reader.NodeType == XmlNodeType.EndElement)
            //            {
            //                Console.WriteLine("</{0}>", reader.Name);
            //            }
            //        }
            //    }
            //}
        }

        private static void settings_ValidationEventHandler(object sender, ValidationEventArgs e) 
        {
            Console.WriteLine("Validation Error Message: {0}", e.Message);
            Console.WriteLine("Validation Error Severity: {0}", e.Severity);
            if (e.Exception != null)
            {
                Console.WriteLine("Validation Error Line Number: {0}", e.Exception.LineNumber);
                Console.WriteLine("Validation Error Line Position: {0}", e.Exception.LinePosition);
                Console.WriteLine("Validation Error Source: {0}", e.Exception.Source);
                Console.WriteLine("Validation Error Source Schema: {0}", e.Exception.SourceSchemaObject);
                Console.WriteLine("Validation Error Source Uri: {0}", e.Exception.SourceUri);
                Console.WriteLine("Validation Error thrown from: {0}", e.Exception.TargetSite);
                Console.WriteLine("Validation Error callstack: {0}", e.Exception.StackTrace);
            }
        }
        #endregion
	
        #region "15.5 Creating an XML Document Programmatically"	
        public static void CreateXml()
        {
            XElement addressBook = new XElement("AddressBook",
                                         new XElement("Contact",
                                             new XAttribute("name", "Tim"),
                                             new XAttribute("phone", "999-888-0000")),
                                         new XElement("Contact",
                                             new XAttribute("name", "Newman"),
                                             new XAttribute("phone", "666-666-6666")),
                                         new XElement("Contact",
                                             new XAttribute("name", "Harold"),
                                             new XAttribute("phone", "777-555-3333")));
            // Display XML
            Console.WriteLine("Generated XML from XElement:\r\n{0}", addressBook.ToString());
            Console.WriteLine();

                                            
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(Console.Out, settings))
            {
                writer.WriteStartElement("AddressBook");
                writer.WriteStartElement("Contact");
                writer.WriteAttributeString("name", "Tim");
                writer.WriteAttributeString("phone", "999-888-0000");
                writer.WriteEndElement();
                writer.WriteStartElement("Contact");
                writer.WriteAttributeString("name", "Newman");
                writer.WriteAttributeString("phone", "666-666-6666");
                writer.WriteEndElement();
                writer.WriteStartElement("Contact");
                writer.WriteAttributeString("name", "Harold");
                writer.WriteAttributeString("phone", "777-555-3333");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            Console.WriteLine();
            Console.WriteLine();

            // Start by making an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            // create a root node for the document
            XmlElement addrBook = xmlDoc.CreateElement("AddressBook");
            xmlDoc.AppendChild(addrBook);
            // create the Tim contact
            XmlElement contact = xmlDoc.CreateElement("Contact");
            contact.SetAttribute("name","Tim");
            contact.SetAttribute("phone","999-888-0000");
            addrBook.AppendChild(contact);
            // create the Newman contact
            contact = xmlDoc.CreateElement("Contact");
            contact.SetAttribute("name","Newman");
            contact.SetAttribute("phone","666-666-6666");
            addrBook.AppendChild(contact);
            // create the Harold contact
            contact = xmlDoc.CreateElement("Contact");
            contact.SetAttribute("name","Harold");
            contact.SetAttribute("phone","777-555-3333");
            addrBook.AppendChild(contact);

            // Display XML
            Console.WriteLine("Generated XML from XmlDocument:\r\n{0}",addrBook.OuterXml);
            Console.WriteLine();
        }
        #endregion
	
        #region "15.6 Detecting Changes to an XML Document"	
        public static void DetectXmlChanges()
        {
            XDocument xDoc = new XDocument(
                                new XDeclaration("1.0", "UTF-8", "yes"),
                                new XComment("My sample XML"),
                                new XProcessingInstruction("myProcessingInstruction", "value"),
                                new XElement("Root",
                                    new XElement("Node1",
                                        new XAttribute("nodeId", "1"), "FirstNode"),
                                    new XElement("Node2",
                                        new XAttribute("nodeId", "2"), "SecondNode"),
                                    new XElement("Node3",
                                        new XAttribute("nodeId", "1"), "ThirdNode"),
                                    new XElement("Node4",
                                        new XCData(@"<>\&'"))
                                )
                             );
            //Create the event handlers.
            xDoc.Changing += xDoc_Changing;
            xDoc.Changed += xDoc_Changed;
            // Add a new element node.
            XElement element = new XElement("Node5", "Fifth Element");
            xDoc.Root.Add(element);

            // Change the first node
            //doc.DocumentElement.FirstChild.InnerText = "1st Node";
            if(xDoc.Root.FirstNode.NodeType == XmlNodeType.Element)
                ((XElement)xDoc.Root.FirstNode).Value = "1st Node";

            // remove the fourth node
            var query = from e in xDoc.Descendants()
                        where e.Name.LocalName == "Node4"
                        select e;
            XElement[] elements = query.ToArray<XElement>();
            foreach (XElement xelem in elements)
            {
                xelem.Remove();
            }
            // write out the new xml
            Console.WriteLine();
            Console.WriteLine(xDoc.ToString());
            Console.WriteLine();

            string xmlFragment = "<?xml version='1.0'?>" +
                "<!-- My sample XML -->" +
                "<?pi myProcessingInstruction value?>" +
                "<Root>" + 
                "<Node1 nodeId='1'>First Node</Node1>" +
                "<Node2 nodeId='2'>Second Node</Node2>" +
                "<Node3 nodeId='3'>Third Node</Node3>" +
                @"<Node4><![CDATA[<>\&']]></Node4>" +
                "</Root>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFragment);

            //Create the event handlers.
            doc.NodeChanging += new XmlNodeChangedEventHandler(NodeChangingEvent);
            doc.NodeChanged += new XmlNodeChangedEventHandler(NodeChangedEvent);
            doc.NodeInserting += new XmlNodeChangedEventHandler(NodeInsertingEvent);
            doc.NodeInserted += new XmlNodeChangedEventHandler(NodeInsertedEvent);
            doc.NodeRemoving += new XmlNodeChangedEventHandler(NodeRemovingEvent);
            doc.NodeRemoved += new XmlNodeChangedEventHandler(NodeRemovedEvent);

            // Add a new element node.
            XmlElement elem = doc.CreateElement("Node5");
            XmlText text = doc.CreateTextNode("Fifth Element");
            doc.DocumentElement.AppendChild(elem);
            doc.DocumentElement.LastChild.AppendChild(text);

            // Change the first node
            doc.DocumentElement.FirstChild.InnerText = "1st Node";

            // remove the fourth node
            XmlNodeList nodes = doc.DocumentElement.ChildNodes;
            foreach(XmlNode node in nodes)
            {
                if(node.Name == "Node4")
                {
                    doc.DocumentElement.RemoveChild(node);
                    break;
                }
            }

            StringBuilder sb = new StringBuilder(doc.OuterXml.Length);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(sb,settings);
            writer.WriteRaw(doc.OuterXml);
            writer.Close();
            Console.WriteLine(sb.ToString());

            // write out the new xml
            Console.WriteLine(doc.OuterXml);
        }

        private static void xDoc_Changed(object sender, XObjectChangeEventArgs e)
        {
            //Add - An XObject has been or will be added to an XContainer.
            //Name - An XObject has been or will be renamed.
            //Remove - An XObject has been or will be removed from an XContainer.
            //Value - The value of an XObject has been or will be changed. In addition, a change in the serialization of an empty element (either from an empty tag to start/end tag pair or vice versa) raises this event. 
            WriteElementInfo("changed", e.ObjectChange, (XObject)sender);
        }

        private static void xDoc_Changing(object sender, XObjectChangeEventArgs e)
        {
            //Add - An XObject has been or will be added to an XContainer.
            //Name - An XObject has been or will be renamed.
            //Remove - An XObject has been or will be removed from an XContainer.
            //Value - The value of an XObject has been or will be changed. In addition, a change in the serialization of an empty element (either from an empty tag to start/end tag pair or vice versa) raises this event. 
            WriteElementInfo("changing", e.ObjectChange, (XObject)sender);
        }

        private static void WriteElementInfo(string action, XObjectChange change, XObject xobj)
        {
            if (xobj != null)
            {
                Console.WriteLine("XObject: <{0}> {1} {2} with value {3}",
                    xobj.NodeType.ToString(), action, change.ToString(), xobj);
            }
            else
                Console.WriteLine("XObject: <{0}> {1} {2} with null value",
                    xobj.NodeType.ToString(), action, change.ToString());
        }
        
        private static void WriteNodeInfo(string action, XmlNode node)
        {
            if (node.Value != null)
            {
                Console.WriteLine("Element: <{0}> {1} with value {2}", 
                    node.Name,action,node.Value);
            }
            else
                Console.WriteLine("Element: <{0}> {1} with null value", 
                    node.Name,action);
        }

        private static void NodeChangingEvent(object source, XmlNodeChangedEventArgs e)
        {
            WriteNodeInfo("changing",e.Node);
        }

        private static void NodeChangedEvent(object source, XmlNodeChangedEventArgs e)
        {
            WriteNodeInfo("changed",e.Node);
        }

        private static void NodeInsertingEvent(object source, XmlNodeChangedEventArgs e)
        {
            WriteNodeInfo("inserting",e.Node);
        }

        private static void NodeInsertedEvent(object source, XmlNodeChangedEventArgs e)
        {
            WriteNodeInfo("inserted",e.Node);
        }

        private static void NodeRemovingEvent(object source, XmlNodeChangedEventArgs e)
        {
            WriteNodeInfo("removing",e.Node);
        }

        private static void NodeRemovedEvent(object source, XmlNodeChangedEventArgs e)
        {
            WriteNodeInfo("removed",e.Node);
        }
        
        #endregion
	
        #region "15.7 Handling Invalid Characters in an XML String"	
        public static void HandleInvalidChars()
        {
            // set up a string with our invalid chars
            string invalidChars = @"<>\&'";
            XElement element = new XElement("Root",
                                   new XElement("InvalidChars1",
                                       new XCData(invalidChars)),
                                   new XElement("InvalidChars2",invalidChars));
            Console.WriteLine("Generated XElement with Invalid Chars:\r\n{0}", element.ToString());
            Console.WriteLine();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(Console.Out, settings))
            {
                writer.WriteStartElement("Root");
                writer.WriteStartElement("InvalidChars1");
                writer.WriteCData(invalidChars);
                writer.WriteEndElement();
                writer.WriteElementString("InvalidChars2", invalidChars);
                writer.WriteEndElement();
            }
            Console.WriteLine();

            XmlDocument xmlDoc = new XmlDocument();
            // create a root node for the document
            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);

            // create the first invalid character node
            XmlElement invalidElement1 = xmlDoc.CreateElement("InvalidChars1");
            // wrap the invalid chars in a CDATA section and use the 
            // InnerXML property to assign the value as it doesn't
            // escape the values, just passes in the text provided
            invalidElement1.AppendChild(xmlDoc.CreateCDataSection(invalidChars));
            // append the element to the root node
            root.AppendChild(invalidElement1);

            // create the second invalid character node
            XmlElement invalidElement2 = xmlDoc.CreateElement("InvalidChars2");
            // Add the invalid chars directly using the InnerText 
            // property to assign the value as it will automatically
            // escape the values
            invalidElement2.InnerText = invalidChars;
            // append the element to the root node
            root.AppendChild(invalidElement2);

            Console.WriteLine("Generated XML with Invalid Chars:\r\n{0}",xmlDoc.OuterXml);
            Console.WriteLine();
        }
        #endregion
	
        #region "15.8 Transforming XML"	
        public static void TransformXml()
        {
            // LINQ way
            XElement personnelData = XElement.Load(@"..\..\Personnel.xml");
            // Create HTML
            XElement personnelHtml = 
                new XElement("html",
                    new XElement("head"),
                    new XElement("body",
                        new XAttribute("title","Personnel"),
                        new XElement("p",
                            new XElement("table",
                                new XAttribute("border","1"),
                                new XElement("thead",
                                    new XElement("tr",
                                        new XElement("td","Employee Name"),
                                        new XElement("td","Employee Title"),
                                        new XElement("td","Years with Company")
                                        )
                                    ),
                                new XElement("tbody",
                                    from p in personnelData.Elements("Employee")
                                    select new XElement("tr",
                                        new XElement("td",p.Attribute("name").Value),
                                        new XElement("td",p.Attribute("title").Value),
                                        new XElement("td", p.Attribute("companyYears").Value)
                                        )
                                    )
                                )
                            )
                        )
                    );

            personnelHtml.Save(@"..\..\Personnel_LINQ.html");

            var queryCSV = from p in personnelData.Elements("Employee")
                           orderby p.Attribute("name").Value descending
                           select p;
            StringBuilder sb = new StringBuilder();
            foreach(XElement e in queryCSV)
            {
                sb.AppendFormat("{0},{1},{2}{3}",e.Attribute("name").Value,
                    e.Attribute("title").Value,e.Attribute("companyYears").Value,
                    Environment.NewLine);
            }
            using(StreamWriter writer = File.CreateText(@"..\..\Personnel_LINQ.csv"))
            {
                writer.Write(sb.ToString());
            }


            // Create a resolver with default credentials.
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // transform the personnel.xml file to html
            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings();
            // disable both of these (the default) for security reasons
            settings.EnableDocumentFunction = false;
            settings.EnableScript = false;
            // load up the stylesheet
            transform.Load(@"..\..\PersonnelHTML.xsl",settings,resolver);
            // perform the transformation
            transform.Transform(@"..\..\Personnel.xml",@"..\..\Personnel.html");


            // transform the personnel.xml file to comma delimited format

            // load up the stylesheet
            transform.Load(@"..\..\PersonnelCSV.xsl",settings,resolver);
            // perform the transformation
            transform.Transform(@"..\..\Personnel.xml",
                @"..\..\Personnel.csv");
        }
        #endregion
	
        #region "15.9 Tearing Apart an XML Document"	
        public static void ProcessInvoice()
        {
            XElement invElement = XElement.Load(@"..\..\Invoice.xml");
            // Process the billing information to Accounting
            CreateInvoiceEnvelope(invElement, "BillingEnvelope", "billInfo",
                @"..\..\BillingEnvelope_LINQ.xml");

            // Process the shipping information to Accounting
            CreateInvoiceEnvelope(invElement, "ShippingEnvelope", "shipInfo",
                @"..\..\ShippingEnvelope_LINQ.xml");

            // Process the item information to Fulfillment
            CreateInvoiceEnvelope(invElement, "FulfillmentEnvelope", "Items/item",
                @"..\..\FulfillmentEnvelope_LINQ.xml");


            XmlDocument xmlDoc = new XmlDocument();
            // pick up invoice from deposited directory
            xmlDoc.Load(@"..\..\Invoice.xml");
            // get the Invoice element node
            XmlNode Invoice = xmlDoc.SelectSingleNode("/Invoice");

            // get the invoice date attribute
            XmlAttribute invDate = 
                (XmlAttribute)Invoice.Attributes.GetNamedItem("invoiceDate");
            // get the invoice number attribute
            XmlAttribute invNum = 
                (XmlAttribute)Invoice.Attributes.GetNamedItem("invoiceNumber");

            // Process the billing information to Accounting
            WriteInformation(@"..\..\BillingEnvelope.xml",
                            "BillingEnvelope",
                            invDate, invNum, xmlDoc,
                            "/Invoice/billInfo");

            // Process the shipping information to Accounting
            WriteInformation(@"..\..\ShippingEnvelope.xml",
                            "ShippingEnvelope",
                            invDate, invNum, xmlDoc,
                            "/Invoice/shipInfo");

            // Process the item information to Fulfillment
            WriteInformation(@"..\..\FulfillmentEnvelope.xml",
                            "FulfillmentEnvelope",
                            invDate, invNum, xmlDoc,
                            "/Invoice/Items/item");

            // Now send the data to the web services …
        }

        private static void CreateInvoiceEnvelope(XElement invElement, 
                                            string topElementName, 
                                            string internalElementName,
                                            string path)
        {
            var query = from i in invElement.DescendantsAndSelf()
                               where i.NodeType == XmlNodeType.Element &&
                                    i.Name == "Invoice"
                               select new XElement(topElementName,
                                              new XAttribute(i.Attribute("invoiceDate").Name,
                                                        i.Attribute("invoiceDate").Value),
                                              new XAttribute(i.Attribute("invoiceNumber").Name,
                                                        i.Attribute("invoiceNumber").Value),
                                              from e in i.XPathSelectElements(internalElementName)
                                              select new XElement(e));
            XElement envelope = query.ElementAt<XElement>(0);
            Console.WriteLine(envelope.ToString());
            // save the envelope
            envelope.Save(path);
        }

        private static void WriteInformation(string path,
                                    string rootNode,
                                    XmlAttribute invDate,
                                    XmlAttribute invNum,
                                    XmlDocument xmlDoc,
                                    string nodePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer =
                XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(rootNode);
                writer.WriteAttributeString(invDate.Name, invDate.Value);
                writer.WriteAttributeString(invNum.Name, invNum.Value);
                XmlNodeList nodeList = xmlDoc.SelectNodes(nodePath);
                // add the billing information to the envelope
                foreach (XmlNode node in nodeList)
                {
                    writer.WriteRaw(node.OuterXml);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        #endregion
	
        #region "15.10 Putting Together an XML Document"	
        public static void ReceiveInvoice()
        {
            // LINQ Way

            // load the billing 
            XElement billingElement = XElement.Load(@"..\..\BillingEnvelope.xml");
            XElement shippingElement = XElement.Load(@"..\..\ShippingEnvelope.xml");
            XElement fulfillmentElement = XElement.Load(@"..\..\FulfillmentEnvelope.xml");
            XElement invElement = new XElement("Invoice",
                                    // get the invoice date attribute
                                    billingElement.Attribute("invoiceDate"),
                                    // get the invoice number attribute
                                    billingElement.Attribute("invoiceNumber"),
                                    // add the billInfo back in
                                    from b in billingElement.Elements("billInfo")
                                    select b,
                                    // add the shipInfo back in
                                    from s in shippingElement.Elements("shipInfo")
                                    select s,
                                    // add the items back in under Items
                                    new XElement("Items",
                                        from f in fulfillmentElement.Elements("item")
                                        select f));
            // display Invoice XML
            Console.WriteLine(invElement.ToString());
            Console.WriteLine();

            // save our reconstitued invoice
            invElement.Save(@"..\..\ReceivedInvoice_LINQ.xml");

            // XML Doc Way
            XmlDocument invoice = new XmlDocument();
            XmlDocument billing = new XmlDocument();
            XmlDocument shipping = new XmlDocument();
            XmlDocument fulfillment = new XmlDocument();

            // set up root invoice node
            XmlElement invoiceElement = invoice.CreateElement("Invoice");
            invoice.AppendChild(invoiceElement);

            // load the billing 
            billing.Load(@"..\..\BillingEnvelope.xml");
            // get the invoice date attribute
            XmlAttribute invDate = (XmlAttribute)
                billing.DocumentElement.Attributes.GetNamedItem("invoiceDate");
            // get the invoice number attribute
            XmlAttribute invNum = (XmlAttribute)
                billing.DocumentElement.Attributes.GetNamedItem("invoiceNumber");
            // set up the invoice with this info
            invoice.DocumentElement.Attributes.SetNamedItem(invDate.Clone());
            invoice.DocumentElement.Attributes.SetNamedItem(invNum.Clone());
            // add the billInfo back in
            XmlNodeList billList = billing.SelectNodes("/BillingEnvelope/billInfo");
            foreach (XmlNode billInfo in billList)
            {
                invoice.DocumentElement.AppendChild(invoice.ImportNode(billInfo, true));
            }

            // load the shipping 
            shipping.Load(@"..\..\ShippingEnvelope.xml");
            // add the shipInfo back in
            XmlNodeList shipList = shipping.SelectNodes("/ShippingEnvelope/shipInfo");
            foreach (XmlNode shipInfo in shipList)
            {
                invoice.DocumentElement.AppendChild(invoice.ImportNode(shipInfo, true));
            }

            // load the items
            fulfillment.Load(@"..\..\FulfillmentEnvelope.xml");

            // Create an Items element in the Invoice to add these under
            XmlElement items = invoice.CreateElement("Items");

            // add the items back in under Items
            XmlNodeList itemList = fulfillment.SelectNodes("/FulfillmentEnvelope/item");
            foreach (XmlNode item in itemList)
            {
                items.AppendChild(invoice.ImportNode(item, true));
            }

            // add it in
            invoice.DocumentElement.AppendChild(items.Clone());

            // display Invoice XML
            Console.WriteLine("Invoice:\r\n{0}", invoice.OuterXml);

            // save our reconstitued invoice
            invoice.Save(@"..\..\ReceivedInvoice.xml");
        }
        #endregion

		#region "15.11 Re-Validate modified XML documents without reloading"
        public class ValidationHandler
        {
            private object _syncRoot = new object();

            public ValidationHandler()
            {
                lock(_syncRoot)
                {
                    // set the initial check for validity to true
                    this.ValidXml = true;
                }
            }

            public bool ValidXml { get; private set; }

            public void HandleValidation(object sender, ValidationEventArgs e)
            {
                lock(_syncRoot)
                {
                    // we got called so this isn't valid
                    ValidXml = false;
                    Console.WriteLine("Validation Error Message: {0}", e.Message);
                    Console.WriteLine("Validation Error Severity: {0}", e.Severity);
                    if (e.Exception != null)
                    {
                        Console.WriteLine("Validation Error Line Number: {0}", e.Exception.LineNumber);
                        Console.WriteLine("Validation Error Line Position: {0}", e.Exception.LinePosition);
                        Console.WriteLine("Validation Error Source: {0}", e.Exception.Source);
                        Console.WriteLine("Validation Error Source Schema: {0}", e.Exception.SourceSchemaObject);
                        Console.WriteLine("Validation Error Source Uri: {0}", e.Exception.SourceUri);
                        Console.WriteLine("Validation Error thrown from: {0}", e.Exception.TargetSite);
                        Console.WriteLine("Validation Error callstack: {0}", e.Exception.StackTrace);
                    }
                }
            }
        }

        public static void TestContinualValidation()
        {
            // Create the schema set
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            // add the new schema with the target namespace
            // (could add all the schema at once here if there are multiple)
            xmlSchemaSet.Add("http://tempuri.org/Book.xsd",
                XmlReader.Create(@"..\..\Book.xsd"));
            XDocument book = XDocument.Load(@"..\..\Book.xml");
            ValidationHandler validationHandler = new ValidationHandler();
            ValidationEventHandler validationEventHandler = validationHandler.HandleValidation;
            // validate after load
            book.Validate(xmlSchemaSet, validationEventHandler);

            // add in a new node that is not in the schema
            // since we have already validated, no callbacks fire during the add...
            book.Root.Add(new XElement("BogusElement","Totally"));
            // now we will do validation of the new stuff we added
            book.Validate(xmlSchemaSet, validationEventHandler);
            
            if (validationHandler.ValidXml)
                Console.WriteLine("Successfully validated modified LINQ XML");
            else
                Console.WriteLine("Modified LINQ XML did not validate successfully");
            Console.WriteLine();

            string xmlFile = @"..\..\Book.xml";
            string xsdFile = @"..\..\Book.xsd";

            // Create the schema set
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            // add the new schema with the target namespace
            // (could add all the schema at once here if there are multiple)
            schemaSet.Add("http://tempuri.org/Book.xsd", XmlReader.Create(xsdFile));

            // load up the xml file
            XmlDocument xmlDoc = new XmlDocument();
            // add the schema
            xmlDoc.Schemas = schemaSet;
            // validate after load
            xmlDoc.Load(xmlFile);
            ValidationHandler handler = new ValidationHandler();
            ValidationEventHandler eventHandler = handler.HandleValidation;
            xmlDoc.Validate(eventHandler);

            // add in a new node that is not in the schema
            // since we have already validated, no callbacks fire during the add...
            XmlNode newNode = xmlDoc.CreateElement("BogusElement");
            newNode.InnerText = "Totally";
            // add the new element
            xmlDoc.DocumentElement.AppendChild(newNode);
            // now we will do validation of the new stuff we added
            xmlDoc.Validate(eventHandler);

            if (handler.ValidXml)
                Console.WriteLine("Successfully validated modified XML");
            else
                Console.WriteLine("Modified XML did not validate successfully");
        }

        #endregion
				
		#region "15.12 Extending transformations"
        public static void TestExtendingTransformations()
        {
            XElement publications = XElement.Load(@"..\..\publications.xml");
            XElement transformedPublications =
                new XElement("PublishedWorks",
                    from b in publications.Elements("Book")
                    select new XElement(b.Name,
                               new XAttribute(b.Attribute("name")),
                               from c in b.Elements("Chapter")
                               select new XElement("Chapter", GetErrata(c))));
            Console.WriteLine(transformedPublications.ToString());
            Console.WriteLine();


            string xmlFile = @"..\..\publications.xml";
            string xslt = @"..\..\publications.xsl";

            //Create the XslCompiledTransform and load the style sheet.
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(xslt);
            // load the xml
            XPathDocument xPathDoc = new XPathDocument(xmlFile);

            // make up the args for the stylesheet with the extension object
            XsltArgumentList xslArg = new XsltArgumentList();
            XslExtensionObject xslExt = new XslExtensionObject();
            xslArg.AddExtensionObject("urn:xslext", xslExt);

            // send output to the console and do the transformation
            using (XmlWriter writer = XmlWriter.Create(Console.Out))
            {
                transform.Transform(xPathDoc, xslArg, writer);
            }
        }

        private static XElement GetErrata(XElement chapter)
        {
            // In here we could go do other lookup calls (XML, database, web service) to get information to
            // add back in to the transformation result
            string errata = string.Format("{0} has {1} errata", chapter.Value, chapter.Value.Length);
            return new XElement("Errata", errata);
        }

        // Our extension object to help with functionality
        public class XslExtensionObject
        {
            public XPathNodeIterator GetErrata(XPathNodeIterator nodeChapter)
            {
                // In here we could go do other lookup calls (XML, database, web service) to get information to
                // add back in to the transformation result
                nodeChapter.MoveNext();
                string errata = string.Format("<Errata>{0} has {1} errata</Errata>", nodeChapter.Current.Value, nodeChapter.Current.Value.Length);
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(errata);
                XPathNavigator xPathNav = xDoc.CreateNavigator();
                xPathNav.MoveToChild(XPathNodeType.Element);
                XPathNodeIterator iter = xPathNav.Select(".");
                return iter;
            }
        }

		#endregion

		#region "15.13 Get your schemas in bulk from existing XML files"
        public static void TestBulkSchema()
        {
            DirectoryInfo di = new DirectoryInfo(@"..\..");
            string dir = di.FullName;
            GenerateSchemasForDirectory(dir);
        }

        public static void GenerateSchemasForFile(string file)
        {
            // set up a reader for the file
            using (XmlReader reader = XmlReader.Create(file))
            {
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                XmlSchemaInference schemaInference =
                                new XmlSchemaInference();

                // get the schema
                schemaSet = schemaInference.InferSchema(reader);

                string schemaPath = string.Empty;
                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    // make schema file path and write it out
                    schemaPath = Path.GetDirectoryName(file) + @"\" +
                                    Path.GetFileNameWithoutExtension(file) + ".xsd";
                    using (FileStream fs =
                        new FileStream(schemaPath, FileMode.OpenOrCreate))
                    {
                        schema.Write(fs);
                    }
                }
            }
        }

        public static void GenerateSchemasForDirectory(string dir)
        {
            // make sure the directory exists
            if (Directory.Exists(dir))
            {
                // get the files in the directory
                string[] files = Directory.GetFiles(dir, "*.xml");
                foreach (string file in files)
                {
                    GenerateSchemasForFile(file);
                }
            }
        }
		#endregion

		#region "15.14 Passing parameters to transforms"
        public static void TestXsltParameters()
        {
            // transform using LINQ instead of XSLT
            string storeTitle = "Hero Comics Inventory";
            string pageDate = DateTime.Now.ToString("F");
            XElement parameterExample = XElement.Load(@"..\..\ParameterExample.xml");
            string htmlPath = @"..\..\ParameterExample_LINQ.htm";
            TransformWithParameters(storeTitle, pageDate, parameterExample, htmlPath);

            // now change the parameters
            storeTitle = "Fabulous Adventures Inventory";
            pageDate = DateTime.Now.ToString("D");
            htmlPath = @"..\..\ParameterExample2_LINQ.htm";
            TransformWithParameters(storeTitle, pageDate, parameterExample, htmlPath);

            //transform using XSLT and parameters
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("storeTitle", "", "Hero Comics Inventory");
            args.AddParam("pageDate", "", DateTime.Now.ToString("F"));

            // Create a resolver with default credentials.
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;

            XslCompiledTransform transform = new XslCompiledTransform();
            // load up the stylesheet
            transform.Load(@"..\..\ParameterExample.xslt", XsltSettings.Default, resolver);
            // perform the transformation
            FileStream fs = null;
            using (fs =
                new FileStream(@"..\..\ParameterExample.htm",
                                FileMode.OpenOrCreate, FileAccess.Write))
            {
                transform.Transform(@"..\..\ParameterExample.xml", args, fs);
                fs.Flush();
            }

            // now change the parameters and reprocess
            args = new XsltArgumentList();
            args.AddParam("storeTitle", "", "Fabulous Adventures Inventory");
            args.AddParam("pageDate", "", DateTime.Now.ToString("D"));
            using (fs = new FileStream(@"..\..\ParameterExample2.htm",
                FileMode.OpenOrCreate, FileAccess.Write))
            {
                transform.Transform(@"..\..\ParameterExample.xml", args, fs);
                fs.Flush();
            }
        }

        private static void TransformWithParameters(string storeTitle, string pageDate, 
            XElement parameterExample, string htmlPath)
        {
            XElement transformedParameterExample = 
                new XElement("html",
                    new XElement("head"),
                    new XElement("body",
                        new XElement("h3", string.Format("Brought to you by {0} on {1}{2}",
                            storeTitle,pageDate,Environment.NewLine)),
                        new XElement("br"),
                        new XElement("table",
                            new XAttribute("border","2"),
                            new XElement("thead",
                                new XElement("tr",
                                    new XElement("td",
                                        new XElement("b","Heroes")),
                                    new XElement("td",
                                        new XElement("b","Edition")))),
                            new XElement("tbody",
                                from cb in parameterExample.Elements("ComicBook")
                                orderby cb.Attribute("name").Value descending
                                select new XElement("tr",
                                        new XElement("td",cb.Attribute("name").Value),
                                        new XElement("td",cb.Attribute("edition").Value))))));
            transformedParameterExample.Save(htmlPath);        
        }
		#endregion
	}
}
