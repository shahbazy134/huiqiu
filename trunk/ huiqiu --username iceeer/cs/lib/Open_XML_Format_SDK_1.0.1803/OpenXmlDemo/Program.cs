using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using System.Xml;
using System.IO;
using System.IO.Packaging;

namespace OpenXmlDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length < 3)
            {
                // if command-line arguments were omitted, show user proper syntax
                Console.WriteLine("SYNTAX: OpenXmlDemo filename.docx \"text\"");
                Console.ReadLine();
                return;
            }
            string Filename = arguments[1];  // 1st argument = output filename
            string BodyText = arguments[2];  // 2nd argument = text to include

            try
            {
                CreateDOCX(Filename, BodyText);

                Console.WriteLine(Filename + " has been created.");

                Console.ReadLine();
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Add a new document part containing custom XML from an external file and then populate the document part
        /// </summary>
        /// <param name="docName">the file name of document</param>
        /// <param name="authorName">the name of author</param>
        public static void WDAcceptRevisions(string docName, string authorName)
        {
            // Given a document name and an author name, accept revisions. 
            // Note: leave author name blank to accept revisions for all    

            const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(docName, true))
            {
                //  Manage namespaces to perform Xml XPath queries.
                NameTable nt = new NameTable();
                XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                nsManager.AddNamespace("w", wordmlNamespace);

                //  Get the document part from the package.
                XmlDocument xdoc = new XmlDocument(nt);
                //  Load the XML in the document part into an XmlDocument instance.
                xdoc.Load(wdDoc.MainDocumentPart.GetStream());

                //  Handle the formatting changes.
                XmlNodeList nodes = null;
                if (string.IsNullOrEmpty(authorName))
                {
                    nodes = xdoc.SelectNodes("//w:pPrChange", nsManager);
                }
                else
                {
                    nodes = xdoc.SelectNodes(string.Format("//w:pPrChange[@w:author='{0}']", authorName), nsManager);
                }
                foreach (System.Xml.XmlNode node in nodes)
                {
                    node.ParentNode.RemoveChild(node);
                }

                //  Handle the deletions.
                if (string.IsNullOrEmpty(authorName))
                {
                    nodes = xdoc.SelectNodes("//w:del", nsManager);
                }
                else
                {
                    nodes = xdoc.SelectNodes(string.Format("//w:del[@w:author='{0}']", authorName), nsManager);
                }

                foreach (System.Xml.XmlNode node in nodes)
                {
                    node.ParentNode.RemoveChild(node);
                }


                //  Handle the insertions.
                if (string.IsNullOrEmpty(authorName))
                {
                    nodes = xdoc.SelectNodes("//w:ins", nsManager);
                }
                else
                {
                    nodes = xdoc.SelectNodes(string.Format("//w:ins[@w:author='{0}']", authorName), nsManager);
                }

                foreach (System.Xml.XmlNode node in nodes)
                {
                    //  You found one or more new content.
                    //  Promote them to the same level as node, and then
                    //  delete the node.
                    XmlNodeList childNodes;
                    childNodes = node.SelectNodes(".//w:r", nsManager);
                    foreach (System.Xml.XmlNode childNode in childNodes)
                    {
                        if (childNode == node.FirstChild)
                        {
                            node.ParentNode.InsertAfter(childNode, node);
                        }
                        else
                        {
                            node.ParentNode.InsertAfter(childNode, node.NextSibling);
                        }
                    }
                    node.ParentNode.RemoveChild(node);

                    //  Remove the modification id from the node 
                    //  so Word can merge it on the next save.
                    node.Attributes.RemoveNamedItem("w:rsidR");
                    node.Attributes.RemoveNamedItem("w:rsidRPr");
                }

                //  Save the document XML back to its document part.
                xdoc.Save(wdDoc.MainDocumentPart.GetStream(FileMode.Create));
            }
        }


        /// <summary>
        /// The CreateDOCX method can be used as the starting point for a document-creation method in a class library,
        /// WinForm app, web page, or web service.
        /// </summary>
        /// <param name="fileName">the document name to create</param>
        /// <param name="BodyText">the text to include</param>
        static void CreateDOCX(string fileName, string BodyText)
        {
            // use the Open XML namespace for WordprocessingML:
            string WordprocessingML = "http://schemas.openxmlformats.org/wordprocessingml/2006/3/main";

            // create the start part, set up the nested structure ...
            XmlDocument xmlStartPart = new XmlDocument();
            XmlElement tagDocument = xmlStartPart.CreateElement("w:document", WordprocessingML);
            xmlStartPart.AppendChild(tagDocument);
            XmlElement tagBody = xmlStartPart.CreateElement("w:body", WordprocessingML);
            tagDocument.AppendChild(tagBody);
            XmlElement tagParagraph = xmlStartPart.CreateElement("w:p", WordprocessingML);
            tagBody.AppendChild(tagParagraph);
            XmlElement tagRun = xmlStartPart.CreateElement("w:r", WordprocessingML);
            tagParagraph.AppendChild(tagRun);
            XmlElement tagText = xmlStartPart.CreateElement("w:t", WordprocessingML);
            tagRun.AppendChild(tagText);

            // Note nesting of tags for the WordprocessingML document (the "start part") ...
            // w:document contains ...
            //     w:body, which contains ...
            //         w:p (paragraph), which contains ...
            //             w:r (run), which contains ...
            //                 w:t (text)

            // insert text into the start part, as a "Text" node ...
            XmlNode nodeText = xmlStartPart.CreateNode(XmlNodeType.Text, "w:t", WordprocessingML);
            nodeText.Value = BodyText;
            tagText.AppendChild(nodeText);
            
            // create a new package (Open XML document) ...
            Package pkgOutputDoc = null;
            pkgOutputDoc = Package.Open(fileName, FileMode.Create, FileAccess.ReadWrite);

            // save the main document part (document.xml) ...
            Uri uri = new Uri("/word/document.xml", UriKind.Relative);
            PackagePart partDocumentXML = pkgOutputDoc.CreatePart(uri, "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml");
            StreamWriter streamStartPart = new StreamWriter(partDocumentXML.GetStream(FileMode.Create, FileAccess.Write));
            xmlStartPart.Save(streamStartPart);
            streamStartPart.Close();
            pkgOutputDoc.Flush();

            // create the relationship part, close the document ...
            pkgOutputDoc.CreateRelationship(uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "rId1");
            pkgOutputDoc.Flush();
            pkgOutputDoc.Close();
        }
    }
}
