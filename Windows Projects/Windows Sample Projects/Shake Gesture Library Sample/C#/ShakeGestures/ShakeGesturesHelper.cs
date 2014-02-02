/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
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
using Microsoft.Phone.Applications.Common;
using System.Collections.Generic;
using System.Threading;

namespace ShakeGestures
{
    public class ShakeGesturesHelper
    {
        #region Gesture parameters

        /// <summary>
        /// Any vector that has a magnitude (after reducing gravitation) bigger than this parameter will be considered as a shake vector
        /// </summary>
        private const double ShakeMagnitudeWithoutGravitationThreshold_Default = 0.2;
        public double ShakeMagnitudeWithoutGravitationThreshold { get; set; }

        /// <summary>
        /// This parameter determines how many consecutive still vectors are required to stop a shake signal
        /// </summary>
        private const int StillCounterThreshold_Default = 20;
        public int StillCounterThreshold { get; set; }

        /// <summary>
        /// This parameter determines the maximum allowed magnitude (after reducing gravitation) for a still vector to be considered to the average
        /// The last still vector is averaged out of still vectors that are under this bound
        /// </summary>
        private const double StillMagnitudeWithoutGravitationThreshold_Default = 0.02;
        public double StillMagnitudeWithoutGravitationThreshold { get; set; }

        /// <summary>
        /// The maximum amount of still vectors needed to create a still vector average
        /// instead of averaging the entire still signal, we just look at the top recent still vectors
        /// </summary>
        private const int MaximumStillVectorsNeededForAverage_Default = 20;
        public int MaximumStillVectorsNeededForAverage { get; set; }

        /// <summary>
        /// The minimum amount of still vectors needed to create a still vector average.
        /// Without enough vectors the average will not be stable and thus ignored
        /// </summary>
        private const int MinimumStillVectorsNeededForAverage_Default = 5;
        public int MinimumStillVectorsNeededForAverage { get; set; }

        /// <summary>
        /// Determines the amount of shake vectors needed that has been classified the same to recognize a shake
        /// </summary>
        private const int MinimumShakeVectorsNeededForShake_Default = 10;
        public int MinimumShakeVectorsNeededForShake { get; set; }

        /// <summary>
        /// Shake vectors with magnitude less than this parameter will not be considered for gesture classification
        /// </summary>
        private const double WeakMagnitudeWithoutGravitationThreshold_Default = 0.2;
        public double WeakMagnitudeWithoutGravitationThreshold { get; set; }

        /// <summary>
        /// Determines the number of moves required to get a shake signal
        /// </summary>
        private const int MinimumRequiredMovesForShake_Default = 3;
        public int MinimumRequiredMovesForShake { get; set; }

        #endregion

        #region Private fields

        // Singleton instance for helper
        private static volatile ShakeGesturesHelper _singletonInstance;

        // private lock object for protecting the singleton instance
        private static Object _syncRoot = new Object();

        // last still vector - average of the last still signal
        // used to eliminate the gravitation effect 
        // initial value has no meaning, it's just a dummy vector to avoid dealing with null values
        private Simple3DVector _lastStillVector = new Simple3DVector(0, -1, 0);

        // flag that indicates whether we are currently in a middle of a shake signal
        private bool _isInShakeState = false;

        // counts the number of still vectors - while in shake signal
        private int _stillCounter = 0;

        // holds shake signal vectors
        private List<Simple3DVector> _shakeSignal = new List<Simple3DVector>();

        // holds shake signal histogram
        private int[] _shakeHistogram = new int[3];

        // hold still signal vectors, newest vectors are first
        private LinkedList<Simple3DVector> _stillSignal = new LinkedList<Simple3DVector>();

        #endregion

        #region Public events

