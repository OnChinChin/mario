/*******************************************************************************************
*
*   raylib [shapes] example - raylib logo animation
*
*   This example has been created using raylib 1.4 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2014 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;

namespace Examples
{
    public class shapes_logo_raylib_anim
    {
        public static int Main()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 800;
            const int screenHeight = 450;

            InitWindow(screenWidth, screenHeight, "raylib [shapes] example - raylib logo animation");

            int logoPositionX = screenWidth / 2 - 128;
            int logoPositionY = screenHeight / 2 - 128;

            int framesCounter = 0;
            int lettersCount = 0;

            int topSideRecWidth = 16;
            int leftSideRecHeight = 16;

            int bottomSideRecWidth = 16;
            int rightSideRecHeight = 16;

            int state = 0;                  // Tracking animation states (State Machine)
            float alpha = 1.0f;             // Useful for fading

            Color outline = new Color(139, 71, 135, 255);

            SetTargetFPS(60);
            //--------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose())    // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                if (state == 0)                 // State 0: Small box blinking
                {
                    framesCounter++;

                    if (framesCounter == 120)
                    {
                        state = 1;
                        framesCounter = 0;      // Reset counter... will be used later...
                    }
                }
                else if (state == 1)            // State 1: Top and left bars growing
                {
                    topSideRecWidth += 4;
                    leftSideRecHeight += 4;

                    if (topSideRecWidth == 256) state = 2;
                }
                else if (state == 2)            // State 2: Bottom and right bars growing
                {
                    bottomSideRecWidth += 4;
                    rightSideRecHeight += 4;

                    if (bottomSideRecWidth == 256) state = 3;
                }
                else if (state == 3)            // State 3: Letters appearing (one by one)
                {
                    framesCounter++;

                    if (framesCounter / 12 != 0)       // Every 12 frames, one more letter!
                    {
                        lettersCount++;
                        framesCounter = 0;
                    }

                    if (lettersCount >= 10)     // When all letters have appeared, just fade out everything
                    {
                        alpha -= 0.02f;

                        if (alpha <= 0.0f)
                        {
                            alpha = 0.0f;
                            state = 4;
                        }
                    }
                }
                else if (state == 4)            // State 4: Reset and Replay
                {
                    if (IsKeyPressed(KEY_R))
                    {
                        framesCounter = 0;
                        lettersCount = 0;

                        topSideRecWidth = 16;
                        leftSideRecHeight = 16;

                        bottomSideRecWidth = 16;
                        rightSideRecHeight = 16;

                        alpha = 1.0f;
                        state = 0;          // Return to State 0
                    }
                }
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();

                ClearBackground(RAYWHITE);

                if (state == 0)
                {
                    if ((framesCounter / 15) % 2 != 0) DrawRectangle(logoPositionX, logoPositionY, 16, 16, outline);
                }
                else if (state == 1)
                {
                    DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, outline);
                    DrawRectangle(logoPositionX, logoPositionY, 16, leftSideRecHeight, outline);
                }
                else if (state == 2)
                {
                    DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, outline);
                    DrawRectangle(logoPositionX, logoPositionY, 16, leftSideRecHeight, outline);

                    DrawRectangle(logoPositionX + 240, logoPositionY, 16, rightSideRecHeight, outline);
                    DrawRectangle(logoPositionX, logoPositionY + 240, bottomSideRecWidth, 16, outline);
                }
                else if (state == 3)
                {
                    DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, Fade(outline, alpha));
                    DrawRectangle(logoPositionX, logoPositionY + 16, 16, leftSideRecHeight - 32, Fade(outline, alpha));

                    DrawRectangle(logoPositionX + 240, logoPositionY + 16, 16, rightSideRecHeight - 32, Fade(outline, alpha));
                    DrawRectangle(logoPositionX, logoPositionY + 240, bottomSideRecWidth, 16, Fade(outline, alpha));

                    DrawRectangle(screenWidth / 2 - 112, screenHeight / 2 - 112, 224, 224, Fade(RAYWHITE, alpha));

                    DrawText("raylib".SubText(0, lettersCount), screenWidth / 2 - 44, screenHeight / 2 + 28, 50, Fade(new Color(155, 79, 151, 255), alpha));
                    DrawText("cs".SubText(0, lettersCount), screenWidth / 2 - 44, screenHeight / 2 + 58, 50, Fade(new Color(155, 79, 151, 255), alpha));
                }
                else if (state == 4)
                {
                    DrawText("[R] REPLAY", 340, 200, 20, GRAY);
                }

                EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            CloseWindow();        // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

            return 0;
        }
    }
}