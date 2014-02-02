/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using ComputeComponent_Phone;
using SdkHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace sdkWindowsRuntimeComponentWP8CS
{
    /// <summary>
    /// The ViewModel for the async computation scenarios.
    /// </summary>
    public class ComputeViewModel : INotifyPropertyChanged
    {
        public ComputeViewModel()
        {
            this._computeComponent = new ComputeComponent();
            this.MinVal = 0;
            this.MaxVal = 200000;
            this.Results = new ObservableCollection<int>();

            // Provide a handler for the Progress event that the asyncResult
            // object raises at regular intervals. This handler updates the progress bar.
            this.MyProgress = new Progress<double>();
            this.MyProgress.ProgressChanged += MyProgress_ProgressChanged;
            this.ProgressPercent = 0;
            this.ReportProgress = false;
            this.CanCancel = false;
            this.Executing = false;
            this.BulkMode = true;
        }

        #region Properties
        bool _bulkMode;

        /// <summary>
        ///  Determines whether the computation will be performed in Bulk mode.
        ///  True if the computation should be performed in Bulk Mode; False, otherwise.
        /// </summary>
        /// <remarks>Bulk mode means that the computation will return a list of results when the computation has completed.
        ///  When not in Bulk mode, the computation will raise an event to pass back each result as it finds one.</remarks>
        public bool BulkMode
        {
            get
            {
                return _bulkMode;
            }
            set
            {
                if (value != _bulkMode)
                {
                    _bulkMode = value;
                    RaisePropertyChanged();
                }
            }
        }
        /// <summary>
        /// A list of results for the calculation.
        /// </summary>
        public ObservableCollection<int> Results { get; private set;}

        /// <summary>
        /// The first value in the range of numbers involved in the computation.
        /// </summary>
        public int MinVal { get; set; }

        /// <summary>
        /// The last value in the range of numbers involved in the computation.
        /// </summary>
        public int MaxVal { get; set; }

        private int _progressPercent;

        /// <summary>
        /// The percentage complete of the current computation, represented as an integer.
        /// </summary>
        public int ProgressPercent
        {
            get
            {
                return _progressPercent;
            }

            private set
            {
                if (value != _progressPercent)
                {
                    _progressPercent = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double? _duration;

        /// <summary>
        /// The number of seconds, or part of a second, the last computation took to complete.
        /// </summary>
        public double? DurationSeconds
        {
            get
            {
                return _duration;
            }

            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the computation should report progress. 
        /// True if progress should be reported; False, otherwise.
        /// </summary>
        public bool ReportProgress { get; set; }

        bool? _canCancel;

        /// <summary>
        /// Gets or sets whether the user should be able to cancel the current operation.
        /// True if the computation is cancellable; False, otherwise.
        /// </summary>
        public bool CanCancel
        {
            get
            {
                return _canCancel.HasValue && _canCancel.Value;
            }

            set
            {
                if (value != _canCancel)
                {
                    _canCancel = value;
                    RaisePropertyChanged();

                    // Since checking the value of CanCancel is part of determining whether
                    // the CancelCommand can be executed, we call the RaiseCanExecuteChanged() to make sure the UI
                    // refreshes its state based on this updated value.
                    CancelCommand.RaiseCanExecuteChanged();
                }
            }
        }

        bool? _executing;

        /// <summary>
        /// Gets whether a computation is currently executing.
        /// </summary>
        public bool Executing
        {
            get
            {
                return _executing.HasValue && _executing.Value;
            }

            private set
            {
                if (value != _executing)
                {
                    _executing = value;
                    RaisePropertyChanged();

                    // Since checking the value of Executing is part of determining whether
                    // the ExecuteCommand and CancelCommand can be executed, we call the RaiseCanExecuteChanged() on both to make sure the UI
                    // refreshes its state based on this updated value.
                    ExecuteCommand.RaiseCanExecuteChanged();
                    CancelCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Progress
        internal Progress<double> MyProgress;   // Progress<T> is defined in the framework and implements IProgress<T>

        // Handle progress update from ComputeComponent
        void MyProgress_ProgressChanged(object sender, double e)
        {
            // Update the ProgressPercent property, which is bound to a Progressbar in the UI.
            // The operation that reports thsi progress, reports percentage as a double. We could
            // modify that code to report as an int and avoid this cast, but that's left as an exercise.
            this.ProgressPercent = (int)e;
        }
        #endregion

        #region Timer
        // Timer used to record the duration of a calculation
        Stopwatch _sw;
        private void StartTimer()
        {
            this.DurationSeconds = null;
            if (_sw == null)
                _sw = new Stopwatch();
            _sw.Reset();
            _sw.Start();

        }

        private void StopTimer()
        {
            if (_sw != null)
            {
                _sw.Stop();
                this.DurationSeconds = _sw.ElapsedMilliseconds / 1000d;
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                // Make sure all property changed events are raised on the UI thread.
                App.RootFrame.Dispatcher.BeginInvoke(() =>
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    });
            }
        }
        #endregion

        #region Commands
        RelayCommand _executeCommand;

        public RelayCommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new RelayCommand(
                      async param => await ExecuteAsync(),
                        param => !this.Executing);
                }
                return _executeCommand;
            }
        }

        RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    // Note the multiple conditions that are checked to determine whether this command can execute.
                    _cancelCommand = new RelayCommand(
                      param => this.Cancel(),
                        param => CanCancel && this.Executing && this.CancelTokenSource != null && this.CancelTokenSource.Token.CanBeCanceled);
                }
                return _cancelCommand;
            }
        }
        #endregion

        #region ComputeComponent 
        private ComputeComponent _computeComponent;
        CancellationTokenSource _cts;

        private CancellationTokenSource CancelTokenSource
        {
            get
            {
                return _cts;
            }
            set
            {
                if (value != _cts)
                {
                    _cts = value;

                    // Since checking the value of CancelTokenSource is part of determining whether
                    // the ExecuteCommand can be executed, we call the RaiseCanExecuteChanged() to make sure the UI
                    // refreshes its state based on this updated value.
                    CancelCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async Task ExecuteAsync()
        {
            // Based on the options selected by the user, perform the calculation asynchronously and
            // possibly with the ability to cancel and report progress.


            if (!this.ReportProgress && !CanCancel)
            {
                await ComputeAsync();
            }
            else if (!this.ReportProgress && CanCancel)
            {
                await ComputeAsyncWithCancel();
            }
            else if (this.ReportProgress && !CanCancel)
            {
                await ComputeAsyncWithProgress();
            }
            else if (this.ReportProgress && CanCancel)
            {
                await ComputeAsyncWithProgressAndCancel();
            }
        }

        #region Scenario: Asynchronous computation - does not report progress and can't be canceled.
        private async Task ComputeAsync()
        {
            this.Executing = true;
            this.Results.Clear();

            if (BulkMode)
            {
                this.StartTimer();
                var primes = await _computeComponent.GetPrimesBulkAsync(MinVal, MaxVal);
                this.StopTimer();

                // Once the computation has completed, add the list of primes returned to the Results collection.
                // ObservableCollection does not have an AddRange method, and deriving our own collection for that purpose is
                // beyond the scope of this sample.
                foreach (var prime in primes)
                {
                    this.Results.Add(prime);
                }
            }
            else
            {
                // Subscribe to primeFoundEvent event
                _computeComponent.primeFoundEvent += OnPrimeFound;
                this.StartTimer();
                await _computeComponent.GetPrimesImmediateAsync(MinVal, MaxVal);
                this.StopTimer();

                // Unregister primeFoundEvent
                _computeComponent.primeFoundEvent -= OnPrimeFound;
            }
            this.Executing = false;
        }
        #endregion

        #region Scenario: Asynchronous computation that can be canceled.
        private async Task ComputeAsyncWithCancel()
        {
            this.Results.Clear();
            this.CancelTokenSource = new CancellationTokenSource();

            if (BulkMode)
            {
                this.Executing = true;
                IList<int> primes = new List<int>();
                this.StartTimer();

                try
                {
                    // To make this call cancellable, pass down a CancellationToken using the AsTask extension method. 
                    primes = await _computeComponent.GetPrimesBulkAsync(MinVal, MaxVal).AsTask(_cts.Token);
                }
                catch (TaskCanceledException)
                {
                    // By design when the task is canceled.
                }
                finally
                {
                    this.StopTimer();

                    // Once the computation has completed, add the list of primes returned to the Results collection.
                    // ObservableCollection does not have an AddRange method, and deriving our own collection for that purpose is
                    // beyond the scope of this sample.
                    foreach (var prime in primes)
                    {
                        this.Results.Add(prime);
                    }
                    this.Executing = false;
                }
            }
            else
            {
                this.Executing = true;
                this.StartTimer();

                // Subscribe to primeFoundEvent event
                _computeComponent.primeFoundEvent += OnPrimeFound;
                try
                {
                    // To make this call cancellable, pass down a CancellationToken using the AsTask extension method. 
                    await _computeComponent.GetPrimesImmediateAsync(MinVal, MaxVal).AsTask(_cts.Token);
                }
                catch (TaskCanceledException)
                {
                    // By design when the task is canceled.
                }
                finally
                {
                    // Unregister primeFoundEvent
                    _computeComponent.primeFoundEvent -= OnPrimeFound;
                    this.StopTimer();
                    this.Executing = false;
                }

            }
        }
        #endregion

        #region Scenario: Asynchronous computation that can't be canceled, but does report progress
        private async Task ComputeAsyncWithProgress()
        {
            this.Executing = true;
            this.Results.Clear();

            if (BulkMode)
            {

                this.StartTimer();

                // In order to report progress, we pass an implementation of IProgress<T> to the method through the AsTask
                // extension method. We have defined our own property, MyProgress, for this purpose. MyProgress is an instance
                // of Progress<T> which is an in-box implementation of IProgress<T>.
                var primes = await _computeComponent.GetPrimesBulkAsync(MinVal, MaxVal).AsTask(MyProgress);
                this.StopTimer();
                foreach (var prime in primes)
                {
                    this.Results.Add(prime);
                }

            }
            else
            {
                // Subscribe to primeFoundEvent event
                _computeComponent.primeFoundEvent += OnPrimeFound;
                this.StartTimer();

                // In order to report progress, we pass an implementation of IProgress<T> to the method through the AsTask
                // extension method. We have defined our own property, MyProgress, for this purpose. MyProgress is an instance
                // of Progress<T> which is an in-box implementation of IProgress<T>.
                await _computeComponent.GetPrimesImmediateAsync(MinVal, MaxVal).AsTask(MyProgress);
                this.StopTimer();

                // Unregister primeFoundEvent
                _computeComponent.primeFoundEvent -= OnPrimeFound;
            }

            this.Executing = false;
        }
        #endregion

        #region Scenario: Asynchronous computation that can be canceled and reports progress
        private async Task ComputeAsyncWithProgressAndCancel()
        {
            this.Results.Clear();
            this.CancelTokenSource = new CancellationTokenSource();

            if (BulkMode)
            {
                this.Executing = true;
                this.StartTimer();
                IList<int> primes = new List<int>();

                // Use the AsTask extension on IAsyncOperationWithProgress to pass down a CancellationToken and an 
                // implementation of IProgress<T> so that this call reports progress and is cancellable.
                try
                {

                    primes = await _computeComponent.GetPrimesBulkAsync(MinVal, MaxVal).AsTask(_cts.Token, this.MyProgress);
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    this.StopTimer();
                    foreach (var prime in primes)
                    {
                        this.Results.Add(prime);
                    }
                    this.Executing = false;
                }
            }
            else
            {
                this.Executing = true;
                this.StartTimer();

                // Subscribe to primeFoundEvent event
                _computeComponent.primeFoundEvent += OnPrimeFound;

                // Use the AsTask extension on IAsyncOperationWithProgress to pass down a CancellationToken and an 
                // implementation of IProgress<T> so that this call reports progress and is cancellable.
                try
                {
                    await _computeComponent.GetPrimesImmediateAsync(MinVal, MaxVal).AsTask(_cts.Token, this.MyProgress);
                }
                catch (TaskCanceledException)
                {

                }
                finally
                {
                    this.StopTimer();
                    this.Executing = false;
                    // Unregister primeFoundEvent
                    _computeComponent.primeFoundEvent -= OnPrimeFound;
                }

            }
        }
        #endregion

        /// <summary>
        /// Event handler for PrimeFound event from ComputeComponent
        /// </summary>
        /// <param name="primeNumber">The prime number that was found.</param>
        void OnPrimeFound(int primeNumber)
        {
            // Because Results is bound to the UI, make sure the add operation
            // takes place on the UI thread.
            App.RootFrame.Dispatcher.BeginInvoke(() =>
                    {
                        Results.Add(primeNumber);
                    });
        }

        private void Cancel()
        {
            if (this.CancelTokenSource != null)
            {
                // Calling cancel will make  the isCanceled() method on the token passed to the task in ComputeComponent
                // return true when it is called. The method in which this check is made will then initiate a cancel and throw a TaskCanceledException.
                this.CancelTokenSource.Cancel();
            }
        }
        #endregion

    }
}
