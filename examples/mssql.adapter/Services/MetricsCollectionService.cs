using Microsoft.Extensions.Options;
using mssql.adapter.Metrics;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.Text;

namespace mssql.adapter
{
    public class MetricsCollectionService : EventListener, IHostedService
    {
        private ConcurrentBag<EventCounterData> _eventsData = new();
        private long _lastEventTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private readonly int _eventInterval;
        private readonly int _eventLogDurationThreshold;
        private readonly ILogger<MetricsCollectionService> _logger;

        public MetricsCollectionService(IOptions<DalServiceOptions> options, ILogger<MetricsCollectionService> logger)
        {
            _eventInterval = options.Value.MetricsLogInterval;
            _eventLogDurationThreshold = options.Value.MetricsLogDurationThreshold;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (!eventSource.Name.Equals("mssql.utils.DataReader"))
            {
                return;
            }

            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All, new Dictionary<string, string?>
            {
                {"EventCounterIntervalSec", _eventInterval.ToString()}
            });
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= _lastEventTime + _eventInterval)
            {
                _lastEventTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var eventsData = Interlocked.Exchange(ref _eventsData, new ConcurrentBag<EventCounterData>());

                PrintToConsole(eventsData);
            }

            var counterData = eventData.ToEventCounterData();

            if (counterData == null)
            {
                _logger.LogWarning("Event received: {eventId} - {eventName}", eventData.EventId, eventData.EventName);
            }
            else if (counterData.Count == 0)
            {
                return;
            }
            else
            {
                _eventsData.Add(counterData);
            }
        }

        private void PrintToConsole(ConcurrentBag<EventCounterData> eventsData)
        {
            var output = new StringBuilder();
            var slowCalls = eventsData.Where(x => x.Max >= _eventLogDurationThreshold).OrderByDescending(x => x.Mean);

            foreach (var eventData in slowCalls)
            {
                output.AppendLine($"{eventData.Name,-70}{eventData.Mean,-10:0}{eventData.Min,-10:0}{eventData.Max,-10:0}{eventData.Count,-10}");
            }

            if (output.Length > 0)
            {
                _logger.LogInformation(
                    "Top slow calls in the last {eventInterval} seconds:\n\n{procedureTitle}{meanTitle}{minTitle}{maxTitle}{countTitle}\n{output}",
                    _eventInterval,
                    "Stored Procedure".PadRight(70),
                    "Mean".PadRight(10),
                    "Min".PadRight(10),
                    "Max".PadRight(10),
                    "Count".PadRight(10),
                    output
                );
            }
        }
    }
}