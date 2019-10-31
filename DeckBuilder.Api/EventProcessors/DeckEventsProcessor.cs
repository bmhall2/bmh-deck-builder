using System.Collections.Generic;
using DeckBuilder.Api.Events;
using DeckBuilder.Api.Models;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace DeckBuilder.Api.EventProcessors
{
    public interface IDeckEventsProcessor
    {
        Deck Execute(List<ResolvedEvent> events);
    }

    public class DeckEventsProcessor : IDeckEventsProcessor
    {
        public Deck Execute(List<ResolvedEvent> events)
        {
            Deck deck = new Deck();
            foreach (var @event in events)
            {
                switch (@event.Event.EventType)
                {
                    case "DeckCreated":
                        var deckCreatedEvent =
                            JsonConvert.DeserializeObject<DeckCreatedEvent>(
                                System.Text.Encoding.UTF8.GetString(@event.Event.Data));
                        deck = deckCreatedEvent.Deck;
                        break;
                    case "CardDrawn":
                        var cardDrawnEvent =
                            JsonConvert.DeserializeObject<CardDrawnEvent>(
                                System.Text.Encoding.UTF8.GetString(@event.Event.Data));
                        deck.DiscardPile.Add(deck.DrawPile[0]);
                        deck.DrawPile.RemoveAt(0);
                        break;
                }
            }

            return deck;
        }
    }
}
