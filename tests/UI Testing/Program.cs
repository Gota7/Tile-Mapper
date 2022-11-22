using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using TileMapper;

Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
Raylib.InitWindow(1280, 800, "Canvas Demo");
Raylib.SetTargetFPS(90);

rlImGui.Setup(true);


TileSet set = new TileSet("./TestTileset.tms");

Boolean circle = true;
float radius = 30f;
TileSelector ts = new TileSelector();
TileMap tMap = new TileMap(15, 15, 30, 30);
TSSelector tss = new TSSelector("./TileSets", ts, tMap);
Canvas c = new Canvas(ts, tMap);

var size = set.GetTileDimensions();
float scale = 25f;
float drawnWidth = set.TileWidth * scale;
float drawnHeight = set.TileHeight * scale;


int tileId = (int)set.GetID(0, 0);


while (!Raylib.WindowShouldClose())
{


    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);


    if (circle)
        Raylib.DrawCircle(100, 100, radius, Color.RED);

    // Redner canvas raylib.
    c.DoDraw();

    ts.DoDraw();

    rlImGui.Begin();
    ImGui.Begin("Red Circle");
    ImGui.Checkbox("draw circle", ref circle);
    ImGui.SliderFloat("raduis", ref radius, 5f, 100f);
    ImGui.End();
    c.DrawUI();
    ts.DrawUI();
    tss.DrawUI();
    rlImGui.End();

    Raylib.EndDrawing();
}

rlImGui.Shutdown();
Raylib.CloseWindow();

// This was to create a new TileSet, keeping if we need to make another one.

// TileSet fla = new TileSet("./grass.png", "grass");
// fla.TileHeight = 32;
// fla.TileWidth = 32;
// fla.TileInitialSpacingX = 0;
// fla.TileInitialSpacingY = 0;
// fla.TilePaddingX = 0;
// fla.TilePaddingY = 0;
// fla.Save("./grass.tms");

