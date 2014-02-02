/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;

namespace CatalogServiceMock
{
    //this service can be used to mock the IAp browse response when building your application and testing using the WP8 Emulator.     
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class IapMockBrowseService
    {
        private static readonly List<IAPDetails> iapCatalog = new List<IAPDetails>();

        //* we will read the xml and create the IAPs in memory at service start
        static IapMockBrowseService()
        {
            string strFilePath = string.Concat(HostingEnvironment.ApplicationPhysicalPath, @"Iapcatalog.xml");


            //read from file               
            using (var fs1 = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {
                readIapCatalog(fs1);
            }
        }

        private static void readIapCatalog(FileStream fs1)
        {
            using (var rd1 = new StreamReader(fs1))
            {
                string xml = rd1.ReadToEnd();

                XDocument xDoc = XDocument.Parse(xml);

                foreach (XElement node in xDoc.Element("Catalog").Elements("Item"))
                {
                    List<string> keywords = (from n in node.Elements("Keyword") select n.Value).ToList();
                    
                    var item = new IAPDetails(keywords.ToArray())
                                   {
                                       IapId = node.Element("ProductId") != null ? node.Element("ProductId").Value : string.Empty, 
                                       IapTitle = node.Element("Title") != null ? node.Element("Title").Value : string.Empty, 
                                       IapDescription = node.Element("Description") != null ? node.Element("Description").Value : string.Empty,
                                       IapType = node.Element("Type") != null ? node.Element("Type").Value : string.Empty, 
                                       IapTag = node.Element("Tag") != null ? node.Element("Tag").Value : string.Empty
                                   };

                    
                    iapCatalog.Add(item);
                }
            }
        }

        [WebInvoke(Method = "POST", UriTemplate = "/catalog/apps/{appid}/iaps", RequestFormat = WebMessageFormat.Xml)]
        public Stream getIAPDetails(string appid, Stream body)
        {
            char[] sep = {','};
            int iapRequestKeywordCount = 0;

            try
            {
                //check if tags are supplied
                string[] iapRequestkeywords = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["iapTag"] == null ? null : WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["iapTag"].Split(sep);
                
                string iapIDrequested = new StreamReader(body).ReadToEnd();
                var validIapRequestKeywords = new string[10];


                WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;

                var productDocument = new XmlDocument();
                productDocument.LoadXml(iapIDrequested);

                //check if there are product IDs provided by the caller
                //@ expected format - <BatchGetIapDetailsRequest ><Resources><Resource><IapId>taptitude_gold</IapId></Resource></Resources></BatchGetIapDetailsRequest>
                XmlNodeList productID = productDocument.GetElementsByTagName("Resource");


                //check if there are any tags in the request query.
                if (iapRequestkeywords != null)
                {
                    for (int p = 0, q = 0; p < iapRequestkeywords.Length; p++)
                    {
                        validIapRequestKeywords[q] = iapRequestkeywords[p].Length == 0 ? string.Empty : iapRequestkeywords[p].Trim();

                        if (validIapRequestKeywords[q] != string.Empty)
                        {
                            q++;
                            iapRequestKeywordCount = q;
                        }
                    }
                }

                //get the IAP details for specific product IDs as listed by the application.
                if (productID.Count != 0)
                {
                    var response = new BatchGetIapDetailsResponse();

                    foreach (IAPDetails iap in iapCatalog)
                    {
                        var iapkeywords = new string[iap.KeywordCount];
                        bool hasProdcutID = false;
                        for (int i = 0; i < iap.KeywordCount; i++)
                            iapkeywords[i] = iap[i];

                        foreach (XmlNode node in productID)
                        {
                            if (iap.IapId.Equals(node.FirstChild.InnerText, StringComparison.CurrentCultureIgnoreCase))
                                hasProdcutID = true;
                        }

                        if (!hasProdcutID)
                            continue;

                        response.Entries.Add(new IAPResponseEntry
                                                 {
                                                     IapId = iap.IapId,
                                                     ParentProductId = new Guid(appid),
                                                     IapProductId = iap.IapProductId,
                                                     IapTitle = iap.IapTitle,
                                                     IapDescription = iap.IapDescription,
                                                     IapType = iap.IapType,
                                                     IapDevData = iap.IapTag,
                                                     IapTags = iapkeywords,
                                                 });
                    }


                    return new MemoryStream(Encoding.UTF8.GetBytes(response.Generate().ToString()));
                }
                
                if (iapRequestKeywordCount != 0)
                {
                    var response = new BatchGetIapDetailsResponse();
                    //return the entire list
                    foreach (IAPDetails iap in iapCatalog)
                    {
                        var iapkeywords = new string[iap.KeywordCount];

                        for (int i = 0; i < iap.KeywordCount; i++)
                            iapkeywords[i] = iap[i];

                        if (matchKeywords(iap, validIapRequestKeywords, iapRequestKeywordCount, 0))
                        {
                            response.Entries.Add(new IAPResponseEntry
                                                     {
                                                         IapId = iap.IapId,
                                                         ParentProductId = new Guid(appid),
                                                         IapProductId = iap.IapProductId,
                                                         IapTitle = iap.IapTitle,
                                                         IapDescription = iap.IapDescription,
                                                         IapType = iap.IapType,
                                                         IapDevData = iap.IapTag,
                                                         IapTags = iapkeywords,
                                                     });
                        }
                    }

                    return new MemoryStream(Encoding.UTF8.GetBytes(response.Generate().ToString()));
                }
                else
                {
                    var response = new BatchGetIapDetailsResponse();

                    //return the entire list
                    foreach (IAPDetails iap in iapCatalog)
                    {
                        var iapkeywords = new string[iap.KeywordCount];

                        for (int i = 0; i < iap.KeywordCount; i++)
                            iapkeywords[i] = iap[i];


                        response.Entries.Add(new IAPResponseEntry
                                                 {
                                                     IapId = iap.IapId,
                                                     ParentProductId = new Guid(appid),
                                                     IapProductId = iap.IapProductId,
                                                     IapTitle = iap.IapTitle,
                                                     IapDescription = iap.IapDescription,
                                                     IapType = iap.IapType,
                                                     IapDevData = iap.IapTag,
                                                     IapTags = iapkeywords,
                                                 });
                    }


                    return new MemoryStream(Encoding.UTF8.GetBytes(response.Generate().ToString()));
                }
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = e.ToString();
                return null;
            }
        }


        [WebInvoke(Method = "GET", UriTemplate = "/catalog/apps/{appid}", RequestFormat = WebMessageFormat.Xml)]
        public Stream getAppDetails(string appid)
        {
            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;

            return new MemoryStream(Encoding.UTF8.GetBytes(AppDetails.GetAppDetails(appid)));
        }

        [WebInvoke(Method = "GET", UriTemplate = "/images/{imageid}", RequestFormat = WebMessageFormat.Xml)]
        public Stream getIAPIcon(string imageid)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";

            string fileName = null;
            byte[] fileBytes = null;

            try
            {
                fileName = string.Concat(HostingEnvironment.ApplicationPhysicalPath, @"iapIcon.png");

                if (File.Exists(fileName))
                {
                    using (FileStream f = File.OpenRead(fileName))
                    {
                        fileBytes = new byte[f.Length];
                        f.Read(fileBytes, 0, Convert.ToInt32(f.Length));
                    }
                }
                else
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            }
            catch
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            }


            return new MemoryStream(fileBytes, 0, fileBytes.Length);
        }

        //check if the iap item matches the supplied tags - a IAP is returned if it matches all the tags supplied on the web request
        private bool matchKeywords(IAPDetails iap, string[] iapkeywords, int keywordcount, int position)
        {
            if (iap.KeywordCount >= keywordcount)
            {
                if (position == keywordcount)
                    return true;

                //loop through each keyword associated with the IAP object
                for (int i = 0; i < iap.KeywordCount; i++)
                {
                    //compare with 
                    bool keywordmatch = iap[i].Equals(iapkeywords[position], StringComparison.InvariantCultureIgnoreCase) ? true : false;
                    if (keywordmatch) return matchKeywords(iap, iapkeywords, keywordcount, position + 1);
                }

                return false;
            }

            return false;
        }
    }
}
