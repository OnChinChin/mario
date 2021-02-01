using Raylib_cs;
using System;
using System.Numerics;
using System.Threading;

namespace AIE_02_Raylib_Mario
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            Raylib.InitWindow(game.windowWidth, game.windowHeight, game.windowTitle);
            Raylib.SetTargetFPS(60);

            game.LoadGame();
            while (Raylib.WindowShouldClose() == false)
            {
                float frameTime = Raylib.GetFrameTime();
                game.Update(frameTime);
                game.Draw();
            }
            Raylib.CloseWindow();
        }
    }
}
