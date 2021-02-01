/*******************************************************************************************
*
*   raylib [textures] example - Bunnymark
*
*   This example has been created using raylib 1.6 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2014-2019 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.MouseButton;

namespace Examples
{
    public class textures_bunnymark
    {
        public const int MAX_BUNNIES = 50000;    // 50K bunnies limit

        // This is the maximum amount of elements (quads) per batch
        // NOTE: This value is defined in [rlgl] module and can be changed there
        public const int MAX_BATCH_ELEMENTS = 8192;

        struct Bunny
        {
            public Vector2 position;
            public Vector2 speed;
            public Color color;
        }

        public static int Main()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 800;
            const int screenHeight = 450;

            InitWindow(screenWidth, screenHeight, "raylib [textures] example - bunnymark");

            // Load bunny texture
            Texture2D texBunny = LoadTexture("resources/wabbit_alpha.png");

            Bunny[] bunnies = new Bunny[MAX_BUNNIES];    // Bunnies array

            int bunniesCount = 0;           // Bunnies counter

            SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
            //--------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose())    // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                if (IsMouseButtonDown(MOUSE_LEFT_BUTTON))
                {
                    // Create more bunnies
                    for (int i = 0; i < 100; i++)
                    {
                        if (bunniesCount < MAX_BUNNIES)
                        {
                            bunnies[bunniesCount].position = GetMousePosition();
                            bunnies[bunniesCount].speed.X = (float)GetRandomValue(-250, 250) / 60.0f;
                            bunnies[bunniesCount].speed.Y = (float)GetRandomValue(-250, 250) / 60.0f;
                            bunnies[bunniesCount].color = new Color(GetRandomValue(50, 240),
                                                               GetRandomValue(80, 240),
                                                               GetRandomValue(100, 240), 255);
                            bunniesCount++;
                        }
                    }
                }

                // Update bunnies
                for (int i = 0; i < bunniesCount; i++)
                {
                    bunnies[i].position.X += bunnies[i].speed.X;
                    bunnies[i].position.Y += bunnies[i].speed.Y;

                    if (((bunnies[i].position.X + texBunny.width / 2) > GetScreenWidth()) ||
                        ((bunnies[i].position.X + texBunny.width / 2) < 0)) bunnies[i].speed.X *= -1;
                    if (((bunnies[i].position.Y + texBunny.height / 2) > GetScreenHeight()) ||
                        ((bunnies[i].position.Y + texBunny.height / 2 - 40) < 0)) bunnies[i].speed.Y *= -1;
                }
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();

                ClearBackground(RAYWHITE);

                for (int i = 0; i < bunniesCount; i++)
                {
                    // NOTE: When internal batch buffer limit is reached (MAX_BATCH_ELEMENTS),
                    // a draw call is launched and buffer starts being filled again;
                    // before issuing a draw call, updated vertex data from internal CPU buffer is send to GPU...
                    // Process of sending data is costly and it could happen that GPU data has not been completely
                    // processed for drawing while new data is tried to be sent (updating current in-use buffers)
                    // it could generates a stall and consequently a frame drop, limiting the number of drawn bunnies
                    DrawTexture(texBunny, (int)bunnies[i].position.X, (int)bunnies[i].position.Y, bunnies[i].color);
                }

                DrawRectangle(0, 0, screenWidth, 40, BLACK);
                DrawText(string.Format("bunnies: {0}", bunniesCount), 120, 10, 20, GREEN);
                DrawText(string.Format("batched draw calls: {0}", 1 + bunniesCount / MAX_BATCH_ELEMENTS), 320, 10, 20, MAROON);

                DrawFPS(10, 10);

                EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            UnloadTexture(texBunny);    // Unload bunny texture

            CloseWindow();              // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

            return 0;
        }
    }
}