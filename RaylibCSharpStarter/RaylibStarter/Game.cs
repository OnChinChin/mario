using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaylibStarter
{
    class Game
    {
        public int windowWidth = 800;
        public int windowHeight = 450;
        public string windowTitle = "Getting Started";

        Texture2D crateTexture;
        float createRotation = 0;
        
        public void LoadGame()
        {
            // TODO: Load game assets here
            crateTexture = Raylib.LoadTexture("./assets/crate_1.png");
        }

        public void Update(float deltaTime)
        {
            // TODO: Update related logic here
            createRotation += 45 * deltaTime;
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RAYWHITE);

            // TODO: Drawing related logic here

            // draws some text
            Raylib.DrawText("Hello World", 10, 10, 32, Color.DARKGRAY);

            // draws a rotating texture in center of screen
            RayLibExt.DrawTexture(crateTexture, windowWidth / 2, windowHeight / 2, 100, 100, 
                Color.WHITE, createRotation, 0.5f, 0.5f);

            // draw a horizontal line
            Raylib.DrawLine(0, windowHeight / 2, windowWidth, windowHeight / 2, Color.DARKGRAY);

            // draw a vertical line
            Raylib.DrawLine(windowWidth / 2, 0, windowWidth / 2, windowHeight, Color.DARKGRAY);

            Raylib.EndDrawing();
        }
    }
}
