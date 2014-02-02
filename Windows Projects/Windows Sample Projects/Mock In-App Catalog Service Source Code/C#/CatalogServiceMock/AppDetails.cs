/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
namespace CatalogServiceMock
{
    public class AppDetails
    {
        #region App Details Response Sample String

        public static string AppDetailsResponse = @"<?xml version=""1.0"" encoding=""utf-8""?>
<a:feed xmlns:a=""http://www.w3.org/2005/Atom"" xmlns:os=""http://a9.com/-/spec/opensearch/1.1/"" xmlns=""http://schemas.zune.net/catalog/apps/2008/02"">
<a:link rel=""self"" type=""application/atom+xml"" href=""/v8/catalog/apps/ee29a261-80d0-4bdf-89bd-28b1ebbc8bd3?os=8.0.9744.0&amp;cc=US&amp;oc=&amp;lang=en-US&amp;hw=486539266&amp;dm=P4301&amp;oemId=Nokia&amp;moId=tmo-us&amp;cf=99-1"" />
<a:updated>2012-07-31T06:13:00.945494Z</a:updated>
<a:title type=""text"">TestAppIAPEmulator</a:title>
<a:id>urn:uuid:%AppID%</a:id>
<a:content type=""html"">Demo application to showcase IAP scenarios</a:content>
<iapCount>6</iapCount>
<sortTitle>IAPTestAPP</sortTitle><releaseDate>2012-06-12T01:16:22.860000Z</releaseDate><visibilityStatus>Live</visibilityStatus>
<publisher>shoapps</publisher><averageUserRating>7.333333</averageUserRating><userRatingCount>3</userRatingCount>
<image><id>urn:uuid:705ad4f6-4361-445f-a89a-e1731b40a0db</id></image>
<screenshots><screenshot><id>urn:uuid:35d244ad-2245-4398-9de1-792c470597a6</id><orientation>0</orientation></screenshot></screenshots>
<categories><category><id>windowsphone.Business</id><title>business</title><isRoot>True</isRoot></category></categories>
<tags><tag>Independent</tag></tags><offers><offer><offerId>urn:uuid:b1039472-4c70-420a-bb48-672f7251ccad</offerId>
<mediaInstanceId>urn:uuid:17eb3f31-e3a1-482d-88dd-86f1fe16e523</mediaInstanceId><clientTypes>
<clientType>WindowsPhone80</clientType></clientTypes>
<paymentTypes><paymentType>Credit Card</paymentType><paymentType>Mobile Operator</paymentType></paymentTypes>
<store>ZEST</store><price>0.99</price><displayPrice>$0.99</displayPrice><priceCurrencyCode>USD</priceCurrencyCode>
<licenseRight>Purchase</licenseRight><expiration>2100-01-01T00:00:00Z</expiration></offer></offers>
<taxString>plus applicable taxes</taxString><backgroundImage><id>urn:uuid:04c64ff6-3fdf-483f-b95b-22746908a4a9</id></backgroundImage>
<publisherId>shoapps</publisherId>
<publisherGuid>urn:uuid:ebb4c0bf-0bc1-4f88-a334-a81441ed1d80</publisherGuid>
<a:entry><a:updated>2012-07-31T06:13:00.945494Z</a:updated>
<a:title type=""text"">IAPTestAPP 1.0.0.0</a:title>
<a:id>urn:uuid:17eb3f31-e3a1-482d-88dd-86f1fe16e523</a:id>
<a:content type=""html"">
This is a test for Marketplace Emulator.</a:content><version>1.0.0.0</version>
<payloadId>urn:uuid:1d98e0ea-32cf-4e7d-8677-280b0c510e90</payloadId>
<skuId>urn:uuid:17eb3f31-e3a1-482d-88dd-86f1fe16e523</skuId><skuLastUpdated>2012-06-12T01:16:22.860000Z</skuLastUpdated>
<isAvailableInStore>true</isAvailableInStore><isClientTypeCompatible>true</isClientTypeCompatible><isHardwareCompatible>true</isHardwareCompatible>
<isBlacklisted>false</isBlacklisted><url>http://marketplacecontent.windowsphone.com/public/1db5bff7-5387-3175-9307-0bd32f95c8ee</url><packageSize>7621632</packageSize>
<installSize>39280640</installSize><clientTypes><clientType>WindowsPhone80</clientType></clientTypes>
<supportedLanguages><supportedLanguage>English</supportedLanguage></supportedLanguages>
<deviceCapabilities>&lt;capability&gt;&lt;id&gt;ID_CAP_APPOINTMENTS&lt;/id&gt;&lt;string&gt;appointments&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_CONTACTS&lt;/id&gt;&lt;string&gt;contacts&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_IDENTITY_DEVICE&lt;/id&gt;&lt;string&gt;phone identity&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_IDENTITY_USER&lt;/id&gt;&lt;string&gt;owner identity&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_ISV_CAMERA&lt;/id&gt;&lt;string&gt;video and still capture&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_LOCATION&lt;/id&gt;&lt;string&gt;location services&lt;/string&gt;&lt;disclosure&gt;DiscloseANDPrompt&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_MICROPHONE&lt;/id&gt;&lt;string&gt;microphone&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_NETWORKING&lt;/id&gt;&lt;string&gt;data services&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_PUSH_NOTIFICATION&lt;/id&gt;&lt;string&gt;push notification service&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;capability&gt;&lt;id&gt;ID_CAP_SENSORS&lt;/id&gt;&lt;string&gt;movement and directional sensor&lt;/string&gt;&lt;disclosure&gt;Disclose&lt;/disclosure&gt;&lt;/capability&gt;&lt;hwCapability&gt;&lt;requirementType&gt;Resolution&lt;/requirementType&gt;&lt;id&gt;ID_RESOLUTION_HD720P&lt;/id&gt;&lt;string&gt;HD720P (720x1280)&lt;/string&gt;&lt;required&gt;true&lt;/required&gt;&lt;/hwCapability&gt;&lt;hwCapability&gt;&lt;requirementType&gt;Resolution&lt;/requirementType&gt;&lt;id&gt;ID_RESOLUTION_WVGA&lt;/id&gt;&lt;string&gt;WVGA (480x800)&lt;/string&gt;&lt;required&gt;true&lt;/required&gt;&lt;/hwCapability&gt;&lt;hwCapability&gt;&lt;requirementType&gt;Resolution&lt;/requirementType&gt;&lt;id&gt;ID_RESOLUTION_WXGA&lt;/id&gt;&lt;string&gt;WXGA (768x1280)&lt;/string&gt;&lt;required&gt;true&lt;/required&gt;&lt;/hwCapability&gt;</deviceCapabilities>
</a:entry><a:author><a:name>Microsoft Corporation</a:name></a:author></a:feed>";

        #endregion

        // Get App Details
        public static string GetAppDetails(string AppID)
        {
            return AppDetailsResponse.Replace("%AppID%", AppID);
        }
    }
}
