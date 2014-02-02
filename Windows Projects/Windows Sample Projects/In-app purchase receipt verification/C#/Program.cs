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
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Security.Cryptography;

namespace VerifySignature
{
    class Program
    {
        /// <summary>
        /// Signature tag name in Xml Signature
        /// </summary>
        private const string SignatureTagName = "Signature";
        
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Usage:VerifySignature.exe Signature-File Certificate-File");
                return;
            }

            bool result = false;
            string text = null;

            using (StreamReader streamReader = new StreamReader(args[0]))
            {
                text = streamReader.ReadToEnd();
            }
            
            var testCertificate = new X509Certificate2(args[1]);

            result = VerifyXmlSignature(text, testCertificate, true);
            Console.WriteLine("Result " + result);
            Console.Read();
        }

        /// <summary>
        /// Verifies the xml signature
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="certificate">Certificate</param>
        /// <param name="verifySignatureOnly">Should cert chain be verified</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool VerifyXmlSignature(string input, X509Certificate2 certificate, bool verifySignatureOnly)
        {
            CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");

            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            XmlDocument document = new XmlDocument();

            document.LoadXml(input);

            SignedXml signer = null;

            signer = new SignedXml(document);

            // Find the "Signature" node and create a new XmlNodeList object.
            XmlNodeList nodeList = document.GetElementsByTagName(SignatureTagName);

            if (nodeList.Count == 0)
            {
                throw new Exception();
            }

            // Load the signature node.
            bool result;

            signer.LoadXml((XmlElement)nodeList[0]);
            result = signer.CheckSignature(certificate, verifySignatureOnly);

            return result;
        }
    }
}
