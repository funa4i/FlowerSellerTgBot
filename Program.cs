using System.Text;
using Telegram.Bot.Types;


namespace FlowerSellerTgBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string tocken;
            // string tocken = Environment.GetEnvironmentVariable("TELEGRAM_TOCKEN_BOT");
            using (StreamReader streamReader = new StreamReader("tocken.txt", Encoding.UTF8))
            {
              tocken = streamReader.ReadToEnd();
            }
            Console.WriteLine(tocken);
            Console.ReadLine();
        }
    }
}
