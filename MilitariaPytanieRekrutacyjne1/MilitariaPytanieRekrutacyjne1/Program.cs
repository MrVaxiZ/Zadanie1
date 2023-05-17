using MilitariaPytanieRekrutacyjne1;

class Program
{
    public static void Main(string[] args)
    {
        //Tworzenie obiektu wiadomosci
        var email = new EmailMessage
        {
            Subject = "Przyklad",
            Body = "To jest przykladowe body wiadomosci",
            Recipient = "exmple@example.com",
            Sender = "exmple@example.com"
        };

        //Wysylanie wiadomosci do RabbitMQ za pomoca producenta
        var producer = new EmailProducer("localhost", "email_queue");
        producer.SendMessage(email);

        //Tworzenie obiektu konsumenta i rozpoczęcie odbierania wiadomości
        var consumer = new EmailConsumer("localhost", "email_queue");
        consumer.StartConsuming();

        Console.WriteLine("Wcisnij jakikolwiek przycisk by wyjsc...");
        Console.ReadLine();
    }
}