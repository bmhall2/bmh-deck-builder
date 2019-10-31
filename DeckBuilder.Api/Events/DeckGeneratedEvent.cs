using DeckBuilder.Api.Models;

namespace DeckBuilder.Api.Events
{
    public class DeckCreatedEvent
    {
        public Deck Deck { get; set; }
    }
}