        /// <summary>
        /// Fires when a new shake gesture has been identified
        /// </summary>
        public event EventHandler<ShakeGestureEventArgs> ShakeGesture;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// Use Instance property to get singleton instance
        /// </summary>
        private ShakeGesturesHelper()
        {
            // set default values for parameters
            ShakeMagnitudeWithoutGravitationThreshold = ShakeMagnitudeWithoutGravitationThreshold_Default;
            StillCounterThreshold = StillCounterThreshold_Default;
            StillMagnitudeWithoutGravitationThreshold = StillMagnitudeWithoutGravitationThreshold_Default;
            MinimumStillVectorsNeededForAverage = MinimumStillVectorsNeededForAverage_Default;
            MaximumStillVectorsNeededForAverage = MaximumStillVectorsNeededForAverage_Default;
            MinimumShakeVectorsNeededForShake = MinimumShakeVectorsNeededForShake_Default;
            WeakMagnitudeWithoutGravitationThreshold = WeakMagnitudeWithoutGravitationThreshold_Default;
            MinimumRequiredMovesForShake = MinimumRequiredMovesForShake_Default;

            // register for acceleromter input
            AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Singleton instance of the Shake Gestures Helper class
        /// </summary>
        public static ShakeGesturesHelper Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new ShakeGesturesHelper();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        /// <summary>
        /// Accelerometer is active and reading value when true
        /// </summary>
        public bool Active
        {
            get
            {
                return AccelerometerHelper.Instance.Active;
            }
            set
            {
                AccelerometerHelper.Instance.Active = value;
            }
        }

        public void Simulate(ShakeType shakeType)
        {
            bool activePreviousState = Active;

            ThreadPool.QueueUserWorkItem(
                (o) =>
                {
                    Active = false;

                    Simulation.CallTo = OnAccelerometerHelperReadingChanged;

                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    switch (shakeType)
                    {
                        case ShakeType.X:
                            Simulation.SimulateShakeX();
                            break;

                        case ShakeType.Y:
                            Simulation.SimulateShakeY();
                            break;

                        case ShakeType.Z:
                            Simulation.SimulateShakeZ();
                            break;
                    }
                    Simulation.CallTo = null;

                    Active = activePreviousState;
                });

        }

        #endregion

        #region Private methods

        /// <summary>
        /// Called when the accelerometer provides a new value
        /// </summary>
        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            // work with optimal vector (without noise)
            Simple3DVector currentVector = e.OptimalyFilteredAcceleration;

            // check if this vector is considered a shake vector
            bool isShakeMagnitude = (Math.Abs(_lastStillVector.Magnitude - currentVector.Magnitude) > ShakeMagnitudeWithoutGravitationThreshold);

            // following is a state machine for detection of shake signal start and end

            // if still --> shake
            if ((!_isInShakeState) && (isShakeMagnitude))
            {
                // set shake state flag
                _isInShakeState = true;

                // clear old shake signal
                ClearShakeSignal();

                // process still signal
                ProcessStillSignal();

                // add vector to shake signal
                AddVectorToShakeSignal(currentVector);
            }
            // if still --> still
            else if ((!_isInShakeState) && (!isShakeMagnitude))
            {
                // add vector to still signal
                AddVectorToStillSignal(currentVector);
            }
            // if shake --> shake
            else if ((_isInShakeState) && (isShakeMagnitude))
            {
                // add vector to shake signal
                AddVectorToShakeSignal(currentVector);

                // reset still counter
                _stillCounter = 0;

                // try to process shake signal
                if (ProcessShakeSignal())
                {
                    // shake signal generated, clear used data
                    ClearShakeSignal();
                }
            }
            // if shake --> still
            else if ((_isInShakeState) && (!isShakeMagnitude))
            {
                // add vector to shake signal
                AddVectorToShakeSignal(currentVector);

                // count still vectors
                _stillCounter++;

                // if too much still samples
                if (_stillCounter > StillCounterThreshold)
                {
                    // clear old still signal
                    _stillSignal.Clear();

                    // add still vectors from shake signal to still signal
                    for (int i = 0; i < StillCounterThreshold; ++i)
                    {
                        // calculate current index element
                        int currentSampleIndex = _shakeSignal.Count - StillCounterThreshold + i;

                        // add vector to still signal
                        AddVectorToStillSignal(currentVector);
                    }

                    // remove last samples from shake signal
                    _shakeSignal.RemoveRange(_shakeSignal.Count - StillCounterThreshold, StillCounterThreshold);

                    // reset shake state flag
                    _isInShakeState = false;
                    _stillCounter = 0;

                    // try to process shake signal
                    if (ProcessShakeSignal())
                    {
                        ClearShakeSignal();
                    }
                }
            }
        }

        private void AddVectorToStillSignal(Simple3DVector currentVector)
        {
            // add current vector to still signal, newest vectors are first
            _stillSignal.AddFirst(currentVector);

            // if still signal is getting too big, remove old items
            if (_stillSignal.Count > 2 * MaximumStillVectorsNeededForAverage) 
            {
                _stillSignal.RemoveLast();
            }
        }

        /// <summary>
        /// Add a vector the shake signal and does some preprocessing
        /// </summary>
        private void AddVectorToShakeSignal(Simple3DVector currentVector)
        {
            // remove still vector from current vector
            Simple3DVector currentVectorWithoutGravitation = currentVector - _lastStillVector;

            // add current vector to shake signal
            _shakeSignal.Add(currentVectorWithoutGravitation);

            // skip weak vectors
            if (currentVectorWithoutGravitation.Magnitude < WeakMagnitudeWithoutGravitationThreshold)
            {
                return;
            }

            // classify vector 
            ShakeType vectorShakeType = ClassifyVectorShakeType(currentVectorWithoutGravitation);

            // count vector to histogram
            _shakeHistogram[(int)vectorShakeType]++;
        }

