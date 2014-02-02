namespace System.Windows.Controls
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Collections.Generic;
    using System.Linq;

    public class LinkLabel : Control
    {
        public const string DefaultUriPattern = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
        public const string DefaultLinkPattern = "\\[link=\"(?<link>(.|\n)*?)\"\\s*(target=\"(?<target>(.|\n)*?)\")?](?<text>(.|\n)*?)\\[\\/link\\]";
        public const string DefaultLinkPatternUriGroupName = "link";
        public const string DefaultLinkPatternTextGroupName = "text";
        public const string DefaultLinkPatternTargetGroupName = "target";

        // input new line entered in XAML
        public const string XamlNewLine = "\n";

        // input new line entered runtime
        public const string RuntimeNewLine = "\r";
        public const string Space = " ";
        public static readonly DependencyProperty LinkStyleProperty = DependencyProperty.Register( "LinkStyle", typeof( Style ), typeof( LinkLabel ), null );
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register( "TextStyle", typeof( Style ), typeof( LinkLabel ), null );
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register( "Text", typeof( string ), typeof( LinkLabel ), new PropertyMetadata( new PropertyChangedCallback( OnTextChanged ) ) );
        private WrapPanel layoutRoot;
        private LinkMatchMethod linkMatchMethod = LinkMatchMethod.ByUriAndLinkPattern;
        private string linkPattern;
        private string linkPatternLinkGroupName;
        private string linkPatternTextGroupName;
        private string linkPatternTargetGroupName;
        private string uriPattern;

        public LinkLabel()
            : base()
        {
            DefaultStyleKey = typeof( LinkLabel );
        }

        public event EventHandler<LinkClickEventArgs> LinkClick;

        /// <summary>
        /// Gets or sets the method of how the links are matched
        /// </summary>
        public LinkMatchMethod LinkMatchMethod
        {
            get
            {
                return this.linkMatchMethod;
            }

            set
            {
                this.linkMatchMethod = value;
            }
        }

        /// <summary>
        /// Get or sets the width of the element used to insert a line break.
        /// </summary>
        /// <remarks>The width should be larger or eqal to the LinkLabel width property</remarks>
        public double LineBreakElementWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the style of the added Hyperlink controls
        /// </summary>
        public Style LinkStyle
        {
            get
            {
                return ( Style )this.GetValue( LinkStyleProperty );
            }

            set
            {
                base.SetValue( LinkStyleProperty, ( DependencyObject )value );
            }
        }

        /// <summary>
        /// Gets or sets the style of the added TextBlock controls
        /// </summary>
        public Style TextStyle
        {
            get
            {
                return ( Style )this.GetValue( TextStyleProperty );
            }

            set
            {
                base.SetValue( TextStyleProperty, ( DependencyObject )value );
            }
        }

        /// <summary>
        /// Gets or sets the text of the LinkLabel control
        /// </summary>
        public string Text
        {
            get
            {
                return ( string )this.GetValue( TextProperty );
            }

            set
            {
                base.SetValue( TextProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the regular expression pattern used to match URIs
        /// </summary>
        public string UriPattern
        {
            get
            {
                if ( string.IsNullOrEmpty( this.uriPattern ) )
                {
                    return DefaultUriPattern;
                }

                return this.uriPattern;
            }

            set
            {
                this.uriPattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the regular expression pattern used to match links
        /// </summary>
        public string LinkPattern
        {
            get
            {
                if ( string.IsNullOrEmpty( this.linkPattern ) )
                {
                    return DefaultLinkPattern;
                }

                return this.linkPattern;
            }

            set
            {
                this.linkPattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the GroupName of the link in the RegularExpression.Match
        /// </summary>
        public string LinkPatternUriGroupName
        {
            get
            {
                if ( this.LinkPattern.Equals( DefaultLinkPattern, StringComparison.OrdinalIgnoreCase ) )
                {
                    return DefaultLinkPatternUriGroupName;
                }

                return this.linkPatternLinkGroupName;
            }

            set
            {
                this.linkPatternLinkGroupName = value;
            }
        }

        /// <summary>
        /// Gets or sets the GroupName of the text in the RegularExpression.Match
        /// </summary>
        public string LinkPatternTextGroupName
        {
            get
            {
                if ( this.LinkPattern.Equals( DefaultLinkPattern, StringComparison.OrdinalIgnoreCase ) )
                {
                    return DefaultLinkPatternTextGroupName;
                }

                return this.linkPatternTextGroupName;
            }

            set
            {
                this.linkPatternTextGroupName = value;
            }
        }

        /// <summary>
        /// Gets or sets the GroupName of the target in the RegularExpression.Match
        /// </summary>
        public string LinkPatternTargetGroupName
        {
            get
            {
                if ( this.LinkPattern.Equals( DefaultLinkPattern, StringComparison.OrdinalIgnoreCase ) )
                {
                    return DefaultLinkPatternTargetGroupName;
                }

                return this.linkPatternTargetGroupName;
            }

            set
            {
                this.linkPatternTargetGroupName = value;
            }
        }

        public override void OnApplyTemplate()
        {
            this.layoutRoot = this.GetTemplateChild( "LayoutRoot" ) as WrapPanel;
            Debug.Assert( this.layoutRoot != null, "LayoutRoot is null" );
            this.ProcessText();

            base.OnApplyTemplate();
        }

        private static void OnTextChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            LinkLabel linkLabel = d as LinkLabel;
            Debug.Assert( linkLabel != null, "LaytouRoot is null" );

            linkLabel.ProcessText();
        }

        private void ClickLink( object sender, RoutedEventArgs e )
        {
            if ( this.LinkClick != null )
            {
                HyperlinkButton button = ( HyperlinkButton )sender;
                LinkClickEventArgs args = new LinkClickEventArgs()
                {
                    NavigateUri = button.NavigateUri,
                    Action = button.Tag as string
                };

                this.LinkClick( this, args );
            }
        }

        private void AddLinks( LinkCollection links )
        {
            string linkLabelText = this.Text;
            string preUri = string.Empty;
            List<string> preUriWords = null;
            string postUri = string.Empty;
            string[] postUriWords = null;
            int startIndexOfUri = 0;
            char[] delimiter = { ' ' };
            bool insertSpaceBeforeTheLink = false;
            bool insertSpaceAfterTheLink = false;

            // no uris found
            if (links == null || links.Count == 0)
            {
                string[] allwords = linkLabelText.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in allwords)
                {
                    this.layoutRoot.Children.Add(new TextBlock()
                    {
                        Text = word + " ",
                        Style = this.TextStyle
                    });
                }
            }

            foreach ( Link link in links )
            {
                startIndexOfUri = linkLabelText.IndexOf( link.Key, StringComparison.OrdinalIgnoreCase );
                preUri = linkLabelText.Substring( 0, startIndexOfUri );
                postUri = linkLabelText.Substring( preUri.Length + link.Key.Length );
                linkLabelText = postUri;

                insertSpaceAfterTheLink = preUri.StartsWith( Space, StringComparison.OrdinalIgnoreCase );
                insertSpaceBeforeTheLink = preUri.EndsWith( Space, StringComparison.OrdinalIgnoreCase );

                // put all the words before the current Uri
                preUriWords = new List<string>( preUri.Split( delimiter, StringSplitOptions.RemoveEmptyEntries ).AsEnumerable() );

                for ( int i = 0; i < preUriWords.Count; i++ )
                {
                    if ( WordContainsNewLineInBody( preUriWords[ i ] ) )
                    {
                        List<string> newWords = ReplaceNewLines( preUriWords[ i ] );

                        preUriWords.RemoveAt( i );

                        if ( preUriWords.Count == 0 )
                        {
                            preUriWords.AddRange( newWords );
                        }
                        else
                        {
                            preUriWords.InsertRange( i, newWords );
                        }
                    }
                }

                for ( int i = 0; i < preUriWords.Count; i++ )
                {
                    this.AddWord( preUriWords[ i ], this.layoutRoot, insertSpaceAfterTheLink, ( i == ( preUriWords.Count - 1 ) && !insertSpaceBeforeTheLink ) );
                    insertSpaceAfterTheLink = false;
                }

                // insert the Uri
                HyperlinkButton hyperlink = new HyperlinkButton()
                {
                    //Content = new TextBlock()
                    //{
                    //    Text = link.Text,
                    //    TextWrapping = TextWrapping.Wrap,
                    //},
                    Content = link.Text,
                    NavigateUri = link.NavigateUri,
                    TargetName = link.TargetName,
                    Style = this.LinkStyle,
                    Tag = link.Action,
                };
                hyperlink.Click += new RoutedEventHandler( this.ClickLink );
                this.layoutRoot.Children.Add( hyperlink );
            }

            // append the text after the last uri found
            if ( !string.IsNullOrEmpty( linkLabelText ) )
            {
                insertSpaceAfterTheLink = postUri.StartsWith( Space, StringComparison.OrdinalIgnoreCase );
                postUriWords = postUri.Split( delimiter, StringSplitOptions.RemoveEmptyEntries );
                foreach ( string postWord in postUriWords )
                {
                    this.AddWord( postWord + Space, this.layoutRoot, insertSpaceAfterTheLink, false );
                    insertSpaceAfterTheLink = false;
                }
            }
        }

        #region Word Processing
        private void AddWord( string w, Panel root, bool insertSpaceAfterTheLink, bool insertSpaceBeforeTheLink )
        {
            if ( WordStartsWithNewLine( w ) )
            {
                this.AddNewLine( root );
            }

            bool endsWithNewLine = WordEndsWithNewLine( w );
            ClearNewLines( ref w );

            root.Children.Add( new TextBlock()
            {
                Text = ( insertSpaceAfterTheLink ? Space : string.Empty ) + w + ( insertSpaceBeforeTheLink ? string.Empty : Space ),
                Style = this.TextStyle
            } );

            if ( endsWithNewLine )
            {
                this.AddNewLine( root );
            }
        }

        private void AddNewLine( Panel root )
        {
            root.Children.Add( new TextBlock()
            {
                Text = string.Empty,
                Width = this.LineBreakElementWidth
            } );
        }

        private static void ClearNewLines( ref string w )
        {
            w = w.Replace( Environment.NewLine, string.Empty );
            w = w.Replace( XamlNewLine, string.Empty );
            w = w.Replace( RuntimeNewLine, string.Empty );
        }

        private static List<string> ReplaceNewLines( string w )
        {
            w = w.Replace( Environment.NewLine, Space + Environment.NewLine );
            w = w.Replace( XamlNewLine, Space + XamlNewLine );
            w = w.Replace( RuntimeNewLine, Space + RuntimeNewLine );

            return w.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
        }

        private static bool WordStartsWithNewLine( string w )
        {
            return w.StartsWith( Environment.NewLine, StringComparison.OrdinalIgnoreCase ) || w.StartsWith( XamlNewLine, StringComparison.OrdinalIgnoreCase ) || w.StartsWith( RuntimeNewLine, StringComparison.OrdinalIgnoreCase );
        }

        private static bool WordEndsWithNewLine( string w )
        {
            return w.EndsWith( Environment.NewLine, StringComparison.OrdinalIgnoreCase ) || w.EndsWith( XamlNewLine, StringComparison.OrdinalIgnoreCase ) || w.EndsWith( RuntimeNewLine, StringComparison.OrdinalIgnoreCase );
        }

        private static bool WordContainsNewLineInBody( string w )
        {
            return ( w.Contains( Environment.NewLine ) || w.Contains( XamlNewLine ) || w.Contains( RuntimeNewLine ) ) && !WordStartsWithNewLine( w ) && !WordEndsWithNewLine( w );
        }
        #endregion

        private LinkCollection GetLinkMatches( ref string text )
        {
            LinkCollection uriCollection = null;
            Uri currentUri = null;
            string action = string.Empty;

            Regex uriLocator = new Regex( this.LinkPattern );
            MatchCollection uriMatches = uriLocator.Matches( text );

            // no uris found
            if ( uriMatches == null || uriMatches.Count == 0 )
            {
                return null;
            }

            foreach ( Match uri in uriMatches )
            {
                // Not a valid URI - consider an action
                if ( !Uri.TryCreate(
                    uri.Groups[ LinkPatternUriGroupName ].Value,
                    UriKind.RelativeOrAbsolute,
                    out currentUri ) )
                {
                    currentUri = null;
                    action = uri.Groups[ LinkPatternUriGroupName ].Value;
                }
                else
                {
                    // Absolute URI, relative URI or action
                    action = string.Empty;

                    // relative URI or action
                    if ( !currentUri.IsAbsoluteUri )
                    {
                        // Relative URI or action
                        string originalString = currentUri.OriginalString;
                        if ( !string.IsNullOrEmpty( originalString ) && ( originalString[ 0 ] != '/' ) )
                        {
                            // No preceeding '/' character - consider an action
                            currentUri = null;
                            action = originalString;
                        }
                        else
                        {
                            //// Preceeding '/' character - consider a relative URI
                            //currentUri = new Uri( Application.Current.Host.Source, currentUri );
                        }
                    }
                }

                if ( uriCollection == null )
                {
                    uriCollection = new LinkCollection();
                }

                uriCollection.Add( new Link( uri.Value, currentUri, action )
                {
                    Text = uri.Groups[ LinkPatternTextGroupName ].Value,
                    TargetName = ( uri.Groups[ LinkPatternTargetGroupName ].Value.Length == 0 ? "_self" : uri.Groups[ LinkPatternTargetGroupName ].Value )
                } );

                text = text.Replace( uri.Value, string.Empty );
            }

            return uriCollection;
        }

        private LinkCollection GetUriMatches( string text )
        {
            LinkCollection uriCollection = null;
            Uri currentUri = null;

            Regex uriLocator = new Regex( this.UriPattern );
            MatchCollection uriMatches = uriLocator.Matches( text );

            // no uris found
            if ( uriMatches == null || uriMatches.Count == 0 )
            {
                return null;
            }

            foreach ( Match uri in uriMatches )
            {
                // not valid uri - continue with next match
                if ( !Uri.TryCreate( uri.Value, UriKind.RelativeOrAbsolute, out currentUri ) )
                {
                    continue;
                }

                if ( uriCollection == null )
                {
                    uriCollection = new LinkCollection();
                }

                uriCollection.Add( new Link( uri.Value.Trim(), currentUri ) );
            }

            return uriCollection;
        }

        private void OrderLinksByAppearenceInTheText( LinkCollection links )
        {
            int i, j;
            Link l;

            for ( i = links.Count - 1; i > 0; i-- )
            {
                for ( j = 0; j < i; j++ )
                {
                    if ( this.Text.IndexOf( links[ j ].Key, StringComparison.Ordinal ) > this.Text.IndexOf( links[ j + 1 ].Key, StringComparison.Ordinal ) )
                    {
                        l = links[ j ];
                        links[ j ] = links[ j + 1 ];
                        links[ j + 1 ] = l;
                    }
                }
            }
        }

        private void ProcessText()
        {
            if ( this.layoutRoot == null )
            {
                return;
            }

            // clear current text
            this.layoutRoot.Children.Clear();

            if ( string.IsNullOrEmpty( this.Text ) )
            {
                return;
            }

            string tempText = this.Text;
            if ( this.LinkMatchMethod == LinkMatchMethod.ByLinkPattern )
            {
                this.AddLinks( this.GetLinkMatches( ref tempText ) );
            }
            else if ( this.LinkMatchMethod == LinkMatchMethod.ByUriPattern )
            {
                this.AddLinks( this.GetUriMatches( tempText ) );
            }
            else if ( this.LinkMatchMethod == LinkMatchMethod.ByUriAndLinkPattern )
            {
                LinkCollection allLinks = new LinkCollection();

                LinkCollection linkMatchesCollection = this.GetLinkMatches( ref tempText );

                if ( linkMatchesCollection != null )
                {
                    foreach ( Link link in linkMatchesCollection )
                    {
                        allLinks.Add( link );
                    }
                }

                LinkCollection uriMatchesCollection = this.GetUriMatches( tempText );

                if ( uriMatchesCollection != null )
                {
                    foreach ( Link link in uriMatchesCollection )
                    {
                        allLinks.Add( link );
                    }
                }

                this.OrderLinksByAppearenceInTheText( allLinks );
                this.AddLinks( allLinks );
            }
        }
    }
}