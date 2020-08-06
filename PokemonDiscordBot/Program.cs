using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace PokemonDiscordBot
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;

        //This is gonna be the section of code that does all the stuff regarding the config file
        //The plan currently is to check if a config file exists, and if not, 
        //ask the user for input of discord token, and wait times between both inputs and screenshots
        //and if so, read that input from the config and assign them to public variables
        //I can't really be fucked at the moment to deal with the possibility that the user edits said config file
        //I'm tired
        public Program()
        {
            new Config();
        }

        static void Main(string[] args)
        {
            new Program().MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        async Task MainAsync()
        {
            //Creates a connection with discord using Token

            //TODO: read token from file, making code distributables
            //(If you're reading this, I didn't do it because I'm lazy)
            discord = new DiscordClient(
                new DiscordConfiguration
                {
                    Token = Config.CurrentGlobal.BotAPIToken, //Replace with your bot token
                    TokenType = TokenType.Bot,
                    UseInternalLogHandler = true,
                    LogLevel = LogLevel.Debug
                }
            );

            //Initializes command parsing, and also sets the prefix
            //You can change the prefix to whatever you want ^_^
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<Commands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
