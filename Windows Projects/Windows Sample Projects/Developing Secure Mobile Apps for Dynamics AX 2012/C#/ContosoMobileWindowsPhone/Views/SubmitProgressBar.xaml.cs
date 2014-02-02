using System.Windows;
using System.Windows.Controls;

namespace ContosoMobile
{
    public partial class SubmitProgressBar : UserControl
    {
        public SubmitProgressBar()
        {            
            InitializeComponent();
            PopupPanel.Height = ((FrameworkElement)App.Current.RootVisual).ActualHeight;
            PopupPanel.Width = ((FrameworkElement)App.Current.RootVisual).ActualWidth;
        }
    }
}