using ChloeBot.Core;

namespace ChloeBot
{
    /// <summary>
    /// ChloeBot의 시작지점
    /// </summary>
    public class DiscordBotMain
    {
        /// <summary>
        /// ChloeBot Main function
        /// </summary>
        /// <param name="args">Not used</param>
        public static void Main(string[] args) =>
            new DiscordBot().RunBotAsync().GetAwaiter().GetResult();
    }
}
