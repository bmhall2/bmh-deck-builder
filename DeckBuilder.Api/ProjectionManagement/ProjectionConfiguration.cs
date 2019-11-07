using System.Collections.Generic;

namespace DeckBuilder.Api.ProjectionManagement
{
    public static class ProjectionConfiguration
    {
        public static Dictionary<string, string> ContinuousProjections { get; }

        static ProjectionConfiguration()
        {
            ContinuousProjections = new Dictionary<string, string>()
            {
                { "card-drawn-counter", "card-drawn-counter.json" },
                { "card-drawn-partition-counter", "card-drawn-partition-counter.json" }
            };
        }
    }
}
