using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitariaPytanieRekrutacyjne1
{
    /**
        Reprezentuje wiadomość e-mail.
    */
    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
    }
}
