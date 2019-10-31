using System.Collections.Generic;

namespace DeckBuilder.Api.Models
{
    public class Deck
    {
        public List<string> DrawPile { get; set; }
        public List<string> DiscardPile { get; set; }
    }
}
