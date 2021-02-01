/*******************************************************************************************
*
*   raylib [textures] example - sprite button
*
*   This example has been created using raylib 2.5 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2019 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.MouseButton;

namespace Examples
{
    public class textures_sprite_button
    {
        public const int NUM_FRAMES = 3;       // Number of frames (rectangles) for the button sprite texture

        public static int Main()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 800;
            const int screenHeight = 450;

            InitWindow(screenWidth, screenHeight, "raylib [textures] example - sprite button");

            InitAudioDevice();      // Initialize audio device

            Sound fxButton = LoadSound("resources/buttonfx.wav");   // Load button sound
            Texture2D button = LoadTexture("resources/button.png"); // Load button texture

            // Define frame rectangle for drawing
            int frameHeight = button.height / NUM_FRAMES;
            Rectangle sourceRec = new Rectangle(0, 0, button.width, frameHeight);

            // Define button bounds on screen
            Rectangle btnBounds = new Rectangle(screenWidth / 2 - button.width / 2, screenHeight / 2 - button.height / NUM_FRAMES / 2, button.width, frameHeight);

            int btnState = 0;               // Button state: 0-NORMAL, 1-MOUSE_HOVER, 2-PRESSED
            bool btnAction = false;         // Button action should be activated

            Vector2 mousePoint = new Vector2(0.0f, 0.0f);

            SetTargetFPS(60);
            //--------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose())    // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                mousePoint = GetMousePosition();
                btnAction = false;

                // Check button state
                if (CheckCollisionPointRec(mousePoint, btnBounds))
                {
                    if (IsMouseButtonDown(MOUSE_LEFT_BUTTON)) btnState = 2;
                    else btnState = 1;

                    if (IsMouseButtonReleased(MOUSE_LEFT_BUTTON)) btnAction = true;
                }
                else btnState = 0;

                if (btnAction)
                {
                    PlaySound(fxButton);

                    // TODO: Any desired action
                }

                // Calculate button frame rectangle to draw depending on button state
                sourceRec.y = btnState * frameHeight;
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();

                ClearBackground(RAYWHITE);

                DrawTextureRec(button, sourceRec, new Vector2(btnBounds.x, btnBounds.y), WHITE); // Draw button frame

                EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            UnloadTexture(button);  // Unload button texture
            UnloadSound(fxButton);  // Unload sound

            CloseAudioDevice();     // Close audio device

            CloseWindow();          // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

            return 0;
        }
    }
}