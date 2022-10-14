using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using WindowTest;

Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
Raylib.InitWindow(1280, 800, "raylib-Extras-cs [ImGui] example - simple ImGui Demo");
Raylib.SetTargetFPS(144);
TestWindow test = new TestWindow();
TestGraphicsWindow testGraphics = new TestGraphicsWindow();

rlImGui.Setup(true);

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    // Best to batch draw calls here.
    testGraphics.DoDraw();

    rlImGui.Begin();
    test.DrawUI();
    testGraphics.DrawUI();
    rlImGui.End();

    Raylib.EndDrawing();
    test.Update();
    testGraphics.Update();
}

rlImGui.Shutdown();
Raylib.CloseWindow();