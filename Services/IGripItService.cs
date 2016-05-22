using System;

namespace gripit_client
{
    public delegate void NewForceProjectionEventHandler(object source, ForceProjection e);

    public interface IGripItService
    {
        void Start();

        void Stop();

        IObservable<ForceProjection> OnNewForceProjection { get; }
    }
}