using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper {
    
    public class Canvas : GraphicsWindow {

        int mapHeight, mapWidth;

        int unitSize;

        TileMap tMap;

        TileLayer currentLayer;

        TileSelector ts;

        TileSet set = null;

        int trueWidth, trueHeight;
        float currentWidth, currentHeight;

        // Scale = currernt/true.
        float scaleX, scaleY; 

        bool sizeSet = false;


        // The width and height should be that of the tile map.
        public Canvas(TileSelector ts, TileMap tMap) : base(0,0) {
            
            this.tMap = tMap;

            mapWidth = tMap.GetRows();
            mapHeight = tMap.GetCols();
            unitSize = tMap.GetUnitWidth();
            trueWidth = mapWidth*unitSize;
            trueHeight = mapHeight*unitSize;

            this.ResizeRenderTarget(trueWidth, trueHeight);

            this.ts = ts;
        }

        public override void DrawUI() {

            // Canvas is 600x600, other window elements are 8x27x8x8 found from GetWindowContentRegionMin.

            if (!sizeSet){
                ImGui.SetNextWindowSize(new Vector2(trueWidth + 8 + 8,trueHeight + 27 + 8));
                sizeSet = true;
            }

            ImGui.Begin("Canvas", ref _open);

            var size = ImGui.GetContentRegionAvail();

            currentWidth = size.X;
            currentHeight = size.Y;
            scaleX = currentWidth / trueWidth;
            scaleY = currentHeight / trueHeight;

            //Console.WriteLine(currentWidth + " " + currentWidth + " " + scaleX + " " + scaleY + " " + trueWidth + " " + trueHeight);

            currentLayer = tMap.GetCurrentLayer();

            // Canvas click, tile placement.
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {

                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - 8;
                int y = (int)mousePos.Y - (int)windowPos.Y - 27;

                //Console.WriteLine(x + " : " + y);

                x = (int)(x/(unitSize*scaleX));
                y = (int)(y/(unitSize*scaleY));

                
                
                try {
                    Tile t = ts.GetTileSelected();
                    currentLayer.SetTile((uint)x, (uint)y, t.Id, t.TileSet);
                } catch (Exception e) {}

            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right)) {
                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - 8;
                int y = (int)mousePos.Y - (int)windowPos.Y - 27;

                //Console.WriteLine(x + " : " + y);

                x = (int)(x/(unitSize*scaleX));
                y = (int)(y/(unitSize*scaleY));

                try {
                    currentLayer.SetTile((uint)x, (uint)y, -1, "");
                } catch (Exception e) {}
            }

            // Draw target.
            DrawRenderTarget((int)currentWidth, (int)currentHeight);

            ImGui.End();
        }

        public override void Update() {
        }

        // Draw according to trueSize not current.
        protected override void Draw() {

            Raylib.ClearBackground(Color.DARKBLUE);

            // Draw each layer.
            for (int k = 0; k < tMap.GetLayerCount(); k++) {

                TileLayer layer = tMap.GetLayer(k);

                for (uint i = 0; i < mapWidth; i++) {
                    for (uint j = 0; j < mapHeight; j++) {

                        Tile t = layer.GetTile(i,j);

                        set = tMap.NameToSet(t.TileSet);

                        // Null check.
                        if (set == null)
                            continue;

                        if (t.Id != -1) {
                            //Raylib.DrawRectangle(i*unitSize,j*unitSize,unitSize,unitSize,Color.DARKGREEN);
                            float scale = (float)unitSize/set.TileWidth;
                            set.Draw(i*unitSize,j*unitSize,(uint)t.Id, scale);
                        }
                    }
                }
            }

            DrawTileBorders();
        }

        // Draws the tile borders.
        private void DrawTileBorders() {
            for (uint i = 0; i < mapWidth; i++) {
                for (uint j = 0; j < mapHeight; j++) {
                    //draw tile borders
                    Raylib.DrawRectangleLinesEx(new Rectangle(i*unitSize,j*unitSize,unitSize,unitSize), 0.5f, Color.BLACK);
                }
            }
        }

    }
}