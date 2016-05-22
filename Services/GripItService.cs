using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace gripit_client
{
    public class GripItService : IGripItService
    {
        private const string PipeName = "gripit";
        private StreamReader _reader;
        private NamedPipeClientStream _client;
        private readonly Subject<ForceProjection> _forceProjectionSubject;
        private IDisposable _forceProjectionSubscription;

        public IObservable<ForceProjection> OnNewForceProjection { get; private set; }

        public GripItService()
        {
            _forceProjectionSubject = new Subject<ForceProjection>();
            OnNewForceProjection = _forceProjectionSubject;
        }

        public void Start()
        {
            _client = new NamedPipeClientStream(PipeName);
            _client.Connect();
            _reader = new StreamReader(_client);
            _forceProjectionSubscription = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100))
                .Select(_ => new ForceProjection(ReadData()))
                .Do(forceProjection => _forceProjectionSubject.OnNext(forceProjection))
                .Subscribe();
        }

        public void Stop()
        {
            _forceProjectionSubscription?.Dispose();
            _reader.Close();
            _client.Dispose();
        }

        private string ReadData()
        {
            return _reader.ReadLine();
        }
    }
}