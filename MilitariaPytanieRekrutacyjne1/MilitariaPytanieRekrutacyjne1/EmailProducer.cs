using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MilitariaPytanieRekrutacyjne1
{
    /**
        Odpowiada za serializację i wysyłanie wiadomości e-mail do RabbitMQ.
    */
    public class EmailProducer
    {
        private readonly IModel _channel;

        /**
        * hostname - nazwa hosta RabbitMQ
        * queueName - nazwa kolejki RabbitMQ
        */
        public EmailProducer(string hostname, string queueName)
        {
            // Utworzenie połączenia z RabbitMQ
            var connectionFactory = new ConnectionFactory() { HostName = hostname };
            var connection = connectionFactory.CreateConnection();

            // Utworzenie modelu kanału komunikacyjnego
            _channel = connection.CreateModel();

            // Deklaracja kolejki RabbitMQ
            _channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        /**
         * Serializuje i wysyła wiadomość e-mail do RabbitMQ
         *
         * email - obiekt EmailMessage zawierający dane wiadomości e-mail
         */
        public void SendMessage(EmailMessage email)
        {
            // Serializacja wiadomości do formatu JSON
            var message = JsonConvert.SerializeObject(email);
            var body = Encoding.UTF8.GetBytes(message);

            // Wysłanie wiadomości do RabbitMQ
            _channel.BasicPublish(
                exchange: "",
                routingKey: "email_queue",
                basicProperties: null,
                body: body
            );

            // Wyświetlenie informacji o wysłaniu wiadomości
            Console.WriteLine("Wiadomość została wysłana do RabbitMQ: {0}", message);
        }
    }
}