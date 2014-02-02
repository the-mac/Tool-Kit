using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Windows.Threading;

namespace sdkRawSensorDataCS
{
    public partial class CompassPage : PhoneApplicationPage
    {
        Compass compass;
        DispatcherTimer timer;

        double magneticHeading;
        double trueHeading;
        double headingAccuracy;
        Vector3 rawMagnetometerReading;
        bool isDataValid;

        bool calibrating = false;

        Accelerometer accelerometer;

        // Constructor
        public CompassPage()
        {
            InitializeComponent();

            Application.Current.Host.Settings.EnableFrameRateCounter = false;


            if (!Compass.IsSupported)
            {
                // The device on which the application is running does not support
                // the compass sensor. Alert the user and hide the
                // application bar.
                statusTextBlock.Text = "device does not support compass";
                ApplicationBar.IsVisible = false;
            }
            else
            {
                // Initialize the timer and add Tick event handler, but don't start it yet.
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(30);
                timer.Tick += new EventHandler(timer_Tick);
            }
        }



        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (compass != null && compass.IsDataValid)
            {
                // Stop data acquisition from the compass.
                compass.Stop();
                timer.Stop();
                statusTextBlock.Text = "compass stopped.";

                // Detect compass axis
                accelerometer.Stop();
            }
            else
            {
                if (compass == null)
                {
                    // Instantiate the compass.
                    compass = new Compass();


                    // Specify the desired time between updates. The sensor accepts
                    // intervals in multiples of 20 ms.
                    compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);

                    // The sensor may not support the requested time between updates.
                    // The TimeBetweenUpdates property reflects the actual rate.
                    timeBetweenUpdatesTextBlock.Text = compass.TimeBetweenUpdates.TotalMilliseconds + " ms";


                    compass.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<CompassReading>>(compass_CurrentValueChanged);
                    compass.Calibrate += new EventHandler<CalibrationEventArgs>(compass_Calibrate);
                }

                try
                {
                    statusTextBlock.Text = "starting compass.";
                    compass.Start();
                    timer.Start();

                    // Start accelerometer for detecting compass axis
                    accelerometer = new Accelerometer();
                    accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
                    accelerometer.Start();
                }
                catch (InvalidOperationException)
                {
                    statusTextBlock.Text = "unable to start compass.";
                }

            }

        }

        void compass_CurrentValueChanged(object sender, SensorReadingEventArgs<CompassReading> e)
        {
            // Note that this event handler is called from a background thread
            // and therefore does not have access to the UI thread. To update 
            // the UI from this handler, use Dispatcher.BeginInvoke() as shown.
            // Dispatcher.BeginInvoke(() => { statusTextBlock.Text = "in CurrentValueChanged"; });


            isDataValid = compass.IsDataValid;

            trueHeading = e.SensorReading.TrueHeading;
            magneticHeading = e.SensorReading.MagneticHeading;
            headingAccuracy = Math.Abs(e.SensorReading.HeadingAccuracy);
            rawMagnetometerReading = e.SensorReading.MagnetometerReading;

        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!calibrating)
            {
                if (isDataValid)
                {
                    statusTextBlock.Text = "receiving data from compass.";
                }

                // Update the textblocks with numeric heading values
                magneticTextBlock.Text = magneticHeading.ToString("0.0");
                trueTextBlock.Text = trueHeading.ToString("0.0");
                accuracyTextBlock.Text = headingAccuracy.ToString("0.0");

                // Update the line objects to graphically display the headings
                double centerX = headingGrid.ActualWidth / 2.0;
                double centerY = headingGrid.ActualHeight / 2.0;
                magneticLine.X2 = centerX - centerY * Math.Sin(MathHelper.ToRadians((float)magneticHeading));
                magneticLine.Y2 = centerY - centerY * Math.Cos(MathHelper.ToRadians((float)magneticHeading));
                trueLine.X2 = centerX - centerY * Math.Sin(MathHelper.ToRadians((float)trueHeading));
                trueLine.Y2 = centerY - centerY * Math.Cos(MathHelper.ToRadians((float)trueHeading));

                // Update the textblocks with numeric raw magnetometer readings
                xTextBlock.Text = rawMagnetometerReading.X.ToString("0.00");
                yTextBlock.Text = rawMagnetometerReading.Y.ToString("0.00");
                zTextBlock.Text = rawMagnetometerReading.Z.ToString("0.00");

                // Update the line objects to graphically display raw data
                xLine.X2 = xLine.X1 + rawMagnetometerReading.X * 4;
                yLine.X2 = yLine.X1 + rawMagnetometerReading.Y * 4;
                zLine.X2 = zLine.X1 + rawMagnetometerReading.Z * 4;
            }
            else
            {
                if (headingAccuracy <= 10)
                {
                    calibrationTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    calibrationTextBlock.Text = "Complete!";
                }
                else
                {
                    calibrationTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    calibrationTextBlock.Text = headingAccuracy.ToString("0.0");
                }
            }
        }

        void compass_Calibrate(object sender, CalibrationEventArgs e)
        {
            Dispatcher.BeginInvoke(() => { calibrationStackPanel.Visibility = Visibility.Visible; });
            calibrating = true;

        }


        private void calibrationButton_Click(object sender, RoutedEventArgs e)
        {
            calibrationStackPanel.Visibility = Visibility.Collapsed;
            calibrating = false;
        }

        /*
         * Determine compass axis
         **/
        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Vector3 v = e.SensorReading.Acceleration;

            bool isCompassUsingNegativeZAxis = false;

            if (Math.Abs(v.Z) < Math.Cos(Math.PI / 4) &&
                (v.Y < Math.Sin(7 * Math.PI / 4)))
            {
                isCompassUsingNegativeZAxis = true;
            }

            Dispatcher.BeginInvoke(() => { orientationTextBlock.Text = (isCompassUsingNegativeZAxis) ? "portrait mode" : "flat mode"; });
        }
    }
}