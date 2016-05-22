using System;
using System.Windows;

namespace gripit_client {
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        #region fields

        private int _x = 225;
        private int _y = 225;
        private IDisposable _disposable;
        private IGripItService _gripItService;
        private bool _started;
        private int _viewX;
        private int _viewY;
        private bool _isAnimating;
        private const int _domainForValues = 10000;
        private const int _centerOffset = 5000;
        private const int _viewCenter = 150;
        private const int _maxAmplitude = 150;

        #endregion

        #region properties

        public void WindowLoaded(object Window)
        {
            
        }

        public IGripItService GripItService
        {
            get { return _gripItService; }
            set
            {
                _gripItService = value;
                _disposable = _gripItService.OnNewForceProjection.Subscribe(args =>
                {
                    X = args.X;
                    Y = args.Y;
                    IsAnimating = true;
                });
            }
        }

        public ShellViewModel(IGripItService gripItService)
        {
            GripItService = gripItService;
        }

        // coordinates values that come from the service
        public int X
        {
            get { return _x; }
            set
            {
                if (value == _x) return;
                OldX = _x;
                _x = value;
                NotifyOfPropertyChange(() => X);
                NotifyOfPropertyChange(() => ViewX);
                NotifyOfPropertyChange(() => Angle);
            }
        }

        public int OldX { get; set; }

        // coordinates values that come from the service
        public int Y
        {
            get { return _y; }
            set
            {
                if (value == _y) return;
                OldY = _y;
                _y = value;
                NotifyOfPropertyChange(() => Y);
                NotifyOfPropertyChange(() => ViewY);
                NotifyOfPropertyChange(() => Angle);
            }
        }

        public int OldY { get; set; }

        public bool Started
        {
            get { return _started; }
            set
            {
                if (value == _started) return;
                _started = value;
                NotifyOfPropertyChange(() => Started);
                NotifyOfPropertyChange(() => Stopped);
            }
        }

        public int ViewX => (X + _centerOffset) * 300 / _domainForValues;

        public int ViewY
        {
            get
            {
                var result = (Math.Abs(Y)*_maxAmplitude/_centerOffset);
                if (Y < 0)
                {
                    result = result + _viewCenter;
                }
                else
                {
                    result = _viewCenter - result;
                }

                return result;
            }
        }

        public double Angle
        {
            get
            {
                var radians = Math.Atan2(Y, X);
                return radians*(180/Math.PI);
            }
        }

        public bool Stopped => !_started;

        public bool IsAnimating
        {
            get { return _isAnimating; }
            set
            {
                _isAnimating = value;
                NotifyOfPropertyChange(() => IsAnimating);
            }
        }

        #endregion

        public void Start()
        {
            GripItService.Start();
            Started = true;
        }

        public void Stop()
        {
            GripItService.Stop();
            Started = false;
        }

        public void Exit()
        {
            _disposable.Dispose();
            Application.Current.Shutdown();
        }
    }
}