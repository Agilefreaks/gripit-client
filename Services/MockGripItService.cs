using System;
using System.Reactive.Linq;

namespace gripit_client
{
    public class MockGripItService : IGripItService
    {
        private IDisposable _observer;

        public event NewForceProjectionEventHandler onNewForceProjection;

        public IObservable<ForceProjection> OnNewForceProjection
        {
            get
            {
                return Observable.FromEventPattern<NewForceProjectionEventHandler, ForceProjection>(
                    h => onNewForceProjection += h,
                    h => onNewForceProjection -= h)
                    .Select(x => x.EventArgs);
            }
        }

        public void Start()
        {
            var random = new Random();
            _observer = Observable
                .Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(200))
                .Subscribe(l => onNewForceProjection(this, new ForceProjection(random.Next(10000) - 5000, random.Next(10000) - 5000)));
        }

        public void Stop()
        {
            _observer.Dispose();
        }

    }
}