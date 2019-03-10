using cube.Core;

namespace Core
{
    class Program
    {
        static void Main(string[] args) => new DiscordBot().RunBotAsync().GetAwaiter().GetResult();
    }
}
