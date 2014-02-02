using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace Wazup.Services
{
    /// <summary>
    /// Provides facade for accessing twitter service
    /// </summary>
    public static class TwitterService
    {
        private const string Wazup_WhatTheTrendApplicationKey = "replace-with-your-private-key";
        private const string WhatTheTrendSearchQuery = "http://api.whatthetrend.com/api/v2/trends.json?api_key={0}";
        private const string TwitterSearchQuery = "http://search.twitter.com/search.json?q={0}";

        /// <summary>
        /// Gets the trends.
        /// </summary>
        /// <param name="onGetTrendsCompleted">The on get trends completed.</param>
        /// <param name="onError">The on error.</param>
        public static void GetTrends(Action<IEnumerable<Trend>> onGetTrendsCompleted = null, Action<Exception> onError = null, Action onFinally = null)
        {
            WebClient webClient = new WebClient();

            // register on download complete event
            webClient.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
                {
                    // report error
                    if (e.Error != null)
                    {
                        if (onError != null)
                        {
                            onError(e.Error);
                        }
                        return;
                    }

                    // convert json result to model
                    Stream stream = e.Result;
                    DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TrendsResults));
                    TrendsResults trendsResults = (TrendsResults)dataContractJsonSerializer.ReadObject(stream);

                    // notify completed callback
                    if (onGetTrendsCompleted != null)
                    {
                        onGetTrendsCompleted(trendsResults.trends);
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

            webClient.OpenReadAsync(new Uri(string.Format(WhatTheTrendSearchQuery, Wazup_WhatTheTrendApplicationKey)));
        }

        /// <summary>
        /// Searches the specified search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="onSearchCompleted">The on search completed.</param>
        /// <param name="onError">The on error.</param>
        public static void Search(string searchText, Action<IEnumerable<Tweet>> onSearchCompleted = null, Action<Exception> onError = null, Action onFinally = null)
        {
            WebClient webClient = new WebClient();

            // register on download complete event
            webClient.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
                {
                    // report error
                    if (e.Error != null)
                    {
                        if (onError != null)
                        {
                            onError(e.Error);
                        }
                        return;
                    }

                    // convert json result to model
                    Stream stream = e.Result;
                    DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TwitterResults));
                    TwitterResults twitterResults = (TwitterResults)dataContractJsonSerializer.ReadObject(stream);

                    // notify completed callback
                    if (onSearchCompleted != null)
                    {
                        onSearchCompleted(twitterResults.results);
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

            string encodedSearchText = HttpUtility.UrlEncode(searchText);
            webClient.OpenReadAsync(new Uri(string.Format(TwitterSearchQuery, encodedSearchText)));
        }
    }
}
