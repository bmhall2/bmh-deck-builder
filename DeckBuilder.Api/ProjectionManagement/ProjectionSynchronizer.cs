using System;
using System.IO;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;

namespace DeckBuilder.Api.ProjectionManagement
{
    public class ProjectionSynchronizer
    {
        private readonly ILogger _logger;
        private readonly ProjectionsManager _projectionsManager;

        public ProjectionSynchronizer()
        {
            _logger = new ConsoleLogger();
            _projectionsManager = new ProjectionsManager(_logger, new DnsEndPoint("127.0.0.1", 2113), new TimeSpan(0, 1, 0));
        }

        public void SynchronizeProjections()
        {
            var existingProjections = _projectionsManager.ListContinuousAsync(new UserCredentials("admin", "changeit")).Result;

            foreach (var continuousProjectionConfiguration in ProjectionConfiguration.ContinuousProjections)
            {
                var projectionName = continuousProjectionConfiguration.Key;
                if (!existingProjections.Exists(ep => ep.Name == projectionName))
                {
                    var fileContents = ReadProjectionJsonFile(continuousProjectionConfiguration.Value);
                    _projectionsManager.CreateContinuousAsync(projectionName, fileContents,
                        new UserCredentials("admin", "changeit")).Wait();
                }
            }
        }

        private string ReadProjectionJsonFile(string fileName)
        {
            return File.ReadAllText(Path.Combine("ProjectionManagement\\ProjectionFiles", fileName));
        }
    }
}
