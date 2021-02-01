/*******************************************************************************************
*
*   raylib [shaders] example - basic lighting
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version.
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3).
*
*   This example has been created using raylib 2.5 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Example contributed by Chris Camacho (@codifies) and reviewed by Ramon Santamaria (@raysan5)
*
*   Chris Camacho (@codifies -  http://bedroomcoders.co.uk/) notes:
*
*   This is based on the PBR lighting example, but greatly simplified to aid learning...
*   actually there is very little of the PBR example left!
*   When I first looked at the bewildering complexity of the PBR example I feared
*   I would never understand how I could do simple lighting with raylib however its
*   a testement to the authors of raylib (including rlights.h) that the example
*   came together fairly quickly.
*
*   Copyright (c) 2019 Chris Camacho (@codifies) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using static Raylib_cs.ConfigFlag;
using static Raylib_cs.CameraMode;
using static Raylib_cs.CameraType;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.ShaderLocationIndex;
using static Raylib_cs.MaterialMapType;
using static Examples.Rlights;

namespace Examples
{
    public class shaders_basic_lighting
    {
        const int GLSL_VERSION = 330;

        public unsafe static int Main()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 800;
            const int screenHeight = 450;

            SetConfigFlags(FLAG_MSAA_4X_HINT);  // Enable Multi Sampling Anti Aliasing 4x (if available)
            InitWindow(screenWidth, screenHeight, "raylib [shaders] example - basic lighting");

            // Define the camera to look into our 3d world
            Camera3D camera = new Camera3D();
            camera.position = new Vector3(2.0f, 2.0f, 6.0f);    // Camera position
            camera.target = new Vector3(0.0f, 0.5f, 0.0f);      // Camera looking at point
            camera.up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
            camera.fovy = 45.0f;                                // Camera field-of-view Y
            camera.type = CAMERA_PERSPECTIVE;                   // Camera mode type

            // Load models
            Model modelA = LoadModelFromMesh(GenMeshTorus(0.4f, 1.0f, 16, 32));
            Model modelB = LoadModelFromMesh(GenMeshCube(1.0f, 1.0f, 1.0f));
            Model modelC = LoadModelFromMesh(GenMeshSphere(0.5f, 32, 32));

            // Load models texture
            Texture2D texture = LoadTexture("resources/texel_checker.png");

            // Assign texture to default model material
            Utils.SetMaterialTexture(ref modelA, 0, MAP_ALBEDO, ref texture);
            Utils.SetMaterialTexture(ref modelB, 0, MAP_ALBEDO, ref texture);
            Utils.SetMaterialTexture(ref modelC, 0, MAP_ALBEDO, ref texture);

            Shader shader = LoadShader("resources/shaders/glsl330/base_lighting.vs",
                                       "resources/shaders/glsl330/lighting.fs");

            // Get some shader loactions
            int *locs = (int*)shader.locs.ToPointer();
            locs[(int)LOC_MATRIX_MODEL] = GetShaderLocation(shader, "matModel");
            locs[(int)LOC_VECTOR_VIEW] = GetShaderLocation(shader, "viewPos");

            // ambient light level
            int ambientLoc = GetShaderLocation(shader, "ambient");
            Utils.SetShaderValue(shader, ambientLoc, new float[] { 0.2f, 0.2f, 0.2f, 1.0f }, ShaderUniformDataType.UNIFORM_VEC4);

            float angle = 6.282f;

            // All models use the same shader
            Utils.SetMaterialShader(ref modelA, 0, ref shader);
            Utils.SetMaterialShader(ref modelB, 0, ref shader);
            Utils.SetMaterialShader(ref modelC, 0, ref shader);

            // Using 4 point lights, white, red, green and blue
            Light[] lights = new Light[MAX_LIGHTS];
            lights[0] = CreateLight(LightType.LIGHT_POINT, new Vector3(4, 2, 4), Vector3Zero(), WHITE, shader);
            lights[1] = CreateLight(LightType.LIGHT_POINT, new Vector3(4, 2, 4), Vector3Zero(), RED, shader);
            lights[2] = CreateLight(LightType.LIGHT_POINT, new Vector3(0, 4, 2), Vector3Zero(), GREEN, shader);
            lights[3] = CreateLight(LightType.LIGHT_POINT, new Vector3(0, 4, 2), Vector3Zero(), BLUE, shader);

            SetCameraMode(camera, CAMERA_ORBITAL);  // Set an orbital camera mode

            SetTargetFPS(60);                       // Set our game to run at 60 frames-per-second
            //--------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose())            // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                if (IsKeyPressed(KEY_W)) { lights[0].enabled = !lights[0].enabled; }
                if (IsKeyPressed(KEY_R)) { lights[1].enabled = !lights[1].enabled; }
                if (IsKeyPressed(KEY_G)) { lights[2].enabled = !lights[2].enabled; }
                if (IsKeyPressed(KEY_B)) { lights[3].enabled = !lights[3].enabled; }

                UpdateCamera(ref camera);              // Update camera

                // Make the lights do differing orbits
                angle -= 0.02f;
                lights[0].position.X = (float)Math.Cos(angle) * 4.0f;
                lights[0].position.Z = (float)Math.Sin(angle) * 4.0f;
                lights[1].position.X = (float)Math.Cos(-angle * 0.6f) * 4.0f;
                lights[1].position.Z = (float)Math.Sin(-angle * 0.6f) * 4.0f;
                lights[2].position.Y = (float)Math.Cos(angle * 0.2f) * 4.0f;
                lights[2].position.Z = (float)Math.Sin(angle * 0.2f) * 4.0f;
                lights[3].position.Y = (float)Math.Cos(-angle * 0.35f) * 4.0f;
                lights[3].position.Z = (float)Math.Sin(-angle * 0.35f) * 4.0f;

                UpdateLightValues(shader, lights[0]);
                UpdateLightValues(shader, lights[1]);
                UpdateLightValues(shader, lights[2]);
                UpdateLightValues(shader, lights[3]);

                // Rotate the torus
                modelA.transform = MatrixMultiply(modelA.transform, MatrixRotateX(-0.025f));
                modelA.transform = MatrixMultiply(modelA.transform, MatrixRotateZ(0.012f));

                // Update the light shader with the camera view position
                float[] cameraPos = { camera.position.X, camera.position.Y, camera.position.Z };
                Utils.SetShaderValue(shader, locs[(int)LOC_VECTOR_VIEW], cameraPos, ShaderUniformDataType.UNIFORM_VEC3);
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();

                ClearBackground(RAYWHITE);

                BeginMode3D(camera);

                // Draw the three models
                DrawModel(modelA, Vector3Zero(), 1.0f, WHITE);
                DrawModel(modelB, new Vector3(-1.6f, 0, 0), 1.0f, WHITE);
                DrawModel(modelC, new Vector3(1.6f, 0, 0), 1.0f, WHITE);

                // Draw markers to show where the lights are
                if (lights[0].enabled) { DrawSphereEx(lights[0].position, 0.2f, 8, 8, WHITE); }
                if (lights[1].enabled) { DrawSphereEx(lights[1].position, 0.2f, 8, 8, RED); }
                if (lights[2].enabled) { DrawSphereEx(lights[2].position, 0.2f, 8, 8, GREEN); }
                if (lights[3].enabled) { DrawSphereEx(lights[3].position, 0.2f, 8, 8, BLUE); }

                DrawGrid(10, 1.0f);

                EndMode3D();

                DrawFPS(10, 10);

                DrawText("Keys RGB & W toggle lights", 10, 30, 20, DARKGRAY);

                EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            UnloadModel(modelA);        // Unload the modelA
            UnloadModel(modelB);        // Unload the modelB
            UnloadModel(modelC);        // Unload the modelC

            UnloadTexture(texture);     // Unload the texture
            UnloadShader(shader);       // Unload shader

            CloseWindow();              // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

            return 0;
        }
    }
}
