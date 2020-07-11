using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Worker.Schedule;

namespace Fuji
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Notifications.NotificationsClient(channel);
            using var call = client.Listen();
            var _ = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var note = call.ResponseStream.Current;
                    Console.WriteLine("Received " + note);
                }
            });
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out var buffer))
                {
                   await call.RequestStream.WriteAsync(new ListenRequest {Count = buffer}); 
                }
                else
                {
                    await call.RequestStream.WriteAsync(new ListenRequest {Count = 10}); 
                }
            }
        }
    }
}