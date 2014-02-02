using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using PhotoFun.Effects;

namespace PhotoFun
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Members

        private PhotoCamera camera; //The device's camera
        private static ManualResetEvent pauseFramesEvent = new ManualResetEvent(true); //Used to avoid collisions between one image processing and another
        private WriteableBitmap previewWriteableBitmap; //Preview image source
        private Thread previewImageProcessingThread; //Preview image processing thread
        private bool pumpEffectedFrames; //While true, the image processing preview will continue
        private bool capturingImage;

        private PhotofunDataContext PhotofunDataContext
        {
            get { return this.DataContext as PhotofunDataContext; }
        }

        #endregion

        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fires when this pages was navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // TODO: Initialize the camera

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // TODO: Free the camera

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Fires when the camera initialized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void camera_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            if(e.Succeeded)
            {
                var res = from resolution in camera.AvailableResolutions
                          where resolution.Width == 640 
                          select resolution;

                camera.Resolution = res.First();
                this.Dispatcher.BeginInvoke(delegate()
                {
                    EffectSelected();
                });
            }
        }

        /// <summary>
        /// Fires when the camera button was half pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void camera_ButtonHalfPress(object sender, EventArgs e)
        {
            camera.Focus();
        }

        /// <summary>
        /// Fires when the camera button was fully pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void camera_ButtonFullPress(object sender, EventArgs e)
        {
            if(capturingImage)
                return;

            //pauseFramesEvent.WaitOne();

            //Capture image for saving
            pauseFramesEvent.Reset();
            capturingImage = true;

            camera.CaptureImage();
        }

        /// <summary>
        /// Saves the picture taken by the camera after processing it 
        /// with the selected effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void camera_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate()
            {
                SavePicture(e.ImageStream);
            });
        }

        private void camera_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            pauseFramesEvent.Set();
            capturingImage = false;
        }

        /// <summary>
        /// This method is called whenever an effect is selected
        /// </summary>
        private void EffectSelected()
        {
            bool pumpSelectedFramesStarted = pumpEffectedFrames;
            var selectedEffect = PhotofunDataContext.SelectedEffect;

            pumpEffectedFrames = selectedEffect != null && selectedEffect.Effect != null && !(selectedEffect.Effect is EchoEffect);

            if(!pumpEffectedFrames)
                ShowRegularVideo();
            else
                ShowProcessedPreview();

            FrameImage.Visibility = PhotofunDataContext.SelectedFrame.FrameUri != null ? Visibility.Visible : Visibility.Collapsed;

            //Start the image processing if it hasn't started yet
            if(pumpEffectedFrames && !pumpSelectedFramesStarted)
            {
                StartImageProcessing();
            }
        }

        private void StartImageProcessing()
        {
            //Set the source of MainImage t the preview writeable bitmap
            previewWriteableBitmap = new WriteableBitmap((int)camera.PreviewResolution.Width, (int)camera.PreviewResolution.Height);
            this.MainImage.Source = previewWriteableBitmap;

            //Process the images on another thread
            previewImageProcessingThread = new Thread(PumpEffectFrames);
            previewImageProcessingThread.Start();
        }

        /// <summary>
        /// Sets the layout to show the processed preview
        /// </summary>
        private void ShowProcessedPreview()
        {
            //Set the source of MainImage to the PreviewdWriteable bitmap
            MainImage.Source = previewWriteableBitmap;

            //Show MainImage - Hide videoRectangle
            MainImage.Width = this.ActualWidth;
            MainImage.Height = this.ActualHeight;
            videoRectangle.Width = 1;
            videoRectangle.Height = 1;
        }

        private void ShowRegularVideo()
        {
            //Hide MainImage - Show the videoRectangle
            MainImage.Width = 1;
            MainImage.Height = 1;
            videoRectangle.Width = this.ActualWidth;
            videoRectangle.Height = this.ActualHeight;
        }

        private void SavePicture(Stream imageStream)
        {
            WriteableBitmap bitmap = CreateWriteableBitmap(imageStream);

            WriteableBitmap processedBitmap = ProcessCapturedImage(bitmap);

            PhotofunDataContext.Previews.Add(processedBitmap);

            SaveCapturedImage(processedBitmap);
        }

        private WriteableBitmap CreateWriteableBitmap(Stream imageStream)
        {
            WriteableBitmap bitmap = new WriteableBitmap((int)camera.Resolution.Width, (int)camera.Resolution.Height);

            imageStream.Position = 0;
            //Load the captured image stream to the bitmap
            bitmap.LoadJpeg(imageStream);
            return bitmap;
        }

        private WriteableBitmap ProcessCapturedImage(WriteableBitmap bitmap)
        {
            IEffect processingEffect = PhotofunDataContext.SelectedEffect.Effect;
            int[] processedImage = processingEffect.ProcessImage(bitmap.Pixels);

            ChromeEffect selectedFrameEffect = new ChromeEffect(PhotofunDataContext.SelectedFrame.FrameUri);
            int[] pixels = selectedFrameEffect.ProcessImage(processedImage);

            WriteableBitmap imageToSave = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            //Copy the processed pixels to the bitmap used to save the image
            pixels.CopyTo(imageToSave.Pixels, 0);

            return imageToSave;
        }

        private void SaveCapturedImage(WriteableBitmap imageToSave)
        {
            MemoryStream stream = new MemoryStream();
            imageToSave.SaveJpeg(stream, imageToSave.PixelWidth, imageToSave.PixelHeight, 0, 100);

            //Take the stream back to its beginning because it will be read again 
            //when saving to the library
            stream.Position = 0;

            //Save picture to device media library
            MediaLibrary library = new MediaLibrary();
            string fileName = string.Format("{0:yyyy-MM-dd-HH-mm-ss}.jpg", DateTime.Now);
            library.SavePicture(fileName, stream);
        }

        /// <summary>
        /// Fires when the user clicks one of the effect's radio buttons
        /// </summary>
        /// <param name="sender">Button clicked</param>
        /// <param name="e"></param>
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rbSender = sender as RadioButton;

            string effectName = rbSender.Content as string;

            PhotofunDataContext.SelectedEffect = PhotofunDataContext.Effects.Where(item => item.Name == effectName).FirstOrDefault();

            EffectSelected();
        }

        /// <summary>
        /// Continuously changes the preview bitmap source 
        /// to have the preview image as the regular preview video 
        /// but with image processing
        /// </summary>
        private void PumpEffectFrames()
        {
            try
            {
                //Points the camera from another pointer 
                while(pumpEffectedFrames)
                {
                    if(capturingImage)
                        continue;

                    pauseFramesEvent.WaitOne();

                    pauseFramesEvent.Reset();

                    Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        PushProcessedFrame();
                        pauseFramesEvent.Set();
                    }); 
                }
            }
            catch
            {                
            }
        }

        /// <summary>
        /// Manages processed frames from the preview
        /// </summary>
        /// <param name="phCam"></param>
        private void PushProcessedFrame()
        {
            if(pumpEffectedFrames && !capturingImage)
            {
                int[] previewBuffer = GetPreviewBuffer();

                //Process the preview image
                int[] imageData = PhotofunDataContext.SelectedEffect.Effect.ProcessImage(previewBuffer);

                //Copy to WriteableBitmap
                imageData.CopyTo(previewWriteableBitmap.Pixels, 0);

                previewWriteableBitmap.Invalidate();
            }
        }

        /// <summary>
        /// Takes the preview buffer pixel array from camera
        /// </summary>
        /// <returns>An array of pixels representing the camera's preview buffer</returns>
        private int[] GetPreviewBuffer()
        {
            int[] pixelData = new int[(int)(camera.PreviewResolution.Width * camera.PreviewResolution.Height)];
            camera.GetPreviewBufferArgb32(pixelData);
            return pixelData;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Select the first effect
            PhotofunDataContext.SelectedEffect = PhotofunDataContext.Effects.FirstOrDefault(effect => effect.Effect is EchoEffect);
        }

        private void frameRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rbSender = sender as RadioButton;

            string frameName = rbSender.Content as string;
            PhotofunDataContext.SelectedFrame = PhotofunDataContext.Frames.Where(frame => frame.Name == frameName).FirstOrDefault();
            EffectSelected();
        }

        /// <summary>
        /// Shows preview image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewImageClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            pumpEffectedFrames = false;
            Image contentImage = sender as Image;
            PhotofunDataContext.SelectedPreview = PhotofunDataContext.Previews.Where(image => image == (contentImage.Source as WriteableBitmap)).FirstOrDefault();
            NavigationService.Navigate(new Uri("/PreviewPage.xaml", UriKind.Relative));
        }
    }
}
