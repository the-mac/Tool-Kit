// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using PhotoHubSample.Helpers;
using PhotoHubSample.Models;
using PhotoHubSample.Services;

namespace PhotoHubSample.ViewModels
{
    public class PhotosViewModel
    {
        public List<KeyedList<string, Photo>> GroupedPhotos
        {
            get
            {
                var photos = DataService.GetPhotos();

                var groupedPhotos =
                    from photo in photos
                    orderby photo.TimeStamp
                    group photo by photo.TimeStamp.ToString("y") into photosByMonth
                    select new KeyedList<string, Photo>(photosByMonth);

                return new List<KeyedList<string, Photo>>(groupedPhotos);
            }
        }
    }
}
