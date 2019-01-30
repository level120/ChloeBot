using cube.Core;
using System;

namespace Core
{
    class Program
    {
        static void Main(string[] args) => new DiscordBot().RunBotAsync().GetAwaiter().GetResult();
    }
}
