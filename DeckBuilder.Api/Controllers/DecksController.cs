using System;
using DeckBuilder.Api.EventProcessors;
using DeckBuilder.Api.Events;
using DeckBuilder.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeckBuilder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly IEventStoreManager _eventStoreManager;

        public DecksController(IEventStoreManager eventStoreManager)
        {
            _eventStoreManager = eventStoreManager;
        }

        // POST api/deck
        [HttpPost]
        public string Create()
        {
            var deckGenerator = new DeckGenerator();
            var deckCreatedEvent = new DeckCreatedEvent
            {
                Deck = deckGenerator.Generate()
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
            return new DeckEventsProcessor().Execute(streamEvents);
        }
    }
}