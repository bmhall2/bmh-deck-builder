using System.Collections.Generic;
using EventStore.ClientAPI;

namespace DeckBuilder.Api.EventProcessors
{
    interface IEventsProcessor<T>
    {
        T Execute(List<ResolvedEvent> events);
    }
}
