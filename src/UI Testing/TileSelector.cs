using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper {

    public class TileSelector : GraphicsWindow {

        public static int TilesPerRow = 5;
        
        public static int UnitSize = 50;

        public static int TileGap = 20, RowGap = 10;

        int trueWidth, trueHeight;
        float currentWidth, currentHeight;

        float scaleX, scaleY; //scale = currernt/true

        //number of columns
        int rowNum;

        private int TileSelctedID = -1;

        private int[] tileList;
        
        TileSet set = new TileSet("./TestTileset.tms");

        public TileSelector(/*TileSet t, int unit*/) : base(260, 50) { 
            var dim = set.GetTileDimensions();
            uint tileNum = dim.Item1*dim.Item2;
            tileList = new int[tileNum];

            rowNum = (int)Math.Ceiling((double)tileNum/TilesPerRow);

            trueWidth = TilesPerRow*UnitSize + (TilesPerRow-1)*TileGap;
            trueHeight = rowNum*UnitSize + (rowNum-1)*RowGap;
            this.ResizeRenderTarget(trueWidth, trueHeight);
        }

        public override void DrawUI() {

            ImGui.SetNextWindowSize(new Vector2(trueWidth + 8 + 8, trueHeight + 27 + 8));

            ImGui.Begin("Tile Selector");

            var size = ImGui.GetContentRegionAvail();

             //TileSelector click, tile selection
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered()) {

                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - 8;
                int y = (int)mousePos.Y - (int)windowPos.Y - 27;

                Console.WriteLine(x + " : " + y);

                x = (int)(x/(UnitSize+20));
                y = (int)(y/(UnitSize+20));

                if (x >= 0)
                    TileSelctedID = tileList[x];

            }

            DrawRenderTarget((int)size.X, (int)size.Y);

            ImGui.End();
        }

        public override void Update() {

        }

        protected override void Draw() {

            Raylib.ClearBackground(Color.DARKBLUE);

            var t = set.GetTileDimensions();

            int col = 0, row = 0;
            for (uint i = 0, count = 0; i < t.Item1; i++) {
                for (uint j = 0; j < t.Item2; j++) {
                    float scale = UnitSize/set.TileWidth;
                    set.Draw(row*UnitSize + row*TileGap, col*UnitSize+RowGap*col, set.GetID(j,i), scale);
                    tileList[count] = (int)set.GetID(i,j);
                    count++;
                    row++;

                    if(row >= TilesPerRow) {
                        col++;
                        row = 0;
                    }
                }
            }        
        }

        public int GetTileSelectedTD() {
            return TileSelctedID;
        }

    }
}