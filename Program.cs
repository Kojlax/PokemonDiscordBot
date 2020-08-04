using System;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Runtime.InteropServices;
using System.IO;

namespace PokemonBot
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;

        //This is another blackbox
        //I don't know how this works, I don't know why it works
        //All I know is that if I put this code in 12 hours ago this project would have finished 11 hours ago
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag


        //some variables I'm using for the config read/write thing
        //There's probably a better option than what I'm doing here, but anything else I tried broke everything and made many red squiggly lines, so here we are
        public static string InputToken;
        public static UInt16 HoldLength;
        public static UInt16 ScreenshotDelay;
        public static string ScreenshotPath;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public class MyCommands
        //All commands go here!
        {
            [Command("a")]
            public async Task APress(CommandContext ctx)
            {
                keybd_event(0x80, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f17
                Thread.Sleep(HoldLength);
                keybd_event(0x80, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input A");
                await ImgReturn(ctx);
            }
            [Command("b")]
            public async Task BPress(CommandContext ctx)
            {
                keybd_event(0x81, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f18
                Thread.Sleep(HoldLength);
                keybd_event(0x81, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input B");
                await ImgReturn(ctx);
            }
            [Command("start")]
            public async Task StartPress(CommandContext ctx)
            {
                keybd_event(0x83, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f20
                Thread.Sleep(HoldLength);
                keybd_event(0x83, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input start");
                await ImgReturn(ctx);
            }
            [Command("select")]
            public async Task SelectPress(CommandContext ctx)
            {
                keybd_event(0x82, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f19
                Thread.Sleep(HoldLength);
                keybd_event(0x82, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input select");
                await ImgReturn(ctx);
            }
            [Command("up")]
            public async Task UpPress(CommandContext ctx)
            {
                keybd_event(0x7E, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f15
                Thread.Sleep(HoldLength);
                keybd_event(0x7E, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input Up");
                await ImgReturn(ctx);
            }
            [Command("down")]
            public async Task DownPress(CommandContext ctx)
            {
                keybd_event(0x7F, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f16
                Thread.Sleep(HoldLength);
                keybd_event(0x7F, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input Down");
                await ImgReturn(ctx);
            }
            [Command("left")]
            public async Task LeftPress(CommandContext ctx)
            {
                keybd_event(0x7D, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f14
                Thread.Sleep(HoldLength);
                keybd_event(0x7D, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input Left");
                await ImgReturn(ctx);
            }
            [Command("right")]
            public async Task RightPress(CommandContext ctx)
            {
                keybd_event(0x7C, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f13
                Thread.Sleep(HoldLength);
                keybd_event(0x7C, 0, KEYEVENTF_KEYUP, 0);
                Console.WriteLine("Recieved Input Right");
                await ImgReturn(ctx);
            }
        }

        public static async Task ImgReturn(CommandContext ctx)
        {
            //Waits for game to catch up, presses the F21 key, then gives the emulator time to screenshot
            Thread.Sleep(ScreenshotDelay);
            keybd_event(0x84, 0, KEYEVENTF_EXTENDEDKEY, 0); //presses f21
            Thread.Sleep(HoldLength);
            keybd_event(0x84, 0, KEYEVENTF_KEYUP, 0);
            Thread.Sleep(100);  //This time is to give the emulator time to take a screenshot. Feel free to increase/decrease depending on computer speed

            //Takes most recent image in scrnshot folder, saves it as a png, scaling it up, and sending it off
            string botimg = ScreenshotPath;
            string[] initlist = System.IO.Directory.GetFiles(botimg);
            string currentimg = initlist[initlist.Length - 1];
            Console.WriteLine(currentimg);
            SaveBmpAsPNG(currentimg);

            //Just copying this over from my SaveImageAsPNG function
            //this still feels just as weird and shitty as it did before
            //idk if there's a better way to do this without declaring the variable as public, or if I should be declaring it as public in the first place
            //but hey, if it works it works
            string temp = Path.GetTempPath();
            temp += "state.png";

            await ctx.RespondWithFileAsync(temp);
        }

        //I'm going to be honest, this code was almost exclusively written by mpen on stackoverflow
        //This was a gigantic headache that could have all been avoided if bgb just outputted a higher res image
        //but it doesn't
        //Fuck me
        //The only changes I made to it were to disable smoothing and pixel offset, 
        //and to set interpolation to nearest neighbor
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.PixelOffsetMode = PixelOffsetMode.None;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private static void SaveBmpAsPNG(string currentimg)
        //saves bmp as png (duh)
        {

            Bitmap bmp1 = new Bitmap(currentimg);

            Bitmap resizedbmp = ResizeImage(bmp1, 800, 720);

            //this bit of code feels shit but I guess it works
            //I really haven't done nearly as much as I'd like to involving file stuff
            string temp = Path.GetTempPath();
            temp += "state.png";

            resizedbmp.Save(temp, System.Drawing.Imaging.ImageFormat.Png);
            resizedbmp.Dispose();
            bmp1.Dispose();
        }

        private static void BotConfig()
        {
            Console.WriteLine("Please enter your bot token.");
            Console.WriteLine("You can get this from the discord developer portal, at https://discord.com/developers/applications");

            InputToken = Console.ReadLine();

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

            string[] Config = { InputToken, HoldLength.ToString(), ScreenshotDelay.ToString(), ScreenshotPath };

            File.WriteAllLines("bot.cfg", Config);

            Console.WriteLine("Configuration complete! Starting bot.");
        }

        static async Task MainAsync(string[] args)
        {
            //This is gonna be the section of code that does all the stuff regarding the config file

            //The plan currently is to check if a config file exists, and if not, 
            //ask the user for input of discord token, and wait times between both inputs and screenshots
            //and if so, read that input from the config and assign them to public variables
            //I can't really be fucked at the moment to deal with the possibility that the user edits said config file
            //I'm tired

            if (File.Exists("bot.cfg"))
            {
                Console.WriteLine("Config file found!");

                while (true)
                {
                    Console.WriteLine("Configure bot? (Y/N)");
                    string garbage = Console.ReadLine().Trim().ToLower();
                    if (garbage == "y")
                    {
                        BotConfig();
                        break;
                    }
                    else if (garbage == "n")
                    {
                        //Sets various public variables to values previously set in bot.cfg in the root directory
                        string[] ConfigTemp = File.ReadAllLines("bot.cfg");
                        InputToken = ConfigTemp[0];
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
            }
            else
            {
                Console.WriteLine("No config file detected!");
                BotConfig();
            }

            //Creates a connection with discord using Token

            //TODO: read token from file, making code distributables
            //(If you're reading this, I didn't do it because I'm lazy)
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = InputToken, //Replace with your bot token
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            //Initializes command parsing, and also sets the prefix
            //You can change the prefix to whatever you want ^_^
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<MyCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}