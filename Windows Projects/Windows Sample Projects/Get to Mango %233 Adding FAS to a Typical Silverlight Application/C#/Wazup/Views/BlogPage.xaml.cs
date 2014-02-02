using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Phone.Controls;
using Wazup.Services;

namespace Wazup.Views
{
    public partial class BlogPage : PhoneApplicationPage
    {
        public BlogPage()
        {
            InitializeComponent();
        }

        #region LastPosts

        /// <summary>
        /// LastPosts Dependency Property
        /// </summary>
        public static readonly DependencyProperty LastPostsProperty =
            DependencyProperty.Register("LastPosts", typeof(ObservableCollection<RssItem>), typeof(BlogPage),
                new PropertyMetadata((ObservableCollection<RssItem>)null));

        /// <summary>
        /// Gets or sets the LastPosts property. This dependency property 
        /// indicates what are the last posts.
        /// </summary>
        public ObservableCollection<RssItem> LastPosts
        {
            get { return (ObservableCollection<RssItem>)GetValue(LastPostsProperty); }
            set { SetValue(LastPostsProperty, value); }
        }

        #endregion

        #region Posts

        /// <summary>
        /// Posts Dependency Property
        /// </summary>
        public static readonly DependencyProperty PostsProperty =
            DependencyProperty.Register("Posts", typeof(ObservableCollection<RssItem>), typeof(BlogPage),
                new PropertyMetadata((ObservableCollection<RssItem>)null));

        /// <summary>
        /// Gets or sets the Posts property. This dependency property 
        /// indicates what are all the posts.
        /// </summary>
        public ObservableCollection<RssItem> Posts
        {
            get { return (ObservableCollection<RssItem>)GetValue(PostsProperty); }
            set { SetValue(PostsProperty, value); }
        }

        #endregion

        #region Comments

        /// <summary>
        /// Comments Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommentsProperty =
            DependencyProperty.Register("Comments", typeof(ObservableCollection<RssItem>), typeof(BlogPage),
                new PropertyMetadata((ObservableCollection<RssItem>)null));

        /// <summary>
        /// Gets or sets the Comments property. This dependency property 
        /// indicates what are the posts comments.
        /// </summary>
        public ObservableCollection<RssItem> Comments
        {
            get { return (ObservableCollection<RssItem>)GetValue(CommentsProperty); }
            set { SetValue(CommentsProperty, value); }
        }

        #endregion

        #region Images

        /// <summary>
        /// Images Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImagesProperty =
            DependencyProperty.Register("Images", typeof(ObservableCollection<ImageItem>), typeof(BlogPage),
                new PropertyMetadata((ObservableCollection<ImageItem>)null));

        /// <summary>
        /// Gets or sets the Images property. This dependency property 
        /// indicates what are the images.
        /// </summary>
        public ObservableCollection<ImageItem> Images
        {
            get { return (ObservableCollection<ImageItem>)GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        #endregion

        #region IsPostsLoading

        /// <summary>
        /// IsPostsLoading Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsPostsLoadingProperty =
            DependencyProperty.Register("IsPostsLoading", typeof(bool), typeof(BlogPage),
                new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsPostsLoading property. This dependency property 
        /// indicates whether we are currently loading posts.
        /// </summary>
        public bool IsPostsLoading
        {
            get { return (bool)GetValue(IsPostsLoadingProperty); }
            set { SetValue(IsPostsLoadingProperty, value); }
        }

        #endregion

        #region IsCommentsLoading

        /// <summary>
        /// IsCommentsLoading Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsCommentsLoadingProperty =
            DependencyProperty.Register("IsCommentsLoading", typeof(bool), typeof(BlogPage),
                new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsCommentsLoading property. This dependency property 
        /// indicates whether we are currently loading comments.
        /// </summary>
        public bool IsCommentsLoading
        {
            get { return (bool)GetValue(IsCommentsLoadingProperty); }
            set { SetValue(IsCommentsLoadingProperty, value); }
        }

        #endregion
        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {            
            if (LastPosts != null)
            {
                return;
            }

            // if data wasn't loaded we get it from the blog service
            IsPostsLoading = true;
            BlogService.GetBlogPosts(
                delegate(IEnumerable<RssItem> rssItems)
                {
                    const int NumberOfLastPosts = 5;

                    LastPosts = new ObservableCollection<RssItem>();
                    Posts = new ObservableCollection<RssItem>();

                    foreach (RssItem rssItem in rssItems)
                    {
                        IsPostsLoading = false;

                        Posts.Add(rssItem);

                        if (LastPosts.Count < NumberOfLastPosts)
                        {
                            LastPosts.Add(rssItem);
                        }
                    }
                },
                delegate(Exception exception)
                {
                    IsPostsLoading = false;
                    System.Diagnostics.Debug.WriteLine(exception);
                });

            IsCommentsLoading = true;
            BlogService.GetBlogComments(
                delegate(IEnumerable<RssItem> rssItems)
                {
                    IsCommentsLoading = false;

                    Comments = new ObservableCollection<RssItem>();

                    foreach (RssItem rssItem in rssItems)
                    {
                        Comments.Add(rssItem);
                    }
                },
                delegate(Exception exception)
                {
                    IsCommentsLoading = false;
                    System.Diagnostics.Debug.WriteLine(exception);
                });

            // load images from somewhere
            Images = new ObservableCollection<ImageItem>();
            IEnumerable<ImageItem> images = ImageService.GetImages();
            foreach (ImageItem imageItem in images)
            {
                Images.Add(imageItem);
            }
        }

        #region Appbar handlers

        /// <summary>
        /// Handles the Click event of the AppbarButtonDigg control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonDigg_Click(object sender, EventArgs e)
        {
            this.GoToPage(ApplicationPages.Digg);
        }

        /// <summary>
        /// Handles the Click event of the AppbarButtonTwitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonTwitter_Click(object sender, EventArgs e)
        {
            this.GoToPage(ApplicationPages.Trends);
        }

        /// <summary>
        /// Handles the Click event of the AppbarButtonBlog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonBlog_Click(object sender, EventArgs e)
        {
            // Set "last 5 posts" as default item
            // this has the side effect of changing the selected item
            PanoramaControl.DefaultItem = PanoramaControl.Items[0];
        }

        #endregion
    }
}