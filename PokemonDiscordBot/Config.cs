using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PokemonDiscordBot
{
    //some variables I'm using for the config read/write thing
    //There's probably a better option than what I'm doing here, but anything else I tried broke everything and made many red squiggly lines, so here we are
    class Config
    {
        public static Config CurrentGlobal { get; private set; }
        public string BotAPIToken { get; private set; }
        public UInt16 HoldLength  { get; set; }
        public UInt16 ScreenshotDelay { get; set; }
        public string ScreenshotPath { get; private set; }

        public Config() 
        {
            if (Parse())
                return;

            if (ParseLegacy())
                return;

            FirstTimeBotConfig();

            Console.WriteLine("Configuration complete! Starting bot.");

            CurrentGlobal = this;
        }

        public Config(string token, UInt16 length, UInt16 delay, string screenshot_path)
        {
            BotAPIToken = token;
            HoldLength = length;
            ScreenshotDelay = delay;
            ScreenshotPath = screenshot_path;

            CurrentGlobal = this;
        }

        // TODO: json config.
        public bool Parse() 
        {
            if (!File.Exists("bot.json"))
                return false;
            return false;
        }

        [Obsolete("ParseLegacy is an un-recommended way of parsing config. please use implement and use Parse instead.")]
        public bool ParseLegacy() 
        {
            if (!File.Exists("bot.cfg"))
                return false;

            Console.WriteLine("Legacy Config file found!");

            while (true)
            {
                Console.WriteLine("Configure bot? (Y/N)");
                string garbage = Console.ReadLine().Trim().ToLower();
                if (garbage == "y")
                {
                    FirstTimeBotConfig();
                    break;
                }
                else if (garbage == "n")
                {
                    //Sets various public variables to values previously set in bot.cfg in the root directory
                    string[] ConfigTemp = File.ReadAllLines("bot.cfg");
                    BotAPIToken = ConfigTemp[0];
                    HoldLength = UInt16.Parse(ConfigTemp[1]);
                    ScreenshotDelay = UInt16.Parse(ConfigTemp[2]);
                    ScreenshotPath = ConfigTemp[3];
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter a valid input");
                }
            }

            return true;
        }

        public void FirstTimeBotConfig()
        {
            Console.WriteLine("Please enter your bot token.");
            Console.WriteLine("You can get this from the discord developer portal, at https://discord.com/developers/applications");

            BotAPIToken = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Please enter the amount of time in milliseconds buttons should be held down for.");
            Console.WriteLine("It should be long enough that a directional input makes the player walk one step,");
            Console.WriteLine("but not long enough to make them walk more than one.");
            Console.WriteLine("Bear in mind that if the button isn't held for long enough, the player will just turn on the spot instead of walking.");

            HoldLength = UInt16.Parse(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine("Please enter the amount of time in milliseconds to wait before taking a screenshot.");
            Console.WriteLine("This depends on the speed that your emulator is running at, but I personally recommend however long it takes for at least one action of a battle to happen.");

            ScreenshotDelay = UInt16.Parse(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine("Please enter the filepath that the emulator places the screenshots.");
            ScreenshotPath = Console.ReadLine();

            Console.WriteLine();

            string[] Config = { BotAPIToken, HoldLength.ToString(), ScreenshotDelay.ToString(), ScreenshotPath };
            File.WriteAllLines("bot.cfg", Config);
        }
    }
}
