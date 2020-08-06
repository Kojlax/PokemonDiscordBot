using System;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace PokemonDiscordBot
{
    //All commands go here!
    public class Commands
    {
        // TODO: use an existing keycodes enum in windows instead.
        public enum KeyCodes : byte
        {
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84
        };

        //This is another blackbox
        //I don't know how this works, I don't know why it works
        //All I know is that if I put this code in 12 hours ago this project would have finished 11 hours ago
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        // key flags. 
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; 
        public const int KEYEVENTF_KEYUP = 0x0002;

        public void KeyPress(KeyCodes keycode) 
        {
            keybd_event((byte)keycode, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(Config.CurrentGlobal.HoldLength);
            keybd_event((byte)keycode, 0, KEYEVENTF_KEYUP, 0);
        }

        public void KeyPress(KeyCodes keycode, UInt16 hold_length)
        {
            keybd_event((byte)keycode, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(hold_length);
            keybd_event((byte)keycode, 0, KEYEVENTF_KEYUP, 0);
        }

        // FIXME: 
        [Command("a")]
        public async Task APress(CommandContext ctx)
        {
            this.KeyPress(KeyCodes.F17);
            Console.WriteLine("Recieved Input A");
            await ImgReturn(ctx);
        }

        [Command("b")]
        public async Task BPress(CommandContext ctx)
        {
            KeyPress(KeyCodes.F18);
            Console.WriteLine("Recieved Input B");
            await ImgReturn(ctx);
        }

        [Command("start")]
        public async Task StartPress(CommandContext ctx)
        {
            KeyPress(KeyCodes.F20);
            Console.WriteLine("Recieved Input start");
            await ImgReturn(ctx);
        }

        [Command("select")]
        public async Task SelectPress(CommandContext ctx)
        {
            KeyPress(KeyCodes.F19);
            Console.WriteLine("Recieved Input select");
            await ImgReturn(ctx);
        }

        [Command("up")]
        public async Task UpPress(CommandContext ctx)
        {
            KeyPress(KeyCodes.F15);
            Console.WriteLine("Recieved Input Up");
            await ImgReturn(ctx);
        }

        [Command("down")]
        public async Task DownPress(CommandContext ctx)
        {
            KeyPress(KeyCodes.F16);
            Console.WriteLine("Recieved Input Down");
            await ImgReturn(ctx);
        }

        [Command("left")]
        public async Task LeftPress(CommandContext ctx)
        {
            KeyPress(KeyCodes.F14);
            Console.WriteLine("Recieved Input Left");
            await ImgReturn(ctx);
        }

        [Command("right")]
        public async Task RightPress(CommandContext ctx)
        {
            KeyPress(KeyCodes.F13);
            Console.WriteLine("Recieved Input Right");
            await ImgReturn(ctx);
        }


        public async Task ImgReturn(CommandContext ctx)
        {
            //Waits for game to catch up, presses the F21 key, then gives the emulator time to screenshot
            Thread.Sleep(Config.CurrentGlobal.ScreenshotDelay);

            KeyPress(KeyCodes.F21);
            Thread.Sleep(100);
            //This time is to give the emulator time to take a screenshot. Feel free to increase/decrease depending on computer speed

            //Takes most recent image in scrnshot folder, saves it as a png, scaling it up, and sending it off
            string botimg = Config.CurrentGlobal.ScreenshotPath;
            string[] initlist = System.IO.Directory.GetFiles(botimg);
            string currentimg = initlist[initlist.Length - 1];
            Console.WriteLine(currentimg);

            // TODO: test BmpToMemoryStream

            SaveBmpAsPNG(currentimg);

            //Just copying this over from my SaveImageAsPNG function
            //this still feels just as weird and shitty as it did before
            //idk if there's a better way to do this without declaring the variable as public, or if I should be declaring it as public in the first place
            //but hey, if it works it works
            string temp = Path.GetTempPath();
            temp += "state.png";

            await ctx.RespondWithFileAsync(temp);
        }

        // I'm going to be honest, this code was almost exclusively written by mpen on stackoverflow
        // https://stackoverflow.com/a/24199315
        // This was a gigantic headache that could have all been avoided if bgb just outputted a higher res image
        // but it doesn't
        // Fuck me
        // The only changes I made to it were to disable smoothing and pixel offset, 
        // and to set interpolation to nearest neighbor

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height)
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

        // UNTESTED code and i can't test
        private Stream BmpToMemoryStream(string current_img) 
        {
            MemoryStream stream = new MemoryStream();
            Bitmap bmp1 = new Bitmap(current_img);
            Bitmap resizedbmp = ResizeImage(bmp1, 800, 720);

            resizedbmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            resizedbmp.Dispose();
            bmp1.Dispose();

            return stream;
        }

        /// <summary>
        /// saves bmp as png (duh)
        /// </summary>
        /// <param name="current_img">The image to save.</param>
        /// <returns>void</returns>
        [Obsolete("Try implementing then using BmpToStream instead!")]
        private void SaveBmpAsPNG(string current_img)
        {
            Bitmap bmp1 = new Bitmap(current_img);

            Bitmap resizedbmp = ResizeImage(bmp1, 800, 720);

            //this bit of code feels shit but I guess it works
            //I really haven't done nearly as much as I'd like to involving file stuff
            string temp = Path.GetTempPath();
            temp += "state.png";

            resizedbmp.Save(temp, System.Drawing.Imaging.ImageFormat.Png);
            resizedbmp.Dispose();
            bmp1.Dispose();
        }
    }
}