        /// <summary>
        /// Clear shake signal and related data
        /// </summary>
        private void ClearShakeSignal()
        {
            // clear shake signal
            _shakeSignal.Clear();

            // create empty histogram
            Array.Clear(_shakeHistogram, 0, _shakeHistogram.Length);
        }

        /// <summary>
        /// Process still signal: calculate average still vector
        /// </summary>
        private void ProcessStillSignal()
        {
            Simple3DVector sumVector = new Simple3DVector(0, 0, 0);
            int count = 0;

            // going over vectors in still signal
            // still signal was saved backwards, i.e. newest vectors are first
            foreach (Simple3DVector currentStillVector in _stillSignal)
            {
                // make sure current vector is very still
                bool isStillMagnitude = (Math.Abs(_lastStillVector.Magnitude - currentStillVector.Magnitude) < StillMagnitudeWithoutGravitationThreshold);

                if (isStillMagnitude)
                {
                    // sum x,y,z values
                    sumVector += currentStillVector;
                    ++count;

                    // 20 samples are sufficent
                    if (count >= MaximumStillVectorsNeededForAverage)
                    {
                        break;
                    }
                }
            }

            // need at least a few vectors to get a good average
            if (count >= MinimumStillVectorsNeededForAverage)
            {
                // calculate average of still vectors
                _lastStillVector = sumVector / count;
            }
        }
    
        /// <summary>
        /// Classify vector shake type
        /// </summary>
        private ShakeType ClassifyVectorShakeType(Simple3DVector v)
        {
            double absX = Math.Abs(v.X);
            double absY = Math.Abs(v.Y);
            double absZ = Math.Abs(v.Z);

            // check if X is the most significant component
            if ((absX >= absY) & (absX >= absZ))
            {
                return ShakeType.X;
            }

            // check if Y is the most significant component
            if ((absY >= absX) & (absY >= absZ))
            {
                return ShakeType.Y;
            }

            // Z is the most significant component
            return ShakeType.Z;
        }

        /// <summary>
        /// Classify shake signal according to shake histogram
        /// </summary>
        private bool ProcessShakeSignal()
        {
            int xCount = _shakeHistogram[0];
            int yCount = _shakeHistogram[1];
            int zCount = _shakeHistogram[2];

            ShakeType? shakeType = null;

            // if X part is strongest and above a minimum
            if ((xCount >= yCount) & (xCount >= zCount) & (xCount >= MinimumShakeVectorsNeededForShake))
            {
                shakeType = ShakeType.X;
            }
            // if Y part is strongest and above a minimum
            else if ((yCount >= xCount) & (yCount >= zCount) & (yCount >= MinimumShakeVectorsNeededForShake))
            {
                shakeType = ShakeType.Y;
            }
            // if Z part is strongest and above a minimum
            else if ((zCount >= xCount) & (zCount >= yCount) & (zCount >= MinimumShakeVectorsNeededForShake))
            {
                shakeType = ShakeType.Z;
            }

            if (shakeType != null)
            {
                int countSignsChanges = CountSignChanges(shakeType.Value);

                // check that we have enough shakes 
                if (countSignsChanges < MinimumRequiredMovesForShake)
                {
                    shakeType = null;
                }
            }

            // if a strong shake was detected
            if (shakeType != null)
            {
                // raise shake gesture event
                EventHandler<ShakeGestureEventArgs> localShakeGesture = ShakeGesture;
                if (localShakeGesture != null)
                {
                    localShakeGesture(this, new ShakeGestureEventArgs(shakeType.Value));
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Count how many shakes the shake signal contains
        /// </summary>
        private int CountSignChanges(ShakeType shakeType)
        {
            int countSignsChanges = 0;
            int currentSign = 0;
            int? prevSign = null;

            for (int i = 0; i < _shakeSignal.Count; ++i)
            {
                // get sign of current vector
                switch (shakeType)
                {
                    case ShakeType.X:
                        currentSign = Math.Sign(_shakeSignal[i].X);
                        break;

                    case ShakeType.Y:
                        currentSign = Math.Sign(_shakeSignal[i].Y);
                        break;

                    case ShakeType.Z:
                        currentSign = Math.Sign(_shakeSignal[i].Z);
                        break;
                }

                // skip sign-less vectors
                if (currentSign == 0)
                {
                    continue;
                }

                // handle sign for first vector
                if (prevSign == null)
                {
                    prevSign = currentSign;
                }

                // check if sign changed
                if (currentSign != prevSign)
                {
                    ++countSignsChanges;
                }

                // save previous sign
                prevSign = currentSign;
            }

            return countSignsChanges;
        }

        #endregion
    }
}
