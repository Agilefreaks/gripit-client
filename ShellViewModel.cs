using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;

namespace gripit_client {
    public class ShellViewModel : Screen, IShell
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
        private IDisposable _animationSchedulerSubscription;
        private Line _line;
        private const int _domainForValues = 10000;
        private const int _centerOffset = 5000;
        private const int _viewCenter = 150;
        private const int _maxAmplitude = 150;
        private const int AnimationDuration = 200;
        private const int RefreshInterval = 300;

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
            _animationSchedulerSubscription = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(RefreshInterval))
                .Do(_ => Application.Current.Dispatcher.Invoke(Animate))
                .Select(_ => Observable.Timer(TimeSpan.FromMilliseconds(AnimationDuration)))
                .Switch()
                .Subscribe();
        }

        public void Stop()
        {
            GripItService.Stop();
            Started = false;
            _animationSchedulerSubscription.Dispose();
        }

        public void Exit()
        {
            _disposable.Dispose();
            Application.Current.Shutdown();
        }

        protected override void OnViewAttached(object view, object context)
        {
            var shellView = (ShellView)view;
            _line = (Line)shellView.FindName("Vector");
            Animate();
        }

        private void Animate()
        {
            var storyboard = new Storyboard();
            var anim1 = new DoubleAnimation(_line.X2, ViewX, TimeSpan.FromMilliseconds(AnimationDuration));
            Storyboard.SetTarget(anim1, _line);
            Storyboard.SetTargetProperty(anim1, new PropertyPath(Line.X2Property));
            var anim2 = new DoubleAnimation(_line.Y2, ViewY, TimeSpan.FromMilliseconds(AnimationDuration));
            Storyboard.SetTarget(anim2, _line);
            Storyboard.SetTargetProperty(anim2, new PropertyPath(Line.Y2Property));
            storyboard.Children.Add(anim1);
            storyboard.Children.Add(anim2);
            storyboard.Begin();
        }
    }
}