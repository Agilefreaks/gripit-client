using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Linq;

namespace gripit_client
{
    public class GripItService : IGripItService
    {
        private IDisposable _observer;
        private const string PipeName = "gripit";
        private StreamReader _reader;
        private IDisposable _dataSubscription;
        private NamedPipeClientStream _client;

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
            _client = new NamedPipeClientStream(PipeName);
            _client.Connect();
            _reader = new StreamReader(_client);

            _dataSubscription = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(20))
                .Select(_ => ReadData())
                .Subscribe();
        }

        public void Stop()
        {
            _dataSubscription.Dispose();
            _client.Dispose();
            _reader.Close();
        }

        private string ReadData()
        {
            return _reader.ReadLine();
        }
    }
}