// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Xml;
using TwitterSample.Model;

namespace TwitterSample.ViewModels
{
    public class TwitterViewModel:INotifyPropertyChanged
    {
        const string SEARCH_URI = "http://search.twitter.com/search.atom?q={0}&page={1}";

        private bool _isLoading = false;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");

            }
        }       

        public TwitterViewModel()
        {
             this.TwitterCollection = new ObservableCollection<TwitterSearchResult>();
             this.IsLoading = false;
             
        }

        public ObservableCollection<TwitterSearchResult> TwitterCollection
        {
            get;
            private set;
        }

        public void LoadPage(string searchTerm, int pageNumber)
        {
            if (pageNumber == 1) this.TwitterCollection.Clear();

            IsLoading = true;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(String.Format(SEARCH_URI, searchTerm, pageNumber)));
       request.BeginGetResponse(new AsyncCallback(ReadCallback), request); 
        }        

        private void ReadCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    //for demo purposes only, dont ship your app with this line of code.
                    System.Threading.Thread.Sleep(700);

                    NameTable nt = new NameTable();

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                    nsmgr.AddNamespace("georss", "http://www.w3.org/2001/XMLSchema-instance");
                    XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                    XmlReaderSettings xset = new XmlReaderSettings();
                    xset.ConformanceLevel = ConformanceLevel.Fragment;


                    XmlReader rdr = XmlReader.Create(reader, xset, context);

                    SyndicationFeed feed = SyndicationFeed.Load(rdr);                    

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {

                        foreach (var item in feed.Items)
                        {

                            this.TwitterCollection.Add(new TwitterSearchResult()
                            {
                                Author = item.Authors[0].Name,
                                ID = GetTweetId(item.Id),
                                Tweet = item.Title.Text,
                                PublishDate = item.PublishDate.DateTime.ToLocalTime(),
                                AvatarUrl = item.Links[1].Uri.AbsoluteUri
                            });

                        }
                        IsLoading = false;
                    });

                }
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                   {
                       MessageBox.Show("Network error occured " + e.Message);
                   });
            }
        }

        private string GetTweetId(string twitterId)
        {
            string[] parts = twitterId.Split(":".ToCharArray());

            return parts[2].ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
