#define DEBUG_AGENT


using System;
using System.Windows;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Scheduler;


namespace sdkBackgroundAgentCS
{
    public partial class MainPage : PhoneApplicationPage
    {

        PeriodicTask periodicTask;
        ResourceIntensiveTask resourceIntensiveTask;

        string periodicTaskName = "PeriodicAgent";
        string resourceIntensiveTaskName = "ResourceIntensiveAgent";
        public bool agentsAreEnabled = true;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void StartPeriodicAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "This demonstrates a periodic task.";

            // Place the call to Add in a try block in case the user has disabled agents
            try
            {
                ScheduledActionService.Add(periodicTask);

                PeriodicStackPanel.DataContext = periodicTask;

                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.
                }
                PeriodicCheckBox.IsChecked = false;
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
                PeriodicCheckBox.IsChecked = false;
            }

        }

        private void StartResourceIntensiveAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            resourceIntensiveTask = ScheduledActionService.Find(resourceIntensiveTaskName) as ResourceIntensiveTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (resourceIntensiveTask != null)
            {
                RemoveAgent(resourceIntensiveTaskName);
            }

            resourceIntensiveTask = new ResourceIntensiveTask(resourceIntensiveTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            resourceIntensiveTask.Description = "This demonstrates a resource-intensive task.";

            // Place the call to Add in a try block in case the user has disabled agents
            try
            {
                ScheduledActionService.Add(resourceIntensiveTask);

                ResourceIntensiveStackPanel.DataContext = resourceIntensiveTask;

                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(resourceIntensiveTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false; 
                }
                ResourceIntensiveCheckBox.IsChecked = false;
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
                ResourceIntensiveCheckBox.IsChecked = false;
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        bool ignoreCheckBoxEvents = false;

        private void PeriodicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            StartPeriodicAgent();
        }
        private void PeriodicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            RemoveAgent(periodicTaskName);
        }
        private void ResourceIntensiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            StartResourceIntensiveAgent();
        }
        private void ResourceIntensiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            RemoveAgent(resourceIntensiveTaskName);
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ignoreCheckBoxEvents = true;

            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
            {
                PeriodicStackPanel.DataContext = periodicTask;
            }

            resourceIntensiveTask = ScheduledActionService.Find(resourceIntensiveTaskName) as ResourceIntensiveTask;
            if (resourceIntensiveTask != null)
            {
                ResourceIntensiveStackPanel.DataContext = resourceIntensiveTask;
            }

            ignoreCheckBoxEvents = false;

        }

    }
}

