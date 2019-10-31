using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace DeckBuilder.Api
{
    public class EventStoreManager
    {
        private readonly IEventStoreConnection _eventStoreConnection;

        public EventStoreManager()
        {
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
    }
}
