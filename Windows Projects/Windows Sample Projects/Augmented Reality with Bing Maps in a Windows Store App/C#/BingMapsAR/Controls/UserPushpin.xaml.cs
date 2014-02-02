
#if WIN_RT
using Windows.UI.Xaml.Controls;
#elif WP8
using System.Windows.Controls;
#endif

namespace BingMapsAR.Controls
{
    public sealed partial class UserPushpin : UserControl
    {
        public UserPushpin()
        {
            this.InitializeComponent();
        }
    }
}
