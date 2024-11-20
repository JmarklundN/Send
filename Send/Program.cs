using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace Send
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest"
            };

            //var factory = new ConnectionFactory { HostName = "rabbitmq",
            //    UserName = "user", Password = "password" };                                  // create a connection factory for RabbitMQ

            using var connection = await Task.Run(() => factory.CreateConnection());         // establish an asynchronous connection
            using var channel = connection.CreateModel();

            await Task.Run(() =>
            {                                                                                //Declare the queue
                channel.QueueDeclare(queue: "hello", durable: false,
                    exclusive: false, autoDelete: false, arguments: null);
            });

            const string message = "Hello World!";                                           // prepare and send the message
            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() =>
            {
                channel.BasicPublish(exchange: string.Empty, routingKey: "hello",            // Publish the message to the queue asynchronously
                    basicProperties: null, body: body);
            });                         


            Console.WriteLine($" [x] Sent: {message}");
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
