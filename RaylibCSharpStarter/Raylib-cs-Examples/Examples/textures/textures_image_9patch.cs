/*******************************************************************************************
*
*   raylib [textures] example - 9-patch drawing
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   This example has been created using raylib 2.0 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2016 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.NPatchType;

namespace Examples
{
    public class textures_image_9patch
    {
        public static int Main()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 800;
            const int screenHeight = 450;

            InitWindow(screenWidth, screenHeight, "raylib [textures] example - 9-patch drawing");

            // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)
            Texture2D nPatchTexture = LoadTexture("resources/ninepatch_button.png");
            Vector2 mousePosition;
            Vector2 origin = new Vector2(0.0f, 0.0f);

            // The location and size of the n-patches.
            Rectangle dstRec1 = new Rectangle(480.0f, 160.0f, 32.0f, 32.0f);
            Rectangle dstRec2 = new Rectangle(160.0f, 160.0f, 32.0f, 32.0f);
            Rectangle dstRecH = new Rectangle(160.0f, 93.0f, 32.0f, 32.0f);   // this rec's height is ignored
            Rectangle dstRecV = new Rectangle(92.0f, 160.0f, 32.0f, 32.0f);   // this rec's width is ignored

            // A 9-patch (NPT_9PATCH) changes its sizes in both axis
            NPatchInfo ninePatchInfo1 = new NPatchInfo { sourceRec = new Rectangle(0.0f, 0.0f, 64.0f, 64.0f), left = 12, top = 40, right = 12, bottom = 12, type = (int)NPT_9PATCH };
            NPatchInfo ninePatchInfo2 = new NPatchInfo { sourceRec = new Rectangle(0.0f, 128.0f, 64.0f, 64.0f), left = 16, top = 16, right = 16, bottom = 16, type = (int)NPT_9PATCH };
            // A horizontal 3-patch (NPT_3PATCH_HORIZONTAL) changes its sizes along the x axis only
            NPatchInfo h3PatchInfo = new NPatchInfo { sourceRec = new Rectangle(0.0f, 64.0f, 64.0f, 64.0f), left = 8, top = 8, right = 8, bottom = 8, type = (int)NPT_3PATCH_HORIZONTAL };
            // A vertical 3-patch (NPT_3PATCH_VERTICAL) changes its sizes along the y axis only
            NPatchInfo v3PatchInfo = new NPatchInfo { sourceRec = new Rectangle(0.0f, 192.0f, 64.0f, 64.0f), left = 6, top = 6, right = 6, bottom = 6, type = (int)NPT_3PATCH_VERTICAL };

            SetTargetFPS(60);
            //---------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose())    // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                mousePosition = GetMousePosition();
                // resize the n-patches based on mouse position.
                dstRec1.width = mousePosition.X - dstRec1.x;
                dstRec1.height = mousePosition.Y - dstRec1.y;
                dstRec2.width = mousePosition.X - dstRec2.x;
                dstRec2.height = mousePosition.Y - dstRec2.y;
                dstRecH.width = mousePosition.X - dstRecH.x;
                dstRecV.height = mousePosition.Y - dstRecV.y;

                // set a minimum width and/or height
                if (dstRec1.width < 1.0f) dstRec1.width = 1.0f;
                if (dstRec1.width > 300.0f) dstRec1.width = 300.0f;
                if (dstRec1.height < 1.0f) dstRec1.height = 1.0f;
                if (dstRec2.width < 1.0f) dstRec2.width = 1.0f;
                if (dstRec2.width > 300.0f) dstRec2.width = 300.0f;
                if (dstRec2.height < 1.0f) dstRec2.height = 1.0f;
                if (dstRecH.width < 1.0f) dstRecH.width = 1.0f;
                if (dstRecV.height < 1.0f) dstRecV.height = 1.0f;
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();

                ClearBackground(RAYWHITE);

                // Draw the n-patches
                DrawTextureNPatch(nPatchTexture, ninePatchInfo2, dstRec2, origin, 0.0f, WHITE);
                DrawTextureNPatch(nPatchTexture, ninePatchInfo1, dstRec1, origin, 0.0f, WHITE);
                DrawTextureNPatch(nPatchTexture, h3PatchInfo, dstRecH, origin, 0.0f, WHITE);
                DrawTextureNPatch(nPatchTexture, v3PatchInfo, dstRecV, origin, 0.0f, WHITE);

                // Draw the source texture
                DrawRectangleLines(5, 88, 74, 266, BLUE);
                DrawTexture(nPatchTexture, 10, 93, WHITE);
                DrawText("TEXTURE", 15, 360, 10, DARKGRAY);

                DrawRectangle(10, 10, 250, 73, Fade(SKYBLUE, 0.5f));
                DrawRectangleLines(10, 10, 250, 73, BLUE);

                DrawText("9-Patch and 3-Patch example", 20, 20, 10, BLACK);
                DrawText("  Move the mouse to stretch or", 40, 40, 10, DARKGRAY);
                DrawText("  shrink the n-patches.", 40, 60, 10, DARKGRAY);

                EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            UnloadTexture(nPatchTexture);       // Texture unloading

            CloseWindow();                // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

            return 0;
        }

    }

}