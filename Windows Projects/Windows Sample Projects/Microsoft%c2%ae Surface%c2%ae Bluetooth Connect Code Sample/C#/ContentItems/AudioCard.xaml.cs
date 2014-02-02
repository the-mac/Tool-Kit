using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface.Presentation.Controls;

namespace MobileConnectSample.ContentItems
{
    /// <summary>
    /// Interaction logic for AudioCard.xaml
    /// </summary>
    public partial class AudioCard : ScatterViewItem
    {
        public AudioCard()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                SurfaceBluetooth.ObexAudio oa = this.DataContext as SurfaceBluetooth.ObexAudio;
                if (oa.IsPlaying)
                {
                    oa.Pause();
                }
                else
                {
                    oa.Play();
                }
            }
        }
    }
}
