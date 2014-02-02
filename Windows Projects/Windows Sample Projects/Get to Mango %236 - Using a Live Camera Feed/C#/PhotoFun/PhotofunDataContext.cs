using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using PhotoFun.Effects;

namespace PhotoFun
{
    /// <summary>
    /// This class acts as data context for the application screen
    /// </summary>
    public class PhotofunDataContext : INotifyPropertyChanged
    {
        #region Members
        private List<EffectDetails> _effects;
        private EffectDetails _selectedEffect;
        private List<FrameDetails> _frames;
        private FrameDetails _selectedFrame;
        private WriteableBitmap _selectedPreview;
        #endregion

        #region Properties
        public List<EffectDetails> Effects
        {
            get
            {
                return _effects;
            }
            set
            {
                _effects = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Effects"));
            }
        }

        public List<FrameDetails> Frames
        {
            get
            {
                return _frames;
            }
            set
            {
                _frames = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Frames"));
            }
        }

        /// <summary>
        /// Selected effect
        /// </summary>
        public EffectDetails SelectedEffect
        {
            get
            {
                return _selectedEffect;
            }
            set
            {
                if (value != _selectedEffect)
                {
                    if (_selectedEffect != null)
                        _selectedEffect.IsSelected = false;
                    value.IsSelected = true;
                    _selectedEffect = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedEffect"));
                }
            }
        }

        /// <summary>
        /// Selected frame details
        /// </summary>
        public FrameDetails SelectedFrame
        {
            get
            {
                return _selectedFrame;
            }
            set
            {
                if (value != _selectedFrame)
                {
                    if (_selectedFrame != null)
                        _selectedFrame.IsSelected = false;
                    value.IsSelected = true;
                    _selectedFrame = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedFrame"));
                }
            }
        }

        /// <summary>
        /// Images saved by the application 
        /// </summary>
        public ObservableCollection<WriteableBitmap> Previews { get; set; }

        public WriteableBitmap SelectedPreview
        {
            get
            {
                return _selectedPreview;
            }
            set
            {
                _selectedPreview = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedPreview"));
            }
        }

        #endregion

        public PhotofunDataContext()
        {
            SetEffects();
            SetFrames();
            Previews = new ObservableCollection<WriteableBitmap>();
            SelectedEffect = Effects[0];
            SelectedFrame = Frames[0];
        }

        #region Methods

        private void SetFrames()
        {
            var frames = new List<FrameDetails>();
            frames.Add(new FrameDetails() { FrameUri = null, Name = "None" });
            frames.Add(new FrameDetails() { FrameUri = new Uri("/PhotoFun;component/Frames/FlowerFrame.png", UriKind.RelativeOrAbsolute), Name = "Flowers" });
            frames.Add(new FrameDetails() { FrameUri = new Uri("/PhotoFun;component/Frames/OldFrame.png", UriKind.RelativeOrAbsolute), Name = "Old Frame" });
            Frames = frames;
        }

        /// <summary>
        /// Sets the effects to be shown on the list
        /// </summary>
        private void SetEffects()
        {
            var effects = new List<EffectDetails>();
            effects.Add(new EffectDetails { Effect = new EchoEffect(), Name = "None" });
            effects.Add(new EffectDetails { Effect = new GrayScaleEffect(), Name = "Gray scale" });
            effects.Add(new EffectDetails { Effect = new SepiaEffect(), Name = "Sepia" });
            effects.Add(new EffectDetails { Effect = new NegativeEffect(), Name = "Negative" });
            Effects = effects;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Effect details
    /// </summary>
    public class EffectDetails
    {
        /// <summary>
        /// Effect displayed name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Image processing effect
        /// </summary>
        public IEffect Effect { get; set; }

        /// <summary>
        /// Gets whether this effect is selected
        /// </summary>
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// Frame details
    /// </summary>
    public class FrameDetails
    {
        /// <summary>
        /// Gets whether this frame is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Frame display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Uri to the frame picture file
        /// </summary>
        public Uri FrameUri { get; set; }
    }
}
