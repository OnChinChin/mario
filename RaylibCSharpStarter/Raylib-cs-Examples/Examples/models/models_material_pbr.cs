/*******************************************************************************************
*
*   raylib [models] example - PBR material
*
*   This example has been created using raylib 1.8 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2017 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using static Raylib_cs.CameraMode;
using static Raylib_cs.CameraType;
using static Raylib_cs.TextureFilterMode;
using static Examples.Rlights;

namespace Examples
{
    public class models_material_pbr
    {
        public const int CUBEMAP_SIZE = 512;
        public const int IRRADIANCE_SIZE = 32;
        public const int PREFILTERED_SIZE = 256;
        public const int BRDF_SIZE = 512;

        public const float LIGHT_DISTANCE = 3.5f;
        public const float LIGHT_HEIGHT = 1.0f;

        public unsafe static int Main()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 800;
            const int screenHeight = 450;

            SetConfigFlags(ConfigFlag.FLAG_MSAA_4X_HINT);  // Enable Multi Sampling Anti Aliasing 4x (if available)
            InitWindow(screenWidth, screenHeight, "raylib [models] example - pbr material");

            // Define the camera to look into our 3d world
            Camera3D camera = new Camera3D();
            camera.position = new Vector3(4.0f, 4.0f, 4.0f);
            camera.target = new Vector3(0.0f, 0.5f, 0.0f);
            camera.up = new Vector3(0.0f, 1.0f, 0.0f);
            camera.fovy = 45.0f;
            camera.type = CAMERA_PERSPECTIVE;

            // Load model and PBR material
            Model model = LoadModel("resources/pbr/trooper.obj");

            // Unsafe pointers into model arrays.
            Material *materials = (Material *)model.materials.ToPointer();
            Mesh* meshes = (Mesh *)model.meshes.ToPointer();

            // Mesh tangents are generated... and uploaded to GPU
            // NOTE: New VBO for tangents is generated at default location and also binded to mesh VAO
            MeshTangents(ref meshes[0]);

            UnloadMaterial(materials[0]); // get rid of default material
            materials[0] = LoadMaterialPBR(new Color(255, 255, 255, 255), 1.0f, 1.0f);

            // Define lights attributes
            // NOTE: Shader is passed to every light on creation to define shader bindings internally
            CreateLight(LightType.LIGHT_POINT, new Vector3( LIGHT_DISTANCE, LIGHT_HEIGHT, 0.0f ), new Vector3( 0.0f, 0.0f, 0.0f ), new Color( 255, 0, 0, 255 ), materials[0].shader);
            CreateLight(LightType.LIGHT_POINT, new Vector3( 0.0f, LIGHT_HEIGHT, LIGHT_DISTANCE ), new Vector3( 0.0f, 0.0f, 0.0f ), new Color( 0, 255, 0, 255 ), materials[0].shader);
            CreateLight(LightType.LIGHT_POINT, new Vector3( -LIGHT_DISTANCE, LIGHT_HEIGHT, 0.0f ), new Vector3( 0.0f, 0.0f, 0.0f ),new Color( 0, 0, 255, 255 ), materials[0].shader);
            CreateLight(LightType.LIGHT_DIRECTIONAL, new Vector3(0.0f, LIGHT_HEIGHT * 2.0f, -LIGHT_DISTANCE ), new Vector3( 0.0f, 0.0f, 0.0f ), new Color(255, 0, 255, 255 ), materials[0].shader);

            SetCameraMode(camera, CAMERA_ORBITAL);  // Set an orbital camera mode

            SetTargetFPS(60);                       // Set our game to run at 60 frames-per-second
            //--------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose()) // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                UpdateCamera(ref camera);              // Update camera

                // Send to material PBR shader camera view position
                float[] cameraPos = { camera.position.X, camera.position.Y, camera.position.Z };
                int *locs = (int *)materials[0].shader.locs.ToPointer();
                Utils.SetShaderValue(materials[0].shader, (int)ShaderLocationIndex.LOC_VECTOR_VIEW, cameraPos, ShaderUniformDataType.UNIFORM_VEC3);

                //----------------------------------------------------------------------------------
                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();

                ClearBackground(RAYWHITE);

                BeginMode3D(camera);

                DrawModel(model, Vector3Zero(), 1.0f, WHITE);

                DrawGrid(10, 1.0f);

                EndMode3D();

                DrawFPS(10, 10);

                EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------

            // Shaders and textures must be unloaded by user,
            // they could be in use by other models
            MaterialMap *maps = (MaterialMap *)materials[0].maps.ToPointer();
            UnloadTexture(maps[(int)MaterialMapType.MAP_ALBEDO].texture);
            UnloadTexture(maps[(int)MaterialMapType.MAP_NORMAL].texture);
            UnloadTexture(maps[(int)MaterialMapType.MAP_METALNESS].texture);
            UnloadTexture(maps[(int)MaterialMapType.MAP_ROUGHNESS].texture);
            UnloadTexture(maps[(int)MaterialMapType.MAP_OCCLUSION].texture);
            UnloadTexture(maps[(int)MaterialMapType.MAP_IRRADIANCE].texture);
            UnloadTexture(maps[(int)MaterialMapType.MAP_PREFILTER].texture);
            UnloadTexture(maps[(int)MaterialMapType.MAP_BRDF].texture);
            UnloadShader(materials[0].shader);

            UnloadModel(model);         // Unload skybox model

            CloseWindow();              // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

            return 0;
        }

        // Load PBR material (Supports: ALBEDO, NORMAL, METALNESS, ROUGHNESS, AO, EMMISIVE, HEIGHT maps)
        // NOTE: PBR shader is loaded inside this function
        unsafe public static Material LoadMaterialPBR(Color albedo, float metalness, float roughness)
        {
            Material mat = Raylib.LoadMaterialDefault();   // NOTE: All maps textures are set to { 0 )

            string PATH_PBR_VS = "resources/shaders/glsl330/pbr.vs";
            string PATH_PBR_FS = "resources/shaders/glsl330/pbr.fs";

            mat.shader = LoadShader(PATH_PBR_VS, PATH_PBR_FS);

            // Temporary unsafe pointers into material arrays.
            MaterialMap *maps = (MaterialMap *)mat.maps.ToPointer();
            int *locs = (int *)mat.shader.locs.ToPointer();

            // Get required locations points for PBR material
            // NOTE: Those location names must be available and used in the shader code
            locs[(int)ShaderLocationIndex.LOC_MAP_ALBEDO] = GetShaderLocation(mat.shader, "albedo.sampler");
            locs[(int)ShaderLocationIndex.LOC_MAP_METALNESS] = GetShaderLocation(mat.shader, "metalness.sampler");
            locs[(int)ShaderLocationIndex.LOC_MAP_NORMAL] = GetShaderLocation(mat.shader, "normals.sampler");
            locs[(int)ShaderLocationIndex.LOC_MAP_ROUGHNESS] = GetShaderLocation(mat.shader, "roughness.sampler");
            locs[(int)ShaderLocationIndex.LOC_MAP_OCCLUSION] = GetShaderLocation(mat.shader, "occlusion.sampler");
            locs[(int)ShaderLocationIndex.LOC_MAP_IRRADIANCE] = GetShaderLocation(mat.shader, "irradianceMap");
            locs[(int)ShaderLocationIndex.LOC_MAP_PREFILTER] = GetShaderLocation(mat.shader, "prefilterMap");
            locs[(int)ShaderLocationIndex.LOC_MAP_BRDF] = GetShaderLocation(mat.shader, "brdfLUT");

            // Set view matrix location
            locs[(int)ShaderLocationIndex.LOC_MATRIX_MODEL] = GetShaderLocation(mat.shader, "matModel");
            locs[(int)ShaderLocationIndex.LOC_VECTOR_VIEW] = GetShaderLocation(mat.shader, "viewPos");

            // Set PBR standard maps
            maps[(int)MaterialMapType.MAP_ALBEDO].texture = LoadTexture("resources/pbr/trooper_albedo.png");
            maps[(int)MaterialMapType.MAP_NORMAL].texture = LoadTexture("resources/pbr/trooper_normals.png");
            maps[(int)MaterialMapType.MAP_METALNESS].texture = LoadTexture("resources/pbr/trooper_metalness.png");
            maps[(int)MaterialMapType.MAP_ROUGHNESS].texture = LoadTexture("resources/pbr/trooper_roughness.png");
            maps[(int)MaterialMapType.MAP_OCCLUSION].texture = LoadTexture("resources/pbr/trooper_ao.png");

            // Set environment maps
            const string PATH_CUBEMAP_VS = "resources/shaders/glsl330/cubemap.vs"; // Path to equirectangular to cubemap vertex shader
            const string PATH_CUBEMAP_FS = "resources/shaders/glsl330/cubemap.fs"; // Path to equirectangular to cubemap fragment shader
            const string PATH_SKYBOX_VS = "resources/shaders/glsl330/skybox.vs";  // Path to skybox vertex shader
            const string PATH_IRRADIANCE_FS = "resources/shaders/glsl330/irradiance.fs"; // Path to irradiance (GI) calculation fragment shader
            const string PATH_PREFILTER_FS = "resources/shaders/glsl330/prefilter.fs"; // Path to reflection prefilter calculation fragment shader
            const string PATH_BRDF_VS = "resources/shaders/glsl330/brdf.vs"; // Path to bidirectional reflectance distribution function vertex shader
            const string PATH_BRDF_FS = "resources/shaders/glsl330/brdf.fs"; // Path to bidirectional reflectance distribution function fragment shader

            Shader shdrCubemap = LoadShader(PATH_CUBEMAP_VS, PATH_CUBEMAP_FS);
            Shader shdrIrradiance = LoadShader(PATH_SKYBOX_VS, PATH_IRRADIANCE_FS);
            Shader shdrPrefilter = LoadShader(PATH_SKYBOX_VS, PATH_PREFILTER_FS);
            Shader shdrBRDF = LoadShader(PATH_BRDF_VS, PATH_BRDF_FS);

            // Setup required shader locations
            Utils.SetShaderValue(shdrCubemap, GetShaderLocation(shdrCubemap, "equirectangularMap"), 0);
            Utils.SetShaderValue(shdrIrradiance, GetShaderLocation(shdrIrradiance, "environmentMap"), 0);
            Utils.SetShaderValue(shdrPrefilter, GetShaderLocation(shdrPrefilter, "environmentMap"), 0);

            Texture2D texHDR = LoadTexture("resources/dresden_square.hdr");
            Texture2D cubemap = GenTextureCubemap(shdrCubemap, texHDR, CUBEMAP_SIZE);
            maps[(int)MaterialMapType.MAP_IRRADIANCE].texture = GenTextureIrradiance(shdrIrradiance, cubemap, IRRADIANCE_SIZE);
            maps[(int)MaterialMapType.MAP_PREFILTER].texture = GenTexturePrefilter(shdrPrefilter, cubemap, PREFILTERED_SIZE);
            maps[(int)MaterialMapType.MAP_BRDF].texture = GenTextureBRDF(shdrBRDF, BRDF_SIZE);
            UnloadTexture(cubemap);
            UnloadTexture(texHDR);

            // Unload already used shaders (to create specific textures)
            UnloadShader(shdrCubemap);
            UnloadShader(shdrIrradiance);
            UnloadShader(shdrPrefilter);
            UnloadShader(shdrBRDF);

            // Set textures filtering for better quality
            SetTextureFilter(maps[(int)MaterialMapType.MAP_ALBEDO].texture, FILTER_BILINEAR);
            SetTextureFilter(maps[(int)MaterialMapType.MAP_NORMAL].texture, FILTER_BILINEAR);
            SetTextureFilter(maps[(int)MaterialMapType.MAP_METALNESS].texture, FILTER_BILINEAR);
            SetTextureFilter(maps[(int)MaterialMapType.MAP_ROUGHNESS].texture, FILTER_BILINEAR);
            SetTextureFilter(maps[(int)MaterialMapType.MAP_OCCLUSION].texture, FILTER_BILINEAR);

            // Enable sample usage in shader for assigned textures
            Utils.SetShaderValue(mat.shader, GetShaderLocation(mat.shader, "albedo.useSampler"), 1);
            Utils.SetShaderValue(mat.shader, GetShaderLocation(mat.shader, "normals.useSampler"), 1);
            Utils.SetShaderValue(mat.shader, GetShaderLocation(mat.shader, "metalness.useSampler"), 1);
            Utils.SetShaderValue(mat.shader, GetShaderLocation(mat.shader, "roughness.useSampler"), 1);
            Utils.SetShaderValue(mat.shader, GetShaderLocation(mat.shader, "occlusion.useSampler"), 1);

            int renderModeLoc = GetShaderLocation(mat.shader, "renderMode");
            Utils.SetShaderValue(mat.shader, renderModeLoc, 0);

            // Set up material properties color
            maps[(int)MaterialMapType.MAP_ALBEDO].color = albedo;
            maps[(int)MaterialMapType.MAP_NORMAL].color = new Color(128, 128, 255, 255);
            maps[(int)MaterialMapType.MAP_METALNESS].value = metalness;
            maps[(int)MaterialMapType.MAP_ROUGHNESS].value = roughness;
            maps[(int)MaterialMapType.MAP_OCCLUSION].value = 1.0f;
            maps[(int)MaterialMapType.MAP_EMISSION].value = 0.5f;
            maps[(int)MaterialMapType.MAP_HEIGHT].value = 0.5f;

            return mat;
        }
    }
}
