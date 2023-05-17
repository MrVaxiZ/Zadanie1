using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MilitariaPytanieRekrutacyjne1
{
    /**
        Odpowiada za odbieranie wiadomości z RabbitMQ i wysyłanie ich za pomocą protokołu SMTP.
    */
    public class EmailConsumer
    {
        private readonly IModel _channel;

        /**
        * Konstruktor klasy EmailConsumer
        *
        * hostname - nazwa hosta RabbitMQ
        * queueName - nazwa kolejki RabbitMQ
        */
        public EmailConsumer(string hostname, string queueName)
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
         * Rozpoczyna odbieranie wiadomości z RabbitMQ i przetwarza je
         */
        public void StartConsuming()
        {
            // Utworzenie obiektu konsumenta RabbitMQ
            var consumer = new EventingBasicConsumer(_channel);

            // Zdarzenie wywoływane po otrzymaniu wiadomości
            consumer.Received += (model, args) =>
            {
                // Pobranie treści wiadomości
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Wyświetlenie otrzymanej wiadomości
                Console.WriteLine("Otrzymano wiadomość od RabbitMQ: {0}", message);

                // Deserializacja wiadomości do obiektu EmailMessage
                var email = JsonConvert.DeserializeObject<EmailMessage>(message);

                // Wysłanie wiadomości e-mail
                SendEmail(email);

                // Rozpoczęcie ponownego odbierania wiadomości
                _channel.BasicConsume(
                    queue: "email_queue",
                    autoAck: false,
                    consumer: consumer
                );

                // Wyświetlenie informacji o rozpoczęciu odbierania wiadomości
                Console.WriteLine("Consumer został wystartowany. Czeka na wiadomości...");
            };
        }

        /**
         * Wysyła wiadomość e-mail za pomocą protokołu SMTP
         *
         * email - obiekt EmailMessage zawierający dane wiadomości e-mail
         */
        private void SendEmail(EmailMessage email)
        {
            // Utworzenie klienta SMTP
            using (var smtpClient = new SmtpClient("smtp.client"))
            {
                // Utworzenie obiektu MailMessage
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(email.Sender),
                    Subject = email.Body,
                    IsBodyHtml = true
                };

                // Dodanie odbiorcy do wiadomości
                mailMessage.To.Add(email.Recipient);

                // Wysłanie wiadomości
                smtpClient.Send(mailMessage);

                // Wyświetlenie informacji o wysłaniu wiadomości
                Console.WriteLine("E-mail został wysłany do: {0}", email.Recipient);
            }
        }
    }
}
