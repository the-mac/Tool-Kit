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
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;

using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace SurfaceBluetooth
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        BluetoothMonitor monitor;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for Application activation events
            AddActivationHandlers();

        }

        void monitor_DiscoveryCompleted(object sender, EventArgs e)
        {
            // Hides the discovery animation
            this.Dispatcher.Invoke(new UpdateVisibilityDelegate(UpdateVisibility), Visibility.Hidden);
        }

        void monitor_DiscoveryStarted(object sender, EventArgs e)
        {
            // Shows the discovery animation
            this.Dispatcher.Invoke(new UpdateVisibilityDelegate(UpdateVisibility), Visibility.Visible);
        }

        delegate void UpdateVisibilityDelegate(Visibility v);

        void UpdateVisibility(Visibility v)
        {
            Rings.Visibility = v;

            // Additionally display a text description of what the surface is currently doing
            if (v == Visibility.Visible)
            {
                DiscoveryStatusText.Text = "listening for Bluetooth devices...";
            }
            else
            {
                DiscoveryStatusText.Text = "waiting...";
            }
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for Application activation events
            RemoveActivationHandlers();
        }

        /// <summary>
        /// Adds handlers for Application activation events.
        /// </summary>
        private void AddActivationHandlers()
        {
            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;
        }

        /// <summary>
        /// Removes handlers for Application activation events.
        /// </summary>
        private void RemoveActivationHandlers()
        {
            // Unsubscribe from surface application activation events
            ApplicationLauncher.ApplicationActivated -= OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed -= OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated -= OnApplicationDeactivated;
        }

        /// <summary>
        /// Loads the sample content from separate SampleContent.XAML file
        /// </summary>
        private void LoadContent()
        {
            System.IO.FileStream contentStream = new System.IO.FileStream(System.IO.Path.Combine(Environment.CurrentDirectory, "SampleContent\\SampleContent.xaml"), System.IO.FileMode.Open);
            SampleContent content = (SampleContent)System.Windows.Markup.XamlReader.Load(contentStream);

            foreach (ObexContactItem oci in content.Contacts)
            {
                MobileConnectSample.ContentItems.ContactCard cc = new MobileConnectSample.ContentItems.ContactCard();
                cc.DataContext = oci;
                DragDropScatterView.SetAllowDrag(cc, true);
                Scatter.Items.Add(cc);
            }
            foreach (ObexImage oi in content.Photos)
            {
                MobileConnectSample.ContentItems.ImageCard ic = new MobileConnectSample.ContentItems.ImageCard();
                ic.DataContext = oi;
                DragDropScatterView.SetAllowDrag(ic, true);
                Scatter.Items.Add(ic);
            }
            foreach (ObexAudio oa in content.Audio)
            {
                MobileConnectSample.ContentItems.AudioCard ac = new MobileConnectSample.ContentItems.AudioCard();
                ac.DataContext = oa;
                DragDropScatterView.SetAllowDrag(ac, true);
                Scatter.Items.Add(ac);
            }
        }

        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here

            if (monitor == null)
            {
                LoadContent();

                try
                {
                    monitor = new BluetoothMonitor();
                }
                catch
                {
                    // Can't function without a bluetooth radio present so alert the user
                    Microsoft.Surface.UserNotifications.RequestNotification("Surface Bluetooth", "No Bluetooth Hardware Detected");
                    return;
                }

                // Data bind the list control to the current list of available devices
                DeviceList.ItemsSource = monitor.Devices;

                // Defines how long a single search pass will last
                monitor.DiscoveryDuration = new TimeSpan(0, 0, 5);
                // Defines how long to wait between searches
                monitor.IdleDuration = new TimeSpan(0, 0, 5);
                monitor.DiscoveryStarted += new EventHandler(monitor_DiscoveryStarted);
                monitor.DiscoveryCompleted += new EventHandler(monitor_DiscoveryCompleted);
                // Show the Surface's Bluetooth radio name on screen
                RadioNameText.Text = monitor.RadioFriendlyName;

                AddHandler(ScatterViewItem.ScatterManipulationStartedEvent, new ScatterManipulationStartedEventHandler(OnManipulationStarted));
                AddHandler(ScatterViewItem.ScatterManipulationDeltaEvent, new ScatterManipulationDeltaEventHandler(OnManipulationDelta));
                AddHandler(ScatterViewItem.ScatterManipulationCompletedEvent, new ScatterManipulationCompletedEventHandler(OnManipulationCompleted));

                SurfaceDragDrop.AddDropHandler(DeviceList, OnCursorDrop);
            }

            // Starts listening loop for detecting nearby devices
            monitor.StartDiscovery();
        }

        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
            if (monitor != null)
            {
                // Stop discovering and raising events
                monitor.StopDiscovery();
            }
        }

        private ScatterViewItem GetSourceItem(FrameworkElement element)
        {
            FrameworkElement thisElement = element;

            while (thisElement != null)
            {
                if (thisElement is ScatterViewItem)
                {
                    return thisElement as ScatterViewItem;
                }
                else
                {
                    if (thisElement.Parent != null)
                    {
                        thisElement = thisElement.Parent as FrameworkElement;
                    }
                    else
                    {
                        thisElement = null;
                    }
                }
            }
            return null;
        }

        private void OnManipulationStarted(object sender, ScatterManipulationStartedEventArgs args)
        {
            ScatterViewItem svi = args.OriginalSource as ScatterViewItem;
            if (svi != null && DragDropScatterView.GetAllowDrag(svi))
            {
                svi.Style = FindResource("CursorStyle") as Style;
                svi.Tag = "Dragging";
                
                ObexItem data = svi.DataContext as ObexItem;

                data.DraggedElement = svi;
                data.OriginalCenter = svi.Center;
                data.OriginalOrientation = svi.Orientation;

                bool dragDrop = svi.BeginDragDrop(svi.DataContext);

                if (dragDrop)
                {
                    args.Handled = true;
                }
            }
        }

        private void OnManipulationDelta(object sender, ScatterManipulationDeltaEventArgs args)
        {
        }

        private void OnManipulationCompleted(object sender, ScatterManipulationCompletedEventArgs args)
        {
            ScatterViewItem svi = args.OriginalSource as ScatterViewItem;
            svi.Tag = null;
        }

        private void OnCursorDrop(object sender, SurfaceDragDropEventArgs args)
        {
        }

        // Runs in a threadpool thread and performs the actual obex exchange
        private void BeamObject(object context)
        {
            ObexWebRequest owr = context as ObexWebRequest;

            try
            {
                InTheHand.Net.ObexWebResponse response = (InTheHand.Net.ObexWebResponse)owr.GetResponse();
                
                // Remove once-off pairing
                BluetoothSecurity.RemoveDevice(BluetoothAddress.Parse(owr.RequestUri.Host));
            }
            catch (System.Net.WebException we)
            {
                System.Diagnostics.Debug.WriteLine(we.ToString());
            }
            finally
            {
                // Restart discovery for new devices
                monitor.StartDiscovery();
            }
        }

        private void OnDropTargetDragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement)
            {
                FrameworkElement fe = e.OriginalSource as FrameworkElement;

                if (fe.DataContext  is BluetoothDevice)
                {
                    // Drop target must be a device not the list control
                    e.Cursor.Visual.Tag = "DragEnter";
                }
            }
        }

        private void OnDropTargetDragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "Dragging";
        }

        private void OnDragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            ObexItem data = e.Cursor.Data as ObexItem;
            ScatterViewItem item = data.DraggedElement as ScatterViewItem;
            e.Cursor.Visual.Tag = null;
            if (item != null)
            {
                item.Visibility = Visibility.Visible;
                item.Orientation = e.Cursor.GetOrientation(this);
                item.Center = e.Cursor.GetPosition(this);
            }
        }

        private void OnDrop(object sender, SurfaceDragDropEventArgs e)
        {
            FrameworkElement element = e.OriginalSource as FrameworkElement;
            if (element != null)
            {
                if (element.DataContext is BluetoothDevice)
                {
                    // Target is a Bluetooth device
                    BluetoothDevice device = element.DataContext as BluetoothDevice;

                    // Is dragging item obex?
                    if (e.Cursor.Data is ObexItem)
                    {
                        ObexItem oi = e.Cursor.Data as ObexItem;

                        // Pause discovery as it interferes with/slows down beam process
                        monitor.StopDiscovery();

                        // Create the new request and write the contact details
                        ObexWebRequest owr = new ObexWebRequest(new Uri("obex://" + device.DeviceAddress.ToString() + "/" + oi.FileName));
                        System.IO.Stream s = owr.GetRequestStream();
                        oi.WriteToStream(s);

                        owr.ContentType = oi.ContentType;
                        owr.ContentLength = s.Length;
                        s.Close();


                        // Beam the item on new thread
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(BeamObject), owr);

                        // Return item to the scatter view
                        ScatterViewItem svi = oi.DraggedElement as ScatterViewItem;
                        Scatter.Items.Add(svi);
                        svi.Style = null;
                        svi.Center = oi.OriginalCenter;
                        svi.Orientation = oi.OriginalOrientation;
                    }
                    
                }

                // Otherwise not supported
            }
        }

        //called if drag cancelled - e.g. item is placed back on the scatter control
        // move the item to the cursor position and make visible again
        private void Scatter_PreviewDragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Scatter_PreviewDragCanceled");

            ObexItem data = e.Cursor.Data as ObexItem;
            ScatterViewItem item = data.DraggedElement as ScatterViewItem;
            
            if (item != null)
            {
                item.Visibility = Visibility.Visible;
                item.Orientation = e.Cursor.GetOrientation(this);
                item.Center = e.Cursor.GetPosition(this);
            }
        }

        //drag completed
        //return visibility to original item
        private void Scatter_PreviewDragCompleted(object sender, SurfaceDragCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Scatter_PreviewDragCompleted");


            ObexItem data = e.Cursor.Data as ObexItem;
            ScatterViewItem item = data.DraggedElement as ScatterViewItem;
            if (item != null)
            {
                item.Visibility = Visibility.Visible;
            }
        }
    }
}