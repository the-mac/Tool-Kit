using System;
using System.IO;
using System.Net;
using System.Xml;

namespace ContosoMobile.Authentication
{
    public static class Utilities
    {
        /// <summary>
        /// Helper method to extract the response string from an HttpWebResponse object.
        /// </summary>
        /// <param name="response">The HttpWebResponse object</param>
        /// <returns>The response string contained in the http response.</returns>
        public static string GetResponseStringFromResponse(HttpWebResponse response)
        {
            string responseString = string.Empty;

            if (response != null)
            {
                Stream streamResponse = null;
                try
                {
                    streamResponse = response.GetResponseStream();
                    using (StreamReader streamRead = new StreamReader(streamResponse))
                    {
                        streamResponse = null;
                        responseString = streamRead.ReadToEnd();
                    }
                }
                finally
                {
                    if (streamResponse != null)
                    {
                        streamResponse.Dispose();
                    }
                }
            }
            return responseString;
        }

        /// <summary>
        /// Helper method to extract the SAML token response string.
        /// </summary>
        /// <param name="responseString">Response string from an HttpWebResponse object</param>
        /// <returns></returns>
        public static string GetSamlTokenFromResponseString(string responseString)
        {
            string samlToken = string.Empty;

            //Parse the xml to find the <saml:Assertion> tag as per SAML11 standard.  This is the ADFS token           
            StringReader reader = null;
            try
            {
                reader = new StringReader(responseString);
                using (XmlReader xmlparser = XmlReader.Create(reader))
                {
                    reader = null;
                    while (xmlparser.Read())
                    {
                        switch (xmlparser.NodeType)
                        {
                            case XmlNodeType.Element:
                                string name = xmlparser.Name;
                                if (String.Equals(xmlparser.Name, "saml:Assertion"))
                                {
                                    samlToken = xmlparser.ReadOuterXml();
                                }
                                break;
                        }
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return samlToken;
        }

        /// <summary>
        /// Helper method to look at the response from the Security Token Service and extract the error message.
        /// </summary>
        /// <param name="response">The HttpWebResponse object</param>
        /// <returns>The error string.</returns>
        public static string GetErrorFromStsResponseString(string response)
        {
            string error = String.Empty;
            
            StringReader reader = null;
            try
            {
                reader = new StringReader(response);
                using (XmlReader xmlparser = XmlReader.Create(reader))
                {
                    reader = null;
                    if (xmlparser.ReadToDescendant("s:Subcode"))
                    {
                        if (xmlparser.ReadToDescendant("s:Value"))
                        {
                            error = xmlparser.ReadInnerXml();
                        }
                    }
                    if (String.IsNullOrEmpty(error) && xmlparser.ReadToFollowing("s:Text"))
                    {
                        error = xmlparser.ReadInnerXml();
                    }
                }
            }
            catch (XmlException)
            {
                error = "XmlException in ADFS Error Response";
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return error;
        }
    }
}