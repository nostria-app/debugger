using System.Buffers;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Newtonsoft.Json;
using Nostr.Client.Communicator;
using Nostr.Client.Json;
using Nostr.Client.Requests;
using Nostr.Client.Responses;
using Websocket.Client;

namespace Nostr.Client.Tests.Fakes
{
    public class NostrFakeCommunicator : INostrCommunicator
    {
        private readonly Subject<ResponseMessage> _messageSubject = new();
        private readonly List<string> _sentMessages = new();

        public IReadOnlyCollection<string> SentMessages => _sentMessages;

        public NostrFakeCommunicator()
        {
            Name = $"fake-{Guid.NewGuid()}";
        }

        public void Dispose()
        {
            IsStarted = false;
            IsRunning = false;
        }

        public Task Start()
        {
            IsStarted = true;
            IsRunning = true;
            return Task.CompletedTask;
        }

        public Task StartOrFail()
        {
            IsStarted = true;
            IsRunning = true;
            return Task.CompletedTask;
        }

        public Task<bool> Stop(WebSocketCloseStatus status, string statusDescription)
        {
            IsStarted = false;
            IsRunning = false;
            return Task.FromResult(true);
        }

        public Task<bool> StopOrFail(WebSocketCloseStatus status, string statusDescription)
        {
            IsStarted = false;
            IsRunning = false;
            return Task.FromResult(true);
        }

        public bool Send(string message)
        {
            _sentMessages.Add(message);

            var parsed = JsonConvert.DeserializeObject<NostrEventRequest>(message, NostrSerializer.Settings);
            var response = new NostrEventResponse
            {
                MessageType = parsed?.Type,
                Subscription = "fake-subscription",
                Event = parsed?.Event
            };
            var responseParsed = JsonConvert.SerializeObject(response, NostrSerializer.Settings);

            _messageSubject.OnNext(ResponseMessage.TextMessage(responseParsed));
            return true;
        }

        public bool Send(byte[] message)
        {
            throw new NotImplementedException();
        }

        public bool Send(ArraySegment<byte> message)
        {
            throw new NotImplementedException();
        }

        public bool Send(ReadOnlySequence<byte> message)
        {
            throw new NotImplementedException();
        }

        public Task SendInstant(string message)
        {
            return Task.CompletedTask;
        }

        public Task SendInstant(byte[] message)
        {
            return Task.CompletedTask;
        }

        public bool SendAsText(byte[] message)
        {
            throw new NotImplementedException();
        }

        public bool SendAsText(ArraySegment<byte> message)
        {
            throw new NotImplementedException();
        }

        public bool SendAsText(ReadOnlySequence<byte> message)
        {
            throw new NotImplementedException();
        }

        public Task Reconnect()
        {
            return Task.CompletedTask;
        }

        public Task ReconnectOrFail()
        {
            return Task.CompletedTask;
        }

        public void StreamFakeMessage(ResponseMessage message)
        {
            _messageSubject.OnNext(message);
        }

        public Uri Url { get; set; } = new("wss://fake.local");
        public IObservable<ResponseMessage> MessageReceived => _messageSubject.AsObservable();
        public IObservable<ReconnectionInfo> ReconnectionHappened => Observable.Empty<ReconnectionInfo>();
        public IObservable<DisconnectionInfo> DisconnectionHappened => Observable.Empty<DisconnectionInfo>();
        public TimeSpan ConnectTimeout { get; set; }
        public TimeSpan? ReconnectTimeout { get; set; }
        public TimeSpan? ErrorReconnectTimeout { get; set; }
        public TimeSpan? LostReconnectTimeout { get; set; }
        public string Name { get; set; }
        public bool IsStarted { get; private set; }
        public bool IsRunning { get; private set; }
        public bool TextSenderRunning { get; set; }
        public bool BinarySenderRunning { get; set; }
        public bool IsInsideLock { get; set; }
        public bool IsReconnectionEnabled { get; set; }
        public bool IsTextMessageConversionEnabled { get; set; }
        public bool IsStreamDisposedAutomatically { get; set; }
        public ClientWebSocket NativeClient { get; } = null!;
        public Encoding MessageEncoding { get; set; } = Encoding.UTF8;
    }
}
