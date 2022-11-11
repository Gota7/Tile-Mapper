using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper {
    
    public class Canvas : GraphicsWindow {

        int mapHeight, mapWidth;

        int unitSize;

        int[,] map;

        TileSelector ts;

        TileSet set = new TileSet("./TestTileset.tms");

        int trueWidth, trueHeight;
        float currentWidth, currentHeight;

        float scaleX, scaleY; //scale = currernt/true


        //the width and height should be that of the tile map
        public Canvas(TileSelector ts) : base(15*30,15*30/*must be true size*/) {

            mapWidth = 15;
            mapHeight = 15;
            unitSize = 30;
            trueWidth = mapWidth*unitSize;
            trueHeight = mapHeight*unitSize;

            this.ResizeRenderTarget(trueWidth, trueHeight);

            this.ts = ts;

            map  = new int [mapWidth,mapHeight];
            for (int i = 0; i < mapWidth; i++) {
                for (int j = 0; j < mapHeight; j++) {
                    map[i,j] = -1;
                }
            }
        }

        public override void DrawUI() {

            //canvas is 600x600, other window elements are 8x27x8x8 found from GetWindowContentRegionMin
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(trueWidth + 8 + 8,trueHeight + 27 + 8));

            ImGui.Begin("Canvas", ref _open);

            var size = ImGui.GetContentRegionAvail();

            currentWidth = size.X;
            currentHeight = size.Y;
            scaleX = currentWidth / trueWidth;
            scaleY = currentHeight / trueHeight;

            //Console.WriteLine(currentWidth + " " + currentWidth + " " + scaleX + " " + scaleY + " " + trueWidth + " " + trueHeight);


            //canvas click, tile placement
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {

                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - 8;
                int y = (int)mousePos.Y - (int)windowPos.Y - 27;

                //Console.WriteLine(x + " : " + y);

                x = (int)(x/(unitSize*scaleX));
                y = (int)(y/(unitSize*scaleY));
                
                try {
                    map[x,y] = ts.GetTileSelectedTD();
                } catch (Exception e) {}

            }

            // Draw target.
            DrawRenderTarget((int)currentWidth, (int)currentHeight);

            ImGui.End();
        }

        public override void Update() {
        }

        //draw according to trueSize not current
        protected override void Draw() {

            Raylib.ClearBackground(Color.DARKBLUE);

            for (int i = 0; i < mapWidth; i++) {
                for (int j = 0; j < mapHeight; j++) {

                    if (map[i,j] != -1) {
                        //Raylib.DrawRectangle(i*unitSize,j*unitSize,unitSize,unitSize,Color.DARKGREEN);
                        float scale = unitSize/set.TileWidth;
                        set.Draw(i*unitSize,j*unitSize,(uint)map[i,j], scale);
                    }

                    Raylib.DrawRectangleLinesEx(new Rectangle(i*unitSize,j*unitSize,unitSize,unitSize), 0.5f, Color.BLACK);
                }
            }
        }
    }
}