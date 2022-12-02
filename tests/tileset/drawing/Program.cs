using Raylib_cs;
using TileMapper;

Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
Raylib.InitWindow(1280, 800, "raylib-Extras-cs [ImGui] example - simple ImGui Demo");
Raylib.SetTargetFPS(144);

TileSet ts = new TileSet("TestTileset.tms");

while (!Raylib.WindowShouldClose())
{

    // Begin frame.
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    // Draw each tile.
    var size = ts.GetTileDimensions();
    float scale = 100.0f;
    float drawnWidth = ts.TileWidth * scale;
    float drawnHeight = ts.TileHeight * scale;
    for (uint i = 0; i < size.Item1; i++)
    {
        for (uint j = 0; j < size.Item2; j++)
        {
            ts.Draw(i * drawnWidth, j * drawnHeight, ts.GetID(i, j), scale, scale);
        }
    }

    // Finish drawing.
    Raylib.EndDrawing();

}

ts.Dispose();
Raylib.CloseWindow();