using System;
using System.Collections.Generic;
using DeckBuilder.Api.Models;

namespace DeckBuilder.Api
{
    public interface IDeckGenerator
    {
        Deck Generate();
    }

    public class DeckGenerator : IDeckGenerator
    {
        public Deck Generate()
        {
            var suits = new List<string> { "H", "D", "C", "S" };
            var values = new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            var allCards = new List<string>();
            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    allCards.Add(suit + value);
                }
            }

            var random = new Random();
            var drawPile = new List<string>();
            while (allCards.Count > 0)
            {
                var index = random.Next(0, allCards.Count - 1);
                drawPile.Add(allCards[index]);
                allCards.RemoveAt(index);
            }

            return new Deck
            {
                DrawPile = drawPile,
                DiscardPile = new List<string>()
            };
        }
    }
}
