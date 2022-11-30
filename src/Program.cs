using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using TileMapper.UI;

namespace TileMapper
{
    static class Program
    {

        // Uses many libraries. See the following:
        // ImGui.NET: https://github.com/mellinoe/ImGui.NET
        // Raylib-cs: https://github.com/ChrisDill/Raylib-cs
        // rlImGui-cs: https://github.com/raylib-extras/rlImGui-cs
        public static void Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(1280, 800, "Tile-Mapper");
            Raylib.SetTargetFPS(60);
            //Raylib.SetExitKey(0);
            Core core = new Core();

            rlImGui.Setup(true);

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                core.DoDraw();
                Raylib.ClearBackground(Color.DARKGRAY);

                rlImGui.Begin();
                core.DrawUI();
                rlImGui.End();

                Raylib.EndDrawing();
                core.Update();
            }

            core.Close();
            rlImGui.Shutdown();
            Raylib.CloseWindow();
        }

    }
}