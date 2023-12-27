// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using MassTransit;

namespace RabitMqTest
{
    class Program
    {
        private IBusControl _bus;

        static void Main(string[] args)
        {
                string jsonFile = "CustomerData.json"; // JSON dosyasının adı
                string content = File.ReadAllText($"../../../{jsonFile}");
                List<CustomerData> dataList = JsonSerializer.Deserialize<List<CustomerData>>(content);
                
                    //channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: dataList);
                Send(dataList);
                    
                
                Console.WriteLine("RabbitMQ tüketiciyi başlattı. Çıkmak için bir tuşa basın.");
                Console.ReadKey();
        }
        public async Task SendMessageAsync(string? destination, object message)
        {
            try
            {
                var sendHandle = await _bus.GetSendEndpoint(new Uri($"queue:{destination}"));
                await sendHandle.Send(message);
            }
            catch (Exception e)
            {
                var json = JsonSerializer.Serialize(message);
                Console.WriteLine($"Error occured while sending message to queue - {destination} : {json} Exception : {e}");
                throw;
            }
        }

        public static async Task Send(object message)
        {
            string rabbitMqUri = "rabbitmq://b-8fc70364-066d-4feb-b95a-d8bbe59dea46.mq.eu-central-1.amazonaws.com:5671/CustomerData";
            string queue = "FullData";
            string userName = "binbin";
            string password = "2vx1sguGbpep";
 
            var bus = Bus.Factory.CreateUsingRabbitMq(factory =>
            {
                factory.Host(rabbitMqUri, configurator =>
                {
                    configurator.Username(userName);
                    configurator.Password(password);
                });
            });
 
            var sendToUri = new Uri($"{rabbitMqUri}/{queue}");
            var endPoint = await bus.GetSendEndpoint(sendToUri);
            await bus.StartAsync();
                // Mesajı gönder
                await endPoint.Send(message);
                Console.WriteLine("Mesaj başarıyla gönderildi. Çıkış yapmak için bir tuşa basın.");
                Console.ReadKey();
        }
    }
}
