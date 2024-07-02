namespace FlowerSellerTgBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") );
        }
    }
}
