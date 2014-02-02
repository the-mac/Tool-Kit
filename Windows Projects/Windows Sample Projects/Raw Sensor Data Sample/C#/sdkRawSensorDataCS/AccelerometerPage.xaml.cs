using System;
using Microsoft.Phone.Controls;


using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Windows.Threading;

namespace sdkRawSensorDataCS
{

    public partial class AccelerometerPage : PhoneApplicationPage
    {
        Accelerometer accelerometer;
        DispatcherTimer timer;
        Vector3 acceleration;
        bool isDataValid;

        public AccelerometerPage()
        {
            InitializeComponent();

            if (!Accelerometer.IsSupported)
            {
                // The device on which the application is running does not support
                // the accelerometer sensor. Alert the user and hide the
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
            if (accelerometer != null && accelerometer.IsDataValid)
            {
                // Stop data acquisition from the accelerometer.
                accelerometer.Stop();
                timer.Stop();
                statusTextBlock.Text = "accelerometer stopped.";

            }
            else
            {
                if (accelerometer == null)
                {
                    // Instantiate the accelerometer.
                    accelerometer = new Accelerometer();


                    // Specify the desired time between updates. The sensor accepts
                    // intervals in multiples of 20 ms.
                    accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);

                    // The sensor may not support the requested time between updates.
                    // The TimeBetweenUpdates property reflects the actual rate.
                    timeBetweenUpdatesTextBlock.Text = accelerometer.TimeBetweenUpdates.TotalMilliseconds + " ms";


                    accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
                }

                try
                {
                    statusTextBlock.Text = "starting accelerometer.";
                    accelerometer.Start();
                    timer.Start();
                }
                catch (InvalidOperationException)
                {
                    statusTextBlock.Text = "unable to start accelerometer.";
                }

            }

        }

        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // Note that this event handler is called from a background thread
            // and therefore does not have access to the UI thread. To update 
            // the UI from this handler, use Dispatcher.BeginInvoke() as shown.
            // Dispatcher.BeginInvoke(() => { statusTextBlock.Text = "in CurrentValueChanged"; });


            isDataValid = accelerometer.IsDataValid;

            acceleration = e.SensorReading.Acceleration;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (isDataValid)
            {
                statusTextBlock.Text = "receiving data from accelerometer.";

                // Show the numeric values
                xTextBlock.Text = "X: " + acceleration.X.ToString("0.00");
                yTextBlock.Text = "Y: " + acceleration.Y.ToString("0.00");
                zTextBlock.Text = "Z: " + acceleration.Z.ToString("0.00");

                // Show the values graphically
                xLine.X2 = xLine.X1 + acceleration.X * 100;
                yLine.Y2 = yLine.Y1 - acceleration.Y * 100;
                zLine.X2 = zLine.X1 - acceleration.Z * 50;
                zLine.Y2 = zLine.Y1 + acceleration.Z * 50;
            }
        }
    }
}