using ChloeBot.Core;

namespace ChloeBot
{
    public class DiscordBotMain
    {
        static void Main(string[] args) =>
            new DiscordBot().RunBotAsync().GetAwaiter().GetResult();
    }
}
