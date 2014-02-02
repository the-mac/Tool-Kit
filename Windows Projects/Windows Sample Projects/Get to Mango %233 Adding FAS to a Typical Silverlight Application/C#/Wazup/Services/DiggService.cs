using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;


namespace Wazup.Services
{
    /// <summary>
    /// Provides facade for accessing digg service
    /// </summary>
    public static class DiggService
    {
        private const string Wazup_DiggApplicationKey = "http://www.myapp.com";
        private const string DiggSearchQuery = "http://services.digg.com/search/stories?query={0}&appkey={1}";
        //private const string DiggV2SearchQuery = "http://services.digg.com/2.0/search.search?query={0}";

        /// <summary>
        /// Searches the specified search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="onSearchCompleted">The on search completed.</param>
        /// <param name="onError">The on error.</param>
        public static void Search(string searchText, Action<IEnumerable<DiggStory>> onSearchCompleted = null, Action<string, Exception> onError = null, Action onFinally = null)
        {
            WebClient webClient = new WebClient();
            
            // register on download complete event
            webClient.DownloadStringCompleted += delegate (object sender, DownloadStringCompletedEventArgs e) 
            {
                try
                {
                    // report error
                    if (e.Error != null)
                    {
//#if DEBUG
//                        onSearchCompleted(GetStoriesMock());
//                        return;
//#endif
                        if (onError != null)
                        {
                            onError(searchText, e.Error);
                        }
                        return;
                    }

                    // convert xml result to model
                    XElement storyXml = XElement.Parse(e.Result);

                    var stories = from story in storyXml.Descendants("story")
                                  select new DiggStory(
                                      story.Element("title").Value,
                                      story.Element("description").Value,
                                      story.Attribute("link").Value,
                                      int.Parse(story.Attribute("diggs").Value));

                    // notify completed callback
                    if (onSearchCompleted != null)
                    {
                        onSearchCompleted(stories);
                    }
                }
                finally
                {
                    // notify finally callback
                    if (onFinally != null)
                    {
                        onFinally();
                    }
                }
            };

            // start search
            webClient.DownloadStringAsync(new Uri(string.Format(DiggSearchQuery, searchText, Wazup_DiggApplicationKey)));   
            //webClient.DownloadStringAsync(new Uri(string.Format(DiggV2SearchQuery, searchText)));   
            
        }

        private static IEnumerable<DiggStory> GetStoriesMock()
        {
            yield return new DiggStory(
                "Twitter Just Killed Something Else: Their Own Website. Twitter For iPad Is That Good.",
                "Are you addicted to Twitter? Do you have an iPad? Even if the answer to both is no right now, after you see Twitter for iPad, those answers are goi...",
                "http://techcrunch.com/2010/09/01/twitter-for-ipad/",
                41);

            yield return new DiggStory(
                "Jobs suggests that competitors' device activation tallies may be inflated, Google quickly responds",
                "Not satisfied to simply trump Google's daily device activation numbers, Steve Jobs added insult to injury at the Apple press conference this aftern...",
                "http://digg.com/story/r/jobs_suggests_that_competitors_device_activation_tallies_may_be_inflated_google_quickly_responds",
                49);
        }
    }
}
