namespace WorldCities.Services
{
    public class SendGridEmailSenderOptions
    {
        public string ApiKey { get; set; }

        public string Sender_Email { get; set; }

        public string Sender_Name { get; set; }
    }
}