/*******************************************************************************************
*
*   raylib [core] example - Picking in 3d mode
*
*   This example has been created using raylib 1.3 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2015 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.CameraType;
using static Raylib_cs.CameraMode;
using static Raylib_cs.MouseButton;
using static Raylib_cs.Color;

namespace Examples
{
    public class core_3d_picking
    {
        public static int Main()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 800;
            const int screenHeight = 450;

            InitWindow(screenWidth, screenHeight, "raylib [core] example - 3d picking");

            // Define the camera to look into our 3d world
            Camera3D camera;
            camera.position = new Vector3(10.0f, 10.0f, 10.0f); // Camera3D position
            camera.target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera3D looking at point
            camera.up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera3D up vector (rotation towards target)
            camera.fovy = 45.0f;                                // Camera3D field-of-view Y
            camera.type = (int)CAMERA_PERSPECTIVE;                   // Camera3D mode type

            Vector3 cubePosition = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 cubeSize = new Vector3(2.0f, 2.0f, 2.0f);

            Ray ray = new Ray(new Vector3(0.0f, 0.0f, 0.0f), Vector3.Zero);        // Picking line ray

            bool collision = false;

            SetCameraMode(camera, CAMERA_FREE); // Set a free camera mode

            SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
                                                //--------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose())        // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                UpdateCamera(ref camera);          // Update camera

                if (IsMouseButtonPressed(MOUSE_LEFT_BUTTON))
                {
                    ray = GetMouseRay(GetMousePosition(), camera);

                    // Check collision between ray and box
                    collision = CheckCollisionRayBox(ray,
                                new BoundingBox(new Vector3(cubePosition.X - cubeSize.X / 2, cubePosition.Y - cubeSize.Y / 2, cubePosition.Z - cubeSize.Z / 2),
                                              new Vector3(cubePosition.X + cubeSize.X / 2, cubePosition.Y + cubeSize.Y / 2, cubePosition.Z + cubeSize.Z / 2)));
                }
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();

                ClearBackground(RAYWHITE);

                BeginMode3D(camera);

                if (collision)
                {
                    DrawCube(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, RED);
                    DrawCubeWires(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, MAROON);

                    DrawCubeWires(cubePosition, cubeSize.X + 0.2f, cubeSize.Y + 0.2f, cubeSize.Z + 0.2f, GREEN);
                }
                else
                {
                    DrawCube(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, GRAY);
                    DrawCubeWires(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, DARKGRAY);
                }

                DrawRay(ray, MAROON);
                DrawGrid(10, 1.0f);

                EndMode3D();

                DrawText("Try selecting the box with mouse!", 240, 10, 20, DARKGRAY);

                if (collision) DrawText("BOX SELECTED", (screenWidth - MeasureText("BOX SELECTED", 30)) / 2, (int)(screenHeight * 0.1f), 30, GREEN);

                DrawFPS(10, 10);

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