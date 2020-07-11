using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Worker.Schedule;

namespace Fuji.Server.Services
{
    public class NotificationsService : Notifications.NotificationsBase
    {
        private readonly ILogger<NotificationsService> _logger;

        public NotificationsService(ILogger<NotificationsService> logger)
        {
            _logger = logger;
        }

        public override async Task Listen(IAsyncStreamReader<ListenRequest> requestStream,
            IServerStreamWriter<ListenResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                _logger.LogInformation($"Received request for {request.Count} messages");
                for (var i = 0; i < request.Count; i++)
                {
                    await responseStream.WriteAsync(new ListenResponse {Payload = i});
                    _logger.LogInformation($"Writing {i+1} of {request.Count} messages");
                }

                _logger.LogInformation("Listening...");
            }
        }
    }
}