using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DeckBuilder.Api.Projections;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using ILogger = EventStore.ClientAPI.ILogger;

namespace DeckBuilder.Api
{
    public interface IEventStoreManager
    {
        List<ResolvedEvent> GetResolvedEvents(string streamId);
        void AppendEventToStream(string streamId, string eventType, object @event);
        CardDrawnCounter GetCardDrawnCounterProjection();
    }

    public class EventStoreManager : IEventStoreManager
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger _logger;

        public EventStoreManager()
        {
            _logger = new ConsoleLogger();
            _eventStoreConnection =
                EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "DecksConnection");
            _eventStoreConnection.ConnectAsync().Wait();
        }

        public List<ResolvedEvent> GetResolvedEvents(string streamId)
        {
            var streamEvents = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            long nextSliceStart = 0;
            do
            {
                currentSlice =
                    _eventStoreConnection.ReadStreamEventsForwardAsync(streamId, nextSliceStart,
                            200, false)
                        .Result;

                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            return streamEvents;
        }

        public void AppendEventToStream(string streamId, string eventType, object @event)
        {
            var data = new EventData(Guid.NewGuid(), eventType, true, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)), null);
            _eventStoreConnection.AppendToStreamAsync(streamId, ExpectedVersion.Any, data).Wait();
        }

        public CardDrawnCounter GetCardDrawnCounterProjection()
        {
            var projectionsManager = new ProjectionsManager(_logger, new DnsEndPoint("127.0.0.1", 2113), new TimeSpan(0, 1, 0));
            var result = projectionsManager.GetStateAsync("card-drawn-counter", new UserCredentials("admin", "changeit")).Result;

            return JsonConvert.DeserializeObject<CardDrawnCounter>(result);
        }
    }
}
