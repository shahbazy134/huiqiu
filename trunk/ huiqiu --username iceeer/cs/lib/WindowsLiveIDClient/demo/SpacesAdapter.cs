using System;
using System.Collections.Generic;
using Microsoft.WindowsLive.Id.Client;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace WindowsLiveIDClientSample
{
    //Adapter class for posting a blog to http://spaces.live.com.
    static class SpacesAdapter
    {
        private const string SPACES_API_URL = "https://storage.msn.com/storageservice/MetaWeblog.rpc";
        private const string requestXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>metaWeblog.newPost</methodName><params><param><value><string>{0}</string></value></param><param><value><string>{1}</string></value></param><param><value><string>{2}</string></value></param><param><value><struct><member><name>title</name><value><string>{3}</string></value></member><member><name>link</name><value><string /></value></member><member><name>description</name><value><string>{4}</string></value></member><member><name>categories</name><value><array><data /></array></value></member></struct></value></param><param><value><boolean>1</boolean></value></param></params></methodCall>";

        public static void PostBlog(string subject, string body, string spaceUrl, Identity oId)
        {
            spaceUrl = ReplaceInvalidXMLChars(spaceUrl);
            subject = ReplaceInvalidXMLChars(subject);
            body = ReplaceInvalidXMLChars(body);
            byte[] postData = new UTF8Encoding(false).GetBytes(String.Format(requestXml, "MyBlog", spaceUrl, "", subject, body));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(SPACES_API_URL);
            string ticket = "";
            //Get ticket for storage service.
            try
            {
                ticket = oId.GetTicket("storage.msn.com", "MBI", true);
            }
            catch (WLLogOnException wlex)
            {
                //Check to see if FlowUrl is defined.
                if (wlex.FlowUrl != null)
                {
                    //If FlowUrl is defined, direct user to the web page to correct the error.
                    MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                }
                else
                {
                    //If FlowUrl is not defined, simply display the ErrorString.
                    MessageBox.Show(wlex.ErrorString);
                }
            }
            request.Headers.Add("Authorization", "WLID1.0 " + ticket);
            request.AllowAutoRedirect = false;
            request.UserAgent = "WindowLiveClientSDKSpacesLoginTester";
            request.ContentType = "text/xml";
            request.Pipelined = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(postData, 0, postData.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();              
        }

        public static string ReplaceInvalidXMLChars(string inString)
        {
            inString = inString.Replace("<", "&lt;");
            inString = inString.Replace(">", "&gt;");
            inString = inString.Replace("\"", "&quot;");
            inString = inString.Replace("'", "&apos;");
            inString = inString.Replace("&", "&amp;");

            return inString;
        }
    }
}
