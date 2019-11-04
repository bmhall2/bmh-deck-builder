using System;
using DeckBuilder.Api.EventProcessors;
using DeckBuilder.Api.Events;
using DeckBuilder.Api.Models;
using DeckBuilder.Api.Projections;
using Microsoft.AspNetCore.Mvc;

namespace DeckBuilder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly IEventStoreManager _eventStoreManager;
        private readonly IDeckGenerator _deckGenerator;
        private readonly IDeckEventsProcessor _deckEventsProcessor;

        public DecksController(IEventStoreManager eventStoreManager, IDeckGenerator deckGenerator, IDeckEventsProcessor deckEventsProcessor)
        {
            _eventStoreManager = eventStoreManager;
            _deckGenerator = deckGenerator;
            _deckEventsProcessor = deckEventsProcessor;
        }

        // POST api/deck
        [HttpPost]
        public string Create()
        {
            var deckCreatedEvent = new DeckCreatedEvent
            {
                Deck = _deckGenerator.Generate()
            };

            var streamId = Guid.NewGuid().ToString();
            _eventStoreManager.AppendEventToStream(streamId, "DeckCreated", deckCreatedEvent);

            return streamId;
        }

        [HttpPost("/api/[controller]/{streamId}/drawcard")]
        public void Draw(string streamId)
        {
            _eventStoreManager.AppendEventToStream(streamId, "CardDrawn", new CardDrawnEvent());
        }

        [HttpGet("/api/[controller]/{streamId}")]
        public Deck Get(string streamId)
        {
            var streamEvents = _eventStoreManager.GetResolvedEvents(streamId);
            return _deckEventsProcessor.Execute(streamEvents);
        }

        [HttpGet("/api/[controller]/stats")]
        public CardDrawnCounter GetStats()
        {
            return _eventStoreManager.GetCardDrawnCounterProjection();
        }
    }
}