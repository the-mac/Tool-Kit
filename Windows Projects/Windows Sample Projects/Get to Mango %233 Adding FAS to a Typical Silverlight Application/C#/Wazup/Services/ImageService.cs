using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Wazup.Services
{
    /// <summary>
    /// Provides facade for getting images
    /// </summary>
    public static class ImageService
    {
        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <returns>Images list</returns>
        public static IEnumerable<ImageItem> GetImages()
        {
            yield return new ImageItem(@"/BlogImages/Phone1.png");
            yield return new ImageItem(@"/BlogImages/Phone2.png");
            yield return new ImageItem(@"/BlogImages/Phone3.png");
            yield return new ImageItem(@"/BlogImages/Phone4.png");
            yield return new ImageItem(@"/BlogImages/Phone5.png");
            yield return new ImageItem(@"/BlogImages/Phone6.png");
        }

    }
}
