using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

namespace TileMapper
{
    static class Program
    {

        // This is a demo program. See the following:
        // ImGui.NET: https://github.com/mellinoe/ImGui.NET
        // Raylib-cs: https://github.com/ChrisDill/Raylib-cs
        // rlImGui-cs: https://github.com/raylib-extras/rlImGui-cs
        public static void Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(1280, 800, "raylib-Extras-cs [ImGui] example - simple ImGui Demo");
            Raylib.SetTargetFPS(144);

            rlImGui.Setup(true);

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DARKGRAY);

                rlImGui.Begin();
                ImGui.ShowDemoWindow();
                rlImGui.End();

                Raylib.EndDrawing();
            }

            rlImGui.Shutdown();
            Raylib.CloseWindow();
        }

    }
}