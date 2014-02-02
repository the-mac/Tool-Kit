using System;
using System.Windows;
using Microsoft.Phone.Controls;

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Windows.Threading;


namespace sdkRawSensorDataCS
{
    public partial class GyroscopePage : PhoneApplicationPage
    {
        Gyroscope gyroscope;
        DispatcherTimer timer;

        Vector3 currentRotationRate = Vector3.Zero;
        Vector3 cumulativeRotation = Vector3.Zero;
        DateTimeOffset lastUpdateTime = DateTimeOffset.MinValue;
        bool isDataValid;

        // Constructor
        public GyroscopePage()
        {
            InitializeComponent();

            Application.Current.Host.Settings.EnableFrameRateCounter = false;

            if (!Gyroscope.IsSupported)
            {
                // The device on which the application is running does not support
                // the gyroscope sensor. Alert the user and hide the
                // application bar.
                statusTextBlock.Text = "device does not support gyroscope";
                ApplicationBar.IsVisible = false;
            }
            else
            {
                // Initialize the timer and add Tick event handler, but don't start it yet.
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(60);
                timer.Tick += new EventHandler(timer_Tick);
            }
        }


        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (gyroscope != null && gyroscope.IsDataValid)
            {
                // Stop data acquisition from the gyroscope.
                gyroscope.Stop();
                timer.Stop();
                statusTextBlock.Text = "gyroscope stopped.";
            }
            else
            {
                if (gyroscope == null)
                {
                    // Instantiate the Gyroscope.
                    gyroscope = new Gyroscope();

                    // Specify the desired time between updates. The sensor accepts
                    // intervals in multiples of 20 ms.
                    gyroscope.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);

                    // The sensor may not support the requested time between updates.
                    // The TimeBetweenUpdates property reflects the actual rate.
                    timeBetweenUpdatesTextBlock.Text = "time between updates: " + gyroscope.TimeBetweenUpdates.TotalMilliseconds + " ms";


                    gyroscope.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<GyroscopeReading>>(gyroscope_CurrentValueChanged);
                }

                try
                {
                    statusTextBlock.Text = "starting gyroscope.";
                    gyroscope.Start();
                    timer.Start();
                }
                catch (InvalidOperationException)
                {
                    statusTextBlock.Text = "unable to start gyroscope.";
                }

            }

        }

        void gyroscope_CurrentValueChanged(object sender, SensorReadingEventArgs<GyroscopeReading> e)
        {
            // Note that this event handler is called from a background thread
            // and therefore does not have access to the UI thread. To update 
            // the UI from this handler, use Dispatcher.BeginInvoke() as shown.
            // Dispatcher.BeginInvoke(() => { statusTextBlock.Text = "in CurrentValueChanged"; });


            isDataValid = gyroscope.IsDataValid;


            if (lastUpdateTime.Equals(DateTimeOffset.MinValue))
            {
                // If this is the first time CurrentValueChanged was raised,
                // only update the lastUpdateTime variable.
                lastUpdateTime = e.SensorReading.Timestamp;
            }
            else
            {
                // Get the current rotation rate. This value is in 
                // radians per second.
                currentRotationRate = e.SensorReading.RotationRate;


                // Subtract the previous timestamp from the current one
                // to determine the time between readings
                TimeSpan timeSinceLastUpdate = e.SensorReading.Timestamp - lastUpdateTime;

                // Obtain the amount the device rotated since the last update
                // by multiplying by the rotation rate by the time since the last update.
                // (radians/second) * secondsSinceLastReading = radiansSinceLastReading
                cumulativeRotation += currentRotationRate * (float)(timeSinceLastUpdate.TotalSeconds);

                lastUpdateTime = e.SensorReading.Timestamp;

            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (isDataValid)
            {
                statusTextBlock.Text = "receiving data from gyroscope.";
            }

            currentXTextBlock.Text = currentRotationRate.X.ToString("0.000");
            currentYTextBlock.Text = currentRotationRate.Y.ToString("0.000");
            currentZTextBlock.Text = currentRotationRate.Z.ToString("0.000");

            cumulativeXTextBlock.Text = MathHelper.ToDegrees(cumulativeRotation.X).ToString("0.00");
            cumulativeYTextBlock.Text = MathHelper.ToDegrees(cumulativeRotation.Y).ToString("0.00");
            cumulativeZTextBlock.Text = MathHelper.ToDegrees(cumulativeRotation.Z).ToString("0.00");


            double centerX = cumulativeGrid.ActualWidth / 2.0;
            double centerY = cumulativeGrid.ActualHeight / 2.0;

            currentXLine.X2 = centerX + currentRotationRate.X * 100;
            currentYLine.X2 = centerX + currentRotationRate.Y * 100;
            currentZLine.X2 = centerX + currentRotationRate.Z * 100;

            cumulativeXLine.X2 = centerX - centerY * Math.Sin(cumulativeRotation.X);
            cumulativeXLine.Y2 = centerY - centerY * Math.Cos(cumulativeRotation.X);
            cumulativeYLine.X2 = centerX - centerY * Math.Sin(cumulativeRotation.Y);
            cumulativeYLine.Y2 = centerY - centerY * Math.Cos(cumulativeRotation.Y);
            cumulativeZLine.X2 = centerX - centerY * Math.Sin(cumulativeRotation.Z);
            cumulativeZLine.Y2 = centerY - centerY * Math.Cos(cumulativeRotation.Z);
        }
    }
}